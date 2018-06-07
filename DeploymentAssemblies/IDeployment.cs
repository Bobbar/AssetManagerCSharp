using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentAssemblies
{
    public interface IDeployment
    {

        string DeploymentName { get; }

        void InitUI(IDeploymentUI ui);

        Task<bool> DeployToDevice();

    }
}
