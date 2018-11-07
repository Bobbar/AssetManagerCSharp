﻿using AdvancedDialog;
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
using AssetManager.Tools.Deployment.XmlParsing;
using System.Diagnostics;

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

        private async Task<List<TaskInfo>> GetScripts()
        {
            int depCount = 0;
            var loadTimer = new Stopwatch();
            var taskList = new List<TaskInfo>();

            deploy.LogMessage("Loading deployment scripts...");
            deploy.LogMessage("Path: " + Paths.DeploymentScripts);

            loadTimer.Restart();

            await Task.Run(() =>
            {
                var files = Directory.GetFiles(Paths.DeploymentScripts, "*.xml");
                var readers = new List<DeploymentReader>();

                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);

                    try
                    {
                        var reader = new DeploymentReader(fileInfo.OpenRead(), deploy);
                        readers.Add(reader);

                        deploy.LogMessage(fileInfo.Name + " - OK", MessageType.Success);

                        depCount++;
                    }
                    catch (Exception ex)
                    {
                        deploy.LogMessage(fileInfo.Name + " - ERROR", MessageType.Error);
                        deploy.LogMessage(ex.ToString(), MessageType.Error);
                    }
                   
                }

                // Sort the module instances by deployment priority.
                readers = readers.OrderBy((r) => r.OrderPriority).ToList();

                // Create new tasks and add them to the collection.
                readers.ForEach((r) => taskList.Add(new TaskInfo(() => r.StartDeployment(), r.DeploymentName)));
            });

            var elapTime = loadTimer.ElapsedMilliseconds;

            deploy.LogMessage(depCount + " scripts loaded in " + elapTime + "ms.");

            return taskList;
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
                newDialog.Height = 500;
                newDialog.Width = 260;

                var selectListBox = new CheckedListBox();
                selectListBox.CheckOnClick = true;
                selectListBox.Size = new System.Drawing.Size(300, 250);
                selectListBox.DisplayMember = nameof(TaskInfo.TaskName);

                var depList = await GetScripts();

                foreach (var d in depList)
                {
                    selectListBox.Items.Add(d, false);
                }
                // Add deployment selection list.
                newDialog.AddCustomControl("TaskList", "Select items to install:", selectListBox);

                // Add a 'Select None' button with lamba action.
                var selectNone = newDialog.AddButton("selectNoneButton", "Select None", () =>
                 {
                     for (int i = 0; i < selectListBox.Items.Count; i++)
                     {
                         selectListBox.SetItemChecked(i, false);
                     }
                 });
                selectNone.Width = 200;

                // Add a 'Select All' button with lamba action.
                var selectAll = newDialog.AddButton("selectAllButton", "Select All", () =>
               {
                   for (int i = 0; i < selectListBox.Items.Count; i++)
                   {
                       selectListBox.SetItemChecked(i, true);
                   }
               });
                selectAll.Width = 200;

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
                deploy.LogMessage("Error: " + ex.Message, MessageType.Error);
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