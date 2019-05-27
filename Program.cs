using Microsoft.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;

namespace endiffo
{
    static class Program
    {
        /// Get a string containing the exact output of the printenv command.
        /// This so far seems to work the exact same way on Windows and Linux.
        static string PrintEnv()
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Constants.ENVIRON_VAR_COMMAND,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            string result = process.StandardOutput.ReadToEnd();

            // TODO The application could potentially freeze anywhere it executes an external command.
            process.WaitForExit();
            return result;
        }

        /// Creates empty config file.
        static void CreateEmptyConfigFile()
        {
            ConfigFile config = new ConfigFile(true, true, new List<string>());
            string configJsonStr = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText("endiffo.json", configJsonStr);
        }

        static void RegeditExportKey(string key, string exportFile)
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

        static void HandleRegistry(string endiffoTempPath, List<string> registryKeys)
        {
            var regKeyInfo = new List<RegistryKeyInfo>();
            foreach (string key in registryKeys)
            {
                string filename = System.Guid.NewGuid() + ".reg";
                RegeditExportKey(key, Path.Join(endiffoTempPath, filename));
                regKeyInfo.Add(new RegistryKeyInfo(key, filename));
            }

            string regKeyJsonStr = JsonConvert.SerializeObject(regKeyInfo, Formatting.Indented);
            File.WriteAllText(Path.Join(endiffoTempPath, "keys.json"), regKeyJsonStr);
        }

        /// Start a command-line application which saves a snapshot of the system in a zip file.
        static void Main(string[] args)
        {
            try
            { 
                new Worker.Engine(new string[] { "SimpleScan" }, @"C:\Test\Output.zip");
                return;

                var app = new CommandLineApplication();
                var configOption = app.Option(
                    Constants.CMDLINE_OPT_CONFIG,
                    "Determines the file to load settings from.",
                    CommandOptionType.SingleValue);

                var outputOption = app.Option(
                    Constants.CMDLINE_OPT_OUTPUT,
                    "Determines the file to save a snapshot to.",
                    CommandOptionType.SingleValue);

                app.OnExecute(() =>
                {
                    string endiffoTempPath = Path.Join(Utility.GetTempFolder(), ".endiffo");

                    if (Directory.Exists(endiffoTempPath))
                        Utility.CleanDirectory(endiffoTempPath);
                    else
                        Directory.CreateDirectory(endiffoTempPath);

                    var configFilename = configOption.HasValue()
                        ? configOption.Value()
                        : Constants.DEFAULT_CONFIG_FILENAME;
                    var configJsonStr = File.ReadAllText(configFilename);
                    var config = JsonConvert.DeserializeObject<ConfigFile>(configJsonStr);

                    // Todo: Use streamWriter
                    if (config.EnvVars)
                        File.WriteAllText(Path.Join(endiffoTempPath, "printenv"), PrintEnv());
                    if (config.Hosts)
                        File.Copy(Utility.GetHostsFilePath(), Path.Join(endiffoTempPath, "hosts"));
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        HandleRegistry(endiffoTempPath, config.RegistryKeys);
                    //    RegistryHandler.GetKeys(Path.Join(endiffoTempPath, "registry.txt"));

                    string outputPath = outputOption.HasValue()
                        ? outputOption.Value()
                        : Utility.GetSnapshotFileName();

                    if (string.IsNullOrWhiteSpace(outputPath)
                        || outputPath.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                        throw new ArgumentException("Invalid output path.");

                    ZipFile.CreateFromDirectory(endiffoTempPath, outputPath);

                    Utility.CleanDirectory(endiffoTempPath);

                    return 0;
                });

                app.Execute(args);                
            }
            catch(Exception ex)
            {
                Console.WriteLine (
                    "An error occurred and the application had to terminate." + Environment.NewLine
                    + "Error text: " + ex.Message + Environment.NewLine
                    //+ "Inner exception: " + ex.InnerException + Environment.NewLine
                    + "Stack trace: " + Environment.NewLine + ex.StackTrace
                );
            }
        }
    }
}
