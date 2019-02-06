using System;
using System.IO;

namespace AssetManager.Helpers
{
    public static class Logging
    {
        private const int _maxLogSize = 1000; // Kbytes

        public static void Logger(string message, string logName)
        {
            string fullPath = Paths.AppDir + logName;

            try
            {
                string dateStamp = DateTime.Now.ToString();
                FileInfo infoReader = null;
                infoReader = new FileInfo(fullPath);
                if (!File.Exists(fullPath))
                {
                    Directory.CreateDirectory(Paths.AppDir);
                    using (StreamWriter sw = File.CreateText(fullPath))
                    {
                        sw.WriteLine(dateStamp + ": Log Created...");
                        sw.WriteLine(dateStamp + ": " + message);
                    }
                }
                else
                {
                    if ((infoReader.Length / 1024) < _maxLogSize)
                    {
                        using (StreamWriter sw = File.AppendText(fullPath))
                        {
                            sw.WriteLine(dateStamp + ": " + message);
                        }
                    }
                    else
                    {
                        if (RotateLogs())
                        {
                            using (StreamWriter sw = File.AppendText(fullPath))
                            {
                                sw.WriteLine(dateStamp + ": " + message);
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

        public static void Logger(string message)
        {
            Logger(message, Paths.LogName);
        }

        public static void Exception(Exception ex)
        {
            Logger("ERROR: Type: " + ex.GetType().Name + "  #:" + ex.HResult + "  Message:" + ex.Message);
            Logger("STACK TRACE: " + ex.ToString());
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