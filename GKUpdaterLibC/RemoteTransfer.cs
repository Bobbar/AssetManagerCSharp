using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Forms;

namespace RemoteFileTransferTool
{
    public class RemoteTransfer
    {
        private BackgroundWorker copyWorker;
        private Timer speedTimer;
        private TranferStatus currentStatus;
        private List<string> errorList = new List<string>();
        private string sourcePath;
        private string destinationPath;
        private string targetRoot;
        private string targetHostname;
        private NetworkCredential adminCreds;
        private int currentFileIndex = 0;
        private Stopwatch elapTime = new Stopwatch();
        private ProgressCounter progress = new ProgressCounter();
        private bool createMissingDirectories = true;
        private bool isPaused = false;
        private string transferDescription;

        public RemoteTransfer(string hostname, string sourcePath, string destPath, string transferDescription)
        {
            this.sourcePath = sourcePath;
            destinationPath = destPath;
            targetHostname = hostname;
            targetRoot = @"\\" + hostname + @"\c$";
            this.transferDescription = transferDescription;
            InitWorker();
            InitializeTimer();
        }

        public event EventHandler LogEvent;

        public event EventHandler StatusUpdate;

        public event EventHandler TransferCanceled;

        public event EventHandler TransferComplete;

        protected virtual void OnLogEvent(LogEventArgs e)
        {
            if (LogEvent != null)
            {
                LogEvent(this, e);
            }
        }

        protected virtual void OnStatusUpdate(TransferStatusEventArgs e)
        {
            if (StatusUpdate != null)
            {
                StatusUpdate(this, e);
            }
        }

        protected virtual void OnTransferCanceled(EventArgs e)
        {
            Logger("Canceled by user!");
            if (TransferCanceled != null)
            {
                TransferCanceled(this, e);
            }
        }

        protected virtual void OnTransferComplete(TransferCompleteEventArgs e)
        {
            if (TransferComplete != null)
            {
                TransferComplete(this, e);
            }
        }

        public bool IsDisposed
        {
            get { return disposedValue; }
        }

        public List<string> ErrorList
        {
            get { return errorList; }
        }

        public TranferStatus TransferStatus
        {
            get { return currentStatus; }
        }

        public bool CreateMissingDirectories
        {
            get { return createMissingDirectories; }
            set { createMissingDirectories = value; }
        }

        public void CancelTransfer()
        {
            isPaused = false;
            if (copyWorker.IsBusy)
            {
                copyWorker.CancelAsync();
            }
            else
            {
                OnTransferCanceled(new EventArgs());
            }
            currentFileIndex = 0;
        }

        public void PauseTransfer()
        {
            isPaused = true;
            progress = new ProgressCounter();
            copyWorker.CancelAsync();
        }

        public void ResumeTransfer()
        {
            isPaused = false;
            var newArgs = new WorkerArgs(0, currentFileIndex, adminCreds);
            if (!copyWorker.IsBusy) copyWorker.RunWorkerAsync(newArgs);
        }

        public void StartTransfer(NetworkCredential credentials)
        {
            if (credentials == null)
            {
                throw new Win32Exception(1326);
            }

            if (!copyWorker.IsBusy)
            {
                Logger("------------------------------------------------");
                Logger("Starting " + transferDescription + " to: " + targetHostname + "...");
                errorList.Clear();

                var workArgs = new WorkerArgs(0, 0, credentials);
                adminCreds = credentials;

                progress = new ProgressCounter();
                elapTime = new Stopwatch();
                elapTime.Start();

                copyWorker.RunWorkerAsync(workArgs);
            }
        }

