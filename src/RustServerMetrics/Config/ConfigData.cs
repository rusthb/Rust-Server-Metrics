using Newtonsoft.Json;

namespace RustServerMetrics.Config
{
    class ConfigData
    {
        // InfluxDB 2.x defaults (you MUST change these in the config file)
        public const string DEFAULT_INFLUX_DB_URL      = "http://127.0.0.1:8086";
        public const string DEFAULT_INFLUX_ORG         = "CHANGEME_org";
        public const string DEFAULT_INFLUX_BUCKET      = "CHANGEME_bucket";
        public const string DEFAULT_INFLUX_TOKEN       = "CHANGEME_token";
        public const string DEFAULT_SERVER_TAG         = "CHANGEME-01";

        [JsonProperty(PropertyName = "Enabled")]
        public bool enabled = false;

        // Base URL of your InfluxDB 2.x instance, e.g. "http://127.0.0.1:8086"
        [JsonProperty(PropertyName = "Influx URL")]
        public string databaseUrl = DEFAULT_INFLUX_DB_URL;

        [JsonProperty(PropertyName = "Skip TLS Verification")]
        public bool skipTlsVerification = true;

        // Influx 2.x organisation name or ID
        [JsonProperty(PropertyName = "Influx Organisation")]
        public string databaseUser = DEFAULT_INFLUX_ORG;

        // Influx 2.x bucket name
        [JsonProperty(PropertyName = "Influx Bucket")]
        public string databaseName = DEFAULT_INFLUX_BUCKET;

        // Influx 2.x API token (with write access to the bucket)
        [JsonProperty(PropertyName = "Influx Token")]
        public string databasePassword = DEFAULT_INFLUX_TOKEN;

        [JsonProperty(PropertyName = "Server Tag")]
        public string serverTag = DEFAULT_SERVER_TAG;

        [JsonProperty(PropertyName = "Debug Logging")]
        public bool debugLogging = false;

        [JsonProperty(PropertyName = "Amount of metrics to submit in each request")]
        public ushort batchSize = 1000;

        [JsonProperty(PropertyName = "Gather Player Averages (Client FPS, Player Memory, Player Latency, Player Packet Loss)")]
        public bool gatherPlayerMetrics = true;
    }
}
