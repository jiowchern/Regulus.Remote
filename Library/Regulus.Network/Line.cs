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
        private readonly Queue<SocketMessage> _SendPackages;
        private readonly Queue<SocketMessage> _ReceivePackages;
        private readonly CongestionRecorder _Waiter;
        private long _TimeoutTicks;



        public Line(EndPoint end_point)
        {
            EndPoint = end_point;
            _Dispenser = new BufferDispenser(EndPoint, SocketPackagePool.Instance);
            _Rectifier = new PackageRectifier();
            
            _SendPackages = new Queue<SocketMessage>();
            _ReceivePackages = new Queue<SocketMessage>();
            _Waiter = new CongestionRecorder(3);

          



            _ResetTimeout();

            
        }

        private void _ResetTimeout()
        {
            _TimeoutTicks = Config.TransmitterTimeout;
        }

        public void WriteOperation(PEER_OPERATION operation)
        {
            var package = _Dispenser.PackingOperation(operation , _Rectifier.Serial, _Rectifier.SerialBitFields);
            lock (_SendPackages)
            {
                _SendPackages.Enqueue(package);
            }
            
        }
        public void WriteTransmission(byte[] buffer)
        {
            
                var packages = _Dispenser.PackingTransmission(buffer, _Rectifier.Serial, _Rectifier.SerialBitFields);
                for (int i = 0; i < packages.Length; i++)
                {
                    lock (_SendPackages)
                    {
                        _SendPackages.Enqueue(packages[i]);
                    }
                }
                
            
                
        }

        public SocketMessage Read()
        {
            return _Rectifier.PopPackage();
        }

        

        public int AcknowledgeCount { get { return _Waiter.Count; } }
        public int WaitSendCount { get { return _SendPackages.Count; } }
        public long SendBytes { get; private set; }
        public long ReceiveBytes { get; private set; }
        public long RTT { get { return _Waiter.RTT; } }
        public long RTO { get { return _Waiter.RTO; } }
        public long SendedPackages { get; private set; }
        public long SendLostPackages { get; private set; }
        public long ReceivePackages { get; private set; }
        public long ReceiveInvalidPackages { get; private set; }


        public void Input(SocketMessage message)
        {
            lock (_ReceivePackages)
            {
                _ReceivePackages.Enqueue(message);
            }
            
            

        }

        private void _SendAck(ushort ack,uint ack_bits)
        {
            var package = _Dispenser.PackingOperation(PEER_OPERATION.ACKNOWLEDGE, ack, ack_bits);
            lock (_SendPackages)
            {
                _SendPackages.Enqueue(package);
            }            
        }

        public bool Tick(Timestamp time)
        {
            _TimeoutTicks -= time.DeltaTicks;
            if (_TimeoutTicks < 0)
                return true;
            _HandleReceive(time);
            _HandleResend(time);            
            _HandleSend(time);

            

            return false;
        }

        private void _HandleReceive(Timestamp time)
        {
            SocketMessage message = null;
            while ((message = _Dequeue())!=null)
            {

                var ack = message.GetAck();

                _ResetTimeout();

                _Waiter.ReplyUnder((ushort) (ack - 1),time.Ticks , time.DeltaTicks);
                

                foreach (var ack_id in message.GetAcks())
                {
                    _Waiter.Reply((ushort)(ack_id - 1), time.Ticks, time.DeltaTicks);
                }

                _Waiter.Padding();
                if (_Rectifier.PushPackage(message) == false)
                {
                    ReceiveInvalidPackages++;
                }

                var oper = (PEER_OPERATION)message.GetOperation();
                if (oper != PEER_OPERATION.ACKNOWLEDGE)
                    _SendAck(_Rectifier.Serial, _Rectifier.SerialBitFields);


                ReceiveBytes += message.GetPackageSize();
                ReceivePackages++;
            }

            
        }

        private SocketMessage _Dequeue()
        {
            lock (_ReceivePackages)
            {
                if (_ReceivePackages.Count > 0)
                    return _ReceivePackages.Dequeue();
            }

            return null;
        }

        private void _HandleResend(Timestamp time)
        {
            var rtos = _Waiter.PopLost(time.Ticks,time.DeltaTicks);
            var count = rtos.Count;
            for (int i = 0; i < count; i++)
            {
                lock (_SendPackages)
                {
                    _SendPackages.Enqueue(rtos[i]);
                }
            }

            SendLostPackages += count;
        }

   
        private void _HandleSend(Timestamp time)
        {
            SocketMessage message = null;
            while ((message = _PopSend()) != null)
            {
                if (message.GetOperation() != (byte)PEER_OPERATION.ACKNOWLEDGE)
                    _Waiter.PushWait(message, time.Ticks);

                OutputEvent(message);
                SendBytes += message.GetPackageSize();
                SendedPackages++;
            }
            
        }

        private SocketMessage _PopSend()
        {
            lock (_SendPackages)
            {
                if(_SendPackages.Count>0)
                    return _SendPackages.Dequeue();
            }
            return null;
        }
    }

    
}