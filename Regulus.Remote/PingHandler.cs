using Regulus.Memorys;
using System;

namespace Regulus.Remote
{
    namespace ProviderHelper
    {
        public class PingHandler : ClientExchangeable
        {
            
            private readonly Ping _Ping;

            public float PingTime => _Ping.GetSeconds();
            

            public PingHandler()
            {
                
                _Ping = new Ping(1f);
                _Ping.TriggerEvent += SendPing;
            }

            event Action<ClientToServerOpCode, Memorys.Buffer> _ResponseEvent;
            event Action<ClientToServerOpCode, Memorys.Buffer> Exchangeable<ServerToClientOpCode, ClientToServerOpCode>.ResponseEvent
            {
                add
                {
                    _ResponseEvent += value;
                }

                remove
                {
                    _ResponseEvent -= value;
                }
            }

            private void SendPing()
            {
                _ResponseEvent(ClientToServerOpCode.Ping, new byte[0].AsBuffer());                
            }

            public void HandlePingResponse()
            {
                _Ping.Update();
            }

            void Exchangeable<ServerToClientOpCode, ClientToServerOpCode>.Request(ServerToClientOpCode code, Memorys.Buffer args)
            {
                
            }
        }

    }
}
