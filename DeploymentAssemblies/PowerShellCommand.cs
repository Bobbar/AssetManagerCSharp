using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentAssemblies
{
    public class PowerShellCommand
    {
        public string CommandText { get; private set; }

        public ParameterCollection Parameters;
       

        public PowerShellCommand(string command)
        {
            CommandText = command;
            Parameters = new ParameterCollection();
        }
               
    }

    public class ParameterCollection : IEnumerable<CommandParameter>
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

        public IEnumerator<CommandParameter> GetEnumerator()
        {
            return parameters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
           return GetEnumerator();
        }
    }

    public class CommandParameter
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
