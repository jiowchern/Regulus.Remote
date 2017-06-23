using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Regulus.Framework;
using Regulus.Serialization;
using Regulus.Utility;

namespace Regulus.Network.RUDP
{
    

    public class Transmitter : IUpdatable<Timestamp> , IPeer
    {
        public readonly EndPoint EndPoint;
        private readonly SendHandler _SendHandler;
        
        public const int PackageSize = 548;
        private readonly AckWaiter _AckWaiter;
        private List<uint> _AckTimeouts;
        private long _Ticks;
        private readonly Dictionary<uint,MessagePackage> _WaitPackages;
        private long _LastReceiveTicks;
        private readonly long _TimeoutTicks;
        public event Action TimeoutEvent;
        private readonly BufferDispenser _Dispenser;
        private readonly PackageRectifier _Rectifier;

        public Transmitter(EndPoint end_point,SendHandler send_handler , BufferDispenser buffer_dispenser)
        {
            EndPoint = end_point;
            _SendHandler = send_handler;
            _Rectifier = new PackageRectifier();
            _Dispenser = buffer_dispenser;
            _AckWaiter = new AckWaiter();
            _AckTimeouts = new List<uint>();
            _WaitPackages = new Dictionary<uint, MessagePackage>();

            _TimeoutTicks = Config.TransmitterTimeout;
        }

	    
        

        EndPoint IPeer.EndPoint
        {
            get { return EndPoint; }
        }

        public void Send(byte[] buffer)
		{

            _SendData(buffer);
		    
		}

        private event Action<byte[]> _ReceivedEvent;
        event Action<byte[]> IPeer.ReceivedEvent
        {
            add { _ReceivedEvent += value; }
            remove { _ReceivedEvent -= value; }
        }

        public static Serializer CreateSerializer()
	    {
	        var builder = new DescriberBuilder(typeof(byte), typeof(byte[]),
	            typeof(MessagePackage), typeof(AckPackage), typeof(ConnectRequestPackage) , typeof(ListenAgreePackage) , typeof(ConnectedAckPackage));
	        var lastId = builder.Describers.Length;
            return new Regulus.Serialization.Serializer(builder.Describers.Union(new ITypeDescriber[] { new BlittableDescriber(++lastId, typeof(uint)) ,new BlittableDescriber(++lastId, typeof(int)) }).ToArray()  );
	    }

	    void IBootable.Launch()
	    {
            
	    }


        void IBootable.Shutdown()
	    {
	        

        }

	    bool IUpdatable<Timestamp>.Update(Timestamp time)
	    {
	        _Ticks = time.Ticks;
	        if (_Ticks - _LastReceiveTicks > _TimeoutTicks)
	        {
	            TimeoutEvent();
	            return false;
            }
            _AckHandle();

	        var stream = _Rectifier.PopStream();
	        if (stream.Length > 0)
	            _ReceivedEvent(stream);

            return true;
	    }

        private void _AckHandle()
        {
            
            _AckTimeouts.Clear();
            _AckWaiter.PopTimeout(_Ticks , ref _AckTimeouts);
            for (int i = 0; i < _AckTimeouts.Count; i++)
            {
                _Resend(_AckTimeouts[i]);                
            }
        }

        private void _Resend(uint ack_timeout)
        {
            MessagePackage message;
            if (_WaitPackages.TryGetValue(ack_timeout, out message))
            {
                _WaitPackages.Remove(ack_timeout);            
                _ListenAck(message);
            }
        }

        private void _SendData(byte[] buffer)
        {
            var messages = _Dispenser.Packing(buffer, 0, 0);

            for(int i = 0; i < messages.Length ; ++i)
            {
                var messagePackage = messages[i];
                _SendHandler.PushMessage(ref messagePackage , EndPoint);
                _ListenAck(messagePackage);
            }            
        }

        private void _ListenAck(MessagePackage message_package)
        {
            _WaitPackages.Add(message_package.Serial, message_package);
            _AckWaiter.PushWait(message_package.Serial, _Ticks + System.TimeSpan.FromSeconds(0.2).Ticks);            
        }

        private void _EraseWaitPackage(uint package_serial_number)
        {
            _WaitPackages.Remove(package_serial_number);
            _AckWaiter.EraseReply(package_serial_number);
        }

        public void Receive(ref MessagePackage package)
        {
            _UpdateLastTick();

            if (_Rectifier.PushPackage(package))
            {
                _SendAck(package.Serial);
            }                        
        }

        private void _UpdateLastTick()
        {
            _LastReceiveTicks = _Ticks;
        }

        public void Receive(ref AckPackage package)
        {
            _UpdateLastTick();
            _EraseWaitPackage(package.SerialNumber);
        }

        private void _SendAck(uint package_serial)
        {            
            _SendHandler.PushAck(package_serial , EndPoint);
        }
    }
}