using Newtonsoft.Json;

namespace endiffo
{
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
}