using System;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using RAJ.GrayLog.Interfaces;

namespace RAJ.GrayLog.Client
{
    public class UdpTransportClient : ITransportClient
    {
        public void Send(byte[] datagram, int bytes, Uri endPoint)
        {
            using (var udpClient = new UdpClient())
            {
                var dns = new DnsWrapper();
                var addresses = dns.GetHostAddresses(endPoint.Host);
                var ip = addresses.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
                int result = udpClient.SendAsync(datagram, bytes, new IPEndPoint(ip, endPoint.Port)).Result;
            }
        }
    }
}