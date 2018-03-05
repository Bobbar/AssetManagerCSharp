using AssetManager.Data;
using AssetManager.Data.Classes;
using AssetManager.Data.Communications;
using AssetManager.Data.Functions;
using AssetManager.Helpers;
using AssetManager.Security;
using AssetManager.Tools;
using AssetManager.UserInterface.CustomControls;
using MyDialogLib;
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


namespace AssetManager.UserInterface.Forms
{
    public partial class AttachmentsForm : ExtendedForm
    {
        #region Fields

        private string attachFolderGuid;
        private const short fileSizeMBLimit = 150;
        private AttachmentsBaseCols attachmentColumns;
        private bool allowDrag = true;
        private bool isDragging = false;
        private bool gridFilling;
        private CancellationTokenSource taskCancelTokenSource;
        private bool transferTaskRunning = false;
        private DataObject dragDropDataObj = new DataObject();
        public EventHandler AttachCountChanged;

        /// <summary>
        /// "ftp://  strServerIP  /attachments/  CurrentDB  /"
        /// </summary>
        private string ftpUri;

        private Point mouseStartPos;
        private ProgressCounter progress = new ProgressCounter();
        private Attachment.Folder previousFolder = new Attachment.Folder();
        private Attachment.Folder currentFolder = new Attachment.Folder();

