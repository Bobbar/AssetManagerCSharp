using AssetManager.Security;
using System;
using System.Data;
using System.IO;

namespace AssetManager.Data.Classes
{
    public class Attachment : IDisposable
    {
        private FileInfo fileInfo;
        private string fileName;
        private long fileSize;
        private string extention;
        private Folder folder;
        private string objectGuid;
        private string fileMD5;
        private string streamMD5;
        private string fileGuid;
        private AttachmentsBaseCols attachColumns;
        private Stream dataStream;

        public Attachment()
        {
            fileInfo = null;
            fileName = null;
            fileSize = 0;
            extention = null;
            folder = new Folder();
            objectGuid = null;
            fileMD5 = null;
            streamMD5 = null;
            fileGuid = null;
            attachColumns = null;
            dataStream = null;
        }

        /// <summary>
        /// Creates a new instance from a <see cref="DataRow"/>.
        /// </summary>
        /// <param name="attachRow"><see cref="DataRow"/> which contains the fields to be mapped to this instance.</param>
        /// <param name="columns"><see cref="AttachmentsBaseCols"/> instance which contains the field column names to be mapped.</param>
        public Attachment(DataRow attachRow, AttachmentsBaseCols columns)
        {
            fileInfo = null;
            dataStream = null;
            attachColumns = columns;
            fileName = attachRow[columns.FileName].ToString();
            fileGuid = attachRow[columns.FileGuid].ToString();
            fileMD5 = attachRow[columns.FileHash].ToString();
            streamMD5 = null;
            fileSize = Convert.ToInt64(attachRow[columns.FileSize]);
            extention = attachRow[columns.FileType].ToString();
            folder = new Folder(attachRow[columns.FolderName].ToString(), attachRow[columns.FolderNameGuid].ToString());
            objectGuid = attachRow[columns.FKey].ToString();
        }

        /// <summary>
        /// Creates a new instance from a file path.
        /// </summary>
        /// <param name="filePath">Full path of the file.</param>
        /// <param name="objectGuid">Guid of the DB object to be associated with this attachment. ie. Device Guid or SibiRequest Guid.</param>
        /// <param name="folder">The folder instance to be associated with this attachment.</param>
        /// <param name="columns"><see cref="AttachmentsBaseCols"/> instance which contains the field column names to be mapped.</param>
        public Attachment(string filePath, string objectGuid, Folder folder, AttachmentsBaseCols columns)
        {
            fileInfo = new FileInfo(filePath);
            fileName = Path.GetFileNameWithoutExtension(fileInfo.Name);
            fileGuid = Guid.NewGuid().ToString();
            fileMD5 = null;
            streamMD5 = null;
            fileSize = fileInfo.Length;
            extention = fileInfo.Extension;
            this.folder = folder;
            this.objectGuid = objectGuid;
            attachColumns = columns;
            dataStream = fileInfo.OpenRead();
        }

        public string FileName
        {
            get
            {
                if (fileInfo != null)
                {
                    return Path.GetFileNameWithoutExtension(fileInfo.Name);
                }
                else
                {
                    return fileName;
                }
            }
        }

        public string FullFileName
        {
            get
            {
                if (fileInfo != null)
                {
                    return fileInfo.Name;
                }
                else
                {
                    return fileName + Extension;
                }
            }
        }

        public string Extension
        {
            get
            {
                if (fileInfo != null)
                {
                    return fileInfo.Extension;
                }
                else
                {
                    return extention;
                }
            }
        }

        public long Filesize
        {
            get
            {
                if (fileInfo != null)
                {
                    return fileInfo.Length;
                }
                else
                {
                    return fileSize;
                }
            }
        }

        public string FileGuid
        {
            get { return fileGuid; }
        }

        public string FileMD5
        {
            get
            {
                if (fileMD5 != null)
                {
                    return fileMD5;
                }
                else
                {
                    fileMD5 = FileHash(fileInfo);
                    return fileMD5;
                }
            }
        }

        public string StreamMD5
        {
            get
            {
                return streamMD5;
            }
        }

        public Folder FolderInfo
        {
            get { return folder; }
        }

        public string ObjectGuid
        {
            get { return objectGuid; }
        }

        public Stream DataStream
        {
            get { return dataStream; }
            set { dataStream = value; }
        }

        public AttachmentsBaseCols AttachColumns
        {
            get { return attachColumns; }
            set { attachColumns = value; }
        }

        private string FileHash(FileInfo file)
        {
            using (FileStream fileStream = file.OpenRead())
            {
                return SecurityTools.GetMD5OfStream(fileStream);
            }
        }

        /// <summary>
        /// Returns true if the <see cref="FileMD5"/> matches the computed MD5 of the current <see cref="DataStream"/>.
        /// </summary>
        /// <returns></returns>
        public bool VerifyData()
        {
            if (dataStream != null)
            {
                streamMD5 = SecurityTools.GetMD5OfStream(dataStream);
                if (streamMD5 == fileMD5)
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
                    dataStream?.Dispose();
                    dataStream = null;
                    fileInfo = null;
                }
            }
            disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion "IDisposable Support"
    }
}