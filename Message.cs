using System;
using Newtonsoft.Json;

namespace RAJ.GrayLog
{
    // see http://docs.graylog.org/en/2.2/pages/gelf.html
    public class Message
    {
        [JsonProperty("file")]
        public string File { get; set; }

        [JsonProperty("full_message")]
        public string FullMessage { get; set; }

        [JsonProperty("host")]
        public string Host { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("line")]
        public string Line { get; set; }

        [JsonProperty("short_message")]
        public string ShortMessage { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }
}