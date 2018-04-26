using AssetManager.Data;
using AssetManager.Data.Functions;
using AssetManager.Helpers;
using AssetManager.Security;
using System;
using System.Threading;
using System.Threading.Tasks;
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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += MyApplication_UnhandledException;

            bool connectionSuccessful = false;
            bool cacheAvailable = false;

            try
            {
                NetworkInfo.LocalDomainUser = Environment.UserName;

                ChildFormControl.SplashScreenInstance().Show();

                ProcessCommandArgs();

                Logging.Logger("Starting AssetManager...");

                Status("Checking Server Connection...");
                connectionSuccessful = CheckConnection();
                ServerInfo.ServerPinging = connectionSuccessful;

                Status("Checking Cache State...");
                cacheAvailable = DBCacheFunctions.CacheUpToDate(connectionSuccessful);

                // If connected to DB and cache is out-of-date, rebuild it.
                if (connectionSuccessful && !cacheAvailable)
                {
                    Status("Building Cache DB...");

                    Task.Run(() =>
                    {
                        DBCacheFunctions.RefreshLocalDBCache();
                    }).Wait();

                }

                // No DB connection and cache not ready. Don't run.
                if (!connectionSuccessful & !cacheAvailable)
                {
                    OtherFunctions.Message("Could not connect to server and the local DB cache is unavailable.  The application will now close.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "No Connection");
                    Application.Exit();
                    return;
                }
                // No DB connection but cache is ready. Prompt and run.
                else if (!connectionSuccessful & cacheAvailable)
                {
                    GlobalSwitches.CachedMode = true;
                    OtherFunctions.Message("Could not connect to server. Running from local DB cache.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Cached Mode");
                }

                // Try to populate access groups and user from DB.
                Status("Checking Access Level...");
                SecurityTools.PopulateAccessGroups();
                SecurityTools.PopulateUserAccess();

                // Make sure the current user is allowed to run the software.
                if (!SecurityTools.CanAccess(SecurityGroups.CanRun))
                {
                    OtherFunctions.Message("You do not have permission to run this software.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Access Denied");
                    Application.Exit();
                    return;
                }

                Status("Caching Attributes...");
                AttributeFunctions.PopulateAttributeIndexes();

                Status("Collecting Field Info...");
                DBControlExtensions.GetFieldLengths();

                Status("Ready!");
                Application.Run(new UserInterface.Forms.AssetManagement.MainForm());
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
                Application.Exit();
            }
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