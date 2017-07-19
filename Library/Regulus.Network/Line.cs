using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Regulus.Serialization;

namespace Regulus.Network.RUDP
{
    public class Line : ILine
    {

        public readonly EndPoint EndPoint;
        
        public event Action<SocketMessage> OutputEvent;

        private readonly BufferDispenser _Dispenser;
        private readonly PackageRectifier _Rectifier;

        
        private readonly List<SocketMessage> _SendPackages;
        

        private readonly CongestionRecorder _Waiter;
        
        
        private long _TimeoutTicks;
        

        public Line(EndPoint end_point)
        {
            EndPoint = end_point;
            _Dispenser = new BufferDispenser(EndPoint, SocketPackagePool.Instance);
            _Rectifier = new PackageRectifier();
            
            _SendPackages = new List<SocketMessage>();        

            _Waiter = new CongestionRecorder(3);
            

            

            _ResetTimeout();

            
        }

        private void _ResetTimeout()
        {
            _TimeoutTicks = Config.TransmitterTimeout;
        }

        void ILine.Write(PEER_OPERATION op,byte[] buffer)
        {
            var packages = _Dispenser.Packing(buffer, op);
            _SendPackages.AddRange(packages);
        }

        SocketMessage ILine.Read()
        {
            return _Rectifier.PopPackage();
        }

        EndPoint ILine.EndPoint
        {
            get { return EndPoint; }
        }

        int ILine.TobeSendCount { get { return _SendPackages.Count; } }


        public void Input(SocketMessage message)
        {                        
            
            _Waiter.ReplyUnder(message.GetAck());
            
            foreach (var ack in message.GetAcks())
            {            
                _Waiter.Reply(ack);
            }


            _Waiter.Padding();

            _Rectifier.PushPackage(message);            
            
            _ResetTimeout();
        }

        public bool IsTimeout(Timestamp time)
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
                package.SetAck(_Rectifier.Serial);
                package.SetAckFields(_Rectifier.SerialBitFields);
                
                _Waiter.PushWait(package, time.Ticks + Timestamp.OneSecondTicks);

                OutputEvent(package);                
            }
            _SendPackages.Clear();
        }
       
    }

    
}