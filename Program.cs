using System;
using System.Diagnostics;
using System.IO;

namespace endiffo
{
    class Program
    {
        static string LinuxPrintEnv()
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \"printenv\"",
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

        static string WindowsPrintEnv()
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

        static void Main(string[] args)
        {
            //string res = LinuxPrintEnv();
            string res = WindowsPrintEnv();
            File.WriteAllText("snapshot.endiffo", res);
        }
    }
}
