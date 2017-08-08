using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Regulus.Extension;
using Regulus.Serialization;

namespace Regulus.Network.RUDP
{
    public class Line 
    {

        public readonly EndPoint EndPoint;
        
        public event Action<SocketMessage> OutputEvent;

        private readonly BufferDispenser _Dispenser;
        private readonly PackageRectifier _Rectifier;

        private readonly Queue<SocketMessage> _InputPackages;
        private readonly Queue<SocketMessage> _SendPackages;        
        private readonly Queue<SocketMessage> _ReceivePackages;
        private readonly CongestionRecorder _Waiter;
        private long _TimeoutTicks;
        private long _PingTicks;




        public Line(EndPoint end_point)
        {
            EndPoint = end_point;
            _Dispenser = new BufferDispenser(EndPoint, SocketPackagePool.Instance);
            _Rectifier = new PackageRectifier();
            
            _SendPackages = new Queue<SocketMessage>();
            _ReceivePackages = new Queue<SocketMessage>();
            _InputPackages = new Queue<SocketMessage>();
            
            _Waiter = new CongestionRecorder(3);

          



            _ResetTimeout();

            
        }

        private void _ResetTimeout()
        {
            _TimeoutTicks = (long)(Timestamp.OneSecondTicks * Config.Timeout);
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
                var packages = _Dispenser.PackingTransmission(buffer, (ushort)(_Rectifier.Serial-1), _Rectifier.SerialBitFields);
                for (int i = 0; i < packages.Length; i++)
                {
                    var message = packages[i];
                    _InputPackages.SafeEnqueue(message);                    
                }
        }

        public SocketMessage Read()
        {
            return _Rectifier.PopPackage();
        }

        

        public int AcknowledgeCount { get { return _Waiter.Count; } }
        public int WaitSendCount { get { return _SendPackages.Count; } }
        public int SendBytes { get; private set; }
        public int ReceiveBytes { get; private set; }
        public long SRTT { get { return _Waiter.SRTT; } }
        public long RTO { get { return _Waiter.RTO; } }
        public int SendedPackages { get; private set; }
        public int SendLostPackages { get; private set; }
        public int ReceivePackages { get; private set; }
        public int ReceiveInvalidPackages { get; private set; }
        public long LastRTT { get { return _Waiter.LastRTT; }}
        public int SendBlock { get { return _Waiter.Count; } }
        public long LastRTO { get { return _Waiter.LastRTO; }}
        public int ReceiveBlock { get { return _Rectifier.Count; } }

        public int ReceiveNumber { get { return _Rectifier.Serial; } }

        public int SendNumber { get { return _Dispenser.Serial; } }


        public void Input(SocketMessage message)
        {
            lock (_ReceivePackages)
            {
                _ReceivePackages.Enqueue(message);
            }
            
            

        }

        private void _SendPing()
        {            
            _SendAck((ushort)(_Rectifier.Serial-1), _Rectifier.SerialBitFields);
        }
        
        private void _SendAck(ushort ack,uint ack_bits)
        {
            var package = _Dispenser.PackingAck(ack, ack_bits);
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

            _HandleResend(time);
            _HandleInput(time);
            _HandleReceive(time);
            
            _HandlePing(time);
            _HandleOutput(time);
            return false;
        }

        private void _HandleInput(Timestamp time)
        {
            SocketMessage message = null;
            while (_Waiter.IsFull() == false && (message = _InputPackages.SafeDequeue())!=null)
            {
                _Waiter.PushWait(message , time.Ticks);
                lock (_SendPackages)
                {
                    _SendPackages.Enqueue(message);
                }
            }
        }


        private void _HandlePing(Timestamp time)
        {
            _PingTicks += time.DeltaTicks;
            if (_PingTicks > Timestamp.OneSecondTicks)
            {
                _PingTicks = 0;
                if (_SendPackages.Count == 0)
                {
                    _SendPing();
                }
            }
        }

        private void _HandleReceive(Timestamp time)
        {
            SocketMessage message = null;
            while ((message = _Dequeue())!=null)
            {
                _ResetTimeout();

                var seq = message.GetSeq();
                var ack = message.GetAck();
                var ackFields = message.GetAckFields();                
                var oper = (PEER_OPERATION)message.GetOperation();

                // ack 
                if (_Waiter.Reply(ack, time.Ticks, time.DeltaTicks) == false)
                {
                        
                }
                //_Waiter.ReplyBefore((ushort)(ack - 1), time.Ticks , time.DeltaTicks);
                //_Waiter.ReplyAfter((ushort)(ack - 1), ackFields , time.Ticks, time.DeltaTicks);
                //_Waiter.Padding();

                
                

                if (oper != PEER_OPERATION.ACKNOWLEDGE)
                {
                    if (_Rectifier.PushPackage(message) == false)
                    {
                        ReceiveInvalidPackages++;
                    }
                    _SendAck(seq, _Rectifier.SerialBitFields);
                }
                
                    

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
                return null;
            }

            
        }

        private void _HandleResend(Timestamp time)
        {
            var rtos = _Waiter.PopLost(time.Ticks,time.DeltaTicks);

            var count = rtos.Count;
            for (int i = 0; i < count; i++)
            {
                _SendPackages.SafeEnqueue(rtos[i]);
                
            }

            SendLostPackages += count;
        }

   
        private void _HandleOutput(Timestamp time)
        {
            


            SocketMessage message = null;
            while ((message = _PopSend()) != null  )
            {                
                
                OutputEvent(message);

                SendBytes += message.GetPackageSize();
                SendedPackages++;
            }
            
        }

        private SocketMessage _PopSend()
        {
            lock (_SendPackages)
            {
                if (_SendPackages.Count > 0  )
                {
                    
                    return _SendPackages.Dequeue();
                }
                    
            }
            return null;
        }

        public void MessageSendResult(SocketMessage message)
        {
            
            
        }
    }

    
}