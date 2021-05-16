using System;
using System.Net;
using RAJ.GrayLog.Interfaces;

namespace RAJ.GrayLog.Transport
{
    public class HttpTransport : ITransport
    {

        private readonly IHttpTransportClient _transportClient;
        public HttpTransport(IHttpTransportClient transportClient)
        {
            _transportClient = transportClient;
        }
        public void Send(Uri endPoint, string message)
        {
            _transportClient.Send(message, endPoint);
        }

        public string Scheme
        {
            get { return "http"; }
        }
    }
}