using Microsoft.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;

namespace endiffo
{
    static class Program
    {
        // The printenv command should work on both Windows and Linux.
        // If there are any errors this function can return null.
        static string PrintEnv()
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Constants.ENVIRON_VAR_COMMAND,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            string result = process.StandardOutput.ReadToEnd();

            // TODO The application could potentially freeze here.
            process.WaitForExit();
            return result;
        }

        static void Main(string[] args)
        {
            try
            {
                var app = new CommandLineApplication();

                var outputOption = app.Option(
                    Constants.OUTPUT_CMDLINE_FLAG,
                    "Determines the file to save a snapshot to.",
                    CommandOptionType.SingleValue);
                
                app.OnExecute(() =>
                {
                    string endiffoTempPath = Path.Join(Utility.GetTempFolder(), ".endiffo/");

                    if (!Directory.Exists(endiffoTempPath))
                        Directory.CreateDirectory(endiffoTempPath);

                    // Todo: Use streamWriter
                    File.WriteAllText(Path.Join(endiffoTempPath, "printenv"), PrintEnv());
                    File.Copy(Utility.GetHostsFilename(), Path.Join(endiffoTempPath, "hosts"));
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        RegistryHandler.GetKeys(Path.Join(endiffoTempPath, "registry.txt"));

                    string outputPath = outputOption.HasValue()
                        ? outputOption.Value()
                        : Utility.GetSnapshotFileName();

                    if (string.IsNullOrWhiteSpace(outputPath)
                        || outputPath.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                        throw new ArgumentException("Invalid output path.");

                    ZipFile.CreateFromDirectory(endiffoTempPath, outputPath);

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
