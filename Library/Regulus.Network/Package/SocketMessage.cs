using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Regulus.Network.Package
{
    internal class SocketMessageInternal
    {

        public SocketMessageInternal(int PackageSize)
        {
            
            RemoteEndPoint = new IPEndPoint(IPAddress.Any, port: 0);
            Package = new byte[PackageSize];
        }
        public EndPoint RemoteEndPoint;        
        public byte[] Package;
        public SocketError Error;
    }

    public sealed class SocketMessage : IRecycleable<SocketMessageInternal>
    {
        public const int SeqIndex = 0;
        public const int AckIndex = SeqIndex + 2;
        public const int AckbitsIndex = AckIndex + 2;
        public const int OpIndex = AckbitsIndex + 4;
        public const int PayloadlengthIndex = OpIndex + 1;
        public const int PayloadIndex = PayloadlengthIndex + 2;
        public const int Headsize = PayloadIndex + 2;
        public const int Udppackagesize = ushort.MaxValue;

        private SocketMessageInternal m_Instance;

        public SocketMessage()
        {
            
        }
        public SocketMessage(int I)
        {
            m_Instance = new SocketMessageInternal(I);
        }


        private byte[] _Package { get { return m_Instance.Package; } }

        public EndPoint RemoteEndPoint { get { return m_Instance.RemoteEndPoint; } }

        public byte[] Package {get { return _Package; } }

        public SocketError Error { get { return m_Instance.Error; } }

        void IRecycleable<SocketMessageInternal>.Reset(SocketMessageInternal Instance)
        {
            
            m_Instance = Instance;
        }


        public bool CheckPayload()
        {
            var len = GetPayloadLength();
            return len == _Package.Length - PayloadIndex;
        }

        public int GetPayloadBufferSize()
        {
            var size = _Package.Length - PayloadIndex;
            if (size > 0)
                return size;
            return 0;
        }

        public bool WritePayload(IList<byte> Source, int Offset, int Count)
        {
            var bufferSize = GetPayloadBufferSize();

            if (Count > ushort.MaxValue)
                return false;
            if (bufferSize < Count)
                return false;
            if (Source.Count - Offset < Count)
                return false;


            for (var i = 0; i < Count; i++)
                _Package[PayloadIndex + i] = Source[Offset + i];

            SetPayloadLength((ushort)Count);

            return true;
        }


        private void SetPayloadLength(ushort Value)
        {
            SetUint16(PayloadlengthIndex, Value);
        }

        public ushort GetPayloadLength()
        {
            return GetUint16(PayloadlengthIndex);
        }

        public bool ReadPayload(IList<byte> Payload, int Offset)
        {

            var len = GetPayloadLength();

            if (_Package.Length - PayloadIndex < len)
                return false;
            if (Payload.Count - Offset < len)
                return false;

            for (var i = 0; i < len; ++i)
                Payload[Offset + i] = _Package[PayloadIndex + i];
            return true;
        }

        public void ReadPayload(IList<byte> Payload)
        {
            var len = GetPayloadLength();
            for (var i = 0; i < len; ++i)
                Payload.Add(_Package[PayloadIndex + i]);
        }


        public void SetOperation(byte Value)
        {
            SetUint8(OpIndex, checked((byte)Value));
        }
        public byte GetOperation()
        {
            return GetUint8(OpIndex);
        }




        public void SetAckFields(uint Value)
        {
            SetUint32(AckbitsIndex, Value);
        }

        public uint GetAckFields()
        {
            return GetUint32(AckbitsIndex);
        }


        public ushort GetAck()
        {
            return GetUint16(AckIndex);
        }

        public void SetAck(ushort Value)
        {
            SetUint16(AckIndex, Value);
        }


        public void SetSeq(ushort Sn)
        {
            SetUint16(SeqIndex, (ushort)Sn);
        }

        public ushort GetSeq()
        {
            return GetUint16(SeqIndex);
        }

        private void SetUint32(int Begin, uint Value)
        {
            _Package[Begin + 0] = (byte)(Value >> 0);
            _Package[Begin + 1] = (byte)(Value >> 8);
            _Package[Begin + 2] = (byte)(Value >> 16);
            _Package[Begin + 3] = (byte)(Value >> 24);
        }

        private uint GetUint32(int Begin)
        {
            if (_Package.Length - Begin > 4)
                return (uint)((_Package[0 + Begin] << 0) | (_Package[1 + Begin] << 8) | (_Package[2 + Begin] << 16) | (_Package[3 + Begin] << 24));
            throw new Exception("Illegal operation");
        }

        private void SetUint16(int Begin, ushort Value)
        {
            _Package[Begin + 0] = (byte)(Value >> 0);
            _Package[Begin + 1] = (byte)(Value >> 8);
        }

        private ushort GetUint16(int Begin)
        {
            if (_Package.Length - Begin > 2)
                return (ushort)((_Package[Begin + 0] << 0) | (_Package[Begin + 1] << 8));
            throw new Exception("Illegal operation");
        }

        private byte GetUint8(int Index)
        {
            if (_Package.Length - Index > 1)
                return _Package[Index];
            throw new Exception("Illegal operation");
        }

        private void SetUint8(int Begin, byte Value)
        {
            if (_Package.Length - Begin > 1)
                _Package[Begin] = Value;
        }

        public IEnumerable<ushort> GetAcks()
        {
            var ack = GetAck();
            var fields = GetAckFields();
            ushort mark = 1;
            for (ushort i = 0; i < 32; i++)
                if ((mark & fields) != 0)
                    yield return (ushort)(ack + i + 1);
        }

        public static int GetHeadSize()
        {
            return Headsize;
        }

        public int ReadPayload(int SourceOffset, byte[] Buffer, int TargetOffset, int ReadCount)
        {
            var payloadLength = GetPayloadLength();
            if (payloadLength < SourceOffset)
                return 0;
            if (Buffer.Length < TargetOffset + ReadCount)
                return 0;


            var count = ReadCount;
            var payloadReadCount = payloadLength - SourceOffset;
            if (payloadReadCount < ReadCount)
                count = payloadReadCount;

            for (var i = 0; i < count; i++)
                Buffer[TargetOffset + i] = _Package[PayloadIndex + i + SourceOffset];

            return count;
        }

        public byte ReadPayload(int Offset)
        {
            return _Package[PayloadIndex + Offset];
        }

        public byte[] GetBuffer()
        {
            return _Package;
        }

        public void SetError(SocketError error)
        {
            m_Instance.Error = error;
        }

        public static int GetPayloadSize()
        {
            
            return Config.Default.PackageSize - Headsize;
        }

        public bool IsError()
        {
            return m_Instance.Error != SocketError.Success;
        }

        public void SetEndPoint(EndPoint EndPoint)
        {
            m_Instance.RemoteEndPoint = EndPoint;
        }

        public int GetPackageSize()
        {            
            return Headsize + GetPayloadLength();
        }

        public void ClearPayload()
        {
            SetPayloadLength(Value: 0);
        }
    }

  

}