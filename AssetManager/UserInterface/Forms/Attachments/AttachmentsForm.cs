﻿using AdvancedDialog;
using AssetManager.Data;
using AssetManager.Data.Classes;
using AssetManager.Data.Communications;
using AssetManager.Data.Functions;
using AssetManager.Helpers;
using AssetManager.Security;
using AssetManager.UserInterface.CustomControls;
using SimpleProgressCounter;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Databases.Data.Mapping;

namespace AssetManager.UserInterface.Forms
{
    public partial class AttachmentsForm : ExtendedForm
    {
        #region Fields

        /// <summary>
        /// "ftp://ServerIP/attachments/CurrentDB/"
        /// </summary>
        private string ftpUri = "ftp://" + ServerInfo.MySQLServerIP + "/attachments/" + ServerInfo.CurrentDataBase.ToString() + "/";

        private string attachFolderGuid;
        private const short fileSizeMBLimit = 150;
        private AttachmentsBaseCols attachmentColumns;
        private bool allowDrag = true;
        private bool isDragging = false;
        private bool gridFilling = true;
        private CancellationTokenSource taskCancelTokenSource;
        private bool transferTaskRunning = false;
        private DataObject dragDropDataObj = new DataObject();
        private Point mouseStartPos;
        private ProgressCounter progress = new ProgressCounter();
        private Folder previousFolder = new Folder();
        private Folder currentFolder = new Folder();
        private Font filenameFont = new Font("Consolas", 9.75F, FontStyle.Bold);

        public EventHandler AttachCountChanged;

        private Folder CurrentSelectedFolder
        {
            get
            {
                if (FolderListView.SelectedItems.Count > 0)
                {
                    if (FolderListView.SelectedItems[0].Index == 0)
                    {
                        currentFolder = new Folder();
                    }
                    else
                    {
                        currentFolder.FolderName = FolderListView.SelectedItems[0].Text;
                        currentFolder.FolderNameGuid = (string)FolderListView.SelectedItems[0].Tag;
                    }
                    return currentFolder;
                }
                else
                {
                    return currentFolder;
                }
            }

            set
            {
                currentFolder = value;
                SetActiveFolder(value);
            }
        }

        #endregion Fields

        #region Constructors

        public AttachmentsForm(ExtendedForm parentForm, AttachmentsBaseCols attachTable, MappedObject attachDataObject, EventHandler attachCountChangeHandler = null) : base(parentForm, attachDataObject)
        {
            ftpUri = "ftp://" + ServerInfo.MySQLServerIP + "/attachments/" + ServerInfo.CurrentDataBase.ToString() + "/";

            InitializeComponent();
            AttachCountChanged += attachCountChangeHandler;
            AttachGrid.DefaultCellStyle.SelectionBackColor = GridTheme.CellSelectColor;
            AttachGrid.DoubleBuffered(true);
            FolderListView.DoubleBuffered(true);
            SetStatusBarText("Idle...");
            attachmentColumns = attachTable;

            if (attachDataObject != null)
            {
                attachFolderGuid = attachDataObject.Guid;

                if (attachDataObject is SibiRequest)
                {
                    this.Text = "Sibi Attachments";
                    DeviceGroup.Visible = false;
                    SibiGroup.Dock = DockStyle.Top;
                    FillSibiInfo((SibiRequest)attachDataObject);
                }
                else if (attachDataObject is Device)
                {
                    this.Text = "Device Attachments";
                    SibiGroup.Visible = false;
                    DeviceGroup.Dock = DockStyle.Top;
                    FillDeviceInfo((Device)attachDataObject);
                }
                PopulateFolderList();
            }
            else
            {
                SibiGroup.Visible = false;
            }

            if (SecurityTools.CanAccess(SecurityGroups.ManageAttachment))
            {
                UploadButton.Enabled = true;
                DeleteButton.Enabled = true;
            }
            else
            {
                UploadButton.Enabled = false;
                DeleteButton.Enabled = false;
            }
            this.Show();
        }

        #endregion Constructors

        #region Methods

        private void SetActiveFolder(Folder folder)
        {
            if (folder.FolderNameGuid == "")
            {
                FolderListView.Items[0].Selected = true;
            }
            else
            {
                foreach (ListViewItem item in FolderListView.Items)
                {
                    if ((string)item.Tag == folder.FolderNameGuid)
                    {
                        item.Selected = true;
                    }
                }
            }
            SetFolderListViewStates();
        }

        private void FillSibiInfo(SibiRequest request)
        {
            ReqPOTextBox.Text = request.PO;
            ReqNumberTextBox.Text = request.RequisitionNumber;
            RequestNumTextBox.Text = request.RequestNumber;
            DescriptionTextBox.Text = request.Description;
            this.Text += " - " + request.Description;
        }

        private void FillDeviceInfo(Device device)
        {
            AssetTagTextBox.Text = device.AssetTag;
            SerialTextBox.Text = device.Serial;
            DeviceDescTextBox.Text = device.Description;
            this.Text += " - " + device.CurrentUser;
        }

        public bool ActiveTransfer()
        {
            return transferTaskRunning;
        }

