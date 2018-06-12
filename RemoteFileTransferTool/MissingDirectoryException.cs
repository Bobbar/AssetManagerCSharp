using System;

namespace RemoteFileTransferTool
{
    public class MissingDirectoryException : Exception
    {
        public MissingDirectoryException() : base("Directory not found on target.")
        {
        }
    }
}