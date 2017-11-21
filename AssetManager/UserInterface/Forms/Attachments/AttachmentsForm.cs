﻿using AssetManager.UserInterface.CustomControls;
using AssetManager.UserInterface.Forms.AssetManagement;
using AssetManager.UserInterface.Forms.Sibi;
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
using MyDialogLib;

namespace AssetManager.UserInterface.Forms.Attachments
{
    public partial class AttachmentsForm : ExtendedForm
    {
        #region Fields

        private string AttachFolderUID;
        private const short FileSizeMBLimit = 150;
        private AttachmentsBaseCols _attachTable;
        private DeviceObject AttachDevice;
        private RequestObject AttachRequest;
        private bool bolAllowDrag = true;
        private bool bolDragging = false;
        private bool bolGridFilling;
        private CancellationTokenSource taskCancelTokenSource;
        private bool TransferTaskRunning = false;
        private DataObject dragDropDataObj = new DataObject();

        /// <summary>
        /// "ftp://  strServerIP  /attachments/  CurrentDB  /"
        /// </summary>
        private string FTPUri;

        private Point MouseStartPos;
        private ProgressCounter Progress = new ProgressCounter();
        private string PrevSelectedFolder;
        private string _currentFolder = "";

        private string CurrentSelectedFolder
        {
            get
            {
                if (FolderListView.SelectedItems.Count > 0)
                {
                    if (FolderListView.SelectedItems[0].Index == 0)
                    {
                        _currentFolder = "";
                    }
                    else
                    {
                        _currentFolder = FolderListView.SelectedItems[0].Text;
                    }
                    return _currentFolder;
                }
                else
                {
                    return _currentFolder;
                }
            }
            set
            {
                _currentFolder = value;
                SetActiveFolderByName(value);
            }
        }

        #endregion Fields

        #region Constructors

