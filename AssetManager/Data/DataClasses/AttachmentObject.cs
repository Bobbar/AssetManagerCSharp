using System;
using System.Data;
using System.IO;
using AssetManager.Data;
using AssetManager.Security;

namespace AssetManager.Data.Classes
{
    public class Attachment : IDisposable
    {
        private FileInfo _fileInfo;
        private string _fileName;
        private int _fileSize;
        private string _extention;
        private string _folderName;
        private string _folderGUID;
        private string _MD5;
        private string _computedMD5;
        private string _fileUID;
        private AttachmentsBaseCols _attachTable;

        private Stream _dataStream;

        public Attachment()
        {
            _fileInfo = null;
            _fileName = null;
            _fileSize = 0;
            _extention = null;
            _folderName = null;
            _folderGUID = null;
            _MD5 = null;
            _computedMD5 = null;
            _fileUID = null;
            _attachTable = null;
            _dataStream = null;
        }

        /// <summary>
        /// Create new Attachment from a file path.
        /// </summary>
        /// <param name="newFile">Full path to file.</param>
        /// <param name="attachTable">The table that will be assigned to this instance.</param>
        public Attachment(string newFile, AttachmentsBaseCols attachTable)
        {
            _fileInfo = new FileInfo(newFile);
            _fileName = Path.GetFileNameWithoutExtension(_fileInfo.Name);
            _fileUID = Guid.NewGuid().ToString();
            _MD5 = null;
            _computedMD5 = null;
            _fileSize = Convert.ToInt32(_fileInfo.Length);
            _extention = _fileInfo.Extension;
            _folderName = string.Empty;
            _folderGUID = string.Empty;
            _attachTable = attachTable;
            _dataStream = _fileInfo.OpenRead();
        }

        public Attachment(string newFile, string folderGUID, AttachmentsBaseCols attachTable)
        {
            _fileInfo = new FileInfo(newFile);
            _fileName = Path.GetFileNameWithoutExtension(_fileInfo.Name);
            _fileUID = Guid.NewGuid().ToString();
            _MD5 = null;
            _computedMD5 = null;
            _fileSize = Convert.ToInt32(_fileInfo.Length);
            _extention = _fileInfo.Extension;
            _folderName = string.Empty;
            _folderGUID = folderGUID;
            _attachTable = attachTable;
            _dataStream = _fileInfo.OpenRead();
        }

        public Attachment(DataTable attachInfoTable, AttachmentsBaseCols attachTable)
        {
            DataRow TableRow = attachInfoTable.Rows[0];
            _fileInfo = null;
            _dataStream = null;
            _attachTable = attachTable;
            _fileName = TableRow[attachTable.FileName].ToString();
            _fileUID = TableRow[attachTable.FileUID].ToString();
            _MD5 = TableRow[attachTable.FileHash].ToString();
            _computedMD5 = null;
            _fileSize = Convert.ToInt32(TableRow[attachTable.FileSize]);
            _extention = TableRow[attachTable.FileType].ToString();
            _folderName = TableRow[attachTable.Folder].ToString();
            _folderGUID = TableRow[attachTable.FKey].ToString();
        }

        public Attachment(string newFile, string folderGUID, string selectedFolder, AttachmentsBaseCols attachTable)
        {
            _fileInfo = new FileInfo(newFile);
            _fileName = Path.GetFileNameWithoutExtension(_fileInfo.Name);
            _fileUID = Guid.NewGuid().ToString();
            _MD5 = null;
            _computedMD5 = null;
            _fileSize = Convert.ToInt32(_fileInfo.Length);
            _extention = _fileInfo.Extension;
            _folderName = selectedFolder;
            _folderGUID = folderGUID;
            _attachTable = attachTable;
            _dataStream = _fileInfo.OpenRead();
        }

        public Attachment(DataTable attachInfoTable, string selectedFolder, AttachmentsBaseCols attachTable)
        {
            DataRow TableRow = attachInfoTable.Rows[0];
            _fileInfo = null;
            _dataStream = null;
            _attachTable = attachTable;
            _fileName = TableRow[attachTable.FileName].ToString();
            _fileUID = TableRow[attachTable.FileUID].ToString();
            _MD5 = TableRow[attachTable.FileHash].ToString();
            _computedMD5 = null;
            _fileSize = Convert.ToInt32(TableRow[attachTable.FileSize]);
            _extention = TableRow[attachTable.FileType].ToString();
            _folderName = TableRow[attachTable.Folder].ToString();
            _folderGUID = TableRow[attachTable.FKey].ToString();
        }

        public FileInfo FileInfo
        {
            get { return _fileInfo; }
        }

        public string FileName
        {
            get
            {
                if (_fileInfo != null)
                {
                    return Path.GetFileNameWithoutExtension(_fileInfo.Name);
                }
                else
                {
                    return _fileName;
                }
            }
        }

        public string FullFileName
        {
            get
            {
                if (_fileInfo != null)
                {
                    return _fileInfo.Name;
                }
                else
                {
                    return _fileName + Extension;
                }
            }
        }

        public string Extension
        {
            get
            {
                if (_fileInfo != null)
                {
                    return _fileInfo.Extension;
                }
                else
                {
                    return _extention;
                }
            }
        }

        public long Filesize
        {
            get
            {
                if (_fileInfo != null)
                {
                    return _fileInfo.Length;
                }
                else
                {
                    return _fileSize;
                }
            }
        }

        public string FileUID
        {
            get { return _fileUID; }
        }

        public string MD5
        {
            get
            {
                if (_MD5 != null)
                {
                    return _MD5;
                }
                else
                {
                    _MD5 = GetHash(_fileInfo);
                    return _MD5;
                }
            }
        }

        public string ComputedMD5
        {
            get
            {
                return _computedMD5;
            }
        }

        public string FolderName
        {
            get { return _folderName; }
        }

        public string FolderGUID
        {
            get { return _folderGUID; }
        }

        public Stream DataStream
        {
            get { return _dataStream; }
            set { _dataStream = value; }
        }

        public AttachmentsBaseCols AttachTable
        {
            get { return _attachTable; }
            set { _attachTable = value; }
        }

        private string GetHash(FileInfo Fileinfo)
        {
            using (FileStream HashStream = Fileinfo.OpenRead())
            {
                return SecurityTools.GetMD5OfStream(HashStream);
            }
        }

        public bool VerifyAttachment()
        {
            if (this.DataStream != null)
            {
                _computedMD5 = SecurityTools.GetMD5OfStream((MemoryStream)this.DataStream);
                if (_computedMD5 == this.MD5)
                {
                    return true;
                }
            }
            return false;
        }


        #region "IDisposable Support"

        // To detect redundant calls
        private bool disposedValue;

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (_dataStream != null)
                        _dataStream.Dispose();
                    _fileInfo = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                // TODO: set large fields to null.
            }
            disposedValue = true;
        }

        // TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        //Protected Overrides Sub Finalize()
        //    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        //    Dispose(False)
        //    MyBase.Finalize()
        //End Sub

        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(true);
            // TODO: uncomment the following line if Finalize() is overridden above.
            // GC.SuppressFinalize(Me)
        }

        #endregion "IDisposable Support"
    }
}