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

        /// <summary>
        /// A user friendly name to be displayed to users.
        /// </summary>
        string DeploymentName { get; }

        /// <summary>
        /// This method is called when an instance of this implementor is created and prepared for use.
        /// </summary>
        /// <param name="ui"></param>
        void InitUI(IDeploymentUI ui);

        /// <summary>
        /// The main deployment method for all deployment logic.
        /// </summary>
        /// <returns>True if the deployment was successful.</returns>
        Task<bool> DeployToDevice();

    }
}
