using Database.Data;
using AssetManager.Data;
using AssetManager.Security;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Windows.Forms;


namespace AssetManager.Helpers
{
    internal static class ErrorHandling
    {
        //Suppress additional messages to user.

        private static bool suppressAdditionalMessages = false;

        public static bool ErrHandle(Exception ex, MethodBase method)//, bool suppressMessages = false)
        {
            //Recursive error handler. Returns False for undesired or dangerous errors, True if safe to continue.
            try
            {
                Logging.Logger("ERR STACK TRACE: " + ex.ToString());
                bool errorResult = false;

                if (ex is WebException)
                {
                    var WebEx = (WebException)ex;
                    errorResult = handleWebException(WebEx, method);
                }
                else if (ex is IndexOutOfRangeException)
                {
                    IndexOutOfRangeException handEx = (IndexOutOfRangeException)ex;
                    Logging.Logger("ERROR:  MethodName=" + method.Name + "  Type: " + ex.GetType().Name + "  #:" + handEx.HResult + "  Message:" + handEx.Message);
                    errorResult = true;
                }
                else if (ex.GetType().Name == "MySqlException")
                {
                    errorResult = handleMySQLException(ex, method);
                }
                else if (ex is SqlException)
                {
                    var SQLEx = (SqlException)ex;
                    errorResult = handleSQLException(SQLEx, method);
                }
                else if (ex.GetType().Name == "SQLiteException")
                {
                    errorResult = handleSQLiteException(ex, method);
                }
                else if (ex is InvalidCastException)
                {
                    InvalidCastException handEx = (InvalidCastException)ex;
                    Logging.Logger("CAST ERROR:  MethodName=" + method.Name + "  Type: " + ex.GetType().Name + "  #:" + handEx.HResult + "  Message:" + handEx.Message);
                    PromptUser("An object was cast to an unmatched type.  See log for details.  Log: " + Paths.LogPath, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Invalid Cast Error");
                    if (handEx.HResult == -2147467262)
                    {
                        errorResult = true;
                    }
                }
                else if (ex is IOException)
                {
                    var IOEx = (IOException)ex;
                    errorResult = handleIOException(IOEx, method);
                }
                else if (ex is PingException)
                {
                    var PingEx = (PingException)ex;
                    errorResult = handlePingException(PingEx, method);
                }
                else if (ex is SocketException)
                {
                    var SocketEx = (SocketException)ex;
                    errorResult = handleSocketException(SocketEx, method);
                }
                else if (ex is FormatException)
                {
                    var FormatEx = (FormatException)ex;
                    errorResult = handleFormatException(FormatEx, method);
                }
                else if (ex is Win32Exception)
                {
                    var Win32Ex = (Win32Exception)ex;
                    errorResult = handleWin32Exception(Win32Ex, method);
                }
                else if (ex is InvalidOperationException)
                {
                    var InvalidOpEx = (InvalidOperationException)ex;
                    errorResult = handleOperationException(InvalidOpEx, method);
                }
                else if (ex is NoPingException)
                {
                    var NoPingEx = (NoPingException)ex;
                    errorResult = handleNoPingException(NoPingEx, method);
                }
                else if (ex is NullReferenceException)
                {
                    var NullRefEx = (NullReferenceException)ex;
                    errorResult = handleNullReferenceException(NullRefEx, method);
                }
                else if (ex is NotImplementedException)
                {
                    var NotImplementedEx = (NotImplementedException)ex;
                    errorResult = handleNotImplementedException(NotImplementedEx, method);
                }
                else if (ex is TimeoutException)
                {
                    errorResult = handleTimeoutException((TimeoutException)ex, method);
                }
                else if (ex is InvalidAccessException)
                {
                    errorResult = handleInvalidAccessException((InvalidAccessException)ex, method);
                }
                else
                {
                    UnHandledError(ex, ex.HResult, method);
                    errorResult = false;
                }

                if (ex.InnerException != null)
                {
                    errorResult = ErrHandle(ex.InnerException, method);
                }
                return errorResult;
            }
            finally
            {
                suppressAdditionalMessages = false;
            }
        }

        private static bool handleInvalidAccessException(InvalidAccessException ex, MethodBase Method)
        {
            Logging.Logger("ERROR:  MethodName=" + Method.Name + "  Type: " + ex.GetType().Name + "  #:" + ex.HResult + "  Message:" + ex.Message);

            PromptUser(ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Access Denied");

            return true;
        }

        private static bool handleTimeoutException(TimeoutException ex, MethodBase Method)
        {
            Logging.Logger("ERROR:  MethodName=" + Method.Name + "  Type: " + ex.GetType().Name + "  #:" + ex.HResult + "  Message:" + ex.Message);
            return false;
        }

        private static bool handleNotImplementedException(NotImplementedException ex, MethodBase Method)
        {
            Logging.Logger("ERROR:  MethodName=" + Method.Name + "  Type: " + ex.GetType().Name + "  #:" + ex.HResult + "  Message:" + ex.Message);
            PromptUser("ERROR:  Method not implemented.  See log for details: file://" + Paths.LogPath, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Method Not Implemented");
            return true;
        }

        private static bool handleWin32Exception(Win32Exception ex, MethodBase Method)
        {
            Logging.Logger("ERROR: MethodName =" + Method.Name + "  Type: " + ex.GetType().Name + "  #:" + ex.NativeErrorCode + "  Message:" + ex.Message);
            switch (ex.NativeErrorCode)
            {
                case 1909:
                    //Locked account
                    PromptUser("ERROR:  MethodName=" + Method.Name + "  Type: " + ex.GetType().Name + "  #:" + ex.NativeErrorCode + "  Message:" + ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Network Error");
                    SecurityTools.ClearAdminCreds();
                    return true;

                case 1326:
                    //Bad credentials error. Clear AdminCreds
                    PromptUser("ERROR:  MethodName=" + Method.Name + "  Type: " + ex.GetType().Name + "  #:" + ex.NativeErrorCode + "  Message:" + ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Network Error");
                    SecurityTools.ClearAdminCreds();
                    return true;

                case 86:
                    //Bad credentials error. Clear AdminCreds
                    PromptUser("ERROR:  MethodName=" + Method.Name + "  Type: " + ex.GetType().Name + "  #:" + ex.NativeErrorCode + "  Message:" + ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Network Error");
                    SecurityTools.ClearAdminCreds();
                    return true;

                case 5:
                    //Access denied error. Clear AdminCreds
                    PromptUser("ERROR:  MethodName=" + Method.Name + "  Type: " + ex.GetType().Name + "  #:" + ex.NativeErrorCode + "  Message:" + ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Network Error");
                    SecurityTools.ClearAdminCreds();
                    return true;

                default:
                    PromptUser("ERROR:  MethodName=" + Method.Name + "  Type: " + ex.GetType().Name + "  #:" + ex.NativeErrorCode + "  Message:" + ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Network Error");
                    return true;
            }
        }

        private static bool handleFormatException(FormatException ex, MethodBase Method)
        {
            Logging.Logger("ERROR:  MethodName=" + Method.Name + "  Type: " + ex.GetType().Name + "  #:" + ex.HResult + "  Message:" + ex.Message);
            switch (ex.HResult)
            {
                case -2146233033:
                    return true;

                default:
                    UnHandledError(ex, ex.HResult, Method);
                    break;
            }
            return false;
        }

        private static bool handleMySQLException(Exception ex, MethodBase Method)
        {
            if (ex.Data.Count > 0)
            {
                Logging.Logger("MySQLException Data:");
                foreach (DictionaryEntry item in ex.Data)
                {
                    Console.WriteLine(item.Key.ToString() + " - " + item.Value.ToString());
                    Logging.Logger(item.Key.ToString() + " - " + item.Value.ToString());
                }
            }

            int errorNumber = 0;

            if (ex.Data.Contains("Server Error Code"))
            {
                errorNumber = Convert.ToInt32(ex.Data["Server Error Code"]);
            }

            Logging.Logger("ERROR:  MethodName=" + Method.Name + "  Type: " + ex.GetType().Name + "  #:" + errorNumber + "  Message:" + ex.Message);
            switch (errorNumber)
            {
                case 1042:
                    PromptUser("Unable to connect to server.  Check connection and try again.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Connection Lost");
                    suppressAdditionalMessages = true;
                    return true;

                case 0:
                    PromptUser("Unable to connect to server.  Check connection and try again.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Connection Lost");
                    suppressAdditionalMessages = true;
                    return true;

                case 1064:
                    PromptUser("Something went wrong with the SQL command. See log for details.  Log: " + Paths.LogPath, MessageBoxButtons.OK, MessageBoxIcon.Error, "SQL Syntax Error");
                    return true;

                case 1406:
                    PromptUser(ex.Message + Environment.NewLine + Environment.NewLine + "Log: " + Paths.LogPath, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "SQL Error");
                    return true;

                case 1292:
                    PromptUser("Something went wrong with the SQL command. See log for details.  Log: " + Paths.LogPath, MessageBoxButtons.OK, MessageBoxIcon.Error, "SQL Syntax Error");
                    return true;

                default:
                    UnHandledError(ex, errorNumber, Method);
                    break;
            }

            return false;
        }

        private static bool handlePingException(System.Net.NetworkInformation.PingException ex, MethodBase Method)
        {
            switch (ex.HResult)
            {
                case -2146233079:
                    return false;

                default:
                    UnHandledError(ex, ex.HResult, Method);
                    return false;
            }
        }

        private static bool handleOperationException(InvalidOperationException ex, MethodBase Method)
        {
            switch (ex.HResult)
            {
                case -2146233079:
                    Logging.Logger("UNHANDLED ERROR:  MethodName=" + Method.Name + "  Type: " + ex.GetType().Name + "  #:" + ex.HResult + "  Message:" + ex.Message);
                    return false;

                default:
                    UnHandledError(ex, ex.HResult, Method);
                    return false;
            }
        }

        private static bool handleWebException(WebException ex, MethodBase Method)
        {
            var handResponse = (FtpWebResponse)ex.Response;
            switch (handResponse.StatusCode)
            {
                case System.Net.FtpStatusCode.ActionNotTakenFileUnavailable:
                    PromptUser("FTP File was not found, or access was denied.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Cannot Access FTP File");
                    return true;

                default:
                    switch (ex.HResult)
                    {
                        case -2146233079:
                            Logging.Logger("ERROR:  MethodName=" + Method.Name + "  Type: " + ex.GetType().Name + "  #:" + ex.HResult + "  Message:" + ex.Message);
                            PromptUser("Could not connect to FTP server.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Connection Failure");
                            return true;
                    }
                    UnHandledError(ex, ex.HResult, Method);
                    break;
            }
            return false;
        }

        private static bool handleIOException(IOException ex, MethodBase Method)
        {
            Logging.Logger("ERROR:  MethodName=" + Method.Name + "  Type: " + ex.GetType().Name + "  #:" + ex.HResult + "  Message:" + ex.Message);
            switch (ex.HResult)
            {
                case -2147024864:
                    return true;

                case -2147024843: // Network path not found.
                    return true;

                case -2146232800:
                    return true;

                default:
                    UnHandledError(ex, ex.HResult, Method);
                    break;
            }
            return false;
        }

        private static bool handleSocketException(SocketException ex, MethodBase Method)
        {
            Logging.Logger("ERROR:  MethodName=" + Method.Name + "  Type: " + ex.GetType().Name + "  #:" + ex.SocketErrorCode + "  Message:" + ex.Message);
            switch (ex.SocketErrorCode)
            {
                //FTPSocket timeout
                case SocketError.TimedOut:
                    //10060
                    PromptUser("Lost connection to the server or the server took too long to respond.  See Log.  '" + Paths.LogPath + "'", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Network Socket Timeout");
                    return true;

                case SocketError.HostUnreachable:
                    //10065 'host unreachable
                    return true;

                case SocketError.ConnectionAborted:
                    //10053
                    PromptUser("Lost connection to the server or the server took too long to respond.  See Log.  '" + Paths.LogPath + "'", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Connection Aborted");
                    return true;

                case SocketError.ConnectionReset:
                    //10054 'connection reset
                    PromptUser("Lost connection to the server or the server took too long to respond.  See Log.  '" + Paths.LogPath + "'", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Connection Reset");
                    return true;

                case SocketError.NetworkUnreachable:
                    PromptUser("Could not connect to server.  See Log.  '" + Paths.LogPath + "'", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Network Unreachable");
                    return true;

                case SocketError.HostNotFound:
                    //11001 'host not found.
                    return false;

                default:
                    UnHandledError(ex, (int)ex.SocketErrorCode, Method);
                    return false;
            }
        }

        private static bool handleSQLException(SqlException ex, MethodBase Method)
        {
            Logging.Logger("ERROR:  MethodName=" + Method.Name + "  Type: " + ex.GetType().Name + "  #:" + ex.Number + "  Message:" + ex.Message);
            switch (ex.Number)
            {
                case 18456:
                    PromptUser("Error connecting to MUNIS Database.  Your username may not have access.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "MUNIS Error");
                    return false;

                case 102:
                    PromptUser("ERROR:  MethodName=" + Method.Name + "  Type: " + ex.GetType().Name + "  #:" + ex.Number + "  Message:" + ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "ERROR");
                    return false;

                case 121:
                    PromptUser("Could not connect to MUNIS database.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Network Error");
                    return false;

                case 245:
                    return false;

                case 248:
                    PromptUser("ERROR:  MethodName=" + Method.Name + "  Type: " + ex.GetType().Name + "  #:" + ex.Number + "  Message:" + ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "ERROR");
                    return false;

                case 53:
                    PromptUser("Could not connect to MUNIS database.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Network Error");
                    suppressAdditionalMessages = true;
                    return true;

                default:
                    UnHandledError(ex, ex.Number, Method);
                    return false;
            }
        }

        private static bool handleSQLiteException(Exception ex, MethodBase Method)
        {
            Logging.Logger("ERROR:  MethodName=" + Method.Name + "  Type: " + ex.GetType().Name + "  #:" + ex.HResult + "  Message:" + ex.Message);
            return true;
        }

        private static bool handleNoPingException(NoPingException ex, MethodBase Method)
        {
            switch (ex.HResult)
            {
                case -2146233088:
                    Logging.Logger("ERROR:  MethodName=" + Method.Name + "  Type: " + ex.GetType().Name + "  #:" + ex.HResult + "  Message:" + ex.Message);
                    PromptUser("Unable to connect to server.  Check connection and try again.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Connection Lost");
                    return true;

                default:
                    UnHandledError(ex, ex.HResult, Method);
                    return false;
            }
        }

        private static bool handleNullReferenceException(Exception ex, MethodBase Method)
        {
            Logging.Logger("ERROR:  MethodName=" + Method.Name + "  Type: " + ex.GetType().Name + "  #:" + ex.HResult + "  Message:" + ex.Message);
            if (ServerInfo.ServerPinging)
            {
                PromptUser("ERROR:  MethodName=" + Method.Name + "  Type: " + ex.GetType().Name + "  #:" + ex.HResult + "  Message:" + ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "ERROR");
                OtherFunctions.EndProgram(true);
                return false;
            }
            else
            {
                return true;
            }
        }

        private static void UnHandledError(Exception ex, int ErrorCode, MethodBase Method)
        {
            Logging.Logger("UNHANDLED ERROR:  MethodName=" + Method.Name + "  Type: " + ex.GetType().Name + "  #:" + ErrorCode + "  Message:" + ex.Message);
            PromptUser("UNHANDLED ERROR:  MethodName=" + Method.Name + "  Type: " + ex.GetType().Name + "  #:" + ErrorCode + "  Message:" + ex.Message + Environment.NewLine + Environment.NewLine + "file://" + Paths.LogPath, MessageBoxButtons.OK, MessageBoxIcon.Error, "ERROR");
            OtherFunctions.EndProgram(true);
        }

        private static void PromptUser(string Prompt, MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.Information, string Title = null, Form ParentFrm = null)
        {
            if (!suppressAdditionalMessages)
            {
                OtherFunctions.Message(Prompt, buttons, icon, Title, ParentFrm);
            }
        }
    }
}