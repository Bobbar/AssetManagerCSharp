using DeploymentAssemblies;
using System.Threading.Tasks;

namespace ProviderAssistantModule
{
    public class DeployProviderAssistant : DeploymentAssemblies.IDeployment
    {
        private DeploymentAssemblies.IDeploymentUI deploy;

        public string DeploymentName
        {
            get
            {
                return "Provider Assistant";
            }
        }

        public int DeployOrderPriority
        {
            get
            {
                return 0;
            }
        }

        public void InitUI(IDeploymentUI ui)
        {
            deploy = ui;
            deploy.UsePsExec();
        }

        public async Task<bool> DeployToDevice()
        {
            var filePush = deploy.NewFilePush(DeployDirectory(), InstallDirectory());

            deploy.LogMessage("Pushing files to target computer...");

            if (await filePush.StartCopy())
            {
                deploy.LogMessage("Push successful!");
                filePush.Dispose();
            }
            else
            {
                deploy.LogMessage("Push failed!");
                return false;
            }

            deploy.LogMessage("Copying shortcut...");

            if (await deploy.SimplePSExecCommand(CopyShortcutCommand(), "Copy Shortcut"))
            {
                deploy.LogMessage("Copy shortcut successful!");
            }
            else
            {
                deploy.LogMessage("Copy shortcut failed!");
                return false;
            }

            return true;
        }

        private string CopyShortcutCommand()
        {
            var dest = deploy.GetString("providerassist_shortcut_dir");
            var cmd = "xcopy \"C:" + InstallDirectory() + "\\Provider Assistant.lnk\" " + dest + @" /y";

            return cmd;
        }

        private string InstallDirectory()
        {
            return deploy.GetString("providerassist_app_dir");
        }

        private string DeployDirectory()
        {
            return deploy.GetString("providerassist_deploy_dir");
        }

        private string ShortcutDirectory()
        {
            return deploy.GetString("providerassist_shortcut_dir");
        }
    }
}