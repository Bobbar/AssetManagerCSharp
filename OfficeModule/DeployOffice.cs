using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeploymentAssemblies;

namespace OfficeModule
{
    public class DeployOffice : DeploymentAssemblies.IDeployment
    {
        private DeploymentAssemblies.IDeploymentUI deploy;

        private string deployFilesDirectory;
        private string deployTempDirectory;
        private string fullDeployTempDir;
        private string removeOfficeScriptPath;

        public string DeploymentName
        {
            get
            {
                return "Office 365 Install";
            }
        }

        public void InitUI(IDeploymentUI ui)
        {
            deploy = ui;
            deploy.UsePowerShell();
            deploy.UsePsExec();
        }


        public Task<bool> DeployToDevice()
        {


            // HERE: 






            return null;
        }

        private void GetDirectories()
        {
            deployFilesDirectory = deploy.GetString("office_deploy_dir");
            deployTempDirectory = deploy.GetString("office_temp_dir");
            fullDeployTempDir = "C:" + deployTempDirectory;
            removeOfficeScriptPath = fullDeployTempDir + deploy.GetString("office_remove_script_dir");
        }

    }
}
