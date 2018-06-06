using AssetManager.Data;
using AssetManager.Data.Classes;
using AssetManager.Helpers;
using AssetManager.Security;
using AssetManager.Tools.Deployment;
using PingVisualizer;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Forms;
using WNetConnection;
using DeploymentAssemblies;
using System.Reflection;
using System.Linq;

namespace AssetManager.UserInterface.CustomControls
{
    public partial class RemoteToolsControl : UserControl
    {
        #region Fields

        private const int maxFailedPings = 100;
        private int failedPings = 0;
        private int maxFailedToNotify = 5;
        private PingVis pingVis;
        private ExtendedForm hostForm;
        private Device device;
        private Color successColor = Color.DarkGreen;
        private Color failColor = Color.DarkRed;

        /// <summary>
        /// Gets or sets the number of failed pings required before a <see cref="HostBackOnline"/> event will be fired.
        /// </summary>
        public int MaxFailedUntilNotify
        {
            get
            {
                return maxFailedToNotify;
            }

            set
            {
                maxFailedToNotify = value;
            }
        }

        public Device Device
        {
            get
            {
                return this.device;
            }
            set
            {
                this.device = value;
                InitPingVis();
            }
        }

        /// <summary>
        /// Occurs when a successful ping is received after the number of failed pings has exceeded <see cref="MaxFailedUntilNotify"/>
        /// </summary>
        [Browsable(true)]
        public event EventHandler HostBackOnline;

        [Browsable(true)]
        public event UserPromptEventHandler NewStatusPrompt;
        public delegate void UserPromptEventHandler(object sender, UserPromptEventArgs e);

        [SuppressMessage("Microsoft.Design", "CA1009")]
        [Browsable(true)]
        public event EventHandler<bool> VisibleChanging;

        [SuppressMessage("Microsoft.Design", "CA1009")]
        [Browsable(true)]
        public event EventHandler<bool> HostOnlineStatus;


        #endregion Fields

        #region Constructors

        public RemoteToolsControl()
        {
            InitializeComponent();
            ToolsLayoutPanel.DoubleBuffered(true);
            ToolsGroupBox.DoubleBuffered(true);
            InitPingVis();
        }

        #endregion Constructors

        #region Properties

        private void OnStatusPrompt(string message, int displayTime = -1)
        {
            NewStatusPrompt(this, new UserPromptEventArgs(message, displayTime));
        }

        private void OnStatusPrompt(string message, Color color, int displayTime = -1)
        {
            NewStatusPrompt(this, new UserPromptEventArgs(message, color, displayTime));
        }

        private void OnVisibleChanging(bool newState)
        {
            VisibleChanging(this, newState);
        }

        private void OnHostOnlineStatus(bool isOnline)
        {
            HostOnlineStatus(this, isOnline);
        }

        private void OnHostBackOnline()
        {
            HostBackOnline(this, new EventArgs());
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

        private void PingVis_NewPingResult(object sender, PingVis.PingEventArgs e)
        {
            //InitPingVis();
            SetupNetTools(e.PingReply);
        }

        private void InitPingVis()
        {
            if (this.device == null) return;

            var osName = Attributes.DeviceAttributes.OSType[this.device.OSVersion].DisplayValue.ToUpper();

            if (osName.Contains("WIN"))
            {
                if (ReferenceEquals(pingVis, null))
                {
                    pingVis = new PingVis(ShowIPButton, this.device.HostName + "." + NetworkInfo.CurrentDomain);
                    pingVis.NewPingResult -= PingVis_NewPingResult;
                    pingVis.NewPingResult += PingVis_NewPingResult;
                }
            }
        }

        private void SetupNetTools(PingVis.PingInfo PingResults)
        {
            if (PingResults.Status == IPStatus.Success)
            {
                if (failedPings >= maxFailedToNotify)
                {
                    OnHostBackOnline();
                }

                failedPings = 0;
                OnHostOnlineStatus(true);
            }
            else
            {
                failedPings++;
                if (failedPings > maxFailedPings) failedPings = maxFailedPings;
                OnHostOnlineStatus(false);
            }

            if (!this.Visible && PingResults.Status == IPStatus.Success)
            {
                OnVisibleChanging(true);
                this.Visible = true;
            }

            if (failedPings >= maxFailedPings && this.Visible)
            {
                OnVisibleChanging(false);
                this.Visible = false;
            }
        }

        private void EventViewer()
        {
            CheckRemoteAccess();

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
                    p.StartInfo.WorkingDirectory = Environment.SystemDirectory;
                    p.StartInfo.FileName = "cmd.exe";
                    p.StartInfo.Arguments = "/c " + Environment.SystemDirectory + @"\eventvwr.exe " + this.device.HostName;
                    p.Start();
                }
            }
        }

