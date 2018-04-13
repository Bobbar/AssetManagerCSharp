using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Forms;

namespace GKUpdaterLibC
{
    public class GKUpdater
    {
        private BackgroundWorker CopyWorker;
        private Timer SpeedTimer;
        private Status_Stats CurrentStatus;
        private List<string> ErrList = new List<string>();
        private readonly string GKPath = "\\PSi\\Gatekeeper\\";
        private string GKSourcePath;
        private string DestinationPath;
        private string ClientPath;
        private string ClientHostName;
        private NetworkCredential CurrentCreds;
        private int CurrentFileIndex = 0;
        private Stopwatch ElapTime = new Stopwatch();
        private ProgressCounter Progress = new ProgressCounter();
        private bool bolCreateMissingDirectory = true;
        private bool bolPaused = false;

        private string PushTypeMessage = "GK Update";

        public GKUpdater(string HostName, string SourcePath)
        {
            GKSourcePath = SourcePath;
            DestinationPath = GKPath;
            ClientHostName = HostName;
            ClientPath = "\\\\" + HostName + "\\c$";
            InitWorker();
            InitializeTimer();
        }

        public GKUpdater(string HostName, string SourcePath, string DestPath)
        {
            GKSourcePath = SourcePath;
            DestinationPath = DestPath;
            ClientHostName = HostName;
            ClientPath = "\\\\" + HostName + "\\c$";
            PushTypeMessage = "File Push";
            InitWorker();
            InitializeTimer();
        }

        public event EventHandler LogEvent;

        public event EventHandler StatusUpdate;

        public event EventHandler UpdateCanceled;

        public event EventHandler UpdateComplete;

        protected virtual void OnLogEvent(LogEvents e)
        {
            if (LogEvent != null)
            {
                LogEvent(this, e);
            }
        }

        protected virtual void OnStatusUpdate(GKUpdateEvents e)
        {
            if (StatusUpdate != null)
            {
                StatusUpdate(this, e);
            }
        }

        protected virtual void OnUpdateCanceled(EventArgs e)
        {
            GKLog("Canceled by user!");
            if (UpdateCanceled != null)
            {
                UpdateCanceled(this, e);
            }
        }

        protected virtual void OnUpdateComplete(GKUpdateCompleteEvents e)
        {
            if (UpdateComplete != null)
            {
                UpdateComplete(this, e);
            }
        }

        public bool IsDisposed
        {
            get { return disposedValue; }
        }

        public List<string> ErrorList
        {
            get { return ErrList; }
        }

        public Status_Stats UpdateStatus
        {
            get { return CurrentStatus; }
        }

        public bool CreateMissingDirectories
        {
            get { return bolCreateMissingDirectory; }
            set { bolCreateMissingDirectory = value; }
        }

        public void CancelUpdate()
        {
            bolPaused = false;
            if (CopyWorker.IsBusy)
            {
                CopyWorker.CancelAsync();
            }
            else
            {
                OnUpdateCanceled(new EventArgs());
            }
            CurrentFileIndex = 0;
        }

        public void PauseUpdate()
        {
            bolPaused = true;
            Progress = new ProgressCounter();
            CopyWorker.CancelAsync();
        }

        public void ResumeUpdate()
        {
            bolPaused = false;
            var newArgs = new WorkerArgs(0, CurrentFileIndex, CurrentCreds);
            //NewArgs.StartIndex = CurrentFileIndex;
            //NewArgs.Credentials = CurrentCreds;
            if (!CopyWorker.IsBusy) CopyWorker.RunWorkerAsync(newArgs);
        }

        public void StartUpdate(NetworkCredential Creds)
        {
            if (Creds == null)
            {
                throw new Win32Exception(1326);
                return;
            }
            GKLog("------------------------------------------------");
            GKLog("Starting " + PushTypeMessage + " to: " + ClientHostName);
            GKLog("Starting " + PushTypeMessage + "...");
            ErrList.Clear();
            WorkerArgs WorkArgs = new WorkerArgs();
            WorkArgs.StartIndex = 0;
            WorkArgs.Credentials = Creds;
            CurrentCreds = Creds;
            Progress = new ProgressCounter();
            ElapTime = new Stopwatch();
            ElapTime.Start();
            if (!CopyWorker.IsBusy)
                CopyWorker.RunWorkerAsync(WorkArgs);
        }

