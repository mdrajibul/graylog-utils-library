using System;

namespace RAJ.GrayLog.Interfaces
{
    public interface ITransportClient
    {
        void Send(byte[] datagram, int bytes, Uri ipEndPoint);
    }
}