        private async void BrowseFiles()
        {
            try
            {
                CheckRemoteAccess();

                if (SecurityTools.VerifyAdminCreds())
                {
                    string fullPath = "\\\\" + this.device.HostName + "\\c$";
                    await Task.Run(() =>
                    {
                        using (var netCon = new NetworkConnection(fullPath, SecurityTools.AdminCreds))
                        using (var p = new Process())
                        {
                            p.StartInfo.UseShellExecute = false;
                            p.StartInfo.RedirectStandardOutput = true;
                            p.StartInfo.RedirectStandardError = true;
                            p.StartInfo.FileName = "explorer.exe";
                            p.StartInfo.Arguments = fullPath;
                            p.Start();
                            p.WaitForExit();
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        private async void DeployTeamViewer(Device targetDevice)
        {
            CheckRemoteAccess();

            if (OtherFunctions.Message("Deploy TeamViewer to this device?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, "Are you sure?", hostForm) != DialogResult.Yes)
            {
                return;
            }
            try
            {
                if (SecurityTools.VerifyAdminCreds("For remote runspace access."))
                {
                    var newTVDeploy = new DeployTeamViewer(hostForm, targetDevice);
                    OnStatusPrompt("Deploying TeamViewer...", 0);
                    if (await newTVDeploy.DeployToDevice(targetDevice))
                    {
                        OnStatusPrompt("TeamViewer deployment complete!", successColor);
                    }
                    else
                    {
                        OnStatusPrompt("TeamViewer deployment failed...", failColor);
                    }
                }
            }
            catch (Exception ex)
            {
                OnStatusPrompt("TeamViewer deployment failed...", failColor);
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        private async void DeployOffice(Device targetDevice)
        {
            CheckRemoteAccess();

            if (OtherFunctions.Message("Deploy Office 365 to this device?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, "Are you sure?", hostForm) != DialogResult.Yes)
            {
                return;
            }
            try
            {
                if (SecurityTools.VerifyAdminCreds("For remote runspace access."))
                {
                    var newOfficeDeploy = new DeployOffice(hostForm, targetDevice);
                    OnStatusPrompt("Deploying Office 365...", 0);
                    if (await newOfficeDeploy.DeployToDevice(targetDevice))
                    {
                        OnStatusPrompt("Office 365 deployment complete!", successColor);
                    }
                    else
                    {
                        OnStatusPrompt("Office 365 deployment failed...", failColor);
                    }
                }
            }
            catch (Exception ex)
            {
                OnStatusPrompt("Office 365 deployment failed...", failColor);
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        private async void NewSoftwareDeployment(Device targetDevice)
        {
            CheckRemoteAccess();

            if (OtherFunctions.Message("Start new software deployment?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, "Are you sure?", hostForm) != DialogResult.Yes)
            {
                return;
            }
            try
            {
                if (SecurityTools.VerifyAdminCreds("For remote runspace access."))
                {
                    var newDeviceDeploy = new SoftwareDeployment(hostForm, targetDevice);
                    OnStatusPrompt("Deploying Software...", 0);
                    if (await newDeviceDeploy.DeployToDevice(targetDevice))
                    {
                        OnStatusPrompt("Software Deployment Complete!", successColor);
                    }
                    else
                    {
                        OnStatusPrompt("Software Deployment Failed...", failColor);
                    }
                }
            }
            catch (Exception ex)
            {
                OnStatusPrompt("Software Deployment Failed...", failColor);
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        private void LaunchRDP()
        {
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = "mstsc.exe";
            startInfo.Arguments = "/v:" + this.device.HostName;
            Process.Start(startInfo);
        }

        private void QueueGKUpdate()
        {
            CheckRemoteAccess();

            if (SecurityTools.VerifyAdminCreds())
            {
                var gkInstance = Helpers.ChildFormControl.GKUpdaterInstance();
                gkInstance.AddUpdate(this.device);
                if (!gkInstance.Visible)
                {
                    gkInstance.Show();
                }
            }
        }

        private async void RestartDevice()
        {
            CheckRemoteAccess();

            var blah = OtherFunctions.Message("Click 'Yes' to reboot this Device.", MessageBoxButtons.YesNo, MessageBoxIcon.Question, "Are you sure?", hostForm);
            if (blah == DialogResult.Yes)
            {
                if (SecurityTools.VerifyAdminCreds())
                {
                    string ip = pingVis.CurrentResult.Address.ToString();
                    var restartOutput = await SendRestart(ip);
                    if ((string)restartOutput == "")
                    {
                        OnStatusPrompt("Restart Command Successful!", successColor);
                    }
                    else
                    {
                        OtherFunctions.Message("Failed" + "\r\n" + "\r\n" + "Output: " + restartOutput, MessageBoxButtons.OK, MessageBoxIcon.Information, "Restart Device", hostForm);
                    }
                }
            }
        }

        private async Task<string> SendRestart(string ip)
        {
            var origButtonImage = RestartDeviceButton.Image;
            try
            {
                RestartDeviceButton.Image = Properties.Resources.LoadingAni;
                string FullPath = "\\\\" + ip;
                string output = await Task.Run(() =>
                {
                    using (var netCon = new NetworkConnection(FullPath, SecurityTools.AdminCreds))
                    using (var p = new Process())
                    {
                        string results;
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.CreateNoWindow = true;
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
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            finally
            {
                RestartDeviceButton.Image = origButtonImage;
            }
            return string.Empty;
        }

        private void StartPowerShellSession(Device targetDevice)
        {
            CheckRemoteAccess();

            if (SecurityTools.VerifyAdminCreds())
            {
                using (var p = new Process())
                {
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = false;
                    p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    p.StartInfo.FileName = "PowerShell.exe";

                    string domainUsername = SecurityTools.AdminCreds.Domain + @"\" + SecurityTools.AdminCreds.UserName;
                    string cmdArgs = @"-NoExit -Command ""& { ";
                    cmdArgs += @"$User = '" + domainUsername + "'; ";
                    cmdArgs += @"$PWord = ConvertTo-SecureString -String '" + SecurityTools.AdminCreds.Password + "' -AsPlainText -Force; ";
                    cmdArgs += @"$Creds = New-Object -TypeName 'System.Management.Automation.PSCredential' -ArgumentList $User,$PWord; ";
                    cmdArgs += @"Enter-PSSession -ComputerName " + targetDevice.HostName + " –Credential $Creds; }";

                    p.StartInfo.Arguments = cmdArgs;

                    p.Start();
                }
            }
        }

        private void StartPsExecWindow(Device targetDevice)
        {
            CheckRemoteAccess();

            if (SecurityTools.VerifyAdminCreds())
            {

                var currentInstance = ChildFormControl.FindChildOfType(hostForm, typeof(PSExecCommandForm));

                if (currentInstance == null)
                {
                    new PSExecCommandForm(hostForm, targetDevice);
                }
                else
                {
                    currentInstance.RestoreWindow();
                }
            }
        }

        private void ShowIP()
        {
            if (pingVis.CurrentResult != null)
            {
                string address = pingVis.CurrentResult.Address.ToString();
                var blah = OtherFunctions.Message(address + " - " + NetworkInfo.LocationOfIP(address) + "\r\n" + "\r\n" + "Press 'Yes' to copy to clipboard.", MessageBoxButtons.YesNo, MessageBoxIcon.Information, "IP Address", hostForm);
                if (blah == DialogResult.Yes)
                {
                    Clipboard.SetText(address);
                }
            }
        }

        private async void UpdateChrome(Device targetDevice)
        {
            //CheckRemoteAccess();

            //if (OtherFunctions.Message("Update/Install Chrome on this device?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, "Are you sure?", hostForm) != DialogResult.Yes)
            //{
            //    return;
            //}
            //try
            //{
            //    if (SecurityTools.VerifyAdminCreds("For remote runspace access."))
            //    {
            //        OnStatusPrompt("Installing Chrome...", 0);
            //        var newChromeDeploy = new DeployChrome(hostForm, targetDevice);
            //        if (await newChromeDeploy.DeployToDevice())
            //        {
            //            OnStatusPrompt("Chrome install complete!", successColor);
            //        }
            //        else
            //        {
            //            OnStatusPrompt("Error while installing Chrome!", failColor);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    OnStatusPrompt("Error while installing Chrome!", failColor);
            //    ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            //}
            if (SecurityTools.VerifyAdminCreds("For remote runspace access."))
            {

                //var domain = AppDomain.CreateDomain("ChromeDeploy");
                //var path = domain.BaseDirectory + @"DeployTest.dll";
                //var runnable = domain.CreateInstanceFromAndUnwrap(path, "DeployTest.DeployChrome") as IDeployment;

                //var asm = Assembly.LoadFile(@"\\core.co.fairfield.oh.us\dfs1\fcdd\files\QA\Asset Management\Asset Manager\DeploymentModules\DeployTest.dll");//@"C:\GitHub\DeployTest\DeployTest\bin\Release\DeployTest.dll");
                var asm = Assembly.LoadFile(@"C:\GitHub\AssetManagerCSharp\GatekeeperModule\bin\Debug\GatekeeperModule.dll");//@"C:\GitHub\DeployTest\DeployTest\bin\Release\DeployTest.dll");

                var types = asm.DefinedTypes.ToArray();

                var type = asm.GetType(types[0].FullName);
                var runnable = Activator.CreateInstance(type) as IDeployment;

                if (runnable != null)
                {
                    var deploy = new DeploymentUI(hostForm, targetDevice);

                    runnable.InitUI(deploy);

                    if (await runnable.DeployToDevice())
                    {
                        OnStatusPrompt("Chrome install complete!", successColor);
                    }
                    else
                    {
                        OnStatusPrompt("Error while installing Chrome!", failColor);
                    }


                }
            }




        }

        private void CheckRemoteAccess()
        {
            SecurityTools.CheckForAccess(SecurityGroups.RemoteAccess);
        }

        #endregion Methods

        #region Control Events

        private void RemoteToolsControl_VisibleChanged(object sender, EventArgs e)
        {
            // If control is hidden, clear the pingvis results.
            if (!this.Visible)
            {
                if (pingVis != null)
                {
                    pingVis.ClearResults();
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

        private void NewDeployButton_Click(object sender, EventArgs e)
        {
            NewSoftwareDeployment(this.device);
        }

        private void PowerShellButton_Click(object sender, EventArgs e)
        {
            StartPowerShellSession(this.device);
        }

        private void PSExecButton_Click(object sender, EventArgs e)
        {
            StartPsExecWindow(this.Device);
        }

        #endregion Control Events


    }
}