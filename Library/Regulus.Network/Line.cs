using System;
using System.Collections.Generic;
using System.Net;
using Regulus.Network.Package;
using QueueThreadHelper = Regulus.Utility.QueueThreadHelper;

namespace Regulus.Network
{
    public class Line 
    {

        public readonly EndPoint EndPoint;
        
        public event Action<SocketMessage> OutputEvent;

        private readonly BufferDispenser m_Dispenser;
        private readonly PackageRectifier m_Rectifier;

        private readonly Queue<SocketMessage> m_InputPackages;
        private readonly Queue<SocketMessage> m_SendPackages;        
        private readonly Queue<SocketMessage> m_ReceivePackages;
        private readonly CongestionRecorder m_Waiter;
        private long m_TimeoutTicks;
        private long m_PingTicks;
        public Line(EndPoint end_point)
        {
            EndPoint = end_point;
            m_Dispenser = new BufferDispenser(EndPoint, SocketMessageFactory.Instance);
            m_Rectifier = new PackageRectifier();
            
            m_SendPackages = new Queue<SocketMessage>();
            m_ReceivePackages = new Queue<SocketMessage>();
            m_InputPackages = new Queue<SocketMessage>();
            
            m_Waiter = new CongestionRecorder(HungryLimit: 3);

            ResetTimeout();
        }

        private void ResetTimeout()
        {
            m_TimeoutTicks = (long)(Timestamp.OneSecondTicks * Config.Timeout);
        }

        public void WriteOperation(PeerOperation Operation)
        {
            var package = m_Dispenser.PackingOperation(Operation , m_Rectifier.Serial, m_Rectifier.SerialBitFields);
            lock (m_SendPackages)
            {
                m_SendPackages.Enqueue(package);
            }
            
        }
        public void WriteTransmission(byte[] Buffer)
        {            
                var packages = m_Dispenser.PackingTransmission(Buffer, (ushort)(m_Rectifier.Serial-1), m_Rectifier.SerialBitFields);
                for (var i = 0; i < packages.Length; i++)
                {
                    var message = packages[i];
                    QueueThreadHelper.SafeEnqueue(m_InputPackages, message);                    
                }
        }

        public SocketMessage Read()
        {
            return m_Rectifier.PopPackage();
        }

        

        public int AcknowledgeCount {get { return m_Waiter.Count; } }
        public int WaitSendCount {get { return m_SendPackages.Count; } } 
        public int SendBytes { get; private set; }
        public int ReceiveBytes { get; private set; }
        public long Srtt {get { return m_Waiter.Srtt; } } 
        public long Rto {get { return m_Waiter.Rto; } }
        public int SendedPackages { get; private set; }
        public int SendLostPackages { get; private set; }
        public int ReceivePackages { get; private set; }
        public int ReceiveInvalidPackages { get; private set; }
        public long LastRtt {get { return m_Waiter.LastRtt; }} 
        public int SendBlock { get { return m_Waiter.Count; } } 
        public long LastRto { get { return m_Waiter.LastRto; } } 
        public int ReceiveBlock { get { return m_Rectifier.Count; } } 

        public int ReceiveNumber { get { return m_Rectifier.Serial; } }

        public int SendNumber { get { return m_Dispenser.Serial; } } 

        public void Input(SocketMessage Message)
        {
            lock (m_ReceivePackages)
            {
                m_ReceivePackages.Enqueue(Message);
            }
            
            

        }

        private void SendPing()
        {            
            SendAck((ushort)(m_Rectifier.Serial-1), m_Rectifier.SerialBitFields);
        }
        
        private void SendAck(ushort Ack,uint AckBits)
        {
            var package = m_Dispenser.PackingAck(Ack, AckBits);
            lock (m_SendPackages)
            {
                m_SendPackages.Enqueue(package);
            }            
        }

        public bool Tick(Timestamp Time)
        {
            m_TimeoutTicks -= Time.DeltaTicks;
            if (m_TimeoutTicks < 0)
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
            while (m_Waiter.IsFull() == false && (message = QueueThreadHelper.SafeDequeue(m_InputPackages))!=null)
            {
                m_Waiter.PushWait(message , Time.Ticks);
                lock (m_SendPackages)
                {
                    m_SendPackages.Enqueue(message);
                }
            }
        }


        private void HandlePing(Timestamp Time)
        {
            m_PingTicks += Time.DeltaTicks;
            if (m_PingTicks > Timestamp.OneSecondTicks)
            {
                m_PingTicks = 0;
                if (m_SendPackages.Count == 0)
                    SendPing();
            }
        }

        private void HandleReceive(Timestamp Time)
        {
            SocketMessage message = null;
            while ((message = Dequeue())!=null)
            {
                ResetTimeout();

                var seq = message.GetSeq();
                var ack = message.GetAck();
                var ackFields = message.GetAckFields();                
                var oper = (PeerOperation)message.GetOperation();

                // ack 
                if (m_Waiter.Reply(ack, Time.Ticks, Time.DeltaTicks) == false)
                {
                        
                }
                //_Waiter.ReplyBefore((ushort)(ack - 1), time.Ticks , time.DeltaTicks);
                //_Waiter.ReplyAfter((ushort)(ack - 1), ackFields , time.Ticks, time.DeltaTicks);
                //_Waiter.Padding();

                
                

                if (oper != PeerOperation.Acknowledge)
                {
                    if (m_Rectifier.PushPackage(message) == false)
                        ReceiveInvalidPackages++;
                    SendAck(seq, m_Rectifier.SerialBitFields);
                }
                
                    

                ReceiveBytes += message.GetPackageSize();
                ReceivePackages++;
            }

            
        }

        private SocketMessage Dequeue()
        {
            lock (m_ReceivePackages)
            {
                if (m_ReceivePackages.Count > 0)
                    return m_ReceivePackages.Dequeue();
                return null;
            }

            
        }

        private void HandleResend(Timestamp Time)
        {
            var rtos = m_Waiter.PopLost(Time.Ticks,Time.DeltaTicks);

            var count = rtos.Count;
            for (var i = 0; i < count; i++)
                QueueThreadHelper.SafeEnqueue(m_SendPackages, rtos[i]);

            SendLostPackages += count;
        }

   
        private void HandleOutput(Timestamp Time)
        {
            


            SocketMessage message = null;
            while ((message = PopSend()) != null  )
            {                
                
                OutputEvent(message);

                SendBytes += message.GetPackageSize();
                SendedPackages++;
            }
            
        }

        private SocketMessage PopSend()
        {
            lock (m_SendPackages)
            {
                if (m_SendPackages.Count > 0  )
                    return m_SendPackages.Dequeue();
            }
            return null;
        }

        public void MessageSendResult(SocketMessage Message)
        {
            
            
        }
    }

    
}