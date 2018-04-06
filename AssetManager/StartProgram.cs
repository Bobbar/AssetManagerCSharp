using AssetManager.Data;
using AssetManager.Data.Functions;
using AssetManager.Helpers;
using AssetManager.Security;
using System;
using System.Threading;
using System.Windows.Forms;

namespace AssetManager
{
    internal static class StartProgram
    {
        #region Methods

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            bool connectionSuccessful = false;
            bool cacheAvailable = false;

            NetworkInfo.LocalDomainUser = Environment.UserName;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += MyApplication_UnhandledException;

            Helpers.ChildFormControl.SplashScreenInstance().Show();

            ProcessCommandArgs();

            Logging.Logger("Starting AssetManager...");

            Status("Checking Server Connection...");
            connectionSuccessful = CheckConnection();
            ServerInfo.ServerPinging = connectionSuccessful;

            if (connectionSuccessful)
            {
                Status("Checking Access Level...");
                SecurityTools.PopulateAccessGroups();
                SecurityTools.GetUserAccess();

                if (!SecurityTools.CanAccess(SecurityGroups.CanRun))
                {
                    OtherFunctions.Message("You do not have permission to run this software.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Access Denied");
                    // e.Cancel = true;
                    Application.Exit();
                    return;
                }

                Status("Checking Local Cache...");
                if (!DBCacheFunctions.CacheUpToDate())
                {
                    Status("Building Cache DB...");
                    DBCacheFunctions.RefreshLocalDBCache();
                }
            }
            else
            {
                cacheAvailable = DBCacheFunctions.CacheUpToDate(connectionSuccessful);
            }

            if (!connectionSuccessful & !cacheAvailable)
            {
                OtherFunctions.Message("Could not connect to server and the local DB cache is unavailable.  The application will now close.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "No Connection");
                Application.Exit();
                return;
            }
            else if (!connectionSuccessful & cacheAvailable)
            {
                GlobalSwitches.CachedMode = true;
                OtherFunctions.Message("Could not connect to server. Running from local DB cache.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Cached Mode");
            }

            Status("Caching Attributes...");
            AttributeFunctions.PopulateAttributeIndexes();

            Status("Collecting Field Info...");
            DBControlExtensions.GetFieldLengths();

            Status("Ready!");
            Application.Run(new UserInterface.Forms.AssetManagement.MainForm());
        }

        private static bool CheckConnection()
        {
            try
            {
                using (var conn = DBFactory.GetMySqlDatabase().NewConnection())
                {
                    return DBFactory.GetMySqlDatabase().OpenConnection(conn, true);
                }
            }
            catch
            {
                return false;
            }
        }

        private static void MyApplication_UnhandledException(object sender, ThreadExceptionEventArgs e)
        {
            ErrorHandling.ErrHandle(e.Exception, System.Reflection.MethodBase.GetCurrentMethod());
        }

        private static void ProcessCommandArgs()
        {
            try
            {
                var args = Environment.GetCommandLineArgs();

                for (int i = 1; i <= args.Length - 1; i++)
                {
                    try
                    {
                        var ArgToEnum = (CommandArgs)CommandArgs.Parse(typeof(CommandArgs), (args[i]).ToUpper());
                        switch (ArgToEnum)
                        {
                            case CommandArgs.TESTDB:
                                ServerInfo.CurrentDataBase = Database.test_db;
                                break;

                            case CommandArgs.VINTONDD:
                                ServerInfo.CurrentDataBase = Database.vintondd;
                                break;
                        }
                    }
                    catch (ArgumentException)
                    {
                        Logging.Logger("Invalid argument: " + args[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        private static void Status(string text)
        {
            Helpers.ChildFormControl.SplashScreenInstance().SetStatus(text);
        }

        private enum CommandArgs
        {
            TESTDB,
            VINTONDD
        }

        #endregion Methods
    }
}