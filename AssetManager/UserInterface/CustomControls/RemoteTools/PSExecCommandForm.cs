using AssetManager.Data;
using AssetManager.Data.Classes;
using AssetManager.Helpers;
using AssetManager.Tools;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace AssetManager.UserInterface.CustomControls
{
    /// <summary>
    /// Form and methods for sending manual PSExec commands to a device.
    /// </summary>
    public partial class PSExecCommandForm : ExtendedForm
    {
        private Device targetDevice;
        private PSExecWrapper pSExecWrapper;

        public PSExecCommandForm(ExtendedForm parentForm, Device device) : base(parentForm)
        {
            InitializeComponent();
            this.DisableDoubleBuffering();
            targetDevice = device;
            pSExecWrapper = new PSExecWrapper();
            pSExecWrapper.ErrorReceived += PSExecWrapper_ErrorReceived;
            pSExecWrapper.OutputReceived += PSExecWrapper_OutputReceived;
            this.Text += " - " + device.CurrentUser;
            this.Show();
        }

        /// <summary>
        /// Executes the current command and displays the output.
        /// </summary>
        private async void ExecuteCommand()
        {
            var command = GetCommand();
            bool runAsAdmin = RunAsAdminCheckBox.Checked;

            LogMessage("Command: " + command);

            try
            {
                // Execute the command and catch exceptions.
                StatusMessage("Executing command...");
                var exitCode = await pSExecWrapper.ExecuteRemoteCommand(targetDevice, command, runAsAdmin);
            }
            catch (InvalidOperationException ioe)
            {
                // This occures when the process has been killed and closed.
                if (ioe.HResult == -2146233079)
                {
                    LogMessage("The process has exited.");
                }
                else
                {
                    LogMessage("Error: " + ioe.Message);
                }
            }
            catch (Exception ex)
            {
                // Log all other errors.
                LogMessage("Error: " + ex.Message);
            }
            finally
            {
                StatusMessage("Idle");
            }
        }

        /// <summary>
        /// Builds and returns the final command from user input.
        /// </summary>
        /// <returns></returns>
        private string GetCommand()
        {
            // Collect the command and command prefix.
            string command = CommandBox.Text.Trim();
            string prefix = PrefixComboBox.SelectedItem?.ToString().Trim();
            string finalCommand;

            // Clear the command control.
            CommandBox.Text = string.Empty;

            // Add new unique commands to the combo items for later convenience.
            if (!CommandBox.Items.Contains(command) && !string.IsNullOrEmpty(command))
            {
                CommandBox.Items.Add(command);
            }

            // Add prefix to the final command if one is selected.
            if (!string.IsNullOrEmpty(prefix))
            {
                finalCommand = prefix + " " + command;
            }
            else
            {
                finalCommand = command;
            }

            return finalCommand;
        }

        private void StopProcess()
        {
            StatusMessage("Stopping process...");
            pSExecWrapper.StopProcess();
            StatusMessage("Idle");
        }

        private void PSExecWrapper_OutputReceived(object sender, EventArgs e)
        {
            var args = (DataReceivedEventArgs)e;
            var dataString = DataConsistency.CleanDBValue(args.Data).ToString();
            LogMessage(dataString);
        }

        private void PSExecWrapper_ErrorReceived(object sender, EventArgs e)
        {
            var args = (DataReceivedEventArgs)e;
            var dataString = DataConsistency.CleanDBValue(args.Data).ToString();
            LogMessage(dataString);
        }

        private void StatusMessage(string text)
        {
            StatusLabel.Text = text;
            StatusLabel.Invalidate();
        }

        public override bool OkToClose()
        {
            bool canClose = true;

            if (pSExecWrapper.CurrentProcess != null)
            {
                var blah = OtherFunctions.Message("A process or command is still active. Are you sure you want to close this window?", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, "Command Still Running", this);
                if (blah != DialogResult.OK)
                {
                    canClose = false;
                }
                else
                {
                    StopProcess();
                }
            }

            return canClose;
        }

        private void LogMessage(string text)
        {
            if (!LogTextBox.IsDisposed)
            {
                if (LogTextBox.InvokeRequired)
                {
                    Action d = new Action(() => LogMessage(text));
                    LogTextBox.BeginInvoke(d);
                }
                else
                {
                    LogTextBox.AppendText(text + "\r\n");
                    LogTextBox.SelectionStart = LogTextBox.Text.Length;
                    LogTextBox.ScrollToCaret();
                }
            }
        }

        private void ExecuteButton_Click(object sender, EventArgs e)
        {
            ExecuteCommand();
        }

        private void LogTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.C)
                {
                    StopProcess();
                }
            }
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            StopProcess();
        }

        private void CommandBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.C)
                {
                    StopProcess();
                }
            }
            else
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ExecuteCommand();
                }
            }
        }

        private void CommandBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                CommandBox.SelectionStart = CommandBox.Text.Length;
                CommandBox.SelectionLength = 0;
            }
        }
    }
}