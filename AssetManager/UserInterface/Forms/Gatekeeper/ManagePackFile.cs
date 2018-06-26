using AssetManager.Helpers;
using AssetManager.Security;
using SimpleProgressCounter;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AssetManager.UserInterface.Forms.Gatekeeper
{
    public class ManagePackFile
    {
        private bool cancelCopy = false;

        public ProgressCounter Progress;

        public event EventHandler<string> StatusMessage;

        public ManagePackFile()
        {
            Progress = new ProgressCounter();
        }

        private void OnStatusMessage(string message)
        {
            StatusMessage?.Invoke(this, message);
        }

        /// <summary>
        /// Creates and cleans the pack file directories then downloads a new pack file from the server location.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DownloadPack()
        {
            try
            {
                if (!Directory.Exists(Paths.GKPackFileFDir))
                {
                    Directory.CreateDirectory(Paths.GKPackFileFDir);
                }

                if (Directory.Exists(Paths.GKExtractDir))
                {
                    Directory.Delete(Paths.GKExtractDir, true);
                }

                if (File.Exists(Paths.GKPackFileFullPath))
                {
                    File.Delete(Paths.GKPackFileFullPath);
                }
                Progress = new ProgressCounter();
                return await CopyPackFile(Paths.GKRemotePackFilePath, Paths.GKPackFileFullPath);
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
                return false;
            }
        }

        /// <summary>
        /// Verifies directory structure, checks if pack file is present, then compares local and remote hashes of the pack file.
        ///
        /// Returns False if directory or file is missing, or if the hashes mismatch.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> VerifyPackFile()
        {
            try
            {
                if (!Directory.Exists(Paths.GKPackFileFDir))
                {
                    return false;
                }
                if (!Directory.Exists(Paths.GKExtractDir))
                {
                    return false;
                }
                if (!File.Exists(Paths.GKPackFileFullPath))
                {
                    return false;
                }
                else
                {
                    string localHash = await Task.Run(() => { return SecurityTools.GetMD5OfFile(Paths.GKPackFileFullPath); });

                    string remoteHash = await Task.Run(() => { return GetRemoteHash(); });

                    if (localHash == remoteHash)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (DirectoryNotFoundException dnfe)
            {
                Logging.Exception(dnfe);
                return false;
            }
            catch (FileNotFoundException fnfe)
            {
                Logging.Exception(fnfe);
                return false;
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
                return false;
            }
        }

        /// <summary>
        /// Returns the contents of the hash text file located in <see cref="Paths.GKRemotePackFileDir"/>
        /// </summary>
        /// <returns></returns>
        private string GetRemoteHash()
        {
            using (StreamReader sr = new StreamReader(Paths.GKRemotePackFileDir + Paths.GKPackHashName))
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// Copies a single file to the <paramref name="dest"/> path.
        /// </summary>
        /// <param name="source"></param>
        /// Path of source file.
        /// <param name="dest"></param>
        /// Path of destination.
        /// <returns></returns>
        public async Task<bool> CopyPackFile(string source, string dest)
        {
            if (File.Exists(dest))
            {
                File.Delete(dest);
            }
            return await Task.Run(() =>
            {
                try
                {
                    CopyFile(source, dest);
                    return true;
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    return false;
                }
            });
        }

        public void CancelCopy()
        {
            cancelCopy = true;
        }

        /// <summary>
        /// Performs a buffered file stream transfer.
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Dest"></param>
        private void CopyFile(string Source, string Dest)
        {
            int bufferSize = 256000;
            byte[] buffer = new byte[bufferSize];
            int bytesIn = 1;
            var currentFile = new FileInfo(Source);
            Progress.ResetProgress();
            using (FileStream source = currentFile.OpenRead())
            using (var dest = new FileStream(Dest, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write, bufferSize, FileOptions.None))
            {
                Progress.BytesToTransfer = (int)source.Length;
                while (!(bytesIn < 1) && !cancelCopy)
                {
                    bytesIn = source.Read(buffer, 0, bufferSize);
                    if (bytesIn > 0)
                    {
                        dest.Write(buffer, 0, bytesIn);
                        Progress.BytesMoved = bytesIn;
                    }
                }
                buffer = null;
            }
        }

        /// <summary>
        /// Compresses the local Gatekeeper directory into a new pack file.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> PackGKDir()
        {
            try
            {
                Progress = new ProgressCounter();
                var gzip = new GZipCompress(Progress);
                if (!Directory.Exists(Paths.GKPackFileFDir))
                {
                    Directory.CreateDirectory(Paths.GKPackFileFDir);
                }

                if (File.Exists(Paths.GKPackFileFullPath))
                {
                    File.Delete(Paths.GKPackFileFullPath);
                }
                await Task.Run(() => { gzip.CompressDirectory(Paths.GKLocalInstallDir, Paths.GKPackFileFullPath); });
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
                return false;
            }
        }

        /// <summary>
        /// Decompresses the pack file into a local working directory.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> UnPackGKDir()
        {
            try
            {
                OnStatusMessage("Unpacking....");
                Progress = new ProgressCounter();
                var gzip = new GZipCompress(Progress);
                await Task.Run(() => { gzip.DecompressToDirectory(Paths.GKPackFileFullPath, Paths.GKExtractDir); });
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
                return false;
            }
        }

        /// <summary>
        /// Copies the pack file and hash file to the server directory.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> UploadPackFiles()
        {
            bool done = false;
            OnStatusMessage("Uploading Pack File...");
            Progress = new ProgressCounter();
            done = await CopyPackFile(Paths.GKPackFileFullPath, Paths.GKRemotePackFilePath);

            OnStatusMessage("Uploading Hash File...");
            Progress = new ProgressCounter();
            done = await CopyPackFile(Paths.GKPackFileFDir + Paths.GKPackHashName, Paths.GKRemotePackFileDir + Paths.GKPackHashName);
            return done;
        }

        /// <summary>
        /// Verifies the local pack file and downloads a new one if needed.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ProcessPackFile()
        {
            bool packFileOK = false;
            OnStatusMessage("Verifying Pack File...");
            if (await VerifyPackFile() && !cancelCopy)
            {
                packFileOK = await UnPackGKDir();
            }
            else
            {
                OnStatusMessage("Downloading Pack File...");
                if (await DownloadPack() && !cancelCopy)
                {
                    packFileOK = await UnPackGKDir();
                }
            }

            if (packFileOK)
            {
                OnStatusMessage("Done.");
                await Task.Delay(1000);
                return true;
            }
            else
            {
                if (cancelCopy)
                {
                    OnStatusMessage("Canceled.");
                    return false;
                }
                OnStatusMessage("ERROR!");
                return false;
            }
        }

        /// <summary>
        /// Creates a new pack file and hash file and copies them to the server location.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CreateNewPackFile()
        {
            try
            {
                bool success = false;
                OnStatusMessage("Creating Pack File...");
                Progress = new ProgressCounter();
                success = await PackGKDir();

                OnStatusMessage("Generating Hash...");
                success = await CreateHashFile();

                success = await UploadPackFiles();

                if (success)
                {
                    OnStatusMessage("Done.");
                }
                else
                {
                    OnStatusMessage("Something went wrong...");
                }
                return success;
            }
            catch (Exception ex)
            {
                OnStatusMessage("ERROR!");
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
                return false;
            }
        }

        /// <summary>
        /// Creates a text file containing the hash string of the pack file.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> CreateHashFile()
        {
            if (File.Exists(Paths.GKPackFileFDir + Paths.GKPackHashName))
            {
                File.Delete(Paths.GKPackFileFDir + Paths.GKPackHashName);
            }
            object Hash = await Task.Run(() => { return SecurityTools.GetMD5OfFile(Paths.GKPackFileFullPath); });
            using (StreamWriter sw = File.CreateText(Paths.GKPackFileFDir + Paths.GKPackHashName))
            {
                sw.Write(Hash);
            }
            return true;
        }
    }
}