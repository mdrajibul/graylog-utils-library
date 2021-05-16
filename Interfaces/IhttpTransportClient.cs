using System;

namespace RAJ.GrayLog.Interfaces
{
    public interface IHttpTransportClient
    {
        void Send(string message, Uri ipEndPoint);
    }
}