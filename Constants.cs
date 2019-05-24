
namespace endiffo
{
    public static class Constants
    {
        // TODO: System drive may not be C:/
        public const string WINDOWS_HOSTS_FILE = "C:/Windows/System32/drivers/etc/hosts";
        public const string HOSTS_FILE = "/etc/hosts";
        public const string ENVIRON_VAR_COMMAND = "printenv";
        public const string DEFAULT_SNAPSHOT_FILENAME_BASE = "snapshotUTC_";
        public const string DEFAULT_SNAPSHOT_EXTENSION = "endiffo";
        public const string DATETIME_FORMAT = "yyyy-MM-ddTHH.mm.ssZ";
        public const string OUTPUT_CMDLINE_FLAG = "-o |--output";
    }
}