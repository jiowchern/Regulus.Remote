using System;
using Regulus.Serialization;
using Regulus.Utility;

namespace Regulus.Network.RUDP
{
    internal class PeerDisconnecter : IStage<Timestamp>
    {
        private readonly ILine _Line;
        private readonly Serializer _Serializer;
        public event Action DoneEvent; 

        public PeerDisconnecter(ILine line)
        {
            _Serializer =  Peer.CreateSerialier();
            _Line = line;
        }

        void IStage<Timestamp>.Enter()
        {
            var pkg = new PeerPackage();
            pkg.Step = PEER_COMMAND.CLIENTTOSERVER_REQUEST_DISCONNECT;
            _Line.Write(_Serializer.ObjectToBuffer(pkg));
        }

        void IStage<Timestamp>.Leave()
        {
            /*
             * todo 
             * var buffer = _Line.Read();
            if (buffer.Length > 0)
            {
                PeerPackage pkg;
                if (_Serializer.TryBufferToObject<PeerPackage>(buffer, out pkg))
                {
                    if (pkg.Step == PEER_COMMAND.SERVERTOCLIENT_RESPONSE_DISCONNECT)
                    {
                        DoneEvent();
                    }
                }
            }*/
        }

        void IStage<Timestamp>.Update(Timestamp obj)
        {
            
        }
    }
}