        /// <summary>
        /// Pings the current device. Success returns True. All failures return False.
        /// </summary>
        /// <returns></returns>
        private bool CanPing()
        {
            try
            {
                using (var ping = new Ping())
                {
                    int timeout = 1000;
                    byte[] buff = Encoding.ASCII.GetBytes("pingpingpingpingping");
                    var options = new PingOptions();
                    options.DontFragment = true;

                    PingReply reply = ping.Send(targetHostname, timeout, buff, options);

                    if (reply.Status == IPStatus.Success)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void CopyFile(string source, string dest)
        {
            int bufferSize = 256000;
            byte[] buffer = new byte[bufferSize];
            int bytesIn = 1;
            var sourceFile = new FileInfo(source);

            progress.ResetProgress();

            using (var sourceStream = sourceFile.OpenRead())
            using (var destStream = new FileStream(dest, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write, bufferSize, FileOptions.None))
            {
                currentStatus.CurrentFileProgress = 1;

                progress.BytesToTransfer = sourceStream.Length;
                while (!(bytesIn < 1 | copyWorker.CancellationPending))
                {
                    bytesIn = sourceStream.Read(buffer, 0, bufferSize);
                    if (bytesIn > 0)
                    {
                        destStream.Write(buffer, 0, bytesIn);
                        progress.BytesMoved = bytesIn;
                    }
                }
                buffer = null;
            }
        }

        private void CopyWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!CanPing())
            {
                throw new Exception("Cannot ping device.");
            }

            var workArgs = (WorkerArgs)e.Argument;

            using (var netConnection = new NetworkConnection(targetRoot, workArgs.Credentials))
            {
                string targetDir = targetRoot + destinationPath;
                int fileIndex = workArgs.StartIndex;

                //Get array of full paths of all files in source dir and sub-dirs
                string[] files = Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories);

                //Loop through file array
                for (int i = workArgs.StartIndex; i < files.Length; i++)
                {
                    var file = files[i];

                    if (copyWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    //Counter for progress
                    fileIndex += 1;
                    currentFileIndex = fileIndex;

                    //Modify source path to target path
                    string destPath = file.Replace(sourcePath, targetDir);

                    //Record status for UI updates
                    var status = new TranferStatus(files.Length, fileIndex, destPath, file, currentStatus.CurrentFileProgress, currentStatus.CurrentTransferRate);

                    //Report status
                    copyWorker.ReportProgress(1, status);

                    //Check if file extists on target. Then check if file is read-only and try to change attribs
                    if (File.Exists(destPath))
                    {
                        var fileAttribs = File.GetAttributes(destPath);

                        if ((fileAttribs & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        {
                            copyWorker.ReportProgress(99, "******* File is read-only. Changing attributes...");
                            fileAttribs = fileAttribs & ~FileAttributes.ReadOnly;
                            File.SetAttributes(destPath, fileAttribs);
                        }
                    }
                    else
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(destPath)))
                        {
                            if (createMissingDirectories)
                            {
                                copyWorker.ReportProgress(99, "******* Creating Missing Directory: " + Path.GetDirectoryName(destPath));
                                Directory.CreateDirectory(Path.GetDirectoryName(destPath));
                            }
                            else
                            {
                                throw new MissingDirectoryException();
                            }
                        }
                    }

                    //Copy source to target, overwriting
                    CopyFile(file, destPath);
                }
            }
        }

