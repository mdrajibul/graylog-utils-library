using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using RAJ.GrayLog.Interfaces;

namespace RAJ.GrayLog.Client
{
    public class HttpTransportClient : IHttpTransportClient
    {
        public void Send(string message, Uri endPoint)
        {
            using (var httpClient = new HttpClient())
            {
                var content = new StringContent(message, Encoding.UTF8, "application/json");
                var response = httpClient.PostAsync(endPoint, content).Result;
            }
        }
    }
}