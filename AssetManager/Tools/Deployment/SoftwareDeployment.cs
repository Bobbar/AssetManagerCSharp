using AdvancedDialog;
using AssetManager.Data.Classes;
using AssetManager.Helpers;
using AssetManager.UserInterface.CustomControls;
using DeploymentAssemblies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetManager.Tools.Deployment
{
    public class SoftwareDeployment : IDisposable
    {
        private Queue<TaskInfo> deployments = new Queue<TaskInfo>();

        private ExtendedForm parentForm;

        private DeploymentUI deploy;

        public SoftwareDeployment(ExtendedForm parentForm, Device targetDevice)
        {
            this.parentForm = parentForm;
            deploy = new DeploymentUI(parentForm, targetDevice);
            deploy.UsePowerShell();
            deploy.UsePsExec();
        }

        /// <summary>
        /// Container for holding deployment methods and a descriptive name.
        /// </summary>
        private struct TaskInfo
        {
            public Func<Task<bool>> TaskMethod { get; set; }
            public string TaskName { get; set; }

            public TaskInfo(Func<Task<bool>> taskMethod, string taskName)
            {
                TaskMethod = taskMethod;
                TaskName = taskName;
            }
        }

        private async Task<List<TaskInfo>> GetModules()
        {
            int modCount = 0;
            long startTime = DateTime.Now.Ticks;
            var taskList = new List<TaskInfo>();
            var interfaceName = nameof(IDeployment);

            deploy.LogMessage("Loading deployment modules...");

            await Task.Run(() =>
             {
                 VerifyModules();

                 var files = Directory.GetFiles(Paths.LocalModulesStore, "*.dll");
                 var modules = new List<IDeployment>();

                 foreach (var file in files)
                 {
                     var fileInfo = new FileInfo(file);
                     var asm = Assembly.Load(File.ReadAllBytes(fileInfo.FullName));
                     var types = asm.DefinedTypes.ToArray();
                     var firstType = asm.GetType(types[0].FullName);
                     var typeInterface = firstType.GetInterface(interfaceName);

                     // Make sure the assembly type implements the deployment interface.
                     if (typeInterface != null)
                     {
                         var moduleInstance = Activator.CreateInstance(firstType) as IDeployment;

                         // Init and add module instances to a collection.
                         if (moduleInstance != null)
                         {
                             moduleInstance.InitUI(deploy);
                             modules.Add(moduleInstance);
                         }

                         deploy.LogMessage(asm.ManifestModule.ScopeName);

                         modCount++;
                     }
                 }

                 // Sort the module instances by deployment priority.
                 modules = modules.OrderBy((m) => m.DeployOrderPriority).ToList();

                 // Create new tasks and add them to the collection.
                 modules.ForEach((m) => taskList.Add(new TaskInfo(() => m.DeployToDevice(), m.DeploymentName)));
             });

            var elapTime = (DateTime.Now.Ticks - startTime) / 10000;

            deploy.LogMessage(modCount + " modules loaded in " + elapTime + "ms.");

            return taskList;
        }

        /// <summary>
        /// Syncs the local module store with the remote store if possible.
        /// </summary>
        private void VerifyModules()
        {
            if (!Directory.Exists(Paths.LocalModulesStore))
            {
                Directory.CreateDirectory(Paths.LocalModulesStore);
            }

            // Return silently if remote path cannot be reached.
            if (!Directory.Exists(Paths.RemoteModulesStore)) return;

            // Get the list of remote module files.
            var remoteModules = Directory.GetFiles(Paths.RemoteModulesStore, "*.dll");

            // Return silently if no remote modules found.
            if (remoteModules.Length <= 0) return;

            // Iterate through the module files.
            foreach (var module in remoteModules)
            {
                var remoteFile = new FileInfo(module);
                var localFilePath = Paths.LocalModulesStore + remoteFile.Name;

                // Compare local and remote stores and copy missing or mismatched files.
                if (File.Exists(localFilePath))
                {
                    // Compare hashes and replace if needed.
                    var localHash = Security.SecurityTools.GetMD5OfFile(localFilePath);
                    var remoteHash = Security.SecurityTools.GetMD5OfFile(remoteFile.FullName);

                    if (localHash != remoteHash)
                    {
                        File.Delete(localFilePath);
                        File.Copy(remoteFile.FullName, localFilePath);
                    }
                }
                else
                {
                    // Copy from remote to local.
                    File.Copy(remoteFile.FullName, localFilePath);
                }
            }
        }

        /// <summary>
        /// Prompt user for items to be deployed to the device.
        /// </summary>
        /// <param name="targetDevice"></param>
        private async Task ChooseDeployments(Device targetDevice)
        {
            using (var newDialog = new Dialog(parentForm))
            {
                newDialog.Text = "Select Installs";
                newDialog.AutoSize = false;
                newDialog.Height = 400;
                newDialog.Width = 300;

                var selectListBox = new CheckedListBox();
                selectListBox.CheckOnClick = true;
                selectListBox.Size = new System.Drawing.Size(300, 200);
                selectListBox.DisplayMember = nameof(TaskInfo.TaskName);

                var depList = await GetModules();

                foreach (var d in depList)
                {
                    selectListBox.Items.Add(d, true);
                }
                // Add deployment selection list.
                newDialog.AddCustomControl("TaskList", "Select items to install:", selectListBox);

                // Add a 'Select None' button with lamba action.
                newDialog.AddButton("selectNoneButton", "Select None", () =>
                {
                    for (int i = 0; i < selectListBox.Items.Count; i++)
                    {
                        selectListBox.SetItemChecked(i, false);
                    }
                });

                newDialog.ShowDialog();
                if (newDialog.DialogResult == DialogResult.OK)
                {
                    foreach (TaskInfo task in selectListBox.CheckedItems)
                    {
                        deployments.Enqueue(task);
                    }
                }
            }
        }

        public async Task<bool> DeployToDevice(Device targetDevice)
        {
            try
            {
                if (targetDevice != null && !string.IsNullOrEmpty(targetDevice.HostName))
                {
                    deploy.SetTitle(targetDevice.CurrentUser);

                    await ChooseDeployments(targetDevice);

                    deploy.StartTimer();
                    deploy.LogMessage("-------------------");
                    deploy.LogMessage("Starting software deployment to " + targetDevice.HostName);

                    // Run through the queue and invoke the items sequentially.
                    while (deployments.Any())
                    {
                        // Dequeue returns the next method.
                        var d = deployments.Dequeue();

                        deploy.LogMessage("//////    " + d.TaskName + @"    \\\\\\");

                        // Invoke the method and return only on failures.
                        if (!await d.TaskMethod.Invoke())
                        {
                            return false;
                        }

                        deploy.LogMessage(@"\\\\\\    " + d.TaskName + "    //////");
                        if (deployments.Any()) await Task.Delay(4000);
                    }

                    deploy.LogMessage("Software deployment is complete!");
                    deploy.LogMessage("-------------------");
                    return true;
                }
                else
                {
                    OtherFunctions.Message("The target device is null or does not have a hostname.", MessageBoxButtons.OK, MessageBoxIcon.Information, "Missing Info", parentForm);
                }

                return false;
            }
            catch (Exception ex)
            {
                Logging.Logger(ex.ToString());
                deploy.LogMessage("Error: " + ex.Message);
                return false;
            }
            finally
            {
                deploy.DoneOrError();
            }
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    deploy?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable Support
    }
}