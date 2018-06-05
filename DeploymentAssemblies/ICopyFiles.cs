using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentAssemblies
{
    public interface ICopyFiles
    {
        Task<bool> StartCopy();

        void Dispose();
    }
}
