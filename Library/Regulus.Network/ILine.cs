using System;
using System.Collections.Generic;
using System.Net;

namespace Regulus.Network.RUDP
{
    public interface ILine
    {
        void Write(PEER_OPERATION op, byte[] buffer);
        SegmentPackage Read();
        EndPoint EndPoint { get; }
        int TobeSendCount { get; }
    }
}