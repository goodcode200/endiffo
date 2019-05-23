using System;
using System.Text;

namespace endiffo
{
    public static class Utility
    {
        // Reference: https://stackoverflow.com/questions/11743160/how-do-i-encode-and-decode-a-base64-string
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
    };
}