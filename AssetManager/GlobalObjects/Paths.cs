using AssetManager.Data;
using System;
using System.Deployment.Application;

namespace AssetManager
{
    internal static class Paths
    {
        // Set to true to use deployment modules from the local repo for debugging/testing purposes.
        public static bool UseDebugModules = false;

        //Application paths
        public static readonly string AppDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\AssetManager\";

        public const string PsExecTempDir = @"C:\Temp\";
        public const string PsExecTempPath = @"C:\Temp\paexec.exe";
        public static readonly string PsExecPath = AppDomain.CurrentDomain.BaseDirectory + @"Tools\PsExec\paexec.exe";
        public static readonly string LocalModulesStore = AppDir + @"\DeploymentModulesStore\";
        public static readonly string RemoteSource = @"\\core.co.fairfield.oh.us\dfs1\fcdd\files\QA\Tools\Asset Management\Asset Manager\DeploymentModules\";
        public static readonly string DebugSource = @"C:\GitHub\AssetManagerCSharp\DeploymentModules\bin\";

        public static string RemoteModuleSource()
        {
            if (UseDebugModules)
            {
                return DebugSource;
            }
            else
            {
                return RemoteSource;
            }
        }

        


        public const string LogName = "log.log";
        public static readonly string LogPath = AppDir + LogName;

        public static readonly string DownloadPath = AppDir + @"temp\";
        //SQLite DB paths

        public static string SQLiteDBName
        {
            get { return "cache_" + ServerInfo.CurrentDataBase.ToString() + (!ApplicationDeployment.IsNetworkDeployed ? "_DEBUG" : "").ToString() + ".db"; }
        }

        public static string SQLitePath
        {
            get { return AppDir + @"SQLiteCache\" + SQLiteDBName; }
        }

        public static readonly string SQLiteDir = AppDir + @"SQLiteCache\";

        //Gatekeeper package paths
        public const string GKRemoteDir = @"\PSi\Gatekeeper\";

        public const string GKLocalInstallDir = @"C:\PSi\Gatekeeper";
        public const string GKPackFileName = "GatekeeperPack.gz";
        public const string GKPackHashName = "hash.md5";
        public static readonly string GKPackFileFDir = AppDir + @"GKUpdateFiles\PackFile\";
        public static readonly string GKPackFileFullPath = GKPackFileFDir + GKPackFileName;
        public static readonly string GKExtractDir = AppDir + @"GKUpdateFiles\Gatekeeper\";
        public const string GKRemotePackFileDir = @"\\core.co.fairfield.oh.us\dfs1\fcdd\files\Information Technology\Software\Other\GatekeeperPackFile\";

        public const string GKRemotePackFilePath = GKRemotePackFileDir + GKPackFileName;
    }
}