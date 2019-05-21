using System;
using System.Diagnostics;
using System.IO;

namespace endiffo
{
    class Program
    {
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

        // Works on Windows and Linux
        // Note that this is an insecure way to run an application.
        // An alternative is discussed here:
        // https://stackoverflow.com/questions/48296629/launch-external-process-exe-from-asp-net-core-app
        static string PrintEnv()
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "printenv",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }

        static string GetHosts()
        {
            return File.ReadAllText("/etc/hosts");
        }

        static void Main(string[] args)
        {
            string res = PrintEnv();
            File.WriteAllText("snapshot.endiffo", res);

            string hosts = GetHosts();
            File.WriteAllText("hosts.txt", hosts);
        }
    }
}
