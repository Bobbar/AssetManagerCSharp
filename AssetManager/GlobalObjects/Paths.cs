using AssetManager.Data;
using System;
using System.Deployment.Application;

namespace AssetManager
{
    internal static class Paths
    {
        //Application paths
        public static readonly string AppDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\AssetManager\";

        public const string PsExecTempDir = @"C:\Temp\";
        public const string PsExecTempPath = @"C:\Temp\paexec.exe";
        public static readonly string PsExecPath = AppDomain.CurrentDomain.BaseDirectory + @"Tools\PsExec\paexec.exe";
        public const string DeploymentScripts = @"\\core.co.fairfield.oh.us\dfs1\fcdd\files\Information Technology\Software\Deployments\DeploymentScripts\";

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
        public const string GKRemotePackFileDir = @"\\core.co.fairfield.oh.us\dfs1\fcdd\files\Information Technology\Software\Other\GatekeeperPackFile\";

        public const string GKRemotePackFilePath = GKRemotePackFileDir + GKPackFileName;
    }
}