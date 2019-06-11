using Newtonsoft.Json;
using System.Collections.Generic;

namespace Endiffo
{
    public struct ConfigFile
    {
        // Whether to include /etc/hosts file.
        [JsonProperty]
        public readonly bool Hosts;

        // Whether to include environment variables.
        [JsonProperty]
        public readonly bool EnvVars;

        // List of registry keys to be included recursively.
        [JsonProperty]
        public readonly List<string> RegistryKeys;

        // Basic constructor
        public ConfigFile(bool hosts, bool envVars, List<string> registryKeys)
        {
            Hosts = hosts;
            EnvVars = envVars;
            RegistryKeys = registryKeys;
        }
    };
}