using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentAssemblies
{
    public interface IDeployment
    {
        /// <summary>
        /// Priority that determines install order. Higher value = lower priority = lower in the order.
        /// </summary>
        int DeployOrderPriority { get; }

        string DeploymentName { get; }

        void InitUI(IDeploymentUI ui);

        Task<bool> DeployToDevice();

    }
}