        public void CancelTransfers()
        {
            taskCancelTokenSource.Cancel();
            // Block until transfer is complete to make sure any canceled FTP transfers are deleted.
            while (transferTaskRunning)
            {
                Application.DoEvents();
                Task.Delay(150).Wait();
            }
        }

        /// <summary>
        /// Populates the list of attachements.
        /// </summary>
        private void RefreshAttachments()
        {
            Waiting();
            try
            {
                var query = GetSelectQuery();

                using (var results = DBFactory.GetDatabase().DataTableFromQueryString(query))
                {
                    gridFilling = true;
                    AttachGrid.Populate(results, AttachGridColumns(attachmentColumns));
                }

                AttachGrid.Columns[attachmentColumns.FileName].DefaultCellStyle.Font = filenameFont;
                OnAttachCountChanged(new EventArgs());
                AttachGrid.ClearSelection();

                if (this.Visible)
                {
                    gridFilling = false;
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            finally
            {
                DoneWaiting();
            }
        }

        private void SetStatusBarText(string value)
        {
            StatusLabel.Text = value;
            StatusStrip1.Update();
        }

        public override bool OkToClose()
        {
            if (ActiveTransfer())
            {
                this.WindowState = FormWindowState.Normal;
                this.Activate();
                var blah = OtherFunctions.Message("There are active uploads/downloads. Do you wish to cancel the current operation?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, "Worker Busy", this);
                if (blah == DialogResult.Yes)
                {
                    CancelTransfers();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private void UploadFileDialog()
        {
            try
            {
                SecurityTools.CheckForAccess(SecurityGroups.ManageAttachment);

                using (var fd = new OpenFileDialog())
                {
                    fd.ShowHelp = true;
                    fd.Title = "Select File To Upload - " + fileSizeMBLimit.ToString() + "MB Limit";
                    fd.InitialDirectory = "C:\\";
                    fd.Filter = "All files (*.*)|*.*|All files (*.*)|*.*";
                    fd.FilterIndex = 2;
                    fd.Multiselect = true;
                    fd.RestoreDirectory = true;
                    if (fd.ShowDialog() == DialogResult.OK)
                    {
                        UploadFiles(fd.FileNames);
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        /// <summary>
        /// Copies drag-drop file object to a temp path and returns the full path.
        /// </summary>
        /// <param name="dataObject">The drag-drop data object.</param>
        /// <param name="dataFormat">Format of the data to be retrieved from the object.</param>
        /// <returns></returns>
        private string[] CopyAttachement(IDataObject dataObject, string dataFormat)
        {
            string fileName = GetAttachFileName(dataObject, dataFormat);
            string[] fullPath = new string[1];
            fullPath[0] = Paths.DownloadPath + fileName;
            Directory.CreateDirectory(Paths.DownloadPath);
            using (var streamFileData = (MemoryStream)(dataObject.GetData("FileContents")))
            {
                using (var outputStream = File.Create(fullPath[0]))
                {
                    streamFileData.CopyTo(outputStream);
                    return fullPath;
                }
            }
        }

        public override void DoneWaiting()
        {
            base.DoneWaiting();
            SetStatusBarText("Idle...");
        }

        private async Task<Attachment> DownloadAttachment(string attachGuid)
        {
            if (transferTaskRunning)
            {
                return null;
            }

            transferTaskRunning = true;
            Attachment downloadAttachment = new Attachment();

            try
            {
                taskCancelTokenSource = new CancellationTokenSource();
                CancellationToken cancelToken = taskCancelTokenSource.Token;
                SetStatusBarText("Connecting...");
                downloadAttachment = GetSQLAttachment(attachGuid);
                string ftpFullUri = ftpUri + downloadAttachment.ObjectGuid + "/" + attachGuid;
                //get file size
                progress = new ProgressCounter();
                progress.BytesToTransfer = Convert.ToInt32(FtpComms.ReturnFtpResponse(ftpFullUri, WebRequestMethods.Ftp.GetFileSize).ContentLength);
                //setup download
                SetStatusBarText("Downloading...");
                TransferFeedback(true);
                downloadAttachment.DataStream = await Task.Run(() =>
                {
                    using (var respStream = FtpComms.ReturnFtpResponse(ftpFullUri, WebRequestMethods.Ftp.DownloadFile).GetResponseStream())
                    {
                        var memStream = new MemoryTributary();
                        int bufferSize = 256000;
                        byte[] buffer = new byte[bufferSize];
                        int bytesIn = 0;
                        //ftp download
                        bytesIn = 1;
                        while (!(bytesIn < 1 || cancelToken.IsCancellationRequested))
                        {
                            bytesIn = respStream.Read(buffer, 0, bufferSize);
                            if (bytesIn > 0)
                            {
                                memStream.Write(buffer, 0, bytesIn); //download data to memory before saving to disk
                                progress.BytesMoved = bytesIn;
                            }
                        }
                        buffer = null;
                        return memStream;
                    }
                });

                if (!cancelToken.IsCancellationRequested)
                {
                    if (VerifyAttachment(downloadAttachment))
                    {
                        return downloadAttachment;
                    }
                }

                downloadAttachment.Dispose();
                return null;
            }
            catch (Exception ex)
            {
                downloadAttachment?.Dispose();

                throw ex;
            }
            finally
            {
                transferTaskRunning = false;

                if (!GlobalSwitches.ProgramEnding)
                {
                    TransferFeedback(false);
                }
            }
        }

        private void PopulateFolderList()
        {
            var addedFolders = new List<string>();
            string folderQuery = "SELECT " + attachmentColumns.FolderName + ", " + attachmentColumns.FolderNameGuid + " FROM " + attachmentColumns.TableName + " WHERE " + attachmentColumns.FKey + "='" + attachFolderGuid + "' ORDER BY " + attachmentColumns.FolderName;

            using (var folders = DBFactory.GetDatabase().DataTableFromQueryString(folderQuery))
            {
                FolderListView.Items.Clear();
                var allFolderItem = new ListViewItem("*All");
                allFolderItem.StateImageIndex = 1;
                allFolderItem.Selected = true;
                FolderListView.Items.Add(allFolderItem);
                foreach (DataRow row in folders.Rows)
                {
                    string folderName = row[attachmentColumns.FolderName].ToString().Trim();
                    string folderNameGuid = row[attachmentColumns.FolderNameGuid].ToString().Trim();

                    if (!string.IsNullOrEmpty(folderName))
                    {
                        if (!addedFolders.Contains(folderName))
                        {
                            addedFolders.Add(folderName);

                            var newFolderItem = new ListViewItem(folderName);
                            newFolderItem.StateImageIndex = 0;
                            newFolderItem.Selected = false;
                            newFolderItem.Tag = folderNameGuid;
                            FolderListView.Items.Add(newFolderItem);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns the file name from the specified data object of the specified data format.
        /// </summary>
        /// <param name="attachDataObject"></param>
        /// <param name="dataFormat"></param>
        /// <returns></returns>
        private string GetAttachFileName(IDataObject attachDataObject, string dataFormat)
        {
            switch (dataFormat)
            {
                case "RenPrivateItem":
                    using (var streamFileName = (MemoryStream)(attachDataObject.GetData("FileGroupDescriptor")))
                    {
                        streamFileName.Position = 0;
                        using (var sr = new StreamReader(streamFileName))
                        {
                            string fullString = sr.ReadToEnd();
                            fullString = fullString.Replace("\0", "");
                            fullString = fullString.Replace("\u0001", "");
                            return fullString;
                        }
                    }
            }
            return null;
        }

        /// <summary>
        /// Returns a query for the currently selected folder, or all attachments if no folder selected.
        /// </summary>
        /// <returns></returns>
        private string GetSelectQuery()
        {
            string query = "";

            if (FolderListView.SelectedIndices.Count > 0 && FolderListView.SelectedIndices[0] == 0)
            {
                query = "SELECT * FROM " + attachmentColumns.TableName + " WHERE " + attachmentColumns.FKey + "='" + attachFolderGuid + "' ORDER BY " + attachmentColumns.Timestamp + " DESC";
            }
            else
            {
                query = "SELECT * FROM " + attachmentColumns.TableName + " WHERE " + attachmentColumns.FolderNameGuid + "='" + CurrentSelectedFolder.FolderNameGuid + "' AND " + attachmentColumns.FKey + " ='" + attachFolderGuid + "' ORDER BY " + attachmentColumns.Timestamp + " DESC";
            }

            return query;
        }

        private Attachment GetSQLAttachment(string attachGuid)
        {
            string query = "SELECT * FROM " + attachmentColumns.TableName + " WHERE " + attachmentColumns.FileGuid + "='" + attachGuid + "' LIMIT 1";

            using (var results = DBFactory.GetDatabase().DataTableFromQueryString(query))
            {
                if (results.Rows.Count > 0)
                {
                    return new Attachment(results.Rows[0], attachmentColumns);
                }
                else
                {
                    throw new Exception("Error while retrieving Attachment info from database. No rows were returned.");
                }
            }
        }

        private List<GridColumnAttrib> AttachGridColumns(AttachmentsBaseCols attachtable)
        {
            var columnList = new List<GridColumnAttrib>();
            columnList.Add(new GridColumnAttrib(attachtable.FileType, "", ColumnFormatType.Image));
            columnList.Add(new GridColumnAttrib(attachtable.FileName, "Filename"));
            columnList.Add(new GridColumnAttrib(attachtable.FileSize, "Size", ColumnFormatType.FileSize));
            columnList.Add(new GridColumnAttrib(attachtable.Timestamp, "Date"));
            columnList.Add(new GridColumnAttrib(attachtable.FolderName, "Folder"));
            columnList.Add(new GridColumnAttrib(attachtable.FileGuid, "AttachGuid"));
            columnList.Add(new GridColumnAttrib(attachtable.FileHash, "MD5"));
            return columnList;
        }

        private int InsertSQLAttachment(Attachment attachment, DbTransaction transaction)
        {
            var insertParams = new ParamCollection();
            insertParams.Add(attachment.AttachColumns.FKey, attachment.ObjectGuid);
            insertParams.Add(attachment.AttachColumns.FileName, attachment.FileName);
            insertParams.Add(attachment.AttachColumns.FileType, attachment.Extension);
            insertParams.Add(attachment.AttachColumns.FileSize, attachment.Filesize);
            insertParams.Add(attachment.AttachColumns.FileGuid, attachment.FileGuid);
            insertParams.Add(attachment.AttachColumns.FileHash, attachment.FileMD5);
            insertParams.Add(attachment.AttachColumns.FolderName, attachment.FolderInfo.FolderName);
            insertParams.Add(attachment.AttachColumns.FolderNameGuid, attachment.FolderInfo.FolderNameGuid);
            return DBFactory.GetDatabase().InsertFromParameters(attachment.AttachColumns.TableName, insertParams.Parameters, transaction);
        }

        private async Task<bool> MakeDirectory(string folderGuid)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using (var makeDirResponse = (FtpWebResponse)(FtpComms.ReturnFtpResponse(ftpUri + folderGuid, WebRequestMethods.Ftp.MakeDirectory)))
                    {
                        if (makeDirResponse.StatusCode == FtpStatusCode.PathnameCreated)
                        {
                            return true;
                        }
                        return false;
                    }
                }
                catch (WebException ex)
                {
                    if (ex.Response != null)
                    {
                        var resp = (FtpWebResponse)ex.Response;
                        if (resp.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                        {
                            //directory already exists
                            return true;
                        }
                    }
                    return false;
                }
            });
        }

        private bool MouseIsDragging(Point currentPosition)
        {
            int intMouseMoveThreshold = 50;
            var intDistanceMoved = Math.Sqrt(Math.Pow((mouseStartPos.X - currentPosition.X), 2) + Math.Pow((mouseStartPos.Y - currentPosition.Y), 2));
            if (System.Convert.ToInt32(intDistanceMoved) > intMouseMoveThreshold)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void MoveAttachToFolder(string attachGuid, Folder folder, bool isNew = false)
        {
            SecurityTools.CheckForAccess(SecurityGroups.ManageAttachment);

            try
            {
                Waiting();
                RightClickMenu.Close();

                int rows = 0;

                using (var trans = DBFactory.GetDatabase().StartTransaction())
                using (var conn = trans.Connection)
                {
                    rows += DBFactory.GetDatabase().UpdateValue(attachmentColumns.TableName, attachmentColumns.FolderNameGuid, folder.FolderNameGuid, attachmentColumns.FileGuid, attachGuid, trans);
                    rows += DBFactory.GetDatabase().UpdateValue(attachmentColumns.TableName, attachmentColumns.FolderName, folder.FolderName, attachmentColumns.FileGuid, attachGuid, trans);

                    if (rows == 2)
                    {
                        trans.Commit();
                    }
                    else
                    {
                        trans.Rollback();
                        throw new Exception("Error while moving folders.");
                    }
                }

                PopulateFolderList();

                if (isNew)
                {
                    CurrentSelectedFolder = folder;
                }
                else
                {
                    CurrentSelectedFolder = previousFolder;
                }

                RefreshAttachments();
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            finally
            {
                DoneWaiting();
            }
        }

        private bool OKFileSize(Attachment attachment)
        {
            var fileSizeMB = System.Convert.ToInt32(attachment.Filesize / (float)(1024 * 1024));
            if (fileSizeMB > fileSizeMBLimit)
            {
                return false;
            }
            return true;
        }

        private async void DownloadAndOpenAttachment()
        {
            if (transferTaskRunning)
                return;

            string attachGuid = string.Empty;
            try
            {
                attachGuid = SelectedAttachmentGuid();

                if (string.IsNullOrEmpty(attachGuid)) return;

                using (var saveAttachment = await DownloadAttachment(attachGuid))
                {
                    if (saveAttachment != null)
                    {
                        string strFullPath = TempPathFilename(saveAttachment);
                        SaveAttachmentToDisk(saveAttachment, strFullPath);
                        Process.Start(strFullPath);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Logger("ERROR DOWNLOADING ATTACHMENT: " + this.FormGuid + "/" + attachGuid);
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            finally
            {
                SetStatusBarText("Idle...");
            }
        }

        private string TempPathFilename(Attachment attachment)
        {
            string strTimeStamp = DateTime.Now.ToString("_hhmmss");
            return Paths.DownloadPath + attachment.FileName + strTimeStamp + attachment.Extension;
        }

        private void ProcessAttachGridDrop(IDataObject dropDataObject)
        {
            try
            {
                if (DropIsFromOutside(dropDataObject))
                {
                    ProcessFileDrop(dropDataObject, CurrentSelectedFolder);
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            finally
            {
                isDragging = false;
            }
        }

        private void ProcessFolderListDrop(IDataObject dropDataObject, Folder folder)
        {
            try
            {
                //If a datagridviewrow is present, and the drop not from outside our form.
                if (dropDataObject.GetDataPresent(typeof(DataGridViewRow)))
                {
                    if (!DropIsFromOutside(dropDataObject))
                    {
                        //Cast out the datarow, get the attach Guid, and move the attachment to the new folder.
                        var DragRow = (DataGridViewRow)(dropDataObject.GetData(typeof(DataGridViewRow)));
                        CurrentSelectedFolder = previousFolder;
                        MoveAttachToFolder(DragRow.Cells[attachmentColumns.FileGuid].Value.ToString(), folder);
                    }
                    else
                    {
                        //Drop from another form. Process as a normal file drop.
                        ProcessFileDrop(dropDataObject, folder);
                        CurrentSelectedFolder = folder;
                    }
                }
                else
                {
                    //Otherwise the drag originated from windows explorer or Outlook. Process as a normal file drop.
                    ProcessFileDrop(dropDataObject, folder);
                    CurrentSelectedFolder = folder;
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            finally
            {
                isDragging = false;
            }
        }

        private void ProcessFileDrop(IDataObject dropDataObject, Folder folder)
        {
            //Outlook data object.
            if (dropDataObject.GetDataPresent("RenPrivateItem"))
            {
                string[] outlookFiles = CopyAttachement(dropDataObject, "RenPrivateItem");
                if (outlookFiles != null)
                {
                    UploadAttachments(outlookFiles, folder);
                }
            }
            //Explorer data object.
            else if (dropDataObject.GetDataPresent(DataFormats.FileDrop))
            {
                string[] explorerFiles = (string[])(dropDataObject.GetData(DataFormats.FileDrop));
                if (explorerFiles != null)
                {
                    UploadAttachments(explorerFiles, folder);
                }
            }
        }

        private bool DropIsFromOutside(IDataObject dropDataObject)
        {
            if (dropDataObject.GetDataPresent("FormID"))
            {
                string FormID = (string)(dropDataObject.GetData("FormID"));
                if (FormID != this.FormGuid)
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
            return false;
        }

        private void OnAttachCountChanged(EventArgs e)
        {
            AttachCountChanged(this, e);
        }

        private void UpdateDbAttachementName(string attachGuid, string fileName)
        {
            AssetManagerFunctions.UpdateSqlValue(attachmentColumns.TableName, attachmentColumns.FileName, fileName, attachmentColumns.FileGuid, attachGuid);
        }

        private void BeginRenameAttachment()
        {
            SecurityTools.CheckForAccess(SecurityGroups.ManageAttachment);

            //Enable read/write mode, set current cell to the filename cell and begin edit.
            AttachGrid.ReadOnly = false;
            AttachGrid.SelectionMode = DataGridViewSelectionMode.CellSelect;
            AttachGrid.CurrentRow.Cells[attachmentColumns.FileName].ReadOnly = false;
            AttachGrid.CurrentCell = AttachGrid.CurrentRow.Cells[attachmentColumns.FileName];
            AttachGrid.BeginEdit(true);
        }

        /// <summary>
        /// Updates the attachment filename if the new name is different from the current filename.
        /// </summary>
        /// <param name="row">Row of the attachment to be renamed.</param>
        /// <param name="newFilename">New filename for the attachment.</param>
        private void EndRenameAttachment(int row, string newFilename)
        {
            try
            {
                //The current (old) value is pulled from the DataGrid indexer.
                string oldFilename = AttachGrid[attachmentColumns.FileName, row].Value.ToString().Trim();

                //Make sure filename has changed.
                if (newFilename != oldFilename && !string.IsNullOrEmpty(newFilename))
                {
                    //Get the Guid of the attachment for the update method.
                    string renamedGuid = AttachGrid[attachmentColumns.FileGuid, row].Value.ToString();
                    UpdateDbAttachementName(renamedGuid, newFilename);
                }
                else
                {
                    //If no change, call cancel edit to restore the cell to the previous value.
                    AttachGrid.CancelEdit();
                }
            }
            catch (Exception ex)
            {
                //Expecting a MySQLException if the given filename is too long for the DB column.
                //Cancel edit, then pass the exception to the error handler for logging and user prompt.
                AttachGrid.CancelEdit();
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        private void DeleteCurrentAttachment()
        {
            try
            {
                SecurityTools.CheckForAccess(SecurityGroups.ManageAttachment);

                string attachGuid = SelectedAttachmentGuid();

                if (string.IsNullOrEmpty(attachGuid)) return;

                using (var attachment = GetSQLAttachment(attachGuid))
                {
                    var blah = OtherFunctions.Message("Are you sure you want to delete '" + attachment.FullFileName + "'?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, "Confirm Delete", this);
                    if (blah == DialogResult.Yes)
                    {
                        Waiting();
                        if (AssetManagerFunctions.DeleteSqlAttachment(attachment) > 0)
                        {
                            RefreshAttachments();
                        }
                        else
                        {
                            blah = OtherFunctions.Message("Deletion failed!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Unexpected Results", this);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            finally
            {
                DoneWaiting();
            }
        }

        private void UploadFiles(string[] files)
        {
            SetStatusBarText("Starting Upload...");
            UploadAttachments(files, CurrentSelectedFolder);
        }

        private async void UploadAttachments(string[] files, Folder folder)
        {
            if (transferTaskRunning)
            {
                return;
            }

            transferTaskRunning = true;
            Attachment uploadAttachment = null;

            try
            {
                taskCancelTokenSource = new CancellationTokenSource();
                var cancelToken = taskCancelTokenSource.Token;
                TransferFeedback(true);

                foreach (var file in files)
                {
                    uploadAttachment = new Attachment(file, attachFolderGuid, folder, attachmentColumns);

                    if (!OKFileSize(uploadAttachment))
                    {
                        uploadAttachment.Dispose();
                        OtherFunctions.Message("The file is too large. Please select a file less than " + fileSizeMBLimit.ToString() + "MB.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Size Limit Exceeded", this);
                        continue;
                    }

                    SetStatusBarText("Creating Directory...");
                    if (!await MakeDirectory(uploadAttachment.ObjectGuid))
                    {
                        uploadAttachment.Dispose();
                        OtherFunctions.Message("Error creating FTP directory.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "FTP Upload Error", this);
                        return;
                    }

                    var fileList = new List<string>(files);
                    SetStatusBarText("Uploading... " + (fileList.IndexOf(file) + 1).ToString() + " of " + files.Length.ToString());
                    progress = new ProgressCounter();

                    using (var trans = DBFactory.GetDatabase().StartTransaction())
                    using (var conn = trans.Connection)
                    {
                        try
                        {
                            await Task.Run(() =>
                            {
                                using (var fileStream = (FileStream)(uploadAttachment.DataStream))
                                using (var ftpStream = FtpComms.ReturnFtpRequestStream(ftpUri + uploadAttachment.ObjectGuid + "/" + uploadAttachment.FileGuid, WebRequestMethods.Ftp.UploadFile))
                                {
                                    int bufferSize = 256000;
                                    byte[] buffer = new byte[bufferSize];
                                    int bytesIn = 1;
                                    progress.BytesToTransfer = System.Convert.ToInt32(fileStream.Length);

                                    while (!(bytesIn < 1 || cancelToken.IsCancellationRequested))
                                    {
                                        bytesIn = fileStream.Read(buffer, 0, bufferSize);
                                        if (bytesIn > 0)
                                        {
                                            ftpStream.Write(buffer, 0, bytesIn);
                                            progress.BytesMoved = bytesIn;
                                        }
                                    }
                                    buffer = null;
                                }
                            });

                            if (cancelToken.IsCancellationRequested)
                            {
                                FtpFunctions.DeleteFtpAttachment(uploadAttachment.FileGuid, uploadAttachment.ObjectGuid);
                            }
                            else
                            {
                                var insertedRows = InsertSQLAttachment(uploadAttachment, trans);
                                if (insertedRows > 0)
                                {
                                    trans.Commit();
                                }
                                else
                                {
                                    trans.Rollback();
                                }
                            }
                            uploadAttachment.Dispose();
                        }
                        catch (Exception)
                        {
                            trans.Rollback();
                        }
                    }

                    RefreshAttachments();

                    if (cancelToken.IsCancellationRequested) break;
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            finally
            {
                transferTaskRunning = false;

                if (!GlobalSwitches.ProgramEnding && !this.IsDisposed)
                {
                    uploadAttachment?.Dispose();
                    SetStatusBarText("Idle...");
                    TransferFeedback(false);
                }
            }
        }

        private void StartDragDropAttachment()
        {
            dragDropDataObj = new DataObject();
            dragDropDataObj.SetData(typeof(DataGridViewRow), AttachGrid.CurrentRow);
            dragDropDataObj.SetData("FormID", this.FormGuid);
            AttachGrid.DoDragDrop(dragDropDataObj, DragDropEffects.All);
        }

        private async void DownloadAndSaveAttachment()
        {
            if (transferTaskRunning)
                return;

            try
            {
                string attachGuid = SelectedAttachmentGuid();

                if (string.IsNullOrEmpty(attachGuid)) return;

                using (var saveAttachment = await DownloadAttachment(attachGuid))
                {
                    if (saveAttachment != null)
                    {
                        using (SaveFileDialog saveDialog = new SaveFileDialog())
                        {
                            saveDialog.Filter = "All files (*.*)|*.*";
                            saveDialog.FileName = saveAttachment.FullFileName;
                            if (saveDialog.ShowDialog() == DialogResult.OK)
                            {
                                SaveAttachmentToDisk(saveAttachment, saveDialog.FileName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        private async void AddAttachmentFileToDragDropObject(string attachGuid)
        {
            if (!dragDropDataObj.GetDataPresent(DataFormats.FileDrop) && !transferTaskRunning)
            {
                Waiting();
                using (var saveAttachment = await DownloadAttachment(attachGuid))
                {
                    if (saveAttachment == null)
                    {
                        return;
                    }
                    string strFullPath = TempPathFilename(saveAttachment);
                    StringCollection fileList = new StringCollection();
                    fileList.Add(strFullPath);
                    dragDropDataObj.SetFileDropList(fileList);
                    SaveAttachmentToDisk(saveAttachment, strFullPath);
                    SetStatusBarText("Drag/Drop...");
                    AttachGrid.DoDragDrop(dragDropDataObj, DragDropEffects.All);
                }

                isDragging = false;
                DoneWaiting();
            }
        }

        private bool SaveAttachmentToDisk(Attachment attachment, string savePath)
        {
            try
            {
                SetStatusBarText("Saving to disk...");
                Directory.CreateDirectory(Paths.DownloadPath);
                using (var outputStream = File.Create(savePath))
                {
                    using (var memStream = attachment.DataStream)
                    {
                        memStream.CopyTo(outputStream); //once data is verified we go ahead and copy it to disk
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
                return false;
            }
            finally
            {
                SetStatusBarText("Idle...");
            }
        }

        private bool VerifyAttachment(Attachment attachment)
        {
            SetStatusBarText("Verifying data...");
            if (attachment.VerifyData())
            {
                return true;
            }
            else
            {
                //something is very wrong
                Logging.Logger("FILE VERIFICATION FAILURE: FolderGuid:" + attachment.ObjectGuid + "  FileGuid: " + attachment.FileGuid + " | Expected hash:" + attachment.FileMD5 + " Result hash:" + attachment.StreamMD5);
                OtherFunctions.Message("File verification failed! The file on the database is corrupt or there was a problem reading the data.    Please contact IT about this.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Hash Value Mismatch", this);
                attachment.Dispose();
                OtherFunctions.PurgeTempDir();
                return false;
            }
        }

        public override void Waiting(string message = "Processing...")
        {
            base.Waiting();
            SetStatusBarText(message);
        }

        private void TransferFeedback(bool transferRunning)
        {
            if (!GlobalSwitches.ProgramEnding)
            {
                if (transferRunning)
                {
                    ProgressBar1.Value = 0;
                    ProgressBar1.Visible = true;
                    CancelToolButton.Visible = true;
                    Spinner.Visible = true;
                    DoProgressLoop();
                }
                else
                {
                    progress = new ProgressCounter();
                    ProgressBar1.Value = 0;
                    ProgressBar1.Visible = false;
                    CancelToolButton.Visible = false;
                    Spinner.Visible = false;
                    ThroughputLabel.Text = null;
                    SetStatusBarText("Idle...");
                    DoneWaiting();
                }
            }
        }

        private string SelectedAttachmentGuid()
        {
            string attachGuid = AttachGrid.CurrentRowStringValue(attachmentColumns.FileGuid);
            if (!string.IsNullOrEmpty(attachGuid))
            {
                return attachGuid;
            }
            else
            {
                OtherFunctions.Message("Please select an attachment first.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Nothing Selected", this);
                return string.Empty;
            }
        }

        private void ToggleDragMode()
        {
            allowDrag = !allowDrag;
            AttachGrid.MultiSelect = !allowDrag;
            if (AttachGrid.MultiSelect)
            {
                AttachGrid.SelectionMode = DataGridViewSelectionMode.CellSelect;
            }
            else
            {
                AttachGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
            AllowDragCheckBox.Checked = allowDrag;
        }

        private bool FolderNameExists(string folderName)
        {
            foreach (ListViewItem item in FolderListView.Items)
            {
                if (item.Text == folderName)
                {
                    return true;
                }
            }
            return false;
        }

        private async Task DoProgressLoop()
        {
            while (transferTaskRunning)
            {
                progress.Tick();
                if (progress.BytesMoved > 0)
                {
                    var throughputText = progress.Throughput.ToString("0.00") + " MB/s";

                    if (ThroughputLabel.Text != throughputText)
                        ThroughputLabel.Text = throughputText;

                    if (ProgressBar1.Value != progress.Percent)
                    {
                        if (progress.Percent < 100)
                            ProgressBar1.Value = progress.Percent + 1;
                        ProgressBar1.Value = progress.Percent;
                    }
                }
                else
                {
                    ThroughputLabel.Text = string.Empty;
                }

                await Task.Delay(100);
            }
        }

        #region Control Event Methods

        private void AttachmentsForm_Shown(object sender, EventArgs e)
        {
            AttachGrid.ClearSelection();
            gridFilling = false;
        }

        private void AttachmentsForm_Load(object sender, EventArgs e)
        {
            RefreshAttachments();
        }

        private void AttachGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!AttachGrid.IsCurrentCellInEditMode)
            {
                DownloadAndOpenAttachment();
            }
        }

        private void AttachGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            //Must do this to keep the current cell selection highlight from sticking.
            if (allowDrag)
            {
                AttachGrid.ClearSelection();
            }
            if (AttachGrid.IsCurrentCellInEditMode)
            {
                string newFileName = e.FormattedValue.ToString().Trim();
                EndRenameAttachment(e.RowIndex, newFileName);
            }
        }

        private void AttachGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //After editing, return the datagrid to original read-only mode.
            AttachGrid.ReadOnly = true;
            AttachGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void AttachGrid_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (!gridFilling)
            {
                StyleFunctions.HighlightRow(AttachGrid, GridTheme, e.RowIndex);
            }
        }

        private void AttachGrid_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            StyleFunctions.LeaveRow(AttachGrid, e.RowIndex);
        }

        private void AttachGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                if (e.Button == MouseButtons.Right && !AttachGrid[e.ColumnIndex, e.RowIndex].Selected)
                {
                    AttachGrid.Rows[e.RowIndex].Selected = true;
                    AttachGrid.CurrentCell = AttachGrid[e.ColumnIndex, e.RowIndex];
                }
            }
        }

        private void AttachGrid_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            isDragging = false;
        }

        private void AttachGrid_DragDrop(object sender, DragEventArgs e)
        {
            ProcessAttachGridDrop(e.Data);
        }

        private void AttachGrid_DragLeave(object sender, EventArgs e)
        {
            isDragging = false;
        }

        private void AttachGrid_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void AttachGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (allowDrag)
            {
                mouseStartPos = e.Location;
            }
        }

        private void AttachGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (allowDrag && !isDragging && !transferTaskRunning)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (MouseIsDragging(currentPosition: e.Location) && AttachGrid.CurrentRow != null)
                    {
                        isDragging = true;
                        previousFolder = CurrentSelectedFolder;
                        StartDragDropAttachment();
                    }
                }
            }
        }

        private void AttachGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Menu)
            {
                ToggleDragMode();
            }
        }

        private void AttachGrid_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            var grid = (DataGridView)sender;
            if (grid != null)
            {
                var form = grid.FindForm();
                if (((MousePosition.X) < form.DesktopBounds.Left) ||
                   ((MousePosition.X) > form.DesktopBounds.Right) ||
                   ((MousePosition.Y) < form.DesktopBounds.Top) ||
                   ((MousePosition.Y) > form.DesktopBounds.Bottom))
                {
                    AddAttachmentFileToDragDropObject(SelectedAttachmentGuid());
                }
                else
                {
                    if (isDragging && transferTaskRunning)
                    {
                        CancelTransfers();
                    }
                }
            }
        }

        private void AttachGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Add abbreviation to file size column.
            if (e.ColumnIndex == AttachGrid.ColumnIndex(attachmentColumns.FileSize))
            {
                e.Value = e.Value + " KB";
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            DeleteCurrentAttachment();
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            DownloadAndOpenAttachment();
        }

        private void cmdUpload_Click(object sender, EventArgs e)
        {
            UploadFileDialog();
        }

        private void CopyTextTool_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(this.AttachGrid.GetClipboardContent());
        }

        private void DeleteAttachmentToolItem_Click(object sender, EventArgs e)
        {
            DeleteCurrentAttachment();
        }

        private void OpenTool_Click(object sender, EventArgs e)
        {
            DownloadAndOpenAttachment();
        }

        private void RenameStripMenuItem_Click(object sender, EventArgs e)
        {
            BeginRenameAttachment();
        }

        private void CancelToolButton_Click(object sender, EventArgs e)
        {
            CancelTransfers();
        }

        private void SaveToMenuItem_Click(object sender, EventArgs e)
        {
            DownloadAndSaveAttachment();
        }

        private void FolderListView_ItemActivate(object sender, EventArgs e)
        {
            foreach (ListViewItem item in FolderListView.Items)
            {
                item.StateImageIndex = Convert.ToInt32(item.Selected);
            }
            if (FolderListView.SelectedIndices.Count > 0)
            {
                previousFolder = CurrentSelectedFolder;
            }
            RefreshAttachments();
        }

        private void FolderListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (isDragging)
            {
                SetFolderListViewStates();
            }
        }

        private void SetFolderListViewStates()
        {
            if (FolderListView.Items.Count > 0)
            {
                foreach (ListViewItem item in FolderListView.Items)
                {
                    if (item != null)
                    {
                        item.StateImageIndex = Convert.ToInt32(item.Selected);
                    }
                }
            }
        }

        private void FolderListView_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
            isDragging = true;

            var mousePoint = FolderListView.PointToClient(new Point(e.X, e.Y));
            var dragToItem = FolderListView.GetItemAt(mousePoint.X, mousePoint.Y);

            if (dragToItem != null)
            {
                dragToItem.Selected = true;
            }
        }

        private void FolderListView_DragDrop(object sender, DragEventArgs e)
        {
            //Return if the items are not selected in the ListView control.
            if (FolderListView.SelectedItems.Count == 0)
            {
                return;
            }
            //Returns the location of the mouse pointer in the ListView control.
            var mousePoint = FolderListView.PointToClient(new Point(e.X, e.Y));

            //Obtain the item that is located at the specified location of the mouse pointer.
            var dragToItem = FolderListView.GetItemAt(mousePoint.X, mousePoint.Y);

            if (dragToItem == null)
            {
                return;
            }

            if (dragToItem.Index == 0)
            {
                ProcessFolderListDrop(e.Data, new Folder());
            }
            else
            {
                ProcessFolderListDrop(e.Data, new Folder(dragToItem.Text, (string)dragToItem.Tag));
            }

            isDragging = false;
        }

        private void AllowDragCheckBox_Click(object sender, EventArgs e)
        {
            ToggleDragMode();
        }

        private void NewFolderMenuItem_Click(object sender, EventArgs e)
        {
            SecurityTools.CheckForAccess(SecurityGroups.ManageAttachment);

            string newFolderName;
            using (var FolderDialog = new Dialog(this))
            {
                FolderDialog.AddTextBox("FolderName", "Enter folder name:");
                FolderDialog.ShowDialog();
                if (FolderDialog.DialogResult == DialogResult.OK)
                {
                    newFolderName = FolderDialog.GetControlValue("FolderName").ToString();
                }
                else
                {
                    return;
                }
            }

            if (!FolderNameExists(newFolderName))
            {
                MoveAttachToFolder(SelectedAttachmentGuid(), new Folder(newFolderName), true);
            }
            else
            {
                OtherFunctions.Message("A folder with that name already exists.", MessageBoxButtons.OK, MessageBoxIcon.Information, "Duplicate Name", this);
            }
        }

        private void FolderListView_DragLeave(object sender, EventArgs e)
        {
            CurrentSelectedFolder = previousFolder;
        }

        #endregion Control Event Methods

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (components != null)
                    {
                        components.Dispose();
                    }

                    taskCancelTokenSource?.Dispose();
                    AttachCountChanged = null;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        #endregion Methods
    }
}