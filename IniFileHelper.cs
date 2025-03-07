using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace RivalsAccountManager
{
    public static class IniFileHelper
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public static string ReadValue(string filePath, string section, string key, string defaultValue)
        {
            StringBuilder result = new StringBuilder(255);

            // Make sure the INI file exists
            EnsureDirectoryExists(filePath);

            int bytesReturned = GetPrivateProfileString(section, key, defaultValue, result, 255, filePath);
            return bytesReturned > 0 ? result.ToString() : defaultValue;
        }

        public static void WriteValue(string filePath, string section, string key, string value)
        {
            // Make sure the INI file exists
            EnsureDirectoryExists(filePath);

            WritePrivateProfileString(section, key, value, filePath);
        }

        private static void EnsureDirectoryExists(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(filePath))
            {
                // Create an empty INI file
                File.WriteAllText(filePath, string.Empty);
            }
        }
    }
}