using System.Collections.Generic;
using System.Net;

namespace RemoteFileTransferTool
{
    public interface IRemoteTransfer
    {
        bool CreateMissingDirectories { get; set; }

        void StartTransfer(NetworkCredential credentials);

        void PauseTransfer();

        void ResumeTransfer();

        void CancelTransfer();

        List<string> ErrorList { get; }

        TransferStatus CurrentStatus { get; }

        bool IsDisposed { get; }

        void Dispose();
    }
}