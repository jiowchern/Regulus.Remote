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
                _Rectifier.PushPackage(message);

                var oper = (PEER_OPERATION)message.GetOperation();
                if (oper != PEER_OPERATION.ACKNOWLEDGE)
                    _SendAck(_Rectifier.Serial, _Rectifier.SerialBitFields);
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
        }

   
        private void _HandleSend(Timestamp time)
        {
            SocketMessage message = null;
            while ((message = _PopSend()) != null)
            {
                if (message.GetOperation() != (byte)PEER_OPERATION.ACKNOWLEDGE)
                    _Waiter.PushWait(message, time.Ticks);

                OutputEvent(message);
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

    internal class RetransmissionTimeOut
    {
        private long _RTTVAL;
        private long _SRTT;
        private long  _RTT;
        public long Value { get; set; }

        public RetransmissionTimeOut()
        {
            _RTTVAL = (long)(Timestamp.OneSecondTicks * 0.05);
            _SRTT = (long)(Timestamp.OneSecondTicks * 0.1);
            _RTT = (long)(Timestamp.OneSecondTicks * 0.1);

            Update(_RTT, 0);
        }

        public void Update(long rtt,long delta)
        {
            _SRTT = (long)(0.875 * _RTT + 0.125 * rtt);
            _RTT = rtt;
            _RTTVAL = (long)(0.75 * _RTTVAL + 0.25 * _Abs(_SRTT - rtt));            
            Value = _SRTT + _Max(delta, 4 * _RTTVAL);
        }

        private long _Abs(long val)
        {
            return val < 0 ? 0 - val : val;
        }

        private long _Max(long time_delta_ticks, long rttval)
        {
            return time_delta_ticks > rttval ? time_delta_ticks : rttval;
        }
    }
}