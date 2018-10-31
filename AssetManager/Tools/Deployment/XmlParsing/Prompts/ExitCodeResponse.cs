using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Tools.Deployment.XmlParsing
{
    public struct ExitCodeResponse
    {
        public readonly int ExitCode;
        public readonly bool IsSuccess;
        public List<UIPrompt> Prompts;

        public ExitCodeResponse(int exitCode, bool isSuccess)
        {
            ExitCode = exitCode;
            IsSuccess = isSuccess;
            Prompts = null;
        }

        public ExitCodeResponse(int exitCode, bool isSuccess, List<UIPrompt> prompts)
        {
            ExitCode = exitCode;
            IsSuccess = isSuccess;
            Prompts = prompts;
        }

        public ExitCodeResponse(int exitCode, bool isSuccess, UIPrompt prompt)
        {
            ExitCode = exitCode;
            IsSuccess = isSuccess;
            Prompts = new List<UIPrompt>();
            Prompts.Add(prompt);
        }
    }
}
