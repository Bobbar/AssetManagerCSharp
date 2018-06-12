namespace RemoteFileTransferTool
{
    public struct TransferStatus
    {
        public int CurrentFileIdx { get; }
        public string DestinationFileName { get; }
        public int CurrentFileProgress { get; set; }
        public double CurrentTransferRate { get; set; }
        public string SourceFileName { get; }
        public int TotalFileCount { get; }

        public TransferStatus(int totalFileCount, int currentFileIndex, string destinationFileName, string sourceFileName, int currentProgress, double currentTransferRate)
        {
            TotalFileCount = totalFileCount;
            CurrentFileIdx = currentFileIndex;
            DestinationFileName = destinationFileName;
            SourceFileName = sourceFileName;
            CurrentFileProgress = currentProgress;
            CurrentTransferRate = currentTransferRate;
        }
    }
}