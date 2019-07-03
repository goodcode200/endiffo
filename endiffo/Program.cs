using Microsoft.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Endiffo
{
    public static class Program
    {
        /// <summary>
        /// Start a command-line application which saves a snapshot of the system in a zip file.
        /// </summary>
        /// <param name="args">Parameters for options like specifying files.</param>
        static void Main(string[] args)
        {
            Run(args);
        }

        public static (bool, string) Run(string[] args)
        {
            try
            { 
                var app = new CommandLineApplication();

                var configOption = app.Option(
                    Constants.CMDLINE_OPT_CONFIG,
                    "Determines the file to load settings from.",
                    CommandOptionType.SingleValue);

                var outputOption = app.Option(
                    Constants.CMDLINE_OPT_OUTPUT,
                    "Determines the file to save a snapshot to.",
                    CommandOptionType.SingleValue);

                string outputPath = null;

                app.OnExecute(() =>
                {
                    string endiffoTempPath = Utility.GetEndiffoTempPath();

                    if (Directory.Exists(endiffoTempPath))
                        Utility.CleanTempFolder();
                    else
                        Directory.CreateDirectory(endiffoTempPath);

                    var configFilename = configOption.HasValue()
                        ? configOption.Value()
                        : Constants.DEFAULT_CONFIG_FILENAME;

                    if (Path.GetInvalidPathChars().Any(x => configFilename.Contains(x)))
                        throw new ArgumentException("Config file path contains invalid characters.");

                    if (!File.Exists(configFilename))
                        throw new ArgumentException("Config file does not exist at the specified location.");

                    var config = Utility.ReadConfig(configFilename);

                    outputPath = outputOption.HasValue()
                        ? outputOption.Value()
                        : Utility.GetSnapshotFileName();

                    if (Path.GetInvalidPathChars().Any(x => outputPath.Contains(x)))
                        throw new ArgumentException("Output file path contains invalid characters.");

                    using (var engine = new Worker.Engine(
                        new string[] { "SimpleScan", "HostsScan" },
                        (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                            ? config.RegistryKeys
                            : null),
                        outputPath))
                    {}

                    Utility.CleanTempFolder();

                    return 0;
                });

                app.Execute(args);
                return (true, outputPath);
            }
            catch(Exception ex)
            {
                Console.WriteLine (
                    "An error occurred and the application had to terminate." + Environment.NewLine
                    + "Error text: " + ex.Message + Environment.NewLine
                    + "Stack trace: " + Environment.NewLine + ex.StackTrace
                );
                return (false, null);
            }
        }
    }
}
