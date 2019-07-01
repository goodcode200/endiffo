
namespace Endiffo
{
    public static class Constants
    {
        /// <summary>
        /// TODO: System drive may not be C:/
        /// </summary>
        public const string WINDOWS_HOSTS_FILE_PATH = "C:/Windows/System32/drivers/etc/hosts";
        public const string LINUX_HOSTS_FILE_PATH = "/etc/hosts";
        public const string ENVIRON_VAR_COMMAND = "printenv";
        public const string REGEDIT_COMMAND = "reg";

        /// <summary>
        /// The place where external scripts can dump files before we can stream them.
        /// </summary>
        public const string ENDIFFO_TEMP_FOLDER = ".endiffo";
        public const string DEFAULT_CONFIG_FILENAME = "endiffo.json";

        /// <summary>
        /// The beginning of the default snapshot filename.
        /// Is followed by the DATETIME_FORMAT defined below.
        /// </summary>
        public const string DEFAULT_SNAPSHOT_FILENAME_BASE = "snapshot_UTC_";
        public const string DEFAULT_SNAPSHOT_EXTENSION = "zip";

        /// <summary>
        /// Use dots instead of colons in a filename, or Windows complains.
        /// </summary>
        public const string DATETIME_FORMAT = "yyyy-MM-ddTHH.mm.ssZ";

        /// <summary>
        /// Command-line options
        /// </summary>
        public const string CMDLINE_OPT_CONFIG = "-c |--config";
        public const string CMDLINE_OPT_OUTPUT = "-o |--output";
    }
}