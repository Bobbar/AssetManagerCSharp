using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AssetManager.Data
{
    internal static class DataConsistency
    {
        public const string DBDateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        ///  Returns a <see cref="string.Empty"/> if the value is a <see cref="DBNull"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string NoNull(object value)
        {
            if (value == DBNull.Value)
            {
                return string.Empty;
            }
            else
            {
                return value.ToString();
            }
        }

        /// <summary>
        /// Trims, removes LF and CR chars and returns a DBNull if string is empty.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object CleanDBValue(string value)
        {
            if (value == null) return DBNull.Value;

            string cleanString = Regex.Replace(value.Trim(), "[/\\r?\\n|\\r]+", string.Empty);

            if (cleanString == string.Empty)
            {
                return DBNull.Value;
            }
            else
            {
                return cleanString;
            }
        }

        public static bool ValidPhoneNumber(string value)
        {
            if (!string.IsNullOrEmpty(value.Trim()))
            {
                const int nDigits = 10;
                string phoneNum = "";
                char[] numArray = value.ToCharArray();

                foreach (char num in numArray)
                {
                    if (char.IsDigit(num)) phoneNum += num.ToString();
                }

                if (phoneNum.Length == nDigits)
                {
                    return true;
                }

                return false;
            }
            else
            {
                return true;
            }
        }

        public static string DeviceHostnameFormat(string serial)
        {
            return "D" + serial.Trim();
        }

        public static bool IsValidYear(string year)
        {
            try
            {
                if (!string.IsNullOrEmpty(year.Trim()))
                {
                    if (Enumerable.Range(1900, 200).Contains(Convert.ToInt32(year)))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}