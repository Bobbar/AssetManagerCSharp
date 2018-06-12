using System;

namespace RemoteFileTransferTool
{
    public class TransferCompleteResult
    {
        private Exception exception;

        private bool hasErrors;

        public TransferCompleteResult(bool hasErrors, Exception ex = null)
        {
            this.hasErrors = hasErrors;
            this.exception = ex;
        }

        public Exception Errors
        {
            get { return exception; }
        }

        public bool HasErrors
        {
            get { return hasErrors; }
        }
    }
}