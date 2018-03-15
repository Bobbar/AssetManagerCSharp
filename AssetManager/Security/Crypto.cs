using System;
using System.Security.Cryptography;
using AssetManager.Helpers;

namespace AssetManager.Security
{
    public class Simple3Des : IDisposable
    {
        public Simple3Des(string key)
        {
            // Initialize the crypto provider.
            TripleDes.Key = TruncateHash(key, TripleDes.KeySize / 8);
            TripleDes.IV = TruncateHash("", TripleDes.BlockSize / 8);
        }

        private TripleDESCryptoServiceProvider TripleDes = new TripleDESCryptoServiceProvider();

        private byte[] TruncateHash(string key, int length)
        {
            using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
            {
                // Hash the key.
                byte[] keyBytes = System.Text.Encoding.Unicode.GetBytes(key);
                byte[] hash = sha1.ComputeHash(keyBytes);
                // Truncate or pad the hash.
                Array.Resize(ref hash, length);
                return hash;
            }
        }

        public string EncryptData(string plaintext)
        {
            // Convert the plaintext string to a byte array.
            byte[] plaintextBytes = System.Text.Encoding.Unicode.GetBytes(plaintext);
            // Create the stream.
            // Create the encoder to write to the stream.
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            using (CryptoStream encStream = new CryptoStream(ms, TripleDes.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write))
            {
                // Use the crypto stream to write the byte array to the stream.
                encStream.Write(plaintextBytes, 0, plaintextBytes.Length);
                encStream.FlushFinalBlock();
                // Convert the encrypted stream to a printable string.
                return Convert.ToBase64String(ms.ToArray());
            }

        }

        public string DecryptData(string encryptedtext)
        {
            try
            {
                // Convert the encrypted text string to a byte array.
                byte[] encryptedBytes = Convert.FromBase64String(encryptedtext);
                // ' Create the stream.
                // Create the decoder to write to the stream.
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                using (CryptoStream decStream = new CryptoStream(ms, TripleDes.CreateDecryptor(), System.Security.Cryptography.CryptoStreamMode.Write))
                {
                    // Use the crypto stream to write the byte array to the stream.
                    decStream.Write(encryptedBytes, 0, encryptedBytes.Length);
                    decStream.FlushFinalBlock();
                    // Convert the plaintext stream to a string.
                    encryptedBytes = null;
                    return System.Text.Encoding.Unicode.GetString(ms.ToArray());
                }

            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
                return null;
            }
        }

        #region "IDisposable Support"

        // To detect redundant calls
        private bool disposedValue;

        // IDisposable
        protected void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    TripleDes.Dispose();
                }
            }
            disposedValue = true;
        }
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion "IDisposable Support"
    }
}