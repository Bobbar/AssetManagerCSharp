using System.Net;
using System.IO;
using AssetManager.Security;

namespace AssetManager.Data.Communications
{

    public static class FtpComms
    {

        #region "Fields"

        private const string ftpPass = "BzPOHPXLdGu9CxaHTAEUCXY4Oa5EVM2B/G7O9En28LQ=";
        private const string ftpUser = "asset_manager";
        private static NetworkCredential ftpCreds = new NetworkCredential(ftpUser, SecurityTools.DecodePassword(ftpPass));

        private static int intSocketTimeout = 5000;
        #endregion

        #region "Methods"

        public static Stream ReturnFtpRequestStream(string uri, string method)
        {
            var request = (FtpWebRequest)FtpWebRequest.Create(uri);
            request.Proxy = new WebProxy();
            //set proxy to nothing to bypass .NET auto-detect process. This speeds up the initial connection greatly.
            request.Credentials = ftpCreds;
            request.Method = method;
            request.ReadWriteTimeout = intSocketTimeout;
            request.Timeout = intSocketTimeout;
            return request.GetRequestStream();
        }

        public static WebResponse ReturnFtpResponse(string uri, string method)
        {
            var request = (FtpWebRequest)FtpWebRequest.Create(uri);
            request.Proxy = new WebProxy();
            //set proxy to nothing to bypass .NET auto-detect process. This speeds up the initial connection greatly.
            request.Credentials = ftpCreds;
            request.Method = method;
            request.ReadWriteTimeout = intSocketTimeout;
            request.Timeout = intSocketTimeout;
            return request.GetResponse();
        }

        #endregion

    }
}