        /// <summary>
        /// Pings the current device. Success returns True. All failures return False.
        /// </summary>
        /// <returns></returns>
        private bool CanPing()
        {
            try
            {
                using (Ping MyPing = new Ping())
                {
                    var options = new System.Net.NetworkInformation.PingOptions();
                    string Hostname = ClientHostName;
                    int Timeout = 1000;
                    byte[] buff = Encoding.ASCII.GetBytes("pingpingpingpingping");
                    options.DontFragment = true;
                    PingReply reply = MyPing.Send(Hostname, Timeout, buff, options);
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

        private void CopyFile(string Source, string Dest)
        {
            int BufferSize = 256000;
            byte[] buffer = new byte[BufferSize];
            int bytesIn = 1;
            FileInfo CurrentFile = new FileInfo(Source);
            Progress.ResetProgress();
            using (var fStream = CurrentFile.OpenRead())
            using (var destFile = new FileStream(Dest, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write, BufferSize, FileOptions.None))
            {
                CurrentStatus.CurFileProgress = 1;
                Progress.BytesToTransfer = (int)fStream.Length;
                while (!(bytesIn < 1 | CopyWorker.CancellationPending))
                {
                    bytesIn = fStream.Read(buffer, 0, BufferSize);
                    if (bytesIn > 0)
                    {
                        destFile.Write(buffer, 0, bytesIn);
                        Progress.BytesMoved = bytesIn;
                    }
                }
            }
        }

        private void CopyWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!CanPing())
            {
                throw new Exception("Cannot ping device.");
            }
            WorkerArgs Args = (WorkerArgs)e.Argument;
            using (NetworkConnection NetCon = new NetworkConnection(ClientPath, Args.Credentials))
            {
                string sourceDir = GKSourcePath;
                string targetDir = ClientPath + DestinationPath;
                //GKPath
                int StartIdx = Args.StartIndex;
                int CurFileIdx = Args.StartIndex;
                //Get array of full paths of all files in source dir and sub-dirs

                string[] files = Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories);
                //Loop through file array
                for (int i = StartIdx; i < files.Length; i++)
                {
                    var file__1 = files[i];
                    if (CopyWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    //Counter for progress
                    CurFileIdx += 1;
                    CurrentFileIndex = CurFileIdx;
                    Args.CurrentIndex = CurFileIdx;

                    //Modify source path to target path
                    string cPath = file__1.Replace(sourceDir, targetDir);

                    //Record status for UI updates
                    var Status = new Status_Stats(files.Length, CurFileIdx, cPath, file__1, CurrentStatus.CurFileProgress, CurrentStatus.CurTransferRate);

                    //Report status
                    CopyWorker.ReportProgress(1, Status);
                    e.Result = Args;

                    //Check if file extists on target. Then check if file is read-only and try to change attribs
                    if (File.Exists(cPath))
                    {
                        FileAttributes FileAttrib = File.GetAttributes(cPath);
                        if ((FileAttrib & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        {
                            CopyWorker.ReportProgress(99, "******* File is read-only. Changing attributes...");
                            FileAttrib = FileAttrib & ~FileAttributes.ReadOnly;
                            File.SetAttributes(cPath, FileAttrib);
                        }
                    }
                    else
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(cPath)))
                        {
                            if (bolCreateMissingDirectory)
                            {
                                CopyWorker.ReportProgress(99, "******* Creating Missing Directory: " + Path.GetDirectoryName(cPath));
                                Directory.CreateDirectory(Path.GetDirectoryName(cPath));
                            }
                            else
                            {
                                throw new MissingDirectoryException();
                            }
                        }
                    }
                    //Copy source to target, overwriting
                    CopyFile(file__1, cPath);
                }
            }
        }