        private void CopyWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 1)
            {
                currentStatus = (TranferStatus)e.UserState;
                OnStatusUpdate(new TransferStatusEventArgs(currentStatus));

                Logger(currentStatus.CurrentFileIdx + " of " + currentStatus.TotalFileCount);
                Logger("Source: " + currentStatus.SourceFileName);
                Logger("Dest: " + currentStatus.DestinationFileName);
            }
            else
            {
                Logger(e.UserState.ToString(), true);
            }
        }

        private void CopyWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (!e.Cancelled)
                {
                    elapTime.Stop();
                    Logger("Copy successful!  Errors: " + errorList.Count);

                    if (errorList.Count > 0)
                    {
                        Logger("Listing Errors: ");

                        foreach (var error in errorList)
                        {
                            Logger("---  " + (errorList.IndexOf(error) + 1) + " of " + errorList.Count);
                            Logger(error);
                            Logger("---");
                        }
                    }

                    Logger("All done!");
                    Logger("Elapsed time: " + (elapTime.ElapsedMilliseconds / 1000) + "s");
                    Logger("------------------------------------------------");

                    OnTransferComplete(new TransferCompleteEventArgs(false));
                }
                else
                {
                    if (!isPaused)
                    {
                        OnTransferCanceled(new EventArgs());
                    }
                }
            }
            else
            {
                if (e.Error.HResult == -2147024864 | e.Error.HResult == -2147024891)
                {
                    Logger("******** File in-use error! Resuming next files.", true);
                    ResumeTransfer();
                }
                else
                {
                    Logger("------------------------------------------------");
                    Logger("Unexpected errors during copy!");
                    Logger(e.Error.Message, true);
                    OnTransferComplete(new TransferCompleteEventArgs(true, e.Error));
                }
            }
        }

        private void Logger(string message, bool isError = false)
        {
            var log = new LogMessage(message, isError);
            OnLogEvent(new LogEventArgs(log));

            if (isError)
            {
                string errorMessage = "Error: " + message + "\n Info: \r\n" + currentStatus.CurrentFileIdx + " of " + currentStatus.TotalFileCount + "\n Source: " + currentStatus.SourceFileName + "\n Dest: " + currentStatus.DestinationFileName;
                errorList.Add(errorMessage);
            }
        }

        private void InitializeTimer()
        {
            speedTimer = new Timer();
            speedTimer.Interval = 100;
            speedTimer.Enabled = true;
            speedTimer.Tick += SpeedTimer_Tick;
        }

        private void InitWorker()
        {
            copyWorker = new BackgroundWorker();
            copyWorker.DoWork += CopyWorker_DoWork;
            copyWorker.RunWorkerCompleted += CopyWorker_RunWorkerCompleted;
            copyWorker.ProgressChanged += CopyWorker_ProgressChanged;
            copyWorker.WorkerReportsProgress = true;
            copyWorker.WorkerSupportsCancellation = true;
        }

        private void SpeedTimer_Tick(object sender, EventArgs e)
        {
            if (!isPaused) progress.Tick();

            if (progress.BytesMoved > 0)
            {
                currentStatus.CurrentTransferRate = progress.Throughput;
                currentStatus.CurrentFileProgress = progress.Percent;
            }
        }

        public struct LogMessage
        {
            public string Message { get; set; }
            public bool IsError { get; set; }

            public LogMessage(string message, bool isError)
            {
                Message = message;
                IsError = isError;
            }
        }

        public struct TranferStatus
        {
            public int CurrentFileIdx { get; }
            public string DestinationFileName { get; }
            public int CurrentFileProgress { get; set; }
            public double CurrentTransferRate { get; set; }
            public string SourceFileName { get; }
            public int TotalFileCount { get; }

            public TranferStatus(int totalFileCount, int currentFileIndex, string destinationFileName, string sourceFileName, int currentProgress, double currentTransferRate)
            {
                TotalFileCount = totalFileCount;
                CurrentFileIdx = currentFileIndex;
                DestinationFileName = destinationFileName;
                SourceFileName = sourceFileName;
                CurrentFileProgress = currentProgress;
                CurrentTransferRate = currentTransferRate;
            }
        }

        private struct WorkerArgs
        {
            public int CurrentIndex { get; set; }
            public int StartIndex { get; set; }
            public NetworkCredential Credentials { get; set; }

            public WorkerArgs(int currentIndex, int startIndex, NetworkCredential credential)
            {
                CurrentIndex = currentIndex;
                StartIndex = startIndex;
                Credentials = credential;
            }
        }

        public class TransferCompleteEventArgs : EventArgs
        {
            private Exception exception;

            private bool hasErrors;

            public TransferCompleteEventArgs(bool hasErrors, Exception ex = null)
            {
                this.hasErrors = hasErrors;
                this.exception = ex;
            }

            public Exception Errors
            {
                get { return exception; }
            }

            public bool HasErrors
            {
                get { return hasErrors; }
            }
        }

        public class TransferStatusEventArgs : EventArgs
        {
            private TranferStatus status;

            public TransferStatusEventArgs(TranferStatus status)
            {
                this.status = status;
            }

            public TranferStatus Status
            {
                get { return status; }
            }
        }

        public class LogEventArgs : EventArgs
        {
            private LogMessage message;

            public LogEventArgs(LogMessage message)
            {
                this.message = message;
            }

            public LogMessage Message
            {
                get { return message; }
            }
        }

        public class MissingDirectoryException : Exception
        {
            public MissingDirectoryException() : base("Directory not found on target.")
            {
            }
        }

        private bool disposedValue;

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    copyWorker.Dispose();
                    speedTimer.Dispose();
                    adminCreds = null;
                    elapTime.Stop();
                }
            }
            disposedValue = true;
        }
    }
}