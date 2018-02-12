using AssetManager.Data;
using AssetManager.Data.Classes;
using AssetManager.Data.Communications;
using AssetManager.Helpers;
using AssetManager.Security;
using AssetManager.Tools.Deployment;
using PingVisLib;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetManager.UserInterface.CustomControls
{
    public partial class RemoteToolsControl : UserControl
    {
        #region Fields

        private int failedPings = 0;
        private PingVis pingVis;
        private ExtendedForm hostForm;
        private Device device;

        public Device Device
        {
            get
            {
                return this.device;
            }
            set
            {
                this.device = value;
                CheckRDP();
            }
        }

        [Browsable(true)]
        public event EventHandler<StatusPrompt> NewStatusPrompt;

        [Browsable(true)]
        public event EventHandler<bool> VisibleChanging;

        #endregion Fields

        #region Constructors

        public RemoteToolsControl()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Properties

        private void OnStatusPrompt(string message, int displayTime = -1)
        {
            NewStatusPrompt(this, new StatusPrompt(message, displayTime));
        }

        private void OnVisibleChanging(bool newState)
        {
            VisibleChanging(this, newState);
        }

        #endregion Properties

        #region Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                if (pingVis != null) pingVis.Dispose();
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void EventViewer()
        {
            if (SecurityTools.VerifyAdminCreds())
            {
                using (var p = new Process())
                {
                    p.StartInfo.Domain = SecurityTools.AdminCreds.Domain;
                    p.StartInfo.UserName = SecurityTools.AdminCreds.UserName;
                    p.StartInfo.Password = SecurityTools.AdminCreds.SecurePassword;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    p.StartInfo.FileName = "cmd.exe";
                    p.StartInfo.Arguments = "/c eventvwr.exe " + this.device.HostName;
                    p.Start();
                }
            }
        }

        private async void BrowseFiles()
        {
            try
            {
                if (SecurityTools.VerifyAdminCreds())
                {
                    string FullPath = "\\\\" + this.device.HostName + "\\c$";
                    await Task.Run(() =>
                    {
                        using (NetworkConnection NetCon = new NetworkConnection(FullPath, SecurityTools.AdminCreds))
                        using (Process p = new Process())
                        {
                            p.StartInfo.UseShellExecute = false;
                            p.StartInfo.RedirectStandardOutput = true;
                            p.StartInfo.RedirectStandardError = true;
                            p.StartInfo.FileName = "explorer.exe";
                            p.StartInfo.Arguments = FullPath;
                            p.Start();
                            p.WaitForExit();
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
        }

        private void CheckRDP()
        {
            if (this.device == null) return;
            try
            {
                if (this.device.OSVersion.Contains("WIN"))
                {
                    if (ReferenceEquals(pingVis, null))
                    {
                        pingVis = new PingVis((Control)ShowIPButton, this.device.HostName + "." + NetworkInfo.CurrentDomain);
                    }
                    if (pingVis.CurrentResult != null)
                    {
                        SetupNetTools(pingVis.CurrentResult);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
        }

        private async void DeployTeamViewer(Device device)
        {
            SecurityTools.CheckForAccess(SecurityTools.AccessGroup.IsAdmin);

            if (OtherFunctions.Message("Deploy TeamViewer to this device?", (int)MessageBoxButtons.YesNo + (int)MessageBoxIcon.Question, "Are you sure?", hostForm) != DialogResult.Yes)
            {
                return;
            }
            try
            {
                if (SecurityTools.VerifyAdminCreds("For remote runspace access."))
                {
                    DeployTeamViewer newTVDeploy = new DeployTeamViewer(hostForm);

                    OnStatusPrompt("Deploying TeamViewer...", 0);
                    if (await newTVDeploy.DeployToDevice(this.device))
                    {
                        OnStatusPrompt("TeamViewer deployment complete!");
                    }
                    else
                    {
                        OnStatusPrompt("TeamViewer deployment failed...");
                    }
                }
            }
            catch (Exception ex)
            {
                OnStatusPrompt("TeamViewer deployment failed...");
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
        }

        private async void DeployOffice(Device device)
        {
            SecurityTools.CheckForAccess(SecurityTools.AccessGroup.IsAdmin);

            if (OtherFunctions.Message("Deploy Office 365 to this device?", (int)MessageBoxButtons.YesNo + (int)MessageBoxIcon.Question, "Are you sure?", hostForm) != DialogResult.Yes)
            {
                return;
            }
            try
            {
                if (SecurityTools.VerifyAdminCreds("For remote runspace access."))
                {
                    DeployOffice newOfficeDeploy = new DeployOffice(hostForm);

                    OnStatusPrompt("Deploying Office 365...", 0);
                    if (await newOfficeDeploy.DeployToDevice(this.device))
                    {
                        OnStatusPrompt("Office 365 deployment complete!");
                    }
                    else
                    {
                        OnStatusPrompt("Office 365 deployment failed...");
                    }
                }
            }
            catch (Exception ex)
            {
                OnStatusPrompt("Office 365 deployment failed...");
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
        }

        private void LaunchRDP()
        {
            ProcessStartInfo StartInfo = new ProcessStartInfo();
            StartInfo.FileName = "mstsc.exe";
            StartInfo.Arguments = "/v:" + this.device.HostName;
            Process.Start(StartInfo);
        }

        private void QueueGKUpdate()
        {
            if (SecurityTools.VerifyAdminCreds())
            {
                var GKInstance = Helpers.ChildFormControl.GKUpdaterInstance();
                GKInstance.AddUpdate(this.device);
                if (!GKInstance.Visible)
                {
                    GKInstance.Show();
                }
            }
        }

        private async void RestartDevice()
        {
            var blah = OtherFunctions.Message("Click 'Yes' to reboot this Device.", (int)MessageBoxButtons.YesNo + (int)MessageBoxIcon.Question, "Are you sure?", hostForm);
            if (blah == DialogResult.Yes)
            {
                string IP = pingVis.CurrentResult.Address.ToString();
                var RestartOutput = await SendRestart(IP, this.device.HostName);
                if ((string)RestartOutput == "")
                {
                    OtherFunctions.Message("Success", (int)MessageBoxButtons.OK + (int)MessageBoxIcon.Information, "Restart Device", hostForm);
                }
                else
                {
                    OtherFunctions.Message("Failed" + "\r\n" + "\r\n" + "Output: " + RestartOutput, (int)MessageBoxButtons.OK + (int)MessageBoxIcon.Information, "Restart Device", hostForm);
                }
            }
        }

        private async Task<string> SendRestart(string IP, string DeviceName)
        {
            var OrigButtonImage = RestartDeviceButton.Image;
            try
            {
                if (SecurityTools.VerifyAdminCreds())
                {
                    RestartDeviceButton.Image = Properties.Resources.LoadingAni;
                    string FullPath = "\\\\" + IP;
                    string output = await Task.Run(() =>
                    {
                        using (NetworkConnection NetCon = new NetworkConnection(FullPath, SecurityTools.AdminCreds))
                        using (Process p = new Process())
                        {
                            string results;
                            p.StartInfo.UseShellExecute = false;
                            p.StartInfo.RedirectStandardOutput = true;
                            p.StartInfo.RedirectStandardError = true;
                            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                            p.StartInfo.FileName = "shutdown.exe";
                            p.StartInfo.Arguments = "/m " + FullPath + " /f /r /t 0";
                            p.Start();
                            results = p.StandardError.ReadToEnd();
                            p.WaitForExit();
                            return results.Trim();
                        }
                    });
                    return output;
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
            finally
            {
                RestartDeviceButton.Image = OrigButtonImage;
            }
            return string.Empty;
        }

        private void SetupNetTools(PingVis.PingInfo PingResults)
        {
            if (PingResults.Status != IPStatus.Success)
            {
                failedPings++;
            }
            else
            {
                failedPings = 0;
            }

            if (!this.Visible && PingResults.Status == IPStatus.Success)
            {
                ShowIPButton.Tag = PingResults.Address;
                OnVisibleChanging(true);
                this.Visible = true;
            }
            if (failedPings > 60 && this.Visible)
            {
                OnVisibleChanging(false);
                this.Visible = false;
            }

            if (hostForm != null)
            {
                if (PingResults.Status == IPStatus.Success)
                {
                    hostForm.OnOnlineStatusChanged(new ExtendedForm.OnlineStatusChangedEventArgs(true));
                }
                else
                {
                    hostForm.OnOnlineStatusChanged(new ExtendedForm.OnlineStatusChangedEventArgs(false));
                }
            }
        }

        private void ShowIP()
        {
            if (pingVis.CurrentResult != null)
            {
                string IPAddress = pingVis.CurrentResult.Address.ToString();
                var blah = OtherFunctions.Message(IPAddress + " - " + NetworkInfo.LocationOfIP(IPAddress) + "\r\n" + "\r\n" + "Press 'Yes' to copy to clipboard.", (int)MessageBoxButtons.YesNo + (int)MessageBoxIcon.Information, "IP Address", hostForm);
                if (blah == DialogResult.Yes)
                {
                    Clipboard.SetText(IPAddress);
                }
            }
        }

        private async void UpdateChrome(Device device)
        {
            SecurityTools.CheckForAccess(SecurityTools.AccessGroup.IsAdmin);

            if (OtherFunctions.Message("Update/Install Chrome on this device?", (int)MessageBoxButtons.YesNo + (int)MessageBoxIcon.Question, "Are you sure?", hostForm) != DialogResult.Yes)
            {
                return;
            }
            try
            {
                if (SecurityTools.VerifyAdminCreds("For remote runspace access."))
                {
                    OnStatusPrompt("Installing Chrome...", 0);
                    DeployChrome newChromeDeploy = new DeployChrome(hostForm);

                    if (await newChromeDeploy.DeployToDevice(this.device))
                    {
                        OnStatusPrompt("Chrome install complete!");
                    }
                    else
                    {
                        OnStatusPrompt("Error while installing Chrome!");
                    }
                }
            }
            catch (Exception ex)
            {
                OnStatusPrompt("Error while installing Chrome!");
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
        }

        #endregion Methods

        #region Control Events

        private void RemoteToolsControl_VisibleChanged(object sender, EventArgs e)
        {
            // If control is hidden, dispose the pingVis instance to reset the ping result collections.
            if (!this.Visible)
            {
                if (pingVis != null)
                {
                    pingVis.Dispose();
                    pingVis = null;
                }
            }
        }

        private void RemoteToolsControl_Load(object sender, EventArgs e)
        {
            if (this.ParentForm != null) hostForm = (ExtendedForm)this.ParentForm;
        }

        private void BrowseFilesButton_Click(object sender, EventArgs e)
        {
            BrowseFiles();
        }

        private void DeployTVButton_Click(object sender, EventArgs e)
        {
            DeployTeamViewer(this.device);
        }

        private void GKUpdateButton_Click(object sender, EventArgs e)
        {
            QueueGKUpdate();
        }

        private void RemoteToolsTimer_Tick(object sender, EventArgs e)
        {
            CheckRDP();
        }

        private void RestartDeviceButton_Click(object sender, EventArgs e)
        {
            RestartDevice();
        }

        private void ShowIPButton_Click(object sender, EventArgs e)
        {
            ShowIP();
        }

        private void StartRDPButton_Click(object sender, EventArgs e)
        {
            LaunchRDP();
        }

        private void UpdateChromeButton_Click(object sender, EventArgs e)
        {
            UpdateChrome(this.device);
        }

        private void EventViewerButton_Click(object sender, EventArgs e)
        {
            EventViewer();
        }

        private void DeployOfficeButton_Click(object sender, EventArgs e)
        {
            DeployOffice(this.device);
        }

        #endregion Control Events

        public class StatusPrompt
        {
            public string Message { get; set; }
            public int DisplayTime { get; set; }

            public StatusPrompt(string message, int displayTime)
            {
                Message = message;
                DisplayTime = displayTime;
            }
        }
    }
}