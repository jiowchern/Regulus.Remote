using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Regulus.Serialization;

namespace Regulus.Network.RUDP
{
    public class Line 
    {

        public readonly EndPoint EndPoint;
        
        public event Action<SocketMessage> OutputEvent;

        private readonly BufferDispenser _Dispenser;
        private readonly PackageRectifier _Rectifier;

        
        private readonly List<SocketMessage> _SendPackages;
        

        private readonly CongestionRecorder _Waiter;
        
        
        private long _TimeoutTicks;
        private readonly byte[] _EmptyArray;


        public Line(EndPoint end_point)
        {
            EndPoint = end_point;
            _Dispenser = new BufferDispenser(EndPoint, SocketPackagePool.Instance);
            _Rectifier = new PackageRectifier();
            
            _SendPackages = new List<SocketMessage>();        

            _Waiter = new CongestionRecorder(3);

            _EmptyArray = new byte[0];



            _ResetTimeout();

            
        }

        private void _ResetTimeout()
        {
            _TimeoutTicks = Config.TransmitterTimeout;
        }

        public void WriteOperation(PEER_OPERATION operation)
        {
            var package = _Dispenser.PackingOperation(operation , _Rectifier.Serial, _Rectifier.SerialBitFields);
            _SendPackages.Add(package);
        }
        public void WriteTransmission(byte[] buffer)
        {
            var packages = _Dispenser.PackingTransmission(buffer, _Rectifier.Serial , _Rectifier.SerialBitFields);
            _SendPackages.AddRange(packages);
        }

        public SocketMessage Read()
        {
            return _Rectifier.PopPackage();
        }

        

        public int AcknowledgeCount { get { return _Waiter.Count; } }
        public int WaitSendCount { get { return _SendPackages.Count; } }


        public void Input(SocketMessage message)
        {
            var ack = message.GetAck();
            
            _ResetTimeout();

            _Waiter.ReplyUnder((ushort)(ack -1));
            
            foreach (var ack_id in message.GetAcks())
            {            
                _Waiter.Reply((ushort) (ack_id-1) );
            }


            _Waiter.Padding();
            _Rectifier.PushPackage(message);

            var oper = (PEER_OPERATION)message.GetOperation();
            if (oper != PEER_OPERATION.ACKNOWLEDGE)
                _SendAck(_Rectifier.Serial, _Rectifier.SerialBitFields);

        }

        private void _SendAck(ushort ack,uint ack_bits)
        {
            var package = _Dispenser.PackingOperation(PEER_OPERATION.ACKNOWLEDGE, ack, ack_bits);
            _SendPackages.Add(package);
        }

        public bool Tick(Timestamp time)
        {
            _TimeoutTicks -= time.DeltaTicks;
            if (_TimeoutTicks < 0)
                return true;

            _HandleResend(time);
            _HandleSend(time);

            

            return false;
        }

        private void _HandleResend(Timestamp time)
        {
            var rtos = _Waiter.PopLost(time.Ticks);
            _SendPackages.AddRange(rtos);
        }

        

        

        private void _HandleSend(Timestamp time)
        {            
            foreach (var package in _SendPackages)
            {
                if(package.GetOperation() != (byte)PEER_OPERATION.ACKNOWLEDGE)
                    _Waiter.PushWait(package, time.Ticks + Timestamp.OneSecondTicks);

                OutputEvent(package);                
            }
            _SendPackages.Clear();
        }
       
    }

    
}