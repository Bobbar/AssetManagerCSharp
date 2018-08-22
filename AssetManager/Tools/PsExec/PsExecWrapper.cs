using AssetManager.Security;
using AssetManager.Data;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AssetManager.Tools
{
    public class PSExecWrapper
    {
        private Process currentProcess;
        private string targetHostname;
        private CancellationTokenSource cancelSource;

        public Process CurrentProcess
        {
            get
            {
                return currentProcess;
            }
        }

        public event EventHandler<string> ErrorReceived;

        public event EventHandler<string> OutputReceived;

        public PSExecWrapper(string targetHostname)
        {
            this.targetHostname = targetHostname;
            StageExecutable();
        }

        protected virtual void OnErrorReceived(string data)
        {
            ErrorReceived?.Invoke(this, data);
        }

        protected virtual void OnOutputReceived(string data)
        {
            OutputReceived?.Invoke(this, data);
        }

        private void StageExecutable()
        {
            if (!Directory.Exists(Paths.PsExecTempDir))
            {
                Directory.CreateDirectory(Paths.PsExecTempDir);
            }

            if (!File.Exists(Paths.PsExecTempPath))
            {
                File.Copy(Paths.PsExecPath, Paths.PsExecTempPath);
            }
        }

        public async Task<int> ExecuteRemoteCommand(string command)
        {
            return await ExecuteRemoteCommand(command, !SecurityTools.IsAdministrator());
        }

        public async Task<int> ExecuteRemoteCommand(string command, bool runAsAdmin = true)
        {
            int exitCode = -1;

            if (currentProcess != null)
            {
                throw new Exception("A process is already running.");
            }

            try
            {
                exitCode = await Task.Run(() =>
                {
                    using (Process p = new Process())
                    {
                        currentProcess = p;

                        // Don't run as Admin if we're working on Vinton.
                        if (runAsAdmin && ServerInfo.CurrentDataBase != DatabaseName.vintondd)
                        {
                            p.StartInfo.Domain = SecurityTools.AdminCreds.Domain;
                            p.StartInfo.UserName = SecurityTools.AdminCreds.UserName;
                            p.StartInfo.Password = SecurityTools.AdminCreds.SecurePassword;
                        }

                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.RedirectStandardOutput = true;
                        p.StartInfo.RedirectStandardError = true;
                        p.StartInfo.CreateNoWindow = true;
                        p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        p.StartInfo.WorkingDirectory = Paths.PsExecTempDir;
                        p.StartInfo.FileName = Paths.PsExecTempPath;
                        p.StartInfo.Arguments = @"\\" + targetHostname + " -h -u " + SecurityTools.AdminCreds.Domain + @"\" + SecurityTools.AdminCreds.UserName + " -p " + SecurityTools.AdminCreds.Password + " cmd /c " + command;
                        p.Start();

                        cancelSource = new CancellationTokenSource();
                        ReadError(p.StandardError, cancelSource.Token);
                        ReadOutput(p.StandardOutput, cancelSource.Token);

                        p.WaitForExit();

                        return p.ExitCode;
                    }
                });

                return exitCode;
            }
            finally
            {
                StopProcess();
            }
        }

        private async void ReadError(StreamReader stream, CancellationToken cancelToken)
        {
            string outputBuffer = string.Empty;

            using (stream)
            {
                while (!cancelToken.IsCancellationRequested)
                {
                    // Read the stream one character at a time.
                    // The process seems to deadlock if you try to read more than 1 at a time...
                    int bytesToRead = 1;
                    var buffer = new char[bytesToRead];
                    await stream.ReadAsync(buffer, 0, bytesToRead);
                    var output = new string(buffer);
                    outputBuffer += output;

                    // Once we find a NewLine in the buffer, we know we have a complete line.
                    // Remove the NewLine chars, fire the event, then clear the buffer.
                    if (outputBuffer.Contains(Environment.NewLine))
                    {
                        outputBuffer = outputBuffer.Replace(Environment.NewLine, "");
                        outputBuffer = outputBuffer.Replace("\r", "");
                        OnErrorReceived(outputBuffer);
                        outputBuffer = string.Empty;
                    }
                }
            }
        }

        private async void ReadOutput(StreamReader stream, CancellationToken cancelToken)
        {
            string outputString = string.Empty;

            using (stream)
            {
                while (!cancelToken.IsCancellationRequested)
                {
                    // Read the stream one character at a time.
                    // The process seems to deadlock if you try to read more than 1 at a time...
                    int bytesToRead = 1;
                    var buffer = new char[bytesToRead];
                    await stream.ReadAsync(buffer, 0, bytesToRead);
                    var output = new string(buffer);
                    outputString += output;

                    // Once we find a NewLine in the buffer, we know we have a complete line.
                    // Remove the NewLine chars, fire the event, then clear the buffer.
                    if (outputString.Contains(Environment.NewLine))
                    {
                        outputString = outputString.Replace(Environment.NewLine, "");
                        outputString = outputString.Replace("\r", "");
                        OnOutputReceived(outputString);
                        outputString = string.Empty;
                    }
                }
            }
        }

        public void StopProcess()
        {
            cancelSource.Cancel();

            if (currentProcess != null)
            {
                try
                {
                    if (!currentProcess.HasExited)
                    {
                        currentProcess.Kill();

                        // Wait for process to exit.
                        while (!currentProcess.HasExited)
                        {
                            Thread.Sleep(100);
                        }

                        currentProcess.Close();
                        currentProcess = null;
                    }
                }
                catch (InvalidOperationException ioe)
                {
                    // This occures when the process has already exited.
                    if (ioe.HResult == -2146233079)
                    {
                        currentProcess = null;
                    }
                    else
                    {
                        throw ioe;
                    }
                }
                catch (Exception ex)
                {
                    currentProcess = null;
                    throw ex;
                }
            }
        }
    }
}