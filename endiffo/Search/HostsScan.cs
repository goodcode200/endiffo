using System.IO;
using System.Runtime.InteropServices;

namespace endiffo.Search
{
    /// <summary>
    /// An instance of HostsScan retrieves the entire etc/hosts file.
    /// </summary>
    internal class HostsScan : ISearch
    {
        /// <summary>
        /// The filename to save this search result with.
        /// </summary>
        private readonly string Filename;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filename">The filename to store and then save this result with.</param>
        public HostsScan(string filename)
        {
            Filename = filename;
        }

        /// <summary>
        /// The hosts file isn't parsed yet.
        /// </summary>
        public void GenerateResults() {}

        /// <summary>
        /// Get the filename to save this search result with.
        /// </summary>
        /// <returns>The stored filename</returns>
        public string GetFilename()
        {
            return Filename;
        }

        /// <summary>
        /// Stream the hosts file.
        /// On Windows at least you cannot stream directly from the original location so it is copied to a temp folder first.
        /// </summary>
        /// <returns>A file stream of the hosts file.</returns>
        public Stream WriteResults()
        {
            string tempFilePath = Path.Join(Utility.GetEndiffoTempPath(), Filename);
            File.Copy(GetHostsFilePath(), tempFilePath);
            return new FileStream(tempFilePath, FileMode.Open);
        }

        /// <summary>
        /// Get the full path of the system's hosts file.
        /// </summary>
        /// <returns>The hosts file path string.</returns>
        private static string GetHostsFilePath()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? Constants.WINDOWS_HOSTS_FILE_PATH
                : Constants.LINUX_HOSTS_FILE_PATH;
        }
    }
}