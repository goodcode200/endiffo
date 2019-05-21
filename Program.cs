using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace endiffo
{
    public struct Constants
    {
        public const string HOSTS_FILE = "/etc/hosts";
        public const string ENVIRON_VAR_COMMAND = "printenv";
        public const string DEFAULT_SNAPSHOT_FILE = "snapshot.endiffo";
    }

    public struct SystemSnapshot
    {
        // Environment variables.
        [JsonProperty]
        public readonly string EnvBase64;

        // Contents of HOSTS_FILE.
        [JsonProperty]
        public readonly string HostsBase64;

        public SystemSnapshot(string envBase64, string hostsBase64)
        {
            EnvBase64 = envBase64;
            HostsBase64 = hostsBase64;
        }
    };

    class Program
    {
        // Reference: https://stackoverflow.com/questions/11743160/how-do-i-encode-and-decode-a-base64-string
        static string EncodeToBase64(string unencoded)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(unencoded));
        }

        // static string ExecViaBash()
        // {
        //     var process = new Process()
        //     {
        //         StartInfo = new ProcessStartInfo
        //         {
        //             FileName = "/bin/bash",
        //             Arguments = "-c \"printenv\"",
        //             RedirectStandardOutput = true,
        //             UseShellExecute = false,
        //             CreateNoWindow = true,
        //         }
        //     };

        //     process.Start();
        //     string result = process.StandardOutput.ReadToEnd();
        //     process.WaitForExit();
        //     return result;
        // }

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
                    envBase64: EncodeToBase64(PrintEnv()),
                    hostsBase64: EncodeToBase64(GetHosts())
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
