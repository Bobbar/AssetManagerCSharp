using AssetManager.Data;
using AssetManager.Helpers;
using AssetManager.Security;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Remoting;
using System.Management.Automation.Runspaces;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetManager.Tools
{
    public class PowerShellWrapper
    {

        public event EventHandler InvocationStateChanged;

        protected virtual void OnInvocationStateChanged(PSInvocationStateChangedEventArgs e)
        {
            if (InvocationStateChanged != null)
            {
                InvocationStateChanged(this, e);
            }
        }

        private PowerShell CurrentPowerShellObject;

        private Pipeline CurrentPipelineObject;
        /// <summary>
        /// Execute the specified PowerShell script on the specified host.
        /// </summary>
        /// <param name="hostname">Hostname of the remote computer.</param>
        /// <param name="scriptBytes">PowerShell script as a byte array.</param>
        /// <param name="credentials">Credentials used when creating the remote runspace.</param>
        /// <returns>Returns any error messages.</returns>
        public string ExecuteRemotePSScript(string hostname, byte[] scriptBytes, NetworkCredential credentials)
        {
            try
            {
                var psCreds = new PSCredential(credentials.UserName, credentials.SecurePassword);
                string scriptText = LoadScript(scriptBytes);
                string shellUri = "http://schemas.microsoft.com/powershell/Microsoft.PowerShell";
                WSManConnectionInfo connInfo = new WSManConnectionInfo(false, hostname, 5985, "/wsman", shellUri, psCreds);

                using (Runspace remoteRunSpace = RunspaceFactory.CreateRunspace(connInfo))
                {
                    remoteRunSpace.Open();
                    using (Pipeline pline = remoteRunSpace.CreatePipeline())
                    {
                        pline.Commands.AddScript(scriptText);
                        pline.Commands.Add("Out-String");

                        CurrentPipelineObject = pline;

                        Collection<PSObject> results = pline.Invoke();
                        StringBuilder stringBuilder = new StringBuilder();

                        foreach (var obj in results)
                        {
                            stringBuilder.AppendLine(obj.ToString());
                        }

                        remoteRunSpace.Close();
                        pline.Stop();


                        return DataConsistency.CleanDBValue((stringBuilder.ToString())).ToString();

                    }

                }
            }
            catch (Exception ex)
            {
                //Check for incorrect username/password error and rethrow a Win32Exception to be caught in the error handler.
                //Makes sure that the global admin credentials variable is cleared so that a new prompt will be shown on the next attempt. See: VerifyAdminCreds method.
                if (ex is PSRemotingTransportException)
                {
                    var transportEx = (PSRemotingTransportException)ex;
                    if (transportEx.ErrorCode == 1326)
                    {
                        throw new Win32Exception(1326);
                    }
                }
                return ex.Message;
            }
        }

        public string InvokeRemotePSCommand(string hostname, NetworkCredential credentials, Command PScommand)
        {
            try
            {
                var psCreds = new PSCredential(credentials.UserName, credentials.SecurePassword);

                string shellUri = "http://schemas.microsoft.com/powershell/Microsoft.PowerShell";
                WSManConnectionInfo connInfo = new WSManConnectionInfo(false, hostname, 5985, "/wsman", shellUri, psCreds);

                using (Runspace remoteRunSpace = RunspaceFactory.CreateRunspace(connInfo))
                {
                    remoteRunSpace.Open();
                    remoteRunSpace.SessionStateProxy.SetVariable("cred", psCreds);

                    using (var powerSh = PowerShell.Create())
                    {
                        powerSh.Runspace = remoteRunSpace;
                        powerSh.InvocationStateChanged -= Powershell_InvocationStateChanged;
                        powerSh.InvocationStateChanged += Powershell_InvocationStateChanged;
                        powerSh.Commands.AddCommand(PScommand);
                        CurrentPowerShellObject = powerSh;
                        Collection<PSObject> results = powerSh.Invoke();

                        StringBuilder stringBuilder = new StringBuilder();

                        foreach (var obj in results)
                        {
                            stringBuilder.AppendLine(obj.ToString());
                        }

                        remoteRunSpace.Close();
                        powerSh.Stop();
                        return DataConsistency.CleanDBValue((stringBuilder.ToString())).ToString();

                    }

                }
            }
            catch (Exception ex)
            {
                //Check for incorrect username/password error and rethrow a Win32Exception to be caught in the error handler.
                //Makes sure that the global admin credentials variable is cleared so that a new prompt will be shown on the next attempt. See: VerifyAdminCreds method.
                if (ex is PSRemotingTransportException)
                {
                    var transportEx = (PSRemotingTransportException)ex;
                    if (transportEx.ErrorCode == 1326)
                    {
                        throw new Win32Exception(1326);
                    }
                }
                return ex.Message;
            }
        }

        public async Task<bool> InvokePowerShellSession(PowerShell session)
        {
            CurrentPowerShellObject = session;
            session.InvocationStateChanged -= Powershell_InvocationStateChanged;
            session.InvocationStateChanged += Powershell_InvocationStateChanged;

            try
            {
                var psResults = await Task.Run(() =>
                {
                    Collection<PSObject> results = session.Invoke();
                    StringBuilder stringBuilder = new StringBuilder();

                    foreach (var obj in results)
                    {
                        stringBuilder.AppendLine(obj.ToString());
                    }
                    return DataConsistency.CleanDBValue((stringBuilder.ToString())).ToString();
                });

                if (!string.IsNullOrEmpty(psResults))
                {
                    OtherFunctions.Message(psResults, (int)MessageBoxButtons.OK + (int)MessageBoxIcon.Exclamation, "Error Running Script");
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                session.Runspace.Close();
                session.Runspace.Dispose();
                session.Dispose();
            }
        }

        public async Task<PowerShell> GetNewPSSession(string hostname, NetworkCredential credentials)
        {
            var newPsSession = await Task.Run(() =>
            {
                var psCreds = new PSCredential(credentials.UserName, credentials.SecurePassword);
                string shellUri = "http://schemas.microsoft.com/powershell/Microsoft.PowerShell";

                WSManConnectionInfo connInfo = new WSManConnectionInfo(false, hostname, 5985, "/wsman", shellUri, psCreds);

                Runspace remoteRunSpace = RunspaceFactory.CreateRunspace(connInfo);
                remoteRunSpace.Open();
                //remoteRunSpace.SessionStateProxy.SetVariable("cred", psCreds);

                var powerSh = PowerShell.Create();
                powerSh.Runspace = remoteRunSpace;
                powerSh.Streams.Error.DataAdded += PSEventHandler;

                return powerSh;
            });

            return newPsSession;
        }

        public async Task<bool> ExecutePowerShellScript(string hostname, byte[] scriptByte)
        {
            var scriptResult = await Task.Run(() => { return ExecuteRemotePSScript(hostname, scriptByte, SecurityTools.AdminCreds); });
            if (!string.IsNullOrEmpty(scriptResult))
            {
                OtherFunctions.Message(scriptResult, (int)MessageBoxButtons.OK + (int)MessageBoxIcon.Exclamation, "Error Running Script");
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<bool> InvokePowerShellCommand(string hostname, Command PScommand)
        {
            var scriptResult = await Task.Run(() => { return InvokeRemotePSCommand(hostname, SecurityTools.AdminCreds, PScommand); });
            if (!string.IsNullOrEmpty(scriptResult))
            {
                OtherFunctions.Message(scriptResult, (int)MessageBoxButtons.OK + (int)MessageBoxIcon.Exclamation, "Error Running Script");
                return false;
            }
            else
            {
                return true;
            }
        }

        private void Powershell_InvocationStateChanged(object sender, PSInvocationStateChangedEventArgs e)
        {
            OnInvocationStateChanged(e);
        }

        public void StopPowerShellCommand()
        {
            try
            {
                if (CurrentPowerShellObject != null)
                {
                    CurrentPowerShellObject.Stop();
                }
            }
            catch
            {
                //don't care about errors here
            }
        }

        public void StopPiplineCommand()
        {
            try
            {
                if (CurrentPipelineObject != null)
                {
                    CurrentPipelineObject.Stop();
                }
            }
            catch
            {
                //don't care about errors here
            }
        }


        private void PSEventHandler(object sender, DataAddedEventArgs e)
        {
            //TODO: Fix or remove this.
            //ErrorRecord newRecord = (PSDataCollection<ErrorRecord>)sender(e.Index);

            //Debug.Print(newRecord.Exception.Message);
        }

        private string LoadScript(byte[] scriptBytes)
        {
            try
            {
                // Create an instance of StreamReader to read from our file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(new MemoryStream(scriptBytes), Encoding.ASCII))
                {
                    // use a string builder to get all our lines from the file
                    StringBuilder fileContents = new StringBuilder();

                    // string to hold the current line
                    string curLine = "";

                    // loop through our file and read each line into our
                    // stringbuilder as we go along
                    do
                    {
                        // read each line and MAKE SURE YOU ADD BACK THE
                        // LINEFEED THAT IT THE ReadLine() METHOD STRIPS OFF
                        curLine = sr.ReadLine();
                        fileContents.Append(curLine + Environment.NewLine);
                    } while (!(curLine == null));

                    // close our reader now that we are done
                    sr.Close();

                    // call RunScript and pass in our file contents
                    // converted to a string
                    return fileContents.ToString();
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                string errorText = "The file could not be read:";
                errorText += e.Message + "\\n";
                return errorText;
            }

        }

    }

}
