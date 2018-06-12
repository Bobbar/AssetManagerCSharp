namespace RemoteFileTransferTool
{
    public struct LogMessage
    {
        public string Text { get; set; }
        public bool IsError { get; set; }

        public LogMessage(string text, bool isError)
        {
            Text = text;
            IsError = isError;
        }
    }
}