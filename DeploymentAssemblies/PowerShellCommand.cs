using System.Collections;
using System.Collections.Generic;

namespace DeploymentAssemblies
{
    /// <summary>
    /// Wrapper class for System.Management.Automation.Command.
    /// </summary>
    /// <remarks>
    /// This is used because I don't want to have to reference the System.Management.Automation
    /// assembly in the abstracted deployment modules. Problems with that assembly reference
    /// can break deployment, so the fewer references the better. It's also quite large...
    /// </remarks>
    public sealed class PowerShellCommand
    {
        public string CommandText { get; private set; }

        public bool IsScript { get; private set; }

        public bool UseLocalScope { get; private set; }

        public ParameterCollection Parameters;

        public PowerShellCommand(string command) : this(command, false, false)
        {
        }

        public PowerShellCommand(string command, bool isScript) : this(command, isScript, false)
        {
        }

        public PowerShellCommand(string command, bool isScript, bool useLocalScope)
        {
            CommandText = command;
            IsScript = isScript;
            UseLocalScope = useLocalScope;
            Parameters = new ParameterCollection();
        }
    }

    public sealed class ParameterCollection : IEnumerable<CommandParameter>
    {
        public List<CommandParameter> Parameters
        {
            get
            {
                return parameters;
            }
        }

        private List<CommandParameter> parameters;

        public ParameterCollection()
        {
            parameters = new List<CommandParameter>();
        }

        public void Add(string name, object value)
        {
            parameters.Add(new CommandParameter(name, value));
        }

        public void Add(string name)
        {
            parameters.Add(new CommandParameter(name, null));
        }

        public IEnumerator<CommandParameter> GetEnumerator()
        {
            return parameters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public sealed class CommandParameter
    {
        public string Name { get; private set; }
        public object Value { get; private set; }

        public CommandParameter(string name, object value)
        {
            Name = name;
            Value = value;
        }

    }
}