using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Endiffo
{
    public static class Utility
    {
        /// <summary>
        /// Encodes any string into base64 which can safely be included in a JSON string, URL, etc.
        /// </summary>
        /// <param name="unencoded">The unencoded input string.</param>
        /// <returns>The base64 encoded string.</returns>
        public static string EncodeToBase64(string unencoded)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(unencoded));
        }

        /// <summary>
        /// Handy function to create an empty config file.
        /// </summary>
        static void CreateEmptyConfigFile()
        {
            ConfigFile config = new ConfigFile(true, true, new List<string>());
            string configJsonStr = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText("endiffo.json", configJsonStr);
        }
        
        /// <summary>
        /// Determine the filename of the zip we are saving everything in (minus the path)
        /// </summary>
        /// <returns></returns>
        public static string GetSnapshotFileName()
        {
            return
                Constants.DEFAULT_SNAPSHOT_FILENAME_BASE
                + DateTime.UtcNow.ToString(Constants.DATETIME_FORMAT)
                + "."
                + Constants.DEFAULT_SNAPSHOT_EXTENSION;
        }

        /// <summary>
        /// Deletes all files and folders inside a directory.
        /// Be careful with this!
        /// </summary>
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

        /// <summary>
        /// Determine the path of the temp folder to save files in before zipping.
        /// Might not be needed once we stream data directly into the zip file.
        /// </summary>
        /// <returns>The temp folder path.</returns>
        public static string GetTempFolder()
        {
            return 
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? Path.GetTempPath()
                : "/tmp/";
        }

        /// <summary>
        /// Get Endiffo's temp path.
        /// </summary>
        /// <returns>Endiffo's temp path.</returns>
        public static string GetEndiffoTempPath()
        {
            return Path.Join(Utility.GetTempFolder(), Constants.ENDIFFO_TEMP_FOLDER);
        }
    };
}