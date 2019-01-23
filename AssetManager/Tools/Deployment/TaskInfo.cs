using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Tools.Deployment
{
    /// <summary>
    /// Container for holding deployment methods and a descriptive name.
    /// </summary>
    public struct TaskInfo
    {
        public Func<Task<bool>> TaskMethod { get; set; }
        public string TaskName { get; set; }
        public string Description { get; set; }

        public TaskInfo(Func<Task<bool>> taskMethod, string taskName)
        {
            TaskMethod = taskMethod;
            TaskName = taskName;
            Description = string.Empty;
        }

        public TaskInfo(Func<Task<bool>> taskMethod, string taskName, string description)
        {
            TaskMethod = taskMethod;
            TaskName = taskName;
            Description = description;
        }
    }
}
