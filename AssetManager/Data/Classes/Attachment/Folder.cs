using System;

namespace AssetManager.Data.Classes
{
    /// <summary>
    /// Attachment folder object.
    /// </summary>
    public class Folder
    {
        private string folderName;
        private string folderGuid;

        public string FolderName
        {
            get
            {
                return folderName;
            }
            set
            {
                folderName = value;
            }
        }

        public string FolderNameGuid
        {
            get
            {
                return folderGuid;
            }

            set
            {
                folderGuid = value;
            }
        }

        public Folder()
        {
            this.folderName = string.Empty;
            this.folderGuid = string.Empty;
        }

        public Folder(string folderName)
        {
            this.folderName = folderName;
            this.folderGuid = Guid.NewGuid().ToString();
        }

        public Folder(string folderName, string folderNameGuid)
        {
            this.folderName = folderName;
            this.folderGuid = folderNameGuid;
        }
    }
}