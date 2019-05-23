using Microsoft.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace endiffo
{
    static class Program
    {
        // Returns null in case of an error.
        // Written to work on Windows and Linux
        // Note that this is an insecure way to run an application. An alternative is discussed here:
        // https://stackoverflow.com/questions/48296629/launch-external-process-exe-from-asp-net-core-app
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

        // TODO If the hosts file is unexpectedly large, we might want to consider streaming it,
        // in which case this function signature will change.
        static string GetHosts()
        {
            return File.ReadAllText(Constants.HOSTS_FILE);
        }

        // Command line parsing: https://gstoob-online.netlify.com/posts/parsing-command-line-arguments
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
                    string outputPath = outputOption.HasValue()
                        ? outputOption.Value()
                        : Utility.GetSnapshotFileName();

                    // Reference: https://stackoverflow.com/questions/4650462/easiest-way-to-check-if-an-arbitrary-string-is-a-valid-filename
                    if (string.IsNullOrWhiteSpace(outputPath)
                        || outputPath.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                        throw new ArgumentException("Invalid output path.");

                    if (File.Exists(outputPath))
                        throw new ArgumentException("Output path already exists.");

                    string directory = new FileInfo(outputPath).Directory.FullName;
                    if (string.IsNullOrWhiteSpace(directory))
                        throw new ArgumentException("Invalid output path.");

                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    var snapshot = new SystemSnapshot (
                        envBase64: Utility.EncodeToBase64(PrintEnv()),
                        hostsBase64: Utility.EncodeToBase64(GetHosts())
                    );

                    File.WriteAllText(
                        outputPath,
                        JsonConvert.SerializeObject(snapshot)
                    );

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

// Reference: https://mariusschulz.com/blog/detecting-the-operating-system-in-net-core
// if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
// {
//     Console.WriteLine("Platform is Windows.");
// }
