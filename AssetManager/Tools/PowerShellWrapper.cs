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
        private string targetHostname;

        public event EventHandler InvocationStateChanged;

        public event EventHandler<string> PowershellOutput;

        protected virtual void OnInvocationStateChanged(PSInvocationStateChangedEventArgs e)
        {
            InvocationStateChanged?.Invoke(this, e);
        }

        protected virtual void OnPowershellOutput(string message)
        {
            PowershellOutput?.Invoke(this, message);
        }

        private PowerShell _currentPowerShellObject;
        private Pipeline _currentPipelineObject;

        public PowerShellWrapper()
        {
        }

        public PowerShellWrapper(string targetHostname)
        {
            this.targetHostname = targetHostname;
        }

        /// <summary>
        /// Execute the specified PowerShell script on the specified host.
        /// </summary>
        /// <param name="scriptValue">PowerShell script as a byte array.</param>
        /// <param name="credentials">Credentials used when creating the remote runspace.</param>
        /// <returns>Returns any error messages.</returns>
        public string ExecuteRemotePSScript(byte[] scriptValue, NetworkCredential credentials)
        {
            try
            {
                var psCreds = new PSCredential(NetworkInfo.CurrentDomain + @"\" + credentials.UserName, credentials.SecurePassword);
                string scriptText = LoadScript(scriptValue);
                string shellUri = "http://schemas.microsoft.com/powershell/Microsoft.PowerShell";
                WSManConnectionInfo connInfo = new WSManConnectionInfo(false, targetHostname, 5985, "/wsman", shellUri, psCreds);

                using (Runspace remoteRunSpace = RunspaceFactory.CreateRunspace(connInfo))
                {
                    remoteRunSpace.Open();
                    using (Pipeline pline = remoteRunSpace.CreatePipeline())
                    {
                        pline.Commands.AddScript(scriptText);
                        pline.Commands.Add("Out-String");

                        _currentPipelineObject = pline;

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

        public string InvokeRemotePSCommand(NetworkCredential credentials, Command pScommand)
        {
            try
            {
                var psCreds = new PSCredential(NetworkInfo.CurrentDomain + @"\" + credentials.UserName, credentials.SecurePassword);

                string shellUri = "http://schemas.microsoft.com/powershell/Microsoft.PowerShell";
                WSManConnectionInfo connInfo = new WSManConnectionInfo(false, targetHostname, 5985, "/wsman", shellUri, psCreds);

                using (Runspace remoteRunSpace = RunspaceFactory.CreateRunspace(connInfo))
                {
                    remoteRunSpace.Open();
                    remoteRunSpace.SessionStateProxy.SetVariable("cred", psCreds);

                    using (var powerSh = PowerShell.Create())
                    {
                        powerSh.Runspace = remoteRunSpace;
                        powerSh.InvocationStateChanged -= Powershell_InvocationStateChanged;
                        powerSh.InvocationStateChanged += Powershell_InvocationStateChanged;
                        powerSh.Commands.AddCommand(pScommand);
                        _currentPowerShellObject = powerSh;
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
            _currentPowerShellObject = session;

            session.InvocationStateChanged -= Powershell_InvocationStateChanged;
            session.InvocationStateChanged += Powershell_InvocationStateChanged;

            //session.Streams.Information.DataAdded -= Information_DataAdded;
            //session.Streams.Information.DataAdded += Information_DataAdded;

            session.Streams.Debug.DataAdded -= Debug_DataAdded;
            session.Streams.Debug.DataAdded += Debug_DataAdded;

            session.Streams.Verbose.DataAdded -= Verbose_DataAdded;
            session.Streams.Verbose.DataAdded += Verbose_DataAdded;

            session.Streams.Error.DataAdded -= Error_DataAdded;
            session.Streams.Error.DataAdded += Error_DataAdded;

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
                    string results = psResults.ToLower();

                    if (results.Contains("success") || results.Contains("true"))
                    {
                        return true;
                    }
                    else if (results.Contains("error") || results.Contains("fail"))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
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
                session.InvocationStateChanged -= Powershell_InvocationStateChanged;
                //session.Streams.Information.DataAdded -= Information_DataAdded;
                session.Streams.Debug.DataAdded -= Debug_DataAdded;
                session.Streams.Verbose.DataAdded -= Verbose_DataAdded;
                session.Streams.Error.DataAdded -= Error_DataAdded;

                session.Runspace.Close();
                session.Runspace.Dispose();
                session.Dispose();
            }
        }

        private void Error_DataAdded(object sender, DataAddedEventArgs e)
        {
            var message = _currentPowerShellObject.Streams.Error[e.Index].Exception.Message;
            OnPowershellOutput($@"Error: {message}");
        }

        private void Verbose_DataAdded(object sender, DataAddedEventArgs e)
        {
            var message = _currentPowerShellObject.Streams.Verbose[e.Index].Message;
            OnPowershellOutput(message);
        }

        private void Debug_DataAdded(object sender, DataAddedEventArgs e)
        {
            var message = _currentPowerShellObject.Streams.Debug[e.Index].Message;
            OnPowershellOutput(message);
        }

        //private void Information_DataAdded(object sender, DataAddedEventArgs e)
        //{
        //    var message = _currentPowerShellObject.Streams.Information[e.Index].MessageData.ToString();
        //    OnPowershellOutput(message);
        //}

        public async Task<PowerShell> GetNewPSSession(NetworkCredential credentials)
        {
            var newPsSession = await Task.Run(() =>
            {
                var psCreds = new PSCredential(NetworkInfo.CurrentDomain + @"\" + credentials.UserName, credentials.SecurePassword);
                string shellUri = "http://schemas.microsoft.com/powershell/Microsoft.PowerShell";

                WSManConnectionInfo connInfo = new WSManConnectionInfo(false, targetHostname, 5985, "/wsman", shellUri, psCreds);

                Runspace remoteRunSpace = RunspaceFactory.CreateRunspace(connInfo);
                remoteRunSpace.Open();

                var powerSh = PowerShell.Create();
                powerSh.Runspace = remoteRunSpace;
                powerSh.Streams.Error.DataAdded += PSEventHandler;

                return powerSh;
            });

            return newPsSession;
        }

        public async Task<bool> ExecutePowerShellScript(byte[] scriptByte)
        {
            var scriptResult = await Task.Run(() => { return ExecuteRemotePSScript(scriptByte, SecurityTools.AdminCreds); });
            if (!string.IsNullOrEmpty(scriptResult))
            {
                OtherFunctions.Message(scriptResult, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Error Running Script");
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<bool> InvokePowerShellCommand(Command pScommand)
        {
            var scriptResult = await Task.Run(() => { return InvokeRemotePSCommand(SecurityTools.AdminCreds, pScommand); });
            if (!string.IsNullOrEmpty(scriptResult))
            {
                OtherFunctions.Message(scriptResult, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Error Running Script");
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
                if (_currentPowerShellObject != null)
                {
                    _currentPowerShellObject.Stop();
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
                if (_currentPipelineObject != null)
                {
                    _currentPipelineObject.Stop();
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
                using (var memoryStream = new MemoryStream(scriptBytes))
                using (StreamReader sr = new StreamReader(memoryStream, Encoding.ASCII))
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