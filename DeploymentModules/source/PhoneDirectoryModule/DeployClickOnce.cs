﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeploymentAssemblies;

namespace ClickOnceModule
{
    public class DeployClickOnce : DeploymentAssemblies.IDeployment
    {
        private DeploymentAssemblies.IDeploymentUI deploy;

        public int DeployOrderPriority
        {
            get
            {
                return 0;
            }
        }

        public string DeploymentName
        {
            get
            {
                return "ClickOnce Prerequisites";
            }
        }

        public void InitUI(IDeploymentUI ui)
        {
            deploy = ui;
            deploy.UsePsExec();
        }

        public async Task<bool> DeployToDevice()
        {
            var filePush = deploy.NewFilePush(deploy.GetString("clickonce_cert"), @"\Temp\Deploy\ClickOnce\");

            deploy.LogMessage("Pushing files...");
            if (await filePush.StartCopy())
            {
                deploy.LogMessage("Push successful!");
                filePush.Dispose();
            }
            else
            {
                deploy.LogMessage("Push failed!");
                filePush.Dispose();
                return false;
            }

            var certCommand = @"cmd /c certutil.exe -addstore root C:\Temp\Deploy\ClickOnce\trust.cer";

            deploy.LogMessage("Command: " + certCommand);
            if (await deploy.SimplePSExecCommand(certCommand, "Certificate Install"))
            {
                deploy.LogMessage("Certificate installed!");
            }
            else
            {
                deploy.LogMessage("Certificate install failed!");
                return false;
            }

            var exitCode = await deploy.AdvancedPSExecCommand(deploy.GetString("clickonce_netruntimeinstall"), ".NET 4.7.1 Runtime Install");

            if (exitCode == 3010)
            {
                deploy.LogMessage("Install successful. **REBOOT REQUIRED!**");
            }
            else if (exitCode == 0)
            {
                deploy.LogMessage("Install successful!");
            }
            else
            {
                deploy.LogMessage(string.Format("Install failed! (Exit code: {0})", exitCode));
                return false;
            }

            return true;
        }
    }
}
