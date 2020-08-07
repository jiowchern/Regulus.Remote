using Regulus.Network.Package;
using System;
using System.Collections.Generic;
using System.Net;

namespace Regulus.Network
{
    public class Line
    {

        public readonly EndPoint EndPoint;

        public event Action<SocketMessage> OutputEvent;

        private readonly BufferDispenser _Dispenser;
        private readonly PackageRectifier _Rectifier;

        private readonly System.Collections.Concurrent.ConcurrentQueue<SocketMessage> _InputPackages;
        private readonly System.Collections.Concurrent.ConcurrentQueue<SocketMessage> _SendPackages;
        private readonly System.Collections.Concurrent.ConcurrentQueue<SocketMessage> _ReceivePackages;
        private readonly CongestionRecorder _Waiter;
        private long _TimeoutTicks;
        private long _PingTicks;
        public Line(EndPoint end_point)
        {
            EndPoint = end_point;
            _Dispenser = new BufferDispenser(EndPoint, SocketMessageFactory.Instance);
            _Rectifier = new PackageRectifier();

            _SendPackages = new System.Collections.Concurrent.ConcurrentQueue<SocketMessage>();
            _ReceivePackages = new System.Collections.Concurrent.ConcurrentQueue<SocketMessage>();
            _InputPackages = new System.Collections.Concurrent.ConcurrentQueue<SocketMessage>();

            _Waiter = new CongestionRecorder(HungryLimit: 3);

            ResetTimeout();
        }

        private void ResetTimeout()
        {
            _TimeoutTicks = (long)(Timestamp.OneSecondTicks * Config.Timeout);
        }

        public void WriteOperation(PeerOperation Operation)
        {
            SocketMessage package = _Dispenser.PackingOperation(Operation, _Rectifier.Serial, _Rectifier.SerialBitFields);
            _SendPackages.Enqueue(package);

        }
        public void WriteTransmission(byte[] buffer)
        {
            SocketMessage[] packages = _Dispenser.PackingTransmission(buffer, (ushort)(_Rectifier.Serial - 1), _Rectifier.SerialBitFields);
            for (int i = 0; i < packages.Length; i++)
            {
                SocketMessage message = packages[i];
                _InputPackages.Enqueue(message);
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
        public long Srtt { get { return _Waiter.Srtt; } }
        public long Rto { get { return _Waiter.Rto; } }
        public int SendedPackages { get; private set; }
        public int SendLostPackages { get; private set; }
        public int ReceivePackages { get; private set; }
        public int ReceiveInvalidPackages { get; private set; }
        public long LastRtt { get { return _Waiter.LastRtt; } }
        public int SendBlock { get { return _Waiter.Count; } }
        public long LastRto { get { return _Waiter.LastRto; } }
        public int ReceiveBlock { get { return _Rectifier.Count; } }

        public int ReceiveNumber { get { return _Rectifier.Serial; } }

        public int SendNumber { get { return _Dispenser.Serial; } }

        public void Input(SocketMessage Message)
        {
            _ReceivePackages.Enqueue(Message);



        }

        private void SendPing()
        {
            SendAck((ushort)(_Rectifier.Serial - 1), _Rectifier.SerialBitFields);
        }

        private void SendAck(ushort Ack, uint AckBits)
        {
            SocketMessage package = _Dispenser.PackingAck(Ack, AckBits);
            _SendPackages.Enqueue(package);
        }

        public bool Tick(Timestamp Time)
        {
            _TimeoutTicks -= Time.DeltaTicks;
            if (_TimeoutTicks < 0)
                return true;

            HandleResend(Time);
            HandleInput(Time);
            HandleReceive(Time);

            HandlePing(Time);
            HandleOutput(Time);
            return false;
        }

        private void HandleInput(Timestamp Time)
        {
            SocketMessage message = null;
            while (_Waiter.IsFull() == false && _InputPackages.TryDequeue(out message))
            {
                _Waiter.PushWait(message, Time.Ticks);
                _SendPackages.Enqueue(message);
            }
        }


        private void HandlePing(Timestamp Time)
        {
            _PingTicks += Time.DeltaTicks;
            if (_PingTicks > Timestamp.OneSecondTicks)
            {
                _PingTicks = 0;
                if (_SendPackages.IsEmpty)
                    SendPing();
            }
        }

        private void HandleReceive(Timestamp Time)
        {
            SocketMessage message = null;
            while ((message = Dequeue()) != null)
            {
                ResetTimeout();

                ushort seq = message.GetSeq();
                ushort ack = message.GetAck();
                uint ackFields = message.GetAckFields();
                PeerOperation oper = (PeerOperation)message.GetOperation();

                // ack 
                if (_Waiter.Reply(ack, Time.Ticks, Time.DeltaTicks) == false)
                {

                }
                //_Waiter.ReplyBefore((ushort)(ack - 1), time.Ticks , time.DeltaTicks);
                //_Waiter.ReplyAfter((ushort)(ack - 1), ackFields , time.Ticks, time.DeltaTicks);
                //_Waiter.Padding();




                if (oper != PeerOperation.Acknowledge)
                {
                    if (_Rectifier.PushPackage(message) == false)
                        ReceiveInvalidPackages++;
                    SendAck(seq, _Rectifier.SerialBitFields);
                }



                ReceiveBytes += message.GetPackageSize();
                ReceivePackages++;
            }


        }

        private SocketMessage Dequeue()
        {
            SocketMessage message;
            if (_ReceivePackages.TryDequeue(out message))
                return message;
            return null;



        }

        private void HandleResend(Timestamp Time)
        {
            List<SocketMessage> rtos = _Waiter.PopLost(Time.Ticks, Time.DeltaTicks);

            int count = rtos.Count;
            for (int i = 0; i < count; i++)
                _SendPackages.Enqueue(rtos[i]);


            SendLostPackages += count;
        }


        private void HandleOutput(Timestamp Time)
        {



            SocketMessage message = null;
            while ((message = PopSend()) != null)
            {

                OutputEvent(message);

                SendBytes += message.GetPackageSize();
                SendedPackages++;
            }

        }

        private SocketMessage PopSend()
        {
            SocketMessage message;
            if (_SendPackages.TryDequeue(out message))
                return message;
            return null;
        }

        public void MessageSendResult(SocketMessage Message)
        {


        }
    }


}