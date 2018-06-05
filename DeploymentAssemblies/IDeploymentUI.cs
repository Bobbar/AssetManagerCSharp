using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeploymentAssemblies
{
    public interface IDeploymentUI
    {
        string TargetHostname { get; }

        void UsePowerShell();

        void UsePsExec();

        Task SimplePSExecCommand(string command, string title);

        Task SimplePowerShellScript(byte[] script, string title);

        Task<bool> SimplePowershellCommand(PowerShellCommand command);

        string GetString(string name);

        //  void SetTitle(string title);

        void DoneOrError();

        void StartTimer();



        void LogMessage(string message);

        void UserPrompt(string prompt, string title);

        ICopyFiles NewFilePush(string source, string destination);// <<

        void Dispose();

    }
}
