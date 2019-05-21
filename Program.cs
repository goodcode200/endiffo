using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
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

        static void Main(string[] args)
        {
            try
            {
                var snapshot = new SystemSnapshot (
                    envBase64: Utility.EncodeToBase64(PrintEnv()),
                    hostsBase64: Utility.EncodeToBase64(GetHosts())
                );

                File.WriteAllText(
                    Constants.DEFAULT_SNAPSHOT_FILE,
                    JsonConvert.SerializeObject(snapshot)
                );
            }
            catch(Exception ex)
            {
                Console.WriteLine (
                    "An error occurred and the application had to terminate." + Environment.NewLine
                    + "Error text: " + ex.Message + Environment.NewLine
                    + "Inner exception: " + ex.InnerException + Environment.NewLine
                    + "Stack trace: " + ex.StackTrace
                );
            }
        }
    }
}
