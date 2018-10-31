using DeploymentAssemblies;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssetManager.Tools.Deployment.XmlParsing
{
    public sealed class AdvancedPsExecCommand : DeployCommand
    {
        private Dictionary<int, ExitCodeResponse> _resultReponses = new Dictionary<int, ExitCodeResponse>();

        public AdvancedPsExecCommand(IDeploymentUI ui, string commandText, string title) : base(ui, commandText, title)
        {
        }

        public AdvancedPsExecCommand(IDeploymentUI ui, string commandText, string title, ExitCodeResponse resultResponse) : base(ui, commandText, title)
        {
            _resultReponses.Add(resultResponse.ExitCode, resultResponse);
        }

        public AdvancedPsExecCommand(IDeploymentUI ui, string commandText, string title, List<ExitCodeResponse> resultResponses) : base(ui, commandText, title)
        {
            resultResponses.ForEach(r => _resultReponses.Add(r.ExitCode, r));
        }

        public async override Task<bool> Execute()
        {
            _deploy.UsePsExec();

            var exitCode = await _deploy.AdvancedPSExecCommand(CommandText, Title);

            if (_resultReponses.ContainsKey(exitCode))
            {
                _resultReponses[exitCode].Prompts.ForEach(p => p.Display());

                return _resultReponses[exitCode].IsSuccess;
            }
            else
            {
                _deploy.LogMessage($@"Failed! Unexpected exit code: { exitCode }", MessageType.Error);
                return false;
            }
        }
    }
}