using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Regulus.Network.Package;
using Regulus.Utility;

namespace Regulus.Network
{
    internal class PeerTransmission : IStatus<Timestamp>
    {
     
        private readonly Line _Line;        
        private readonly List<byte> _SendBytes;
        private readonly SegmentStream _Stream;
        public event Action DisconnectEvent ;
        private long _Timeout;
        public PeerTransmission(Line line , SegmentStream stream)
        {
            
            _Line = line;
            
            _SendBytes = new List<byte>();
            _Stream = stream;
        }

        void IStatus<Timestamp>.Enter()
        {
            
        }

        void IStatus<Timestamp>.Leave()
        {
            _Line.WriteOperation(PeerOperation.RequestDisconnect);
        }

        void IStatus<Timestamp>.Update(Timestamp timestamp)
        {



            SocketMessage message ;
            while ((message = _Line.Read()) != null)
            {
                _Timeout = 0;
                var package = message;

                var operation = (PeerOperation)package.GetOperation();
                if (operation == PeerOperation.Transmission)
                    _Stream.Add(package);
                else if (operation == PeerOperation.RequestDisconnect)
                    DisconnectEvent();
            }

            _Timeout += timestamp.DeltaTicks;
            if (_Timeout > Config.Timeout * Timestamp.OneSecondTicks)
            {
                DisconnectEvent();
            }

        }
    }
}