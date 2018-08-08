using AssetManager.Data;
using AssetManager.Data.Classes;
using AssetManager.Helpers;
using AssetManager.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
        private List<string> previousCommands = new List<string>();
        private int currentCommandIdx = 0;

        public PSExecCommandForm(ExtendedForm parentForm, Device device) : base(parentForm)
        {
            InitializeComponent();
            this.DisableDoubleBuffering();
            targetDevice = device;
            pSExecWrapper = new PSExecWrapper(device.HostName);
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

            LogMessage("----> Command: " + command, Color.DarkGreen);

            try
            {
                // Execute the command and catch exceptions.
                StatusMessage("Executing command...");
                var exitCode = await pSExecWrapper.ExecuteRemoteCommand(command, runAsAdmin);
                LogMessage("Exit code = " + exitCode);
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
                    LogMessage("Error: " + ioe.Message, Color.Red);
                }
            }
            catch (Exception ex)
            {
                // Log all other errors.
                LogMessage("Error: " + ex.Message, Color.Red);
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
            // Trim the command text.
            string command = CommandBox.Text.Trim();

            // Save the command for later convenience.
            SaveCommand(command);

            // Clear the command control.
            CommandBox.Text = string.Empty;

            return command;
        }

        private void SaveCommand(string command)
        {
            if (!string.IsNullOrEmpty(command))
            {
                if (previousCommands.Count == 0 || command != previousCommands[currentCommandIdx])
                {
                    previousCommands.Add(command);
                    currentCommandIdx = previousCommands.Count - 1;
                    CommandBox.DataSource = null;
                    CommandBox.DataSource = previousCommands;
                }
            }
        }

        private void NextCommand()
        {
            if (previousCommands.Count != 0)
            {
                if (string.IsNullOrEmpty(CommandBox.Text.Trim()))
                {
                    SetCurrentCommand(previousCommands[currentCommandIdx]);
                }
                else
                {
                    if (currentCommandIdx + 1 <= previousCommands.Count - 1)
                    {
                        currentCommandIdx++;
                        SetCurrentCommand(previousCommands[currentCommandIdx]);
                    }
                    else
                    {
                        SetCurrentCommand(previousCommands[currentCommandIdx]);
                    }
                }
            }
        }

        private void PrevCommand()
        {
            if (previousCommands.Count != 0)
            {
                if (string.IsNullOrEmpty(CommandBox.Text.Trim()))
                {
                    SetCurrentCommand(previousCommands[currentCommandIdx]);
                }
                else
                {
                    if (currentCommandIdx - 1 >= 0)
                    {
                        currentCommandIdx--;
                        SetCurrentCommand(previousCommands[currentCommandIdx]);
                    }
                    else
                    {
                        SetCurrentCommand(previousCommands[currentCommandIdx]);
                    }
                }
            }
        }

        private void SetCurrentCommand(string command)
        {
            CommandBox.Text = command;
            CommandBox.Select(CommandBox.Text.Length, 0);
        }

        private void StopProcess()
        {
            StatusMessage("Stopping process...");
            pSExecWrapper.StopProcess();
            StatusMessage("Idle");
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

        private void LogMessage(string text, Color color)
        {
            if (!LogTextBox.IsDisposed)
            {
                if (LogTextBox.InvokeRequired)
                {
                    Action d = new Action(() => LogMessage(text, color));
                    LogTextBox.BeginInvoke(d);
                }
                else
                {
                    // Apply color and apped text.
                    LogTextBox.SelectionStart = LogTextBox.TextLength;
                    LogTextBox.SelectionLength = 0;
                    LogTextBox.SelectionColor = color;
                    LogTextBox.AppendText(text + "\r\n");
                    LogTextBox.SelectionColor = LogTextBox.ForeColor;
                    LogTextBox.SelectionStart = LogTextBox.Text.Length;
                    LogTextBox.ScrollToCaret();
                }
            }
        }

        private void PSExecWrapper_OutputReceived(object sender, string data)
        {
            LogMessage(data);
        }

        private void PSExecWrapper_ErrorReceived(object sender, string data)
        {
            // Filter out some of the PSExec messages.
            if (!string.IsNullOrEmpty(data))
            {
                if (!data.ToUpper().Contains("CONNECTING") && !data.ToUpper().Contains("STARTING"))
                {
                    LogMessage(data, Color.Red);
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
                else if (e.KeyCode == Keys.Up)
                {
                    e.Handled = true;

                    PrevCommand();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    e.Handled = true;

                    NextCommand();
                }
            }
        }

        private void CommandBox_DropDownClosed(object sender, EventArgs e)
        {
            this.BeginInvoke(new Action(() => { CommandBox.Select(CommandBox.Text.Length, 0); }));
        }

        private void CommandBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            currentCommandIdx = CommandBox.SelectedIndex;
        }

        private void QuickCommandComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(QuickCommandComboBox.Text.Trim()))
            {
                SetCurrentCommand(QuickCommandComboBox.Text.Trim());
                QuickCommandComboBox.SelectedIndex = 0;
            }
        }
    }
}