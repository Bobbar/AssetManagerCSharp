using DeploymentAssemblies;
using System;
using System.Threading.Tasks;

namespace SparkModule
{
    public class DeploySpark : DeploymentAssemblies.IDeployment
    {
        private DeploymentAssemblies.IDeploymentUI deploy;

        public string DeploymentName
        {
            get
            {
                return "Spark Communicator";
            }
        }

        public void InitUI(IDeploymentUI ui)
        {
            deploy = ui;
        }

        public async Task<bool> DeployToDevice()
        {
            try
            {
                await deploy.SimplePSExecCommand(deploy.GetString("spark_install"), "Spark Communicator Install");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}