using System;
using System.IO;

namespace AssetManager.Helpers
{
    public static class Logging
    {
        public static void Logger(string Message)
        {
            try
            {
                short MaxLogSizeKiloBytes = 500;
                string DateStamp = DateTime.Now.ToString();
                FileInfo infoReader = null;
                infoReader = new FileInfo(Paths.LogPath);
                if (!File.Exists(Paths.LogPath))
                {
                    Directory.CreateDirectory(Paths.AppDir);
                    using (StreamWriter sw = File.CreateText(Paths.LogPath))
                    {
                        sw.WriteLine(DateStamp + ": Log Created...");
                        sw.WriteLine(DateStamp + ": " + Message);
                    }
                }
                else
                {
                    if ((infoReader.Length / 1000) < MaxLogSizeKiloBytes)
                    {
                        using (StreamWriter sw = File.AppendText(Paths.LogPath))
                        {
                            sw.WriteLine(DateStamp + ": " + Message);
                        }
                    }
                    else
                    {
                        if (RotateLogs())
                        {
                            using (StreamWriter sw = File.AppendText(Paths.LogPath))
                            {
                                sw.WriteLine(DateStamp + ": " + Message);
                            }
                        }
                    }
                }
            }
            catch
            {
                //Shhhh.
            }
        }

        public static void Exception(Exception ex)
        {
            Logger("ERROR: Type: " + ex.GetType().Name + "  #:" + ex.HResult + "  Message:" + ex.Message);
            Logger("STACK TRACE: " + ex.StackTrace);
        }

        private static bool RotateLogs()
        {
            try
            {
                File.Copy(Paths.LogPath, Paths.LogPath + ".old", true);
                File.Delete(Paths.LogPath);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}