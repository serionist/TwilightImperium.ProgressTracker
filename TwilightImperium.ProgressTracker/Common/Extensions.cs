using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TwilightImperium.ProgressTracker
{
    public static class Extensions
    {
        #region Encrypter



        static readonly string SaltKey = "S@1T&K3YL0L!$$&";
        static readonly string VIKey = "@1B2c3D4e5F6i7H8";
        private static string _passwordHash = "P@@Sw0rD!LoL";
        private static string PasswordHash
        {
            get { return _passwordHash; }
            set { _passwordHash = value; }
        }
        public static string Encrypt(this string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return null;
            }
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }
        public static string Decrypt(this string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText)) return null;
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }
        #endregion

        #region fileSize

        public static long TotalKbytes(long bytes) { return Convert.ToInt64(bytes / 1024); }
        public static long TotalMbytes(long bytes) { return Convert.ToInt64(bytes / 1048576); }
        public static long TotalGbytes(long bytes) { return Convert.ToInt64(bytes / 1073741824); }

        /// <summary>
        /// How string should write byte. Default: "b"
        /// </summary>
        public static string byteFormat = "b";

        /// <summary>
        /// Returns the long string representation. Example: 4GB 5MB 3KB 2B
        /// </summary>
        /// <param name="showByte"></param>
        /// <param name="showKbyte"></param>
        /// <param name="showMbyte"></param>
        /// <param name="showGbyte"></param>
        /// <returns></returns>
        public static string FileSizeToLongString(this long size, bool showByte = true, bool showKbyte = true, bool showMbyte = true, bool showGbyte = true)
        {
            return ToLongString(size, showByte, showKbyte, showMbyte, showGbyte);
        }
        /// <summary>
        /// Returns the long string representation. Example: 4GB 5MB 3KB 2B
        /// </summary>
        /// <param name="showByte"></param>
        /// <param name="showKbyte"></param>
        /// <param name="showMbyte"></param>
        /// <param name="showGbyte"></param>
        /// <returns></returns>
        public static string FileSizeToLongString(this int size, bool showByte = true, bool showKbyte = true, bool showMbyte = true, bool showGbyte = true)
        {
            return ToLongString((long)size, showByte, showKbyte, showMbyte, showGbyte);
        }
        private static string ToLongString(long size, bool showByte = true, bool showKbyte = true, bool showMbyte = true, bool showGbyte = true)
        {
            string ret = "";
            if (TotalGbytes(size) > 0 && showGbyte) ret += TotalGbytes(size).ToString() + "G" + byteFormat + " ";
            if (TotalMbytes(size) > 0 && showMbyte) ret += TotalMbytes(size).ToString() + "M" + byteFormat + " ";
            if (TotalKbytes(size) > 0 && showKbyte) ret += TotalKbytes(size).ToString() + "K" + byteFormat + " ";
            if (size > 0 && showByte) ret += size.ToString() + byteFormat;
            return ret;
        }


        public static string FileSizeToShortString(this long size, int decimals = 2)
        {
            return ToShortString(size, decimals);
        }
        public static string FileSizeToShortString(this int size, int decimals = 2)
        {
            return ToShortString((long)size, decimals);
        }
        private static string ToShortString(long filesize, int decimals = 2)
        {
            double ret;
            string pre = "";
            if (TotalGbytes(filesize) > 0)
            {
                ret = Math.Round((double)((double)filesize / (double)1073741824), decimals);
                pre = "G";
            }
            else if (TotalMbytes(filesize) > 0)
            {
                ret = Math.Round((double)((double)filesize / (double)1048576), decimals);
                pre = "M";
            }
            else if (TotalKbytes(filesize) > 0)
            {
                ret = Math.Round((double)((double)filesize / (double)1024), decimals);
                pre = "K";
            }
            else ret = filesize;

            string r = ret.ToString();
            int digits = !r.Contains(".") ? 0 : r.Length - (r.LastIndexOf(".") + 1);
            int needDigits = decimals - digits;
            if (needDigits > 0)
            {
                if (!r.Contains(".")) r += ".";
                for (int i = 0; i < needDigits; i++) r += "0";
            }
            return r + " " + pre + byteFormat;
        }

        #endregion
    }
}
