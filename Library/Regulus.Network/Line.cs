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
        
        private readonly Serializer _Serializer;
        private long _TimeoutTicks;
        

        public Line(EndPoint end_point)
        {
            EndPoint = end_point;
            _Dispenser = new BufferDispenser(Config.PackageSize);
            _Rectifier = new PackageRectifier();
            
            _SendPackages = new List<SegmentPackage>();        

            _Waiter = new CongestionRecorder(3);
            

            _Serializer = CreateSerializer();

            _ResetTimeout();

            
        }

        private void _ResetTimeout()
        {
            _TimeoutTicks = Config.TransmitterTimeout;
        }

        void ILine.Write(byte[] buffer)
        {
            var packages = _Dispenser.Packing(buffer, 0, 0);
            _SendPackages.AddRange(packages);
        }

        void ILine.Read(Queue<byte[]> packages)
        {
            _Rectifier.PopPackages(packages);
        }

        EndPoint ILine.EndPoint
        {
            get { return EndPoint; }
        }


        public void Input(byte[] package_buffer)
        {            
            var package = (SegmentPackage)_Serializer.BufferToObject(package_buffer) ;


            
            _Waiter.ReplyUnder(package.Ack);
            
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
                package.Ack = _Rectifier.Serial;
                package.AckBits = _Rectifier.SerialBitFields;
                
                _Waiter.PushWait(package, time.Ticks + Timestamp.OneSecondTicks);

                OutputEvent(EndPoint , _Serializer.ObjectToBuffer(package));                
            }
            _SendPackages.Clear();
        }



        public static Serializer CreateSerializer()
        {
            var builder = new DescriberBuilder(typeof(byte), typeof(byte[]),
                typeof(SegmentPackage));
            var lastId = builder.Describers.Length;
            return new Regulus.Serialization.Serializer(builder.Describers.Union(new ITypeDescriber[] { new BlittableDescriber(++lastId, typeof(uint)), new BlittableDescriber(++lastId, typeof(int)) }).ToArray());
        }        
    }

    
}