using AssetManager.Data.Classes;
using AssetManager.Security;
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

        public Process CurrentProcess
        {
            get
            {
                return currentProcess;
            }
        }

        public event EventHandler ErrorReceived;

        public event EventHandler OutputReceived;

        public PSExecWrapper()
        {
            StageExecutable();
        }

        protected virtual void OnErrorReceived(DataReceivedEventArgs e)
        {
            if (ErrorReceived != null)
            {
                ErrorReceived(this, e);
            }
        }

        protected virtual void OnOutputReceived(DataReceivedEventArgs e)
        {
            if (OutputReceived != null)
            {
                OutputReceived(this, e);
            }
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

        public async Task<int> ExecuteRemoteCommand(Device targetDevice, string command)
        {
            return await ExecuteRemoteCommand(targetDevice, command, !SecurityTools.IsAdministrator());
        }

        public async Task<int> ExecuteRemoteCommand(Device targetDevice, string command, bool runAsAdmin = true)
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

                        if (runAsAdmin)
                        {
                            p.StartInfo.Domain = SecurityTools.AdminCreds.Domain;
                            p.StartInfo.UserName = SecurityTools.AdminCreds.UserName;
                            p.StartInfo.Password = SecurityTools.AdminCreds.SecurePassword;
                        }

                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.RedirectStandardOutput = true;
                        p.StartInfo.RedirectStandardError = true;
                        p.StartInfo.CreateNoWindow = true;
                        p.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                        p.StartInfo.WorkingDirectory = Paths.PsExecTempDir;
                        p.StartInfo.FileName = Paths.PsExecTempPath;

                        p.StartInfo.Arguments = "\\\\" + targetDevice.HostName + " -accepteula -nobanner -h -u " + SecurityTools.AdminCreds.Domain + "\\" + SecurityTools.AdminCreds.UserName + " -p " + SecurityTools.AdminCreds.Password + " " + command;

                        p.OutputDataReceived += P_OutputDataReceived;
                        p.ErrorDataReceived += P_ErrorDataReceived;

                        p.Start();

                        p.BeginOutputReadLine();
                        p.BeginErrorReadLine();

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

        public void StopProcess()
        {
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

        private void P_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            OnErrorReceived(e);
        }

        private void P_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            OnOutputReceived(e);
        }
    }
}