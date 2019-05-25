using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace endiffo
{
    public static class Utility
    {
        public static string EncodeToBase64(string unencoded)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(unencoded));
        }

        public static string GetSnapshotFileName()
        {
            return
                Constants.DEFAULT_SNAPSHOT_FILENAME_BASE
                + DateTime.UtcNow.ToString(Constants.DATETIME_FORMAT)
                + "."
                + Constants.DEFAULT_SNAPSHOT_EXTENSION;
        }

        public static string GetTempFolder()
        {
            return 
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? "%temp%/"
                : "/tmp/";
        }

        // TODO If the hosts file is unexpectedly large, we might want to consider streaming it,
        // in which case this function signature will change.
        public static string GetHosts()
        {
            return File.ReadAllText(GetHostsFilename());
        }

        public static string GetHostsFilename()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? Constants.WINDOWS_HOSTS_FILE
                : Constants.HOSTS_FILE;
        }
    };
}