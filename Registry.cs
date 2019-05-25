#if OS_Windows
using Microsoft.Win32;
#endif
using System;
using System.IO;
using System.Collections.Generic;

namespace endiffo
{
    // Reference: https://docs.microsoft.com/en-us/dotnet/api/microsoft.win32.registry?view=netframework-4.8
    // Exceptions NOT to throw: https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/using-standard-exception-types
    public static class RegistryHandler
    {
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
