using System;
using System.Net;
using Regulus.Serialization;
using Regulus.Utility;

namespace Regulus.Network.RUDP
{
    public class PeerConnecter : IStage<Timestamp>
    {
        
        
        private readonly Line _Line;
        
        public event Action DoneEvent;
        public event Action TimeoutEvent;
        private long _TimeoutCount;
        public PeerConnecter(Line line)
        {            
            _Line = line;
        }

        void IStage<Timestamp>.Enter()
        {                        
            _Line.WriteOperation(PEER_OPERATION.CLIENTTOSERVER_HELLO1 );
        }

        void IStage<Timestamp>.Leave()
        {
            
        }

        void IStage<Timestamp>.Update(Timestamp timestamp)
        {
            _TimeoutCount += timestamp.DeltaTicks;
            if (_TimeoutCount > Config.AgentConnectTimeout)
            {
                TimeoutEvent();
                return;
            }

            var pkg = _Line.Read();
            if (pkg != null)
            {
                var operation = (PEER_OPERATION)pkg.GetOperation();
                if (operation == PEER_OPERATION.SERVERTOCLIENT_HELLO1)
                {
                    _Line.WriteOperation(PEER_OPERATION.CLIENTTOSERVER_HELLO2);
                    DoneEvent();
                }
            }
            
        }
    }
}