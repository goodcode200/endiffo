
namespace endiffo
{
    public static class Constants
    {
        // TODO: System drive may not be C:/
        public const string WINDOWS_HOSTS_FILE_PATH = "C:/Windows/System32/drivers/etc/hosts";
        public const string LINUX_HOSTS_FILE_PATH = "/etc/hosts";
        public const string ENVIRON_VAR_COMMAND = "printenv";
        public const string REGEDIT_COMMAND = "reg";

        public const string DEFAULT_CONFIG_FILENAME = "endiffo.json";

        /// The beginning of the default snapshot filename.
        /// Is followed by the DATETIME_FORMAT defined below.
        public const string DEFAULT_SNAPSHOT_FILENAME_BASE = "snapshot_UTC_";
        public const string DEFAULT_SNAPSHOT_EXTENSION = "zip";

        /// Use dots instead of colons in a filename, or Windows complains.
        public const string DATETIME_FORMAT = "yyyy-MM-ddTHH.mm.ssZ";

        /// Command-line options
        public const string CMDLINE_OPT_CONFIG = "-c |--config";
        public const string CMDLINE_OPT_OUTPUT = "-o |--output";
    }
}