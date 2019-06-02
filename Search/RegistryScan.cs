using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace endiffo.Search
{
    /// <summary>
    /// Scans for user-specified registry settings
    /// </summary>
    internal class RegistryScan : ISearch
    {
        /// <summary>
        /// Registry key to scan
        /// </summary>
        private string RegistryKey;

        /// <summary>
        /// File name to save result with.
        /// </summary>
        public string Filename;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="regKey">Initialise registry key to scan.</param>
        public RegistryScan(string regKey, string outputFilename)
        {
            RegistryKey = regKey;
            Filename = outputFilename;
        }

        /// <summary>
        /// Returns the filename to save the result with.
        /// </summary>
        /// <returns>Filename of search result.</returns>
        public string GetFilename()
        {
            return Filename;
        }

        /// <summary>
        /// No code in this implementation of GenerateResults()
        /// because creating a temporary .reg file should only be done when we want to call WriteResults()
        /// </summary>
        public void GenerateResults() {}
        
        public Stream WriteResults()
        {
            string filePath = Path.Join(Utility.GetEndiffoTempPath(), Filename);
            RegeditExportKey(RegistryKey, filePath);
            return new FileStream(filePath, FileMode.Open);
        }

        private static void RegeditExportKey(string key, string exportFile)
        {
            string argumentStr = "export \"" + key + "\" \"" + exportFile + "\"";

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Constants.REGEDIT_COMMAND,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments = argumentStr,
                }
            };

            Console.WriteLine("Running command " + Constants.REGEDIT_COMMAND + " " + argumentStr);

            process.Start();
            process.WaitForExit();
        }
    }
}