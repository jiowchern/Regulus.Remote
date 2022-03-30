using System;

namespace Regulus.Remote.Tools.Protocol.Sources.TestCommon.Tests
{
    public class OpCodeExchanger : IOpCodeExchangeable
    {
        public readonly System.Collections.Generic.Queue<System.Tuple<ClientToServerOpCode, byte[]>> Requests;
        public OpCodeExchanger()
        {
            Requests = new System.Collections.Generic.Queue<Tuple<ClientToServerOpCode, byte[]>>();
        }
        public Action<ServerToClientOpCode, byte[]> Responser; 
        event Action<ServerToClientOpCode, byte[]> IOpCodeExchangeable.ResponseEvent
        {
            add
            {
                Responser += value;
            }

            remove
            {
                Responser -= value;
            }
        }


        public event Action<ClientToServerOpCode, byte[]> RequestEvent;
        void IOpCodeExchangeable.Request(ClientToServerOpCode code, byte[] args)
        {
            Requests.Enqueue(new Tuple<ClientToServerOpCode, byte[]>(code, args) );
        }
        public Tuple<ClientToServerOpCode, byte[]> IgnoreUntil(ClientToServerOpCode code)
        {
            Tuple<ClientToServerOpCode, byte[]> pkg;
            while (Requests.TryDequeue(out pkg))
            {
                if (pkg.Item1 != code)
                    continue;

                return pkg;
            }

            return null;
        }

    }
}