using Regulus.Memorys;
using System;

namespace Regulus.Remote
{
    namespace ProviderHelper
    {
        public class GhostsPingHandler
        {
            private readonly IOpCodeExchangeable _Exchanger;
            private readonly Ping _Ping;

            public float PingTime => _Ping.GetSeconds();

            public GhostsPingHandler(IOpCodeExchangeable exchanger)
            {
                _Exchanger = exchanger;
                _Ping = new Ping(1f);
                _Ping.TriggerEvent += SendPing;
            }           

            private void SendPing()
            {
                _Exchanger.Request(ClientToServerOpCode.Ping, new byte[0].AsBuffer());
            }

            public void HandlePingResponse()
            {
                _Ping.Update();
            }

            
        }

    }
}
