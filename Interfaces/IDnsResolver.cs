using System.Net;

namespace RAJ.GrayLog.Interfaces
{
    public abstract class DnsBase {
        public abstract IPAddress[] GetHostAddresses(string hostNameOrAddress);

    }
}