using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Regulus.Framework;
using Regulus.Network.Package;
using Regulus.Utility;

namespace Regulus.Network
{
    public class Socket : IUpdatable<Timestamp>
    {

        public event Action CloseEvent;
        private readonly Line m_Line;

        private readonly StageMachine<Timestamp> m_Machine;

        private readonly SegmentStream m_Stream;
        private PeerStatus m_Status;

        private class Reader
        {
            public byte[] Buffer;
            public int Offset;
            public int Count;
            public Action<int, SocketError> DoneHandler;
        }
        private readonly List<byte> m_Sends;

        private readonly Reader m_Reader;
        public Action<int, SocketError> WriteDoneHandler;

        private bool m_RequireDisconnect;



        private Socket(Line Line)
        {
            m_Reader = new Reader();
            m_Sends = new List<byte>();
            m_Line = Line;
            m_Machine = new StageMachine<Timestamp>();
            m_Stream = new SegmentStream(new SocketMessage[0]);

        }
        public Socket(Line Line, PeerListener Listener) : this(Line)
        {
            m_Status = PeerStatus.Connecting;
            Listener.DoneEvent += ToTransmission;
            Listener.ErrorEvent += ToClose;
            m_Machine.Push(Listener);
        }

        public Socket(Line Line, PeerConnecter Connecter) : this(Line)
        {
            m_Status = PeerStatus.Connecting;
            Connecter.DoneEvent += ToTransmission;
            m_Machine.Push(Connecter);
        }



        private void ToTransmission()
        {
            m_Status = PeerStatus.Transmission;
            m_Machine.Push(new SimpleStage<Timestamp>(StartTransmission, EndTransmission, UpdateTransmission));
        }

        private void UpdateTransmission(Timestamp Obj)
        {
            lock (m_Sends)
            {
                if (m_Sends.Count > 0)
                {
                    m_Line.WriteTransmission(m_Sends.ToArray());
                    WriteDoneHandler(m_Sends.Count, SocketError.Success);
                    m_Sends.Clear();
                }


            }


            while (m_Stream.Count > 0)
                lock (m_Reader)
                {

                    var handler = m_Reader;
                    if (handler.Buffer != null)
                    {
                        var readCount = m_Stream.Read(handler.Buffer, handler.Offset, handler.Count);
                        if (readCount > 0)
                            handler.DoneHandler(readCount, SocketError.Success);
                    }


                }


            if (m_RequireDisconnect)
            {
                m_Line.WriteOperation(PeerOperation.RequestDisconnect);
                _Disconnect();
            }

            SocketMessage message = null;
            while ((message = m_Line.Read()) != null)
            {
                var package = message;

                var operation = (PeerOperation)package.GetOperation();
                if (operation == PeerOperation.Transmission)
                    m_Stream.Add(package);
                else if (operation == PeerOperation.RequestDisconnect)
                    _Disconnect();
            }


        }

        private void EndTransmission()
        {

        }



        private void StartTransmission()
        {

        }






        bool IUpdatable<Timestamp>.Update(Timestamp Arg)
        {
            m_Machine.Update(Arg);
            return true;
        }

        void IBootable.Launch()
        {

        }



        void IBootable.Shutdown()
        {
            m_Machine.Termination();
        }




        public EndPoint EndPoint {get{return m_Line.EndPoint; } } 

        public void Send(byte[] Buffer, int Offset, int Count, Action<int, SocketError> WriteCompletion)
        {
            var len = Count < Buffer.Length ? Count : Buffer.Length;

            lock (m_Sends)
            {
                for (var i = Offset; i < len; i++)
                    m_Sends.Add(Buffer[i]);
                WriteDoneHandler = WriteCompletion;
            }

        }



        public void Receive(byte[] Buffer, int Offset, int Count, Action<int, SocketError> ReadCompletion)
        {


            lock (m_Reader)
            {
                m_Reader.Buffer = Buffer;
                m_Reader.Count = Count;
                m_Reader.Offset = Offset;
                m_Reader.DoneHandler = ReadCompletion;
            }

        }

        public PeerStatus Status {get { return m_Status; } }

        public void Disconnect()
        {
            m_RequireDisconnect = true;
        }


        private void _Disconnect()
        {
            m_Status = PeerStatus.Disconnect;
            var stage = new PeerDisconnecter(m_Line);
            stage.DoneEvent += ToClose;
            m_Machine.Push(stage);
        }

        private void ToClose()
        {
            CloseEvent();
            m_Status = PeerStatus.Close;
            m_Machine.Empty();
        }



        public void Break()
        {
            if (m_Status != PeerStatus.Close)
                ToClose();
        }
    }
}