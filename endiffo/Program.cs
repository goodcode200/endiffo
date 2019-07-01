using Microsoft.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
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
            // Here we don't need to do anything with the result of Run.
            Run(args);
        }

        /// <summary>
        /// Encapsulates the whole program in a testable form.
        /// </summary>
        /// <param name="args"></param>
        /// <returns>
        /// A tuple used for tests, consisting of:
        /// 1. An integer indicating success or failure.
        /// 2. The snapshot filename. Is null if there was a failure.
        /// </returns>
        public static (bool, string) Run(string [] args)
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

                string endiffoTempPath = Utility.GetEndiffoTempPath();
                string outputFilename = null;

                app.OnExecute(() =>
                {
                    if (Directory.Exists(endiffoTempPath))
                        Utility.CleanTempFolder();
                    else
                        Directory.CreateDirectory(endiffoTempPath);

                    var configFilename = configOption.HasValue()
                        ? configOption.Value()
                        : Constants.DEFAULT_CONFIG_FILENAME;
                    var configJsonStr = File.ReadAllText(configFilename);
                    var config = JsonConvert.DeserializeObject<ConfigFile>(configJsonStr);

                    outputFilename = outputOption.HasValue()
                        ? outputOption.Value()
                        : Utility.GetSnapshotFileName();

                    using
                    (
                        new Worker.Engine
                        (
                            new string[] { "SimpleScan", "HostsScan" },
                            (
                                RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                                ? config.RegistryKeys
                                : null
                            ),
                            outputFilename
                        )
                    ) {}

                    Console.WriteLine("Saving to file: " + outputFilename);

                    Utility.CleanTempFolder();

                    return 0;
                });

                app.Execute(args);
                return (true, outputFilename);               
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
