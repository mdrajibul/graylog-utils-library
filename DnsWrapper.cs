using System.Net;
using RAJ.GrayLog.Interfaces;

namespace RAJ.GrayLog
{
    public class DnsWrapper : DnsBase
    {
        public override IPAddress[] GetHostAddresses(string hostNameOrAddress)
        {
            return Dns.GetHostAddressesAsync(hostNameOrAddress).Result;
        }
    }
}