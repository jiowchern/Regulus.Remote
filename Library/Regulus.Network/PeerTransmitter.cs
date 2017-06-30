using System;
using Regulus.Utility;

namespace Regulus.Network.RUDP
{
    internal class PeerTransmitter : IStage<Timestamp>
    {
        private readonly IStream _Stream;

        public PeerTransmitter(IStream stream)
        {
            _Stream = stream;
        }

        void IStage<Timestamp>.Enter()
        {
            throw new NotImplementedException();
        }

        void IStage<Timestamp>.Leave()
        {
            throw new NotImplementedException();
        }

        void IStage<Timestamp>.Update(Timestamp obj)
        {
            throw new NotImplementedException();
        }
    }
}