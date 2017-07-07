using System;
using System.Net;
using Regulus.Serialization;
using Regulus.Utility;

namespace Regulus.Network.RUDP
{
    public class PeerConnecter : IStage<Timestamp>
    {
        
        
        private readonly ILine _Line;
        private readonly Serializer _Serializer;
        public event Action DoneEvent;
        public PeerConnecter(ILine line)
        {
            _Serializer = Peer.CreateSerialier();        
            _Line = line;
        }

        void IStage<Timestamp>.Enter()
        {
            var pkg = new PeerPackage();
            pkg.Step = PEER_COMMAND.CLIENTTOSERVER_VISIT;
            _Line.Write(_Serializer.ObjectToBuffer(pkg));
        }

        void IStage<Timestamp>.Leave()
        {
            
        }

        void IStage<Timestamp>.Update(Timestamp timestamp)
        {
            /*
             * todo 
             var buffer = _Line.Read();
            PeerPackage peer;
            if (_Serializer.TryBufferToObject(buffer, out peer) && peer.Step == PEER_COMMAND.SERVERTOCLIENT_AGREE)
            {
                var pkg = new PeerPackage();
                pkg.Step = PEER_COMMAND.CLIENTTOSERVER_ACK;
                _Line.Write(_Serializer.ObjectToBuffer(pkg));

                DoneEvent();
            }*/
        }
    }
}