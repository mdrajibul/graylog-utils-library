using System;

namespace RAJ.GrayLog.Interfaces
{
    public interface ITransport
    {
        string Scheme { get; }
        void Send(Uri endPoint, string message);
    }
}