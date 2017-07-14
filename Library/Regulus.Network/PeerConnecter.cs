using System;
using System.Net;
using Regulus.Serialization;
using Regulus.Utility;

namespace Regulus.Network.RUDP
{
    public class PeerConnecter : IStage<Timestamp>
    {
        
        
        private readonly ILine _Line;
        
        public event Action DoneEvent;
        public PeerConnecter(ILine line)
        {            
            _Line = line;
        }

        void IStage<Timestamp>.Enter()
        {                        
            _Line.Write(PEER_OPERATION.CLIENTTOSERVER_HELLO1 , new byte[0]);
        }

        void IStage<Timestamp>.Leave()
        {
            
        }

        void IStage<Timestamp>.Update(Timestamp timestamp)
        {

            var pkg = _Line.Read();
            if (pkg != null)
            {
                var operation = (PEER_OPERATION)pkg.GetOperation();
                if (operation == PEER_OPERATION.SERVERTOCLIENT_HELLO1)
                {
                    _Line.Write(PEER_OPERATION.CLIENTTOSERVER_HELLO2, new byte[0]);
                    DoneEvent();
                }
            }            
        }
    }
}