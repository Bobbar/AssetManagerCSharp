using AssetManager.Data.Classes;
using AssetManager.Security;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AssetManager.Tools
{
    public class PsExecWrapper
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

        public PsExecWrapper()
        {
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

        public async Task<int> ExecuteRemoteCommand(Device targetDevice, string command)
        {
            int exitCode = -1;

            exitCode = await Task.Run(() =>
              {
                  using (Process p = new Process())
                  {
                      currentProcess = p;

                      if (!SecurityTools.IsAdministrator())
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
                      p.StartInfo.FileName = Paths.PsExecPath;

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

        public void StopProcess()
        {
            if (currentProcess != null)
            {
                currentProcess.Kill();
                currentProcess.Close();
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