        private void CopyWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 1)
            {
                CurrentStatus = (Status_Stats)e.UserState;
                OnStatusUpdate(new GKUpdateEvents(CurrentStatus));
                GKLog(CurrentStatus.CurFileIdx + " of " + CurrentStatus.TotFiles);
                GKLog("Source: " + CurrentStatus.SourceFileName);
                GKLog("Dest: " + CurrentStatus.CurFileName);
            }
            else
            {
                GKLog(e.UserState.ToString(), true);
            }
        }

        private void CopyWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (!e.Cancelled)
                {
                    ElapTime.Stop();
                    GKLog("Copy successful!  Errors: " + ErrList.Count);
                    if (ErrList.Count > 0)
                    {
                        GKLog("Listing Errors: ");
                        foreach (var ErrMsg in ErrList)
                        {
                            GKLog("---  " + (ErrList.IndexOf(ErrMsg) + 1) + " of " + ErrList.Count);
                            GKLog(ErrMsg);
                            GKLog("---");
                        }
                    }
                    GKLog("All done!");
                    GKLog("Elapsed time: " + (ElapTime.ElapsedMilliseconds / 1000) + "s");
                    GKLog("------------------------------------------------");
                    OnUpdateComplete(new GKUpdateCompleteEvents(false));
                }
                else
                {
                    if (!bolPaused)
                    {
                        OnUpdateCanceled(new EventArgs());
                    }
                }
            }
            else
            {
                if (e.Error.HResult == -2147024864 | e.Error.HResult == -2147024891)
                {
                    GKLog("******** File in-use error! Resuming next files.", true);
                    ResumeUpdate();
                }
                else
                {
                    GKLog("------------------------------------------------");
                    GKLog("Unexpected errors during copy!");
                    GKLog(e.Error.Message, true);
                    OnUpdateComplete(new GKUpdateCompleteEvents(true, e.Error));
                }
            }
        }

        private void GKLog(string Message, bool ToErrList = false)
        {
            GK_Log_Info NewLog = new GK_Log_Info(Message, ToErrList);
            OnLogEvent(new LogEvents(NewLog));

            if (ToErrList)
            {
                string ErrMsg = "Error: " + Message + "\n Info: \r\n" + CurrentStatus.CurFileIdx + " of " + CurrentStatus.TotFiles + "\n Source: " + CurrentStatus.SourceFileName + "\n Dest: " + CurrentStatus.CurFileName;
                ErrList.Add(ErrMsg);
            }
        }

        private void InitializeTimer()
        {
            SpeedTimer = new Timer();
            SpeedTimer.Interval = 100;
            SpeedTimer.Enabled = true;
            SpeedTimer.Tick += SpeedTimer_Tick;
        }

        private void InitWorker()
        {
            CopyWorker = new BackgroundWorker();
            CopyWorker.DoWork += CopyWorker_DoWork;
            CopyWorker.RunWorkerCompleted += CopyWorker_RunWorkerCompleted;
            CopyWorker.ProgressChanged += CopyWorker_ProgressChanged;
            CopyWorker.WorkerReportsProgress = true;
            CopyWorker.WorkerSupportsCancellation = true;
        }

        private void SpeedTimer_Tick(object sender, EventArgs e)
        {
            if (!bolPaused)
                Progress.Tick();
            if (Progress.BytesMoved > 0)
            {
                CurrentStatus.CurTransferRate = Progress.Throughput;
                CurrentStatus.CurFileProgress = Progress.Percent;
            }
            else
            {
            }
        }

        public struct GK_Log_Info
        {
            public string Message { get; set; }
            public bool ToErrList { get; set; }

            public GK_Log_Info(string Msg, bool ToErrLst)
            {
                Message = Msg;
                ToErrList = ToErrLst;
            }
        }

        public struct Status_Stats
        {
            public int CurFileIdx { get; set; }
            public string CurFileName { get; set; }
            public int CurFileProgress { get; set; }
            public double CurTransferRate { get; set; }
            public string SourceFileName { get; set; }
            public int TotFiles { get; set; }

            public Status_Stats(int tFiles, int CurFIdx, string CurFName, string sFileName, int CurFileProg, double CurTransRate)
            {
                TotFiles = tFiles;
                CurFileIdx = CurFIdx;
                CurFileName = CurFName;
                SourceFileName = sFileName;
                CurFileProgress = CurFileProg;
                CurTransferRate = CurTransRate;
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

        public class GKUpdateCompleteEvents : EventArgs
        {
            private Exception ErrExeption;

            private bool Errs;

            public GKUpdateCompleteEvents(bool Errs, Exception Ex = null)
            {
                this.Errs = Errs;
                this.ErrExeption = Ex;
            }

            public Exception Errors
            {
                get { return ErrExeption; }
            }

            public bool HasErrors
            {
                get { return Errs; }
            }
        }

        public class GKUpdateEvents : EventArgs
        {
            private Status_Stats eStatus;

            public GKUpdateEvents(Status_Stats Status)
            {
                eStatus = Status;
            }

            public Status_Stats CurrentStatus
            {
                get { return eStatus; }
            }
        }

        public class LogEvents : EventArgs
        {
            private GK_Log_Info MyLogInfo;

            public LogEvents(GK_Log_Info LogInfo)
            {
                MyLogInfo = LogInfo;
            }

            public GK_Log_Info LogData
            {
                get { return MyLogInfo; }
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
                    CopyWorker.Dispose();
                    SpeedTimer.Dispose();
                }
            }
            disposedValue = true;
        }
    }
}