        private Attachment.Folder CurrentSelectedFolder
        {
            get
            {
                if (FolderListView.SelectedItems.Count > 0)
                {
                    if (FolderListView.SelectedItems[0].Index == 0)
                    {
                        currentFolder = new Attachment.Folder();
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

        public AttachmentsForm(ExtendedForm parentForm, AttachmentsBaseCols attachTable, MappableObject attachDataObject, EventHandler attachCountChangeHandler = null) : base(parentForm, attachDataObject)
        {
            ftpUri = "ftp://" + ServerInfo.MySQLServerIP + "/attachments/" + ServerInfo.CurrentDataBase.ToString() + "/";

            InitializeComponent();
            AttachCountChanged += attachCountChangeHandler;
            ImageCaching.CacheControlImages(this);
            AttachGrid.DefaultCellStyle.SelectionBackColor = GridTheme.CellSelectColor;
            AttachGrid.DoubleBufferedDataGrid(true);
            SetStatusBar("Idle...");
            attachmentColumns = attachTable;
            if (!ReferenceEquals(attachDataObject, null))
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

            if (SecurityTools.CanAccess(SecurityTools.AccessGroup.ManageAttachment))
            {
                cmdUpload.Enabled = true;
                cmdDelete.Enabled = true;
            }
            else
            {
                cmdUpload.Enabled = false;
                cmdDelete.Enabled = false;
            }
            this.Show();
        }

        #endregion Constructors

        #region Methods

        private void SetActiveFolder(Attachment.Folder folder)
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
            ReqPO.Text = request.PO;
            ReqNumberTextBox.Text = request.RequisitionNumber;
            txtRequestNum.Text = request.RequestNumber;
            txtDescription.Text = request.Description;
            this.Text += " - " + request.Description;
        }

        private void FillDeviceInfo(Device device)
        {
            txtAssetTag.Text = device.AssetTag;
            txtSerial.Text = device.Serial;
            txtDeviceDescription.Text = device.Description;
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

        private void ListAttachments()
        {
            Waiting();
            try
            {
                string strQry = "";
                strQry = GetQry();
                using (DataTable results = DBFactory.GetDatabase().DataTableFromQueryString(strQry))
                {
                    gridFilling = true;
                    AttachGrid.Populate(results, AttachGridColumns(attachmentColumns));
                }

                AttachGrid.Columns[attachmentColumns.FileName].DefaultCellStyle.Font = new Font("Consolas", 9.75F, FontStyle.Bold);
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

        private void SetStatusBar(string Text)
        {
            StatusLabel.Text = Text;
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

        private void AttachmentsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!OkToClose())
            {
                e.Cancel = true;
            }
        }

        private void UploadFileDialog()
        {
            try
            {
                SecurityTools.CheckForAccess(SecurityTools.AccessGroup.ManageAttachment);

                using (OpenFileDialog fd = new OpenFileDialog())
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
                        UploadFile(fd.FileNames);
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

        private string[] CopyAttachement(IDataObject AttachObject, string DataFormat)
        {
            string FileName = GetAttachFileName(AttachObject, DataFormat);
            string[] strFullPath = new string[1];
            strFullPath[0] = Paths.DownloadPath + FileName;
            Directory.CreateDirectory(Paths.DownloadPath);
            using (var streamFileData = (MemoryStream)(AttachObject.GetData("FileContents")))
            {
                using (var outputStream = File.Create(strFullPath[0]))
                {
                    streamFileData.CopyTo(outputStream);
                    return strFullPath;
                }
            }
        }

        private void DoneWaiting()
        {
            OtherFunctions.SetWaitCursor(false, this);
            SetStatusBar("Idle...");
        }

        private async Task<Attachment> DownloadAttachment(string AttachGuid)
        {
            if (transferTaskRunning)
            {
                return null;
            }
            transferTaskRunning = true;
            Attachment dAttachment = new Attachment();
            try
            {
                taskCancelTokenSource = new CancellationTokenSource();
                CancellationToken cancelToken = taskCancelTokenSource.Token;
                SetStatusBar("Connecting...");
                FtpComms LocalFTPComm = new FtpComms();
                dAttachment = GetSQLAttachment(AttachGuid);
                string FtpRequestString = ftpUri + dAttachment.FolderGuid + "/" + AttachGuid;
                //get file size
                progress = new ProgressCounter();
                progress.BytesToTransfer = Convert.ToInt32(LocalFTPComm.ReturnFtpResponse(FtpRequestString, WebRequestMethods.Ftp.GetFileSize).ContentLength);
                //setup download
                SetStatusBar("Downloading...");
                WorkerFeedback(true);
                dAttachment.DataStream = await Task.Run(() =>
                {
                    using (Stream respStream = LocalFTPComm.ReturnFtpResponse(FtpRequestString, WebRequestMethods.Ftp.DownloadFile).GetResponseStream())
                    {
                        MemoryStream memStream = new MemoryStream();
                        byte[] buffer = new byte[1024];
                        int bytesIn = 0;
                        //ftp download
                        bytesIn = 1;
                        while (!(bytesIn < 1 || cancelToken.IsCancellationRequested))
                        {
                            bytesIn = respStream.Read(buffer, 0, 1024);
                            if (bytesIn > 0)
                            {
                                memStream.Write(buffer, 0, bytesIn); //download data to memory before saving to disk
                                progress.BytesMoved = bytesIn;
                            }
                        }
                        return memStream;
                    }
                });

                if (!cancelToken.IsCancellationRequested)
                {
                    if (VerifyAttachment(dAttachment))
                    {
                        return dAttachment;
                    }
                }
                dAttachment.Dispose();
                return null;
            }
            catch (Exception ex)
            {
                if (dAttachment != null)
                {
                    dAttachment.Dispose();
                }
                throw (ex);
            }
            finally
            {
                transferTaskRunning = false;
                if (!GlobalSwitches.ProgramEnding)
                {
                    WorkerFeedback(false);
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

        private string GetAttachFileName(IDataObject AttachObject, string DataFormat)
        {
            switch (DataFormat)
            {
                case "RenPrivateItem":
                    using (MemoryStream streamFileName = (MemoryStream)(AttachObject.GetData("FileGroupDescriptor")))
                    {
                        streamFileName.Position = 0;
                        StreamReader sr = new StreamReader(streamFileName);
                        string fullString = sr.ReadToEnd();
                        fullString = fullString.Replace("\0", "");
                        fullString = fullString.Replace("\u0001", "");
                        return fullString;
                    }
            }
            return null;
        }

        private string GetQry()
        {
            string strQry = "";

            if (FolderListView.SelectedIndices.Count > 0 && FolderListView.SelectedIndices[0] == 0)
            {
                strQry = "Select * FROM " + attachmentColumns.TableName + " WHERE " + attachmentColumns.FKey + "='" + attachFolderGuid + "' ORDER BY " + attachmentColumns.Timestamp + " DESC";
            }
            else
            {
                strQry = "Select * FROM " + attachmentColumns.TableName + " WHERE " + attachmentColumns.FolderNameGuid + "='" + CurrentSelectedFolder.FolderNameGuid + "' AND " + attachmentColumns.FKey + " ='" + attachFolderGuid + "' ORDER BY " + attachmentColumns.Timestamp + " DESC";
            }

            return strQry;
        }

        private Attachment GetSQLAttachment(string AttachGuid)
        {
            string strQry = "SELECT * FROM " + attachmentColumns.TableName + " WHERE " + attachmentColumns.FileGuid + "='" + AttachGuid + "' LIMIT 1";
            return new Attachment(DBFactory.GetDatabase().DataTableFromQueryString(strQry), attachmentColumns);
        }

        private List<GridColumnAttrib> AttachGridColumns(AttachmentsBaseCols attachtable)
        {
            List<GridColumnAttrib> ColList = new List<GridColumnAttrib>();
            ColList.Add(new GridColumnAttrib(attachtable.FileType, "", typeof(Image), ColumnFormatType.Image));
            ColList.Add(new GridColumnAttrib(attachtable.FileName, "Filename", typeof(string)));
            ColList.Add(new GridColumnAttrib(attachtable.FileSize, "Size", typeof(string), ColumnFormatType.FileSize));
            ColList.Add(new GridColumnAttrib(attachtable.Timestamp, "Date", typeof(DateTime)));
            ColList.Add(new GridColumnAttrib(attachtable.FolderName, "Folder", typeof(string)));
            ColList.Add(new GridColumnAttrib(attachtable.FileGuid, "AttachGuid", typeof(string)));
            ColList.Add(new GridColumnAttrib(attachtable.FileHash, "MD5", typeof(string)));
            return ColList;
        }

        private void InsertSQLAttachment(Attachment Attachment, DbTransaction transaction)
        {
            ParamCollection insertParams = new ParamCollection();
            insertParams.Add(Attachment.AttachTable.FKey, Attachment.FolderGuid);
            insertParams.Add(Attachment.AttachTable.FileName, Attachment.FileName);
            insertParams.Add(Attachment.AttachTable.FileType, Attachment.Extension);
            insertParams.Add(Attachment.AttachTable.FileSize, Attachment.Filesize);
            insertParams.Add(Attachment.AttachTable.FileGuid, Attachment.FileGuid);
            insertParams.Add(Attachment.AttachTable.FileHash, Attachment.MD5);
            insertParams.Add(Attachment.AttachTable.FolderName, Attachment.FolderInfo.FolderName);
            insertParams.Add(Attachment.AttachTable.FolderNameGuid, Attachment.FolderInfo.FolderNameGuid);
            DBFactory.GetDatabase().InsertFromParameters(Attachment.AttachTable.TableName, insertParams.Parameters, transaction);
        }

        private async Task<bool> MakeDirectory(string FolderGuid)
        {
            return await Task.Run(() =>
            {
                try
                {
                    FtpComms LocalFTPComm = new FtpComms();
                    using (var MkDirResp = (FtpWebResponse)(LocalFTPComm.ReturnFtpResponse(ftpUri + FolderGuid, WebRequestMethods.Ftp.MakeDirectory)))
                    {
                        if (MkDirResp.StatusCode == FtpStatusCode.PathnameCreated)
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

        private bool MouseIsDragging(Point CurrentPos)
        {
            int intMouseMoveThreshold = 50;
            var intDistanceMoved = Math.Sqrt(Math.Pow((mouseStartPos.X - CurrentPos.X), 2) + Math.Pow((mouseStartPos.Y - CurrentPos.Y), 2));
            if (System.Convert.ToInt32(intDistanceMoved) > intMouseMoveThreshold)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void MoveAttachToFolder(string attachGuid, Attachment.Folder folder, bool isNew = false)
        {
            SecurityTools.CheckForAccess(SecurityTools.AccessGroup.ManageAttachment);

            try
            {
                Waiting();
                RightClickMenu.Close();

                int rows = 0;
                using (var trans = DBFactory.GetDatabase().StartTransaction())
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

                ListAttachments();
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

        private bool OKFileSize(Attachment File)
        {
            var FileSizeMB = System.Convert.ToInt32(File.Filesize / (float)(1024 * 1024));
            if (FileSizeMB > fileSizeMBLimit)
            {
                return false;
            }
            return true;
        }

        private async void DownloadAndOpenAttachment(string AttachGuid)
        {
            try
            {
                if (AttachGuid == "")
                {
                    return;
                }
                using (var saveAttachment = await DownloadAttachment(AttachGuid))
                {
                    if (ReferenceEquals(saveAttachment, null))
                    {
                        return;
                    }
                    string strFullPath = TempPathFilename(saveAttachment);
                    SaveAttachmentToDisk(saveAttachment, strFullPath);
                    Process.Start(strFullPath);
                }
            }
            catch (Exception ex)
            {
                Logging.Logger("ERROR DOWNLOADING ATTACHMENT: " + this.FormGuid + "/" + AttachGuid);
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            finally
            {
                SetStatusBar("Idle...");
            }
        }

        private string TempPathFilename(Attachment attachment)
        {
            string strTimeStamp = DateTime.Now.ToString("_hhmmss");
            return Paths.DownloadPath + attachment.FileName + strTimeStamp + attachment.Extension;
        }

        private void ProcessAttachGridDrop(IDataObject dropObject)
        {
            try
            {
                if (DropIsFromOutside(dropObject))
                {
                    ProcessFileDrop(dropObject, CurrentSelectedFolder);
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

        private void ProcessFolderListDrop(IDataObject dropObject, Attachment.Folder folder)
        {
            try
            {
                //If a datagridviewrow is present, and the drop not from outside our form.
                if (dropObject.GetDataPresent(typeof(DataGridViewRow)))
                {
                    if (!DropIsFromOutside(dropObject))
                    {
                        //Cast out the datarow, get the attach Guid, and move the attachment to the new folder.
                        var DragRow = (DataGridViewRow)(dropObject.GetData(typeof(DataGridViewRow)));
                        CurrentSelectedFolder = previousFolder;
                        MoveAttachToFolder(DragRow.Cells[attachmentColumns.FileGuid].Value.ToString(), folder);
                    }
                    else
                    {
                        //Drop from another form. Process as a normal file drop.
                        ProcessFileDrop(dropObject, folder);
                        CurrentSelectedFolder = folder;
                    }
                }
                else
                {
                    //Otherwise the drag originated from windows explorer or Outlook. Process as a normal file drop.
                    ProcessFileDrop(dropObject, folder);
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

        private void ProcessFileDrop(IDataObject dropObject, Attachment.Folder folder)
        {
            object File = null;
            //Outlook data object.
            if (dropObject.GetDataPresent("RenPrivateItem"))
            {
                File = CopyAttachement(dropObject, "RenPrivateItem");
                if (!ReferenceEquals(File, null))
                {
                    UploadAttachments((string[])File, folder);
                }
                //Explorer data object.
            }
            else if (dropObject.GetDataPresent(DataFormats.FileDrop))
            {
                string[] Files = (string[])(dropObject.GetData(DataFormats.FileDrop));
                if (!ReferenceEquals(Files, null))
                {
                    UploadAttachments(Files, folder);
                }
            }
        }

        private bool DropIsFromOutside(IDataObject dropObject)
        {
            if (dropObject.GetDataPresent("FormID"))
            {
                string FormID = (string)(dropObject.GetData("FormID"));
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

        private void UpdateDbAttachementName(string AttachGuid, string NewFileName)
        {
            AssetManagerFunctions.UpdateSqlValue(attachmentColumns.TableName, attachmentColumns.FileName, NewFileName, attachmentColumns.FileGuid, AttachGuid);
        }

        private void BeginRenameAttachment()
        {
            SecurityTools.CheckForAccess(SecurityTools.AccessGroup.ManageAttachment);

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

        private void DeleteAttachment(string AttachGuid)
        {
            try
            {
                SecurityTools.CheckForAccess(SecurityTools.AccessGroup.ManageAttachment);

                string strFilename = AttachGrid.CurrentRowStringValue(attachmentColumns.FileName);
                var blah = OtherFunctions.Message("Are you sure you want to delete '" + strFilename + "'?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, "Confirm Delete", this);
                if (blah == DialogResult.Yes)
                {
                    Waiting();
                    if (AssetManagerFunctions.DeleteSqlAttachment(GetSQLAttachment(AttachGuid)) > 0)
                    {
                        ListAttachments();
                    }
                    else
                    {
                        blah = OtherFunctions.Message("Deletion failed!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Unexpected Results", this);
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

        private void UploadFile(string[] Files)
        {
            SetStatusBar("Starting Upload...");
            UploadAttachments(Files, CurrentSelectedFolder);
        }

        private async void UploadAttachments(string[] files, Attachment.Folder folder)
        {
            if (transferTaskRunning)
            {
                return;
            }
            transferTaskRunning = true;
            Attachment CurrentAttachment = new Attachment();
            try
            {
                FtpComms LocalFTPComm = new FtpComms();
                taskCancelTokenSource = new CancellationTokenSource();
                CancellationToken cancelToken = taskCancelTokenSource.Token;
                WorkerFeedback(true);
                foreach (string file in files)
                {
                    CurrentAttachment = new Attachment(file, attachFolderGuid, folder, attachmentColumns);
                    if (!OKFileSize(CurrentAttachment))
                    {
                        CurrentAttachment.Dispose();
                        OtherFunctions.Message("The file is too large.   Please select a file less than " + fileSizeMBLimit.ToString() + "MB.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Size Limit Exceeded", this);
                        continue;
                    }
                    SetStatusBar("Creating Directory...");
                    if (!await MakeDirectory(CurrentAttachment.FolderGuid))
                    {
                        CurrentAttachment.Dispose();
                        OtherFunctions.Message("Error creating FTP directory.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "FTP Upload Error", this);
                        return;
                    }
                    var fileList = new List<string>(files);
                    SetStatusBar("Uploading... " + (fileList.IndexOf(file) + 1).ToString() + " of " + files.Length.ToString());
                    progress = new ProgressCounter();
                    using (var trans = DBFactory.GetDatabase().StartTransaction())
                    using (var conn = trans.Connection)
                    {
                        try
                        {
                            await Task.Run(() =>
                            {
                                using (FileStream FileStream = (FileStream)(CurrentAttachment.DataStream))
                                using (System.IO.Stream FTPStream = LocalFTPComm.ReturnFtpRequestStream(ftpUri + CurrentAttachment.FolderGuid + "/" + CurrentAttachment.FileGuid, WebRequestMethods.Ftp.UploadFile))
                                {
                                    byte[] buffer = new byte[1024];
                                    int bytesIn = 1;
                                    progress.BytesToTransfer = System.Convert.ToInt32(FileStream.Length);
                                    while (!(bytesIn < 1 || cancelToken.IsCancellationRequested))
                                    {
                                        bytesIn = FileStream.Read(buffer, 0, 1024);
                                        if (bytesIn > 0)
                                        {
                                            FTPStream.Write(buffer, 0, bytesIn);
                                            progress.BytesMoved = bytesIn;
                                        }
                                    }
                                }
                            });
                            if (cancelToken.IsCancellationRequested)
                            {
                                FtpFunctions.DeleteFtpAttachment(CurrentAttachment.FileGuid, CurrentAttachment.FolderGuid);
                            }
                            else
                            {
                                InsertSQLAttachment(CurrentAttachment, trans);
                            }
                            CurrentAttachment.Dispose();
                            trans.Commit();
                        }
                        catch (Exception)
                        {
                            trans.Rollback();
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
                transferTaskRunning = false;
                if (!GlobalSwitches.ProgramEnding && !this.IsDisposed)
                {
                    if (CurrentAttachment != null)
                    {
                        CurrentAttachment.Dispose();
                    }
                    SetStatusBar("Idle...");
                    WorkerFeedback(false);
                    ListAttachments();
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

        private async void DownloadAndSaveAttachment(string attachGuid)
        {
            try
            {
                if (attachGuid == "")
                {
                    return;
                }
                using (var saveAttachment = await DownloadAttachment(attachGuid))
                {
                    if (ReferenceEquals(saveAttachment, null))
                    {
                        return;
                    }
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
                    if (ReferenceEquals(saveAttachment, null))
                    {
                        return;
                    }
                    string strFullPath = TempPathFilename(saveAttachment);
                    StringCollection fileList = new StringCollection();
                    fileList.Add(strFullPath);
                    dragDropDataObj.SetFileDropList(fileList);
                    SaveAttachmentToDisk(saveAttachment, strFullPath);
                    SetStatusBar("Drag/Drop...");
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
                SetStatusBar("Saving to disk...");
                Directory.CreateDirectory(Paths.DownloadPath);
                using (var outputStream = File.Create(savePath))
                {
                    using (var memStream = (MemoryStream)attachment.DataStream)
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
                SetStatusBar("Idle...");
            }
        }

        private bool VerifyAttachment(Attachment attachment)
        {
            SetStatusBar("Verifying data...");
            if (attachment.VerifyAttachment())
            {
                return true;
            }
            else
            {
                //something is very wrong
                Logging.Logger("FILE VERIFICATION FAILURE: FolderGuid:" + attachment.FolderGuid + "  FileGuid: " + attachment.FileGuid + " | Expected hash:" + attachment.MD5 + " Result hash:" + attachment.ComputedMD5);
                OtherFunctions.Message("File verification failed! The file on the database is corrupt or there was a problem reading the data.    Please contact IT about this.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Hash Value Mismatch", this);
                attachment.Dispose();
                OtherFunctions.PurgeTempDir();
                return false;
            }
        }

        private void Waiting()
        {
            OtherFunctions.SetWaitCursor(true, this);
            SetStatusBar("Processing...");
        }

        private void WorkerFeedback(bool WorkerRunning)
        {
            if (!GlobalSwitches.ProgramEnding)
            {
                if (WorkerRunning)
                {
                    ProgressBar1.Value = 0;
                    ProgressBar1.Visible = true;
                    cmdCancel.Visible = true;
                    Spinner.Visible = true;
                    ProgTimer.Enabled = true;
                }
                else
                {
                    progress = new ProgressCounter();
                    ProgressBar1.Value = 0;
                    ProgressBar1.Visible = false;
                    cmdCancel.Visible = false;
                    Spinner.Visible = false;
                    ProgTimer.Enabled = false;
                    statMBPS.Text = null;
                    SetStatusBar("Idle...");
                    DoneWaiting();
                }
            }
        }

        private string SelectedAttachmentGuid()
        {
            string AttachGuid = AttachGrid.CurrentRowStringValue(attachmentColumns.FileGuid);
            if (!string.IsNullOrEmpty(AttachGuid))
            {
                return AttachGuid;
            }
            else
            {
                throw (new Exception("No Attachment Selected."));
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

        #region Control Event Methods

        private void AttachmentsForm_Shown(object sender, EventArgs e)
        {
            AttachGrid.ClearSelection();
            gridFilling = false;
        }

        private void AttachmentsForm_Load(object sender, EventArgs e)
        {
            ListAttachments();
        }

        private void AttachGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!AttachGrid.IsCurrentCellInEditMode)
            {
                DownloadAndOpenAttachment(SelectedAttachmentGuid());
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
                //MouseIsDragging(e.Location);
            }
        }

        private void AttachGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (allowDrag && !isDragging && !transferTaskRunning)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (MouseIsDragging(CurrentPos: e.Location) && AttachGrid.CurrentRow != null)
                    {
                        isDragging = true;
                        previousFolder = CurrentSelectedFolder;
                        StartDragDropAttachment();
                    }
                }
            }
        }

        private void cmdDelete_Click(object sender, EventArgs e)
        {
            DeleteAttachment(SelectedAttachmentGuid());
        }

        private void cmdOpen_Click(object sender, EventArgs e)
        {
            DownloadAndOpenAttachment(SelectedAttachmentGuid());
        }

        private void cmdUpload_Click(object sender, EventArgs e)
        {
            UploadFileDialog();
        }

        private void CopyTextTool_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(this.AttachGrid.GetClipboardContent());
        }

        private void DeleteAttachmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteAttachment(SelectedAttachmentGuid());
        }

        private void OpenTool_Click(object sender, EventArgs e)
        {
            DownloadAndOpenAttachment(SelectedAttachmentGuid());
        }

        private void ProgTimer_Tick(object sender, EventArgs e)
        {
            progress.Tick();
            if (progress.BytesMoved > 0)
            {
                statMBPS.Text = progress.Throughput.ToString("0.00") + " MB/s";

                ProgressBar1.Value = progress.Percent;
                if (progress.Percent > 1)
                {
                    ProgressBar1.Value -= 1; //doing this bypasses the progressbar control animation. This way it doesn't lag behind and fills completely
                }
                ProgressBar1.Value = progress.Percent;
            }
            else
            {
                statMBPS.Text = string.Empty;
            }
        }

        private void RenameStripMenuItem_Click(object sender, EventArgs e)
        {
            BeginRenameAttachment();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            CancelTransfers();
        }

        private void SaveToMenuItem_Click(object sender, EventArgs e)
        {
            DownloadAndSaveAttachment(SelectedAttachmentGuid());
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
            ListAttachments();
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
            Point p = FolderListView.PointToClient(new Point(e.X, e.Y));
            ListViewItem dragToItem = FolderListView.GetItemAt(p.X, p.Y);
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
            Point p = FolderListView.PointToClient(new Point(e.X, e.Y));
            //Obtain the item that is located at the specified location of the mouse pointer.
            ListViewItem dragToItem = FolderListView.GetItemAt(p.X, p.Y);
            if (ReferenceEquals(dragToItem, null))
            {
                return;
            }

            if (dragToItem.Index == 0)
            {
                ProcessFolderListDrop(e.Data, new Attachment.Folder());
            }
            else
            {
                ProcessFolderListDrop(e.Data, new Attachment.Folder(dragToItem.Text, (string)dragToItem.Tag));
            }
            isDragging = false;
        }

        private void AttachGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Menu)
            {
                ToggleDragMode();
            }
        }

        private void AllowDragCheckBox_Click(object sender, EventArgs e)
        {
            ToggleDragMode();
        }

        private void NewFolderMenuItem_Click(object sender, EventArgs e)
        {
            SecurityTools.CheckForAccess(SecurityTools.AccessGroup.ManageAttachment);

            string newFolderName;
            using (AdvancedDialog FolderDialog = new AdvancedDialog(this))
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
                MoveAttachToFolder(SelectedAttachmentGuid(), new Attachment.Folder(newFolderName), true);
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

        private void AttachGrid_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;
            if (grid != null)
            {
                Form f = grid.FindForm();
                if (((Control.MousePosition.X) < f.DesktopBounds.Left) ||
                        ((Control.MousePosition.X) > f.DesktopBounds.Right) ||
                        ((Control.MousePosition.Y) < f.DesktopBounds.Top) ||
                        ((Control.MousePosition.Y) > f.DesktopBounds.Bottom))
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

        #endregion Control Event Methods

        #endregion Methods
    }
}