#if OS_Windows
using Microsoft.Win32;
#endif
using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;

namespace Endiffo
{
    /// RegistryKeyInfo is intended to be used as elements of a list which is serialised.
    public struct RegistryKeyInfo
    {
        [JsonProperty]
        public readonly string RegistryKey;

        [JsonProperty]
        public readonly string Filename;

        public RegistryKeyInfo(string regKey, string filename)
        {
            RegistryKey = regKey;
            Filename = filename;
        }
    }

    /// Reference: https://docs.microsoft.com/en-us/dotnet/api/microsoft.win32.registry?view=netframework-4.8
    /// Exceptions NOT to throw: https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/using-standard-exception-types
    public static class RegistryHandler
    {
        /// At the moment this is an example method for how to read data from the registry.
        /// It saves collected data in the given file.
        public static void GetKeys(string filename)
        {
            #if !OS_Windows
            throw new Exception("RegistryHandler function not available outside Windows.");
            #else

            var output = new List<string>();

            RegistryKey rkey = Registry.Users;

            output.Add("Subkeys of " + rkey.Name);
            output.Add("-----------------------------------------------");

            // Print the contents of the array to the console.
            foreach (String s in rkey.GetSubKeyNames())
            {
                output.Add(s);
            }

            File.WriteAllLines(filename, output);

            #endif
        }
    }
}
