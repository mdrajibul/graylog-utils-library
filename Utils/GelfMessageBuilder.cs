using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace RAJ.GrayLog.Utils
{
    public class GelfMessageBuilder
    {
        public Message Message { get; set; }
        public IDictionary<object, object> LogProperties { get; set; }

        public GelfMessageBuilder(IDictionary<object, object> logProperties)
        {
            Message = new Message();
            LogProperties = logProperties;
        }

        public GelfMessageBuilder(Message message, IDictionary<object, object> logProperties)
        {
            Message = message;
            LogProperties = logProperties;
        }

        public GelfMessageBuilder AddVersion(string version)
        {
            Message.Version = version;
            return this;
        }

        public GelfMessageBuilder AddHost(string host)
        {
            Message.Host = host;
            return this;
        }

        public GelfMessageBuilder AddShortMessage(string shortMessage)
        {
            Message.ShortMessage = shortMessage;
            return this;
        }

        public GelfMessageBuilder AddFullMessage(string fullMessage)
        {
            Message.FullMessage = fullMessage;
            return this;
        }

        public GelfMessageBuilder AddTimestamp(DateTime timestamp)
        {
            Message.Timestamp = new DateTimeOffset(timestamp).ToUnixTimeSeconds();
            return this;
        }

        public GelfMessageBuilder AddLevel(int logLevel)
        {
            Message.Level = logLevel;
            return this;
        }

        public GelfMessageBuilder AddLine(string line)
        {
            Message.Line = line;
            return this;
        }

        public GelfMessageBuilder AddFile(string file)
        {
            Message.File = file;
            return this;
        }

        public GelfMessageBuilder AddProperty(object key, object value)
        {
            if (!LogProperties.ContainsKey(key))
            {
                LogProperties.Add(key, value);
            }
            return this;
        }

        public JObject Build()
        {
            var jsonObject = JObject.FromObject(Message);

            foreach (var property in LogProperties)
            {
                BuildJObjectFromProperties(jsonObject, property);
            }

            return jsonObject;
        }
        
        private void BuildJObjectFromProperties(IDictionary<string, JToken> jObject, KeyValuePair<object, object> property)
        {
            if (property.Key == ConverterConstants.PromoteObjectPropertiesMarker)
            {
                if (property.Value != null && !(property.Value is string))
                {
                    try
                    {
                        var jo = JObject.FromObject(property.Value);
                        foreach (var joProp in jo)
                        {
                            BuildJObjectFromProperties(jObject, new KeyValuePair<object, object>(joProp.Key, joProp.Value));
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }
                return;
            }

            var key = property.Key as string;
            if (key == null)
                return;

            if (key.Equals("id", StringComparison.OrdinalIgnoreCase))
                key = "id_";

            if (!key.StartsWith("_", StringComparison.OrdinalIgnoreCase))
                key = "_" + key;

            JToken value = null;
            if (property.Value != null)
                value = JToken.FromObject(property.Value);
            if (!jObject.ContainsKey(key))
            {
                jObject.Add(key, value);
            }
        }
    }
}