        public AttachmentsForm(ExtendedForm ParentForm, AttachmentsBaseCols AttachTable, object attachDataObject = null) : base(ParentForm)
        {
            FTPUri = "ftp://" + ServerInfo.MySQLServerIP + "/attachments/" + ServerInfo.CurrentDataBase.ToString() + "/";

            InitializeComponent();
            ImageCaching.CacheControlImages(this);
            AttachGrid.DefaultCellStyle.SelectionBackColor = GridTheme.CellSelectColor;
            ExtendedMethods.DoubleBufferedDataGrid(AttachGrid, true);
            SetStatusBar("Idle...");
            _attachTable = AttachTable;
            if (!ReferenceEquals(attachDataObject, null))
            {
                if (attachDataObject is RequestObject)
                {
                    AttachRequest = (RequestObject)attachDataObject;
                    AttachFolderUID = AttachRequest.GUID;
                    FormUID = AttachFolderUID;
                    this.Text = "Sibi Attachments";
                    DeviceGroup.Visible = false;
                    SibiGroup.Dock = DockStyle.Top;
                    FillSibiInfo();
                }
                else if (attachDataObject is DeviceObject)
                {
                    AttachDevice = (DeviceObject)attachDataObject;
                    AttachFolderUID = AttachDevice.GUID;
                    FormUID = AttachFolderUID;
                    this.Text = "Device Attachments";
                    SibiGroup.Visible = false;
                    DeviceGroup.Dock = DockStyle.Top;
                    FillDeviceInfo();
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

        private void SetActiveFolderByName(string folderName)
        {
            if (folderName == "")
            {
                FolderListView.Items[0].Selected = true;
            }
            else
            {
                foreach (ListViewItem item in FolderListView.Items)
                {
                    if (item.Text == folderName)
                    {
                        item.Selected = true;
                    }
                }
            }
            SetFolderListViewStates();
        }

        private void FillSibiInfo()
        {
            ReqPO.Text = AttachRequest.PO;
            ReqNumberTextBox.Text = AttachRequest.RequisitionNumber;
            txtRequestNum.Text = AttachRequest.RequestNumber;
            txtDescription.Text = AttachRequest.Description;
            this.Text += " - " + AttachRequest.Description;
        }

        private void FillDeviceInfo()
        {
            txtAssetTag.Text = AttachDevice.AssetTag;
            txtSerial.Text = AttachDevice.Serial;
            txtDeviceDescription.Text = AttachDevice.Description;
        }

        public bool ActiveTransfer()
        {
            return TransferTaskRunning;
        }

        public void CancelTransfers()
        {
            taskCancelTokenSource.Cancel();
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
                    bolGridFilling = true;
                    GridFunctions.PopulateGrid(AttachGrid, results, AttachGridColumns(_attachTable));
                }

                AttachGrid.Columns[_attachTable.FileName].DefaultCellStyle.Font = new Font("Consolas", 9.75F, FontStyle.Bold);
                RefreshAttachCount();
                AttachGrid.ClearSelection();
                if (this.Visible)
                {
                    bolGridFilling = false;
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
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

        public override bool OKToClose()
        {
            if (ActiveTransfer())
            {
                this.WindowState = FormWindowState.Normal;
                this.Activate();
                var blah = OtherFunctions.Message("There are active uploads/downloads. Do you wish to cancel the current operation?", (int)MessageBoxButtons.YesNo + (int)MessageBoxIcon.Exclamation, "Worker Busy", this);
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
            if (!OKToClose())
            {
                e.Cancel = true;
            }
        }

        private void UploadFileDialog()
        {
            try
            {
                if (!SecurityTools.CheckForAccess(SecurityTools.AccessGroup.ManageAttachment))
                {
                    return;
                }
                using (OpenFileDialog fd = new OpenFileDialog())
                {
                    fd.ShowHelp = true;
                    fd.Title = "Select File To Upload - " + FileSizeMBLimit.ToString() + "MB Limit";
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
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
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

        private async Task<Attachment> DownloadAttachment(string AttachUID)
        {
            if (TransferTaskRunning)
            {
                return null;
            }
            TransferTaskRunning = true;
            Attachment dAttachment = new Attachment();
            try
            {
                taskCancelTokenSource = new CancellationTokenSource();
                CancellationToken cancelToken = taskCancelTokenSource.Token;
                SetStatusBar("Connecting...");
                FtpComms LocalFTPComm = new FtpComms();
                dAttachment = GetSQLAttachment(AttachUID);
                string FtpRequestString = FTPUri + dAttachment.FolderGUID + "/" + AttachUID;
                //get file size
                Progress = new ProgressCounter();
                Progress.BytesToTransfer = Convert.ToInt32(LocalFTPComm.ReturnFtpResponse(FtpRequestString, WebRequestMethods.Ftp.GetFileSize).ContentLength);
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
                                Progress.BytesMoved = bytesIn;
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
                TransferTaskRunning = false;
                if (!GlobalSwitches.ProgramEnding)
                {
                    WorkerFeedback(false);
                }
            }
        }

        private void PopulateFolderList(string currentFolder = null)
        {
            FolderListView.Items.Clear();
            var folders = DBFactory.GetDatabase().DataTableFromQueryString("SELECT DISTINCT " + _attachTable.Folder + " FROM " + _attachTable.TableName + " WHERE " + _attachTable.FKey + "='" + AttachFolderUID + "' ORDER BY " + _attachTable.Folder);
            var allFolderItem = new ListViewItem("*All");
            allFolderItem.StateImageIndex = 1;
            allFolderItem.Selected = true;
            FolderListView.Items.Add(allFolderItem);
            foreach (DataRow row in folders.Rows)
            {
                if (row[_attachTable.Folder].ToString().Trim() != "")
                {
                    var newFolderItem = new ListViewItem(row[_attachTable.Folder].ToString());
                    newFolderItem.StateImageIndex = 0;
                    newFolderItem.Selected = false;
                    FolderListView.Items.Add(newFolderItem);
                }
            }
            if (currentFolder != null)
            {
                CurrentSelectedFolder = currentFolder;
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
                strQry = "Select * FROM " + _attachTable.TableName + " WHERE " + _attachTable.FKey + "='" + AttachFolderUID + "' ORDER BY " + _attachTable.Timestamp + " DESC";
            }
            else
            {
                strQry = "Select * FROM " + _attachTable.TableName + " WHERE " + _attachTable.Folder + "='" + CurrentSelectedFolder + "' AND " + _attachTable.FKey + " ='" + AttachFolderUID + "' ORDER BY " + _attachTable.Timestamp + " DESC";
            }

            return strQry;
        }

        private Attachment GetSQLAttachment(string AttachUID)
        {
            string strQry = "SELECT * FROM " + _attachTable.TableName + " WHERE " + _attachTable.FileUID + "='" + AttachUID + "' LIMIT 1";
            return new Attachment(DBFactory.GetDatabase().DataTableFromQueryString(strQry), _attachTable);
        }

        private List<DataGridColumn> AttachGridColumns(AttachmentsBaseCols attachtable)
        {
            List<DataGridColumn> ColList = new List<DataGridColumn>();
            ColList.Add(new DataGridColumn(attachtable.FileType, "", typeof(Image), ColumnFormatTypes.Image));
            ColList.Add(new DataGridColumn(attachtable.FileName, "Filename", typeof(string)));
            ColList.Add(new DataGridColumn(attachtable.FileSize, "Size", typeof(string), ColumnFormatTypes.FileSize));
            ColList.Add(new DataGridColumn(attachtable.Timestamp, "Date", typeof(DateTime)));
            ColList.Add(new DataGridColumn(attachtable.Folder, "Folder", typeof(string)));
            ColList.Add(new DataGridColumn(attachtable.FileUID, "AttachUID", typeof(string)));
            ColList.Add(new DataGridColumn(attachtable.FileHash, "MD5", typeof(string)));
            return ColList;
        }

        private void InsertSQLAttachment(Attachment Attachment, DbTransaction transaction)
        {
            List<DBParameter> InsertAttachmentParams = new List<DBParameter>();
            InsertAttachmentParams.Add(new DBParameter(Attachment.AttachTable.FKey, Attachment.FolderGUID));
            InsertAttachmentParams.Add(new DBParameter(Attachment.AttachTable.FileName, Attachment.FileName));
            InsertAttachmentParams.Add(new DBParameter(Attachment.AttachTable.FileType, Attachment.Extension));
            InsertAttachmentParams.Add(new DBParameter(Attachment.AttachTable.FileSize, Attachment.Filesize));
            InsertAttachmentParams.Add(new DBParameter(Attachment.AttachTable.FileUID, Attachment.FileUID));
            InsertAttachmentParams.Add(new DBParameter(Attachment.AttachTable.FileHash, Attachment.MD5));
            InsertAttachmentParams.Add(new DBParameter(Attachment.AttachTable.Folder, Attachment.FolderName));
            DBFactory.GetDatabase().InsertFromParameters(Attachment.AttachTable.TableName, InsertAttachmentParams, transaction);
        }

        private async Task<bool> MakeDirectory(string FolderGUID)
        {
            return await Task.Run(() =>
            {
                try
                {
                    FtpComms LocalFTPComm = new FtpComms();
                    using (var MkDirResp = (FtpWebResponse)(LocalFTPComm.ReturnFtpResponse(FTPUri + FolderGUID, WebRequestMethods.Ftp.MakeDirectory)))
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
            //if (NewStartPos != null)
            //{
            //    MouseStartPos = NewStartPos;
            //}
            //else
            //{
            var intDistanceMoved = Math.Sqrt(Math.Pow((MouseStartPos.X - CurrentPos.X), 2) + Math.Pow((MouseStartPos.Y - CurrentPos.Y), 2));
            if (System.Convert.ToInt32(intDistanceMoved) > intMouseMoveThreshold)
            {
                return true;
            }
            else
            {
                return false;
            }
            // }
            //return false;
        }

        private void MoveAttachToFolder(string AttachUID, string Folder, bool isNew = false)
        {
            if (!SecurityTools.CheckForAccess(SecurityTools.AccessGroup.ManageAttachment))
            {
                return;
            }
            try
            {
                Waiting();
                RightClickMenu.Close();
                GlobalInstances.AssetFunc.UpdateSqlValue(_attachTable.TableName, _attachTable.Folder, Folder, _attachTable.FileUID, AttachUID);
                if (isNew)
                {
                    PopulateFolderList(Folder);
                }
                else
                {
                    PopulateFolderList(CurrentSelectedFolder);
                }
                ListAttachments();
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
            finally
            {
                DoneWaiting();
            }
        }

        private bool OKFileSize(Attachment File)
        {
            var FileSizeMB = System.Convert.ToInt32(File.Filesize / (float)(1024 * 1024));
            if (FileSizeMB > FileSizeMBLimit)
            {
                return false;
            }
            return true;
        }

        private async void DownloadAndOpenAttachment(string AttachUID)
        {
            try
            {
                if (AttachUID == "")
                {
                    return;
                }
                using (var saveAttachment = await DownloadAttachment(AttachUID))
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
                Logging.Logger("ERROR DOWNLOADING ATTACHMENT: " + this.FormUID + "/" + AttachUID);
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
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
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
            finally
            {
                bolDragging = false;
            }
        }

        private void ProcessFolderListDrop(IDataObject dropObject, string folder)
        {
            try
            {
                //If a datagridviewrow is present, and the drop not from outside our form.
                if (dropObject.GetDataPresent(typeof(DataGridViewRow)))
                {
                    if (!DropIsFromOutside(dropObject))
                    {
                        //Cast out the datarow, get the attach UID, and move the attachment to the new folder.
                        var DragRow = (DataGridViewRow)(dropObject.GetData(typeof(DataGridViewRow)));
                        CurrentSelectedFolder = PrevSelectedFolder;
                        MoveAttachToFolder(DragRow.Cells[GridFunctions.GetColIndex(AttachGrid, _attachTable.FileUID)].Value.ToString(), folder);
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
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
            finally
            {
                bolDragging = false;
            }
        }

        private void ProcessFileDrop(IDataObject dropObject, string folder)
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
                if (FormID != this.FormUID)
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

        private void RefreshAttachCount()
        {
            if (Tag is ViewDeviceForm)
            {
                ViewDeviceForm vw = (ViewDeviceForm)Tag;
                vw.SetAttachCount();
            }
            else if (Tag is SibiManageRequestForm)
            {
                SibiManageRequestForm req = (SibiManageRequestForm)Tag;
                req.SetAttachCount();
            }
        }

        private void UpdateDbAttachementName(string AttachUID, string NewFileName)
        {
            GlobalInstances.AssetFunc.UpdateSqlValue(_attachTable.TableName, _attachTable.FileName, NewFileName, _attachTable.FileUID, AttachUID);
        }

        private void BeginRenameAttachment()
        {
            if (!SecurityTools.CheckForAccess(SecurityTools.AccessGroup.ManageAttachment))
            {
                return;
            }
            //Enable read/write mode, set current cell to the filename cell and begin edit.
            AttachGrid.ReadOnly = false;
            AttachGrid.SelectionMode = DataGridViewSelectionMode.CellSelect;
            AttachGrid.CurrentRow.Cells[GridFunctions.GetColIndex(AttachGrid, _attachTable.FileName)].ReadOnly = false;
            AttachGrid.CurrentCell = AttachGrid.CurrentRow.Cells[GridFunctions.GetColIndex(AttachGrid, _attachTable.FileName)];
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
                string oldFilename = AttachGrid[_attachTable.FileName, row].Value.ToString().Trim();

                //Make sure filename has changed.
                if (newFilename != oldFilename && newFilename != "")
                {
                    //Get the UID of the attachment for the update method.
                    string renamedUID = AttachGrid[_attachTable.FileUID, row].Value.ToString();
                    UpdateDbAttachementName(renamedUID, newFilename);
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
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
        }

        private void DeleteAttachment(string AttachUID)
        {
            try
            {
                if (!SecurityTools.CheckForAccess(SecurityTools.AccessGroup.ManageAttachment))
                {
                    return;
                }
                string strFilename = AttachGrid[GridFunctions.GetColIndex(AttachGrid, _attachTable.FileName), AttachGrid.CurrentRow.Index].Value.ToString();
                var blah = OtherFunctions.Message("Are you sure you want to delete '" + strFilename + "'?", (int)MessageBoxButtons.YesNo + (int)MessageBoxIcon.Question, "Confirm Delete", this);
                if (blah == DialogResult.Yes)
                {
                    Waiting();
                    if (GlobalInstances.AssetFunc.DeleteSqlAttachment(GetSQLAttachment(AttachUID)) > 0)
                    {
                        ListAttachments();
                    }
                    else
                    {
                        blah = OtherFunctions.Message("Deletion failed!", (int)MessageBoxButtons.OK + (int)MessageBoxIcon.Exclamation, "Unexpected Results", this);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
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

        private async void UploadAttachments(string[] files, string folder)
        {
            if (TransferTaskRunning)
            {
                return;
            }
            TransferTaskRunning = true;
            Attachment CurrentAttachment = new Attachment();
            try
            {
                FtpComms LocalFTPComm = new FtpComms();
                taskCancelTokenSource = new CancellationTokenSource();
                CancellationToken cancelToken = taskCancelTokenSource.Token;
                WorkerFeedback(true);
                foreach (string file in files)
                {
                    CurrentAttachment = new Attachment(file, AttachFolderUID, folder, _attachTable);
                    if (!OKFileSize(CurrentAttachment))
                    {
                        CurrentAttachment.Dispose();
                        OtherFunctions.Message("The file is too large.   Please select a file less than " + FileSizeMBLimit.ToString() + "MB.", (int)MessageBoxButtons.OK + (int)MessageBoxIcon.Exclamation, "Size Limit Exceeded", this);
                        continue;
                    }
                    SetStatusBar("Creating Directory...");
                    if (!await MakeDirectory(CurrentAttachment.FolderGUID))
                    {
                        CurrentAttachment.Dispose();
                        OtherFunctions.Message("Error creating FTP directory.", (int)MessageBoxButtons.OK + (int)MessageBoxIcon.Exclamation, "FTP Upload Error", this);
                        return;
                    }
                    var fileList = new List<string>(files);
                    SetStatusBar("Uploading... " + (fileList.IndexOf(file) + 1).ToString() + " of " + files.Length.ToString());
                    Progress = new ProgressCounter();
                    using (var trans = DBFactory.GetDatabase().StartTransaction())
                    {
                        using (var conn = trans.Connection)
                        {
                            try
                            {
                                await Task.Run(() =>
                                {
                                    using (FileStream FileStream = (FileStream)(CurrentAttachment.DataStream))
                                    {
                                        using (System.IO.Stream FTPStream = LocalFTPComm.ReturnFtpRequestStream(FTPUri + CurrentAttachment.FolderGUID + "/" + CurrentAttachment.FileUID, WebRequestMethods.Ftp.UploadFile))
                                        {
                                            byte[] buffer = new byte[1024];
                                            int bytesIn = 1;
                                            Progress.BytesToTransfer = System.Convert.ToInt32(FileStream.Length);
                                            while (!(bytesIn < 1 || cancelToken.IsCancellationRequested))
                                            {
                                                bytesIn = FileStream.Read(buffer, 0, 1024);
                                                if (bytesIn > 0)
                                                {
                                                    FTPStream.Write(buffer, 0, bytesIn);
                                                    Progress.BytesMoved = bytesIn;
                                                }
                                            }
                                        }
                                    }
                                });
                                if (cancelToken.IsCancellationRequested)
                                {
                                    GlobalInstances.FTPFunc.DeleteFtpAttachment(CurrentAttachment.FileUID, CurrentAttachment.FolderGUID);
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
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
            finally
            {
                TransferTaskRunning = false;
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
            dragDropDataObj.SetData("FormID", this.FormUID);
            AttachGrid.DoDragDrop(dragDropDataObj, DragDropEffects.All);
        }

        private async void DownloadAndSaveAttachment(string attachUID)
        {
            try
            {
                if (attachUID == "")
                {
                    return;
                }
                using (var saveAttachment = await DownloadAttachment(attachUID))
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
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
        }

        private async void AddAttachmentFileToDragDropObject(string attachUID)
        {
            if (!dragDropDataObj.GetDataPresent(DataFormats.FileDrop) && !TransferTaskRunning)
            {
                Waiting();
                using (var saveAttachment = await DownloadAttachment(attachUID))
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

                bolDragging = false;
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
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
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
                Logging.Logger("FILE VERIFICATION FAILURE: FolderUID:" + attachment.FolderGUID + "  FileUID: " + attachment.FileUID + " | Expected hash:" + attachment.MD5 + " Result hash:" + attachment.ComputedMD5);
                OtherFunctions.Message("File verification failed! The file on the database is corrupt or there was a problem reading the data.    Please contact IT about this.", (int)MessageBoxButtons.OK + (int)MessageBoxIcon.Exclamation, "Hash Value Mismatch", this);
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
                    Progress = new ProgressCounter();
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

        private string SelectedAttachmentUID()
        {
            string AttachUID = AttachGrid[GridFunctions.GetColIndex(AttachGrid, _attachTable.FileUID), AttachGrid.CurrentRow.Index].Value.ToString();
            if (!string.IsNullOrEmpty(AttachUID))
            {
                return AttachUID;
            }
            else
            {
                throw (new Exception("No Attachment Selected."));
            }
        }

        private void ToggleDragMode()
        {
            bolAllowDrag = !bolAllowDrag;
            AttachGrid.MultiSelect = !bolAllowDrag;
            if (AttachGrid.MultiSelect)
            {
                AttachGrid.SelectionMode = DataGridViewSelectionMode.CellSelect;
            }
            else
            {
                AttachGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
            AllowDragCheckBox.Checked = bolAllowDrag;
        }

        private bool FolderNameExists(string folderName)
        {
            foreach (ListViewItem item in FolderListView.Items)
            {
                if (item.Text.ToUpper() == folderName.ToUpper())
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
            bolGridFilling = false;
        }

        private void AttachmentsForm_Load(object sender, EventArgs e)
        {
            ListAttachments();
        }

        private void AttachGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!AttachGrid.IsCurrentCellInEditMode)
            {
                DownloadAndOpenAttachment(SelectedAttachmentUID());
            }
        }

        private void AttachGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            //Must do this to keep the current cell selection highlight from sticking.
            if (bolAllowDrag)
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
            if (!bolGridFilling)
            {
                StyleFunctions.HighlightRow(AttachGrid, GridTheme, e.RowIndex);
            }
        }

        private void AttachGrid_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            StyleFunctions.LeaveRow(AttachGrid, GridTheme, e.RowIndex);
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
            bolDragging = false;
        }

        private void AttachGrid_DragDrop(object sender, DragEventArgs e)
        {
            ProcessAttachGridDrop(e.Data);
        }

        private void AttachGrid_DragLeave(object sender, EventArgs e)
        {
            bolDragging = false;
        }

        private void AttachGrid_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void AttachGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (bolAllowDrag)
            {
                MouseStartPos = e.Location;
                //MouseIsDragging(e.Location);
            }
        }

        private void AttachGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (bolAllowDrag && !bolDragging && !TransferTaskRunning)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (MouseIsDragging(CurrentPos: e.Location) && AttachGrid.CurrentRow != null)
                    {
                        bolDragging = true;
                        PrevSelectedFolder = CurrentSelectedFolder;
                        StartDragDropAttachment();
                    }
                }
            }
        }

        private void cmdDelete_Click(object sender, EventArgs e)
        {
            DeleteAttachment(SelectedAttachmentUID());
        }

        private void cmdOpen_Click(object sender, EventArgs e)
        {
            DownloadAndOpenAttachment(SelectedAttachmentUID());
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
            DeleteAttachment(SelectedAttachmentUID());
        }

        private void OpenTool_Click(object sender, EventArgs e)
        {
            DownloadAndOpenAttachment(SelectedAttachmentUID());
        }

        private void ProgTimer_Tick(object sender, EventArgs e)
        {
            Progress.Tick();
            if (Progress.BytesMoved > 0)
            {
                statMBPS.Text = Progress.Throughput.ToString("0.00") + " MB/s";

                ProgressBar1.Value = Progress.Percent;
                if (Progress.Percent > 1)
                {
                    ProgressBar1.Value -= 1; //doing this bypasses the progressbar control animation. This way it doesn't lag behind and fills completely
                }
                ProgressBar1.Value = Progress.Percent;
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
            DownloadAndSaveAttachment(SelectedAttachmentUID());
        }

        private void AttachmentsForm_Disposed(object sender, EventArgs e)
        {
            OtherFunctions.PurgeTempDir();
        }

        private void FolderListView_ItemActivate(object sender, EventArgs e)
        {
            foreach (ListViewItem item in FolderListView.Items)
            {
                item.StateImageIndex = Convert.ToInt32(item.Selected);
            }
            if (FolderListView.SelectedIndices.Count > 0)
            {
                PrevSelectedFolder = CurrentSelectedFolder;
            }
            ListAttachments();
        }

        private void FolderListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (bolDragging)
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
            bolDragging = true;
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
                ProcessFolderListDrop(e.Data, "");
            }
            else
            {
                ProcessFolderListDrop(e.Data, dragToItem.Text);
            }
            bolDragging = false;
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
            if (!SecurityTools.CheckForAccess(SecurityTools.AccessGroup.ManageAttachment))
            {
                return;
            }
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
                MoveAttachToFolder(SelectedAttachmentUID(), newFolderName, true);
            }
            else
            {
                OtherFunctions.Message("A folder with that name already exists.", (int)MessageBoxButtons.OK + (int)MessageBoxIcon.Information, "Duplicate Name", this);
            }
        }

        private void FolderListView_DragLeave(object sender, EventArgs e)
        {
            CurrentSelectedFolder = PrevSelectedFolder;
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
                    AddAttachmentFileToDragDropObject(SelectedAttachmentUID());
                }
                else
                {
                    if (bolDragging && TransferTaskRunning)
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