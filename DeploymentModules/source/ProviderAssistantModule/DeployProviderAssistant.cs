using DeploymentAssemblies;
using System.Threading.Tasks;

namespace ProviderAssistantModule
{
    public class DeployProviderAssistant : IDeployment
    {
        private IDeploymentUI deploy;

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
            deploy.LogMessage("Copying files to target computer...", MessageType.Notice);

            var copyExitCode = await deploy.AdvancedPSExecCommand(deploy.GetString("providerassist_copy"), "Copy Files");

            if (copyExitCode == 0 || copyExitCode == 1)
            {
                deploy.LogMessage("Copy successful!", MessageType.Success);
            }
            else
            {
                deploy.LogMessage("Copy failed!", MessageType.Error);
                return false;
            }

            deploy.LogMessage("Copying shortcut...", MessageType.Notice);

            if (await deploy.SimplePSExecCommand(CopyShortcutCommand(), "Copy Shortcut"))
            {
                deploy.LogMessage("Copy shortcut successful!", MessageType.Success);
            }
            else
            {
                deploy.LogMessage("Copy shortcut failed!", MessageType.Error);
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

        private string ShortcutDirectory()
        {
            return deploy.GetString("providerassist_shortcut_dir");
        }
    }
}