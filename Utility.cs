using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace endiffo
{
    public static class Utility
    {
        /// Encodes any string into base64 which can safely be included in a JSON string, URL, etc.
        public static string EncodeToBase64(string unencoded)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(unencoded));
        }

        /// Determine the filename of the zip we are saving everything in (minus the path)
        public static string GetSnapshotFileName()
        {
            return
                Constants.DEFAULT_SNAPSHOT_FILENAME_BASE
                + DateTime.UtcNow.ToString(Constants.DATETIME_FORMAT)
                + "."
                + Constants.DEFAULT_SNAPSHOT_EXTENSION;
        }

        /// Deletes all files and folders inside a directory.
        /// Be careful with this!
        public static void CleanTempFolder()
        {
            string path = GetEndiffoTempPath();

            if (string.IsNullOrWhiteSpace(path)
                || path.IndexOfAny(Path.GetInvalidPathChars()) >= 0
                || (!Directory.Exists(path))
                )
                throw new ArgumentException("Invalid path.");

            System.IO.DirectoryInfo di = new DirectoryInfo(path);

            foreach (FileInfo file in di.EnumerateFiles())
            {
                file.Delete(); 
            }
            foreach (DirectoryInfo dir in di.EnumerateDirectories())
            {
                dir.Delete(true); 
            }
        }

        /// Determine the path of the temp folder to save files in before zipping.
        /// Might not be needed once we stream data directly into the zip file.
        public static string GetTempFolder()
        {
            return 
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? Path.GetTempPath()
                : "/tmp/";
        }

        public static string GetEndiffoTempPath()
        {
            return Path.Join(Utility.GetTempFolder(), Constants.ENDIFFO_TEMP_FOLDER);
        }
    };
}