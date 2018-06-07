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
        Form ParentForm { get; }

        string TargetHostname { get; }

        void UsePowerShell();

        void UsePsExec();

        Task<bool> SimplePSExecCommand(string command, string title);

        Task SimplePowerShellScript(byte[] script, string title);

        Task<string> AdvancedPowerShellScript(byte[] script);

        Task<bool> SimplePowerShellCommand(PowerShellCommand command);

        Task<bool> SimplePowerShellCommand(PowerShellCommand[] command);

        string GetString(string name);

        //  void SetTitle(string title);

        void DoneOrError();

        void StartTimer();



        void LogMessage(string message);

        void UserPrompt(string prompt, string title = "");

        ICopyFiles NewFilePush(string source, string destination);// <<

        void Dispose();

    }
}
