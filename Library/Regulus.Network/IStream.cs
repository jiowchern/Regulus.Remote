using System;
using System.Net;

namespace Regulus.Network.RUDP
{
    public interface IStream
    {
        void Write(byte[] buffer);
        byte[] Read();
        EndPoint EndPoint { get; }
    }
}