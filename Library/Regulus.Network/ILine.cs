using System;
using System.Collections.Generic;
using System.Net;

namespace Regulus.Network.RUDP
{
    public interface ILine
    {
        void Write(byte[] buffer);
        void Read(Queue<byte[]> packages);
        EndPoint EndPoint { get; }
    }
}