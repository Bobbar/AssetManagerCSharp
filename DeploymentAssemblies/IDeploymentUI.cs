using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeploymentAssemblies
{
    public interface IDeploymentUI
    {
        Form ParentForm { get; }

        string TargetHostname { get; }

        /// <summary>
        /// Initiate Powershell provider. Call this method prior to using Powershell methods.
        /// </summary>
        void UsePowerShell();

        /// <summary>
        /// Initiate PsExec provider. Call this method prior to using PsExec methods.
        /// </summary>
        void UsePsExec();

        Task<bool> SimplePSExecCommand(string command, string title);

        Task SimplePowerShellScript(byte[] script, string title);

        Task<string> AdvancedPowerShellScript(byte[] script);

        Task<bool> SimplePowerShellCommand(PowerShellCommand command);

        Task<bool> SimplePowerShellCommand(PowerShellCommand[] command);

        string GetString(string name);

        void DoneOrError();

        void StartTimer();

        void LogMessage(string message);

        void UserPrompt(string prompt, string title = "");

        ICopyFiles NewFilePush(string source, string destination);

        void Dispose();
    }
}