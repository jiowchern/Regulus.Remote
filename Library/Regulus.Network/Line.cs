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
        
        public event Action<EndPoint, byte[]> OutputEvent;

        private readonly BufferDispenser _Dispenser;
        private readonly PackageRectifier _Rectifier;

        
        private readonly List<SegmentPackage> _SendPackages;
        

        private readonly CongestionRecorder _Waiter;
        
        
        private long _TimeoutTicks;
        

        public Line(EndPoint end_point)
        {
            EndPoint = end_point;
            _Dispenser = new BufferDispenser(Config.PackageSize);
            _Rectifier = new PackageRectifier();
            
            _SendPackages = new List<SegmentPackage>();        

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

        SegmentPackage ILine.Read()
        {
            return _Rectifier.PopPackage();
        }

        EndPoint ILine.EndPoint
        {
            get { return EndPoint; }
        }

        int ILine.TobeSendCount { get { return _SendPackages.Count; } }


        public void Input(SegmentPackage package)
        {                        
            
            _Waiter.ReplyUnder(package.GetAck());
            
            foreach (var ack in package.GetAcks())
            {            
                _Waiter.Reply(ack);
            }


            _Waiter.Padding();

            _Rectifier.PushPackage(package);            
            
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

                OutputEvent(EndPoint , package.GetBuffer());                
            }
            _SendPackages.Clear();
        }
       
    }

    
}