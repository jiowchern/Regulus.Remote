using System;
using Regulus.Serialization;
using Regulus.Utility;

namespace Regulus.Network.RUDP
{
    internal class PeerDisconnecter : IStage<Timestamp>
    {
        private readonly ILine _Line;        
        public event Action DoneEvent; 

        public PeerDisconnecter(ILine line)
        {
            
            _Line = line;
        }

        void IStage<Timestamp>.Enter()
        {
            
        }

        void IStage<Timestamp>.Leave()
        {
            
        }

        void IStage<Timestamp>.Update(Timestamp obj)
        {
            if(_Line.TobeSendCount == 0)
                DoneEvent();
        }
    }
}