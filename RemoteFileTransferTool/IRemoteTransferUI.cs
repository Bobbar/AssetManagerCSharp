namespace RemoteFileTransferTool
{
    public interface IRemoteTransferUI
    {
        void LogMessage(LogMessage message);

        void StatusUpdate(TransferStatus status);

        void TransferComplete(TransferCompleteResult result);

        void TransferCanceled();
    }
}