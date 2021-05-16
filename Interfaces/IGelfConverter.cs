using Newtonsoft.Json.Linq;
using NLog;

namespace RAJ.GrayLog.Interfaces
{
    public interface IGelfConverter
    {
        JObject Convert(LogEventInfo logEventInfo, string facility);
    }
}
