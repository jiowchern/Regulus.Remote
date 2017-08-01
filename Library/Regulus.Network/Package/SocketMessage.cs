using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Regulus.Game.Data;

namespace Regulus.Network.RUDP
{
    internal class SocketMessageInternal
    {

        public SocketMessageInternal(int package_size)
        {
            
            RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            Package = new byte[package_size];
        }
        public EndPoint RemoteEndPoint;        
        public byte[] Package;
        public SocketError Error;
    }

    public sealed class SocketMessage : IRecycleable<SocketMessageInternal>
    {
        public const int SEQ_INDEX = 0;
        public const int ACK_INDEX = SEQ_INDEX + 2;
        public const int ACKBITS_INDEX = ACK_INDEX + 2;
        public const int OP_INDEX = ACKBITS_INDEX + 4;
        public const int PAYLOADLENGTH_INDEX = OP_INDEX + 1;
        public const int PAYLOAD_INDEX = PAYLOADLENGTH_INDEX + 2;
        public const int HEADSIZE = PAYLOAD_INDEX + 2;
        public const int UDPPACKAGESIZE = ushort.MaxValue;

        private SocketMessageInternal _Instance;

        public SocketMessage()
        {
            
        }
        public SocketMessage(int i)
        {
            _Instance = new SocketMessageInternal(i);
        }


        private byte[] _Package { get { return _Instance.Package; } }

        public EndPoint RemoteEndPoint { get { return _Instance.RemoteEndPoint; } }
        
        public byte[] Package { get { return _Package; } }

        public SocketError Error { get { return _Instance.Error; } }

        void IRecycleable<SocketMessageInternal>.Reset(SocketMessageInternal instance)
        {
            
            _Instance = instance;
        }


        public bool CheckPayload()
        {
            var len = GetPayloadLength();
            return len == _Package.Length - PAYLOAD_INDEX;
        }

        public int GetPayloadBufferSize()
        {
            var size = _Package.Length - PAYLOAD_INDEX;
            if (size > 0)
            {
                return size;
            }
            return 0;
        }

        public bool WritePayload(IList<byte> source, int offset, int count)
        {
            var bufferSize = GetPayloadBufferSize();

            if (count > ushort.MaxValue)
                return false;
            if (bufferSize < count)
                return false;
            if (source.Count - offset < count)
                return false;


            for (int i = 0; i < count; i++)
            {
                _Package[PAYLOAD_INDEX + i] = source[offset + i];
            }

            _SetPayloadLength((ushort)count);

            return true;
        }


        private void _SetPayloadLength(ushort value)
        {
            _SetUint16(PAYLOADLENGTH_INDEX, value);
        }

        public ushort GetPayloadLength()
        {
            return _GetUint16(PAYLOADLENGTH_INDEX);
        }

        public bool ReadPayload(IList<byte> payload, int offset)
        {

            var len = GetPayloadLength();

            if (_Package.Length - PAYLOAD_INDEX < len)
                return false;
            if (payload.Count - offset < len)
                return false;

            for (int i = 0; i < len; ++i)
            {
                payload[offset + i] = _Package[PAYLOAD_INDEX + i];
            }
            return true;
        }

        public void ReadPayload(IList<byte> payload)
        {
            var len = GetPayloadLength();
            for (int i = 0; i < len; ++i)
            {
                payload.Add(_Package[PAYLOAD_INDEX + i]);
            }
        }


        public void SetOperation(byte value)
        {
            _SetUint8(OP_INDEX, checked((byte)value));
        }
        public byte GetOperation()
        {
            return _GetUint8(OP_INDEX);
        }




        public void SetAckFields(uint value)
        {
            _SetUint32(ACKBITS_INDEX, value);
        }

        public uint GetAckFields()
        {
            return _GetUint32(ACKBITS_INDEX);
        }


        public ushort GetAck()
        {
            return _GetUint16(ACK_INDEX);
        }

        public void SetAck(ushort value)
        {
            _SetUint16(ACK_INDEX, value);
        }


        public void SetSeq(ushort sn)
        {
            _SetUint16(SEQ_INDEX, (ushort)sn);
        }

        public ushort GetSeq()
        {
            return _GetUint16(SEQ_INDEX);
        }

        private void _SetUint32(int begin, uint value)
        {
            _Package[begin + 0] = (byte)(value >> 0);
            _Package[begin + 1] = (byte)(value >> 8);
            _Package[begin + 2] = (byte)(value >> 16);
            _Package[begin + 3] = (byte)(value >> 24);
        }

        private uint _GetUint32(int begin)
        {
            if (_Package.Length - begin > 4)
            {
                return (uint)(_Package[0 + begin] << 0 | _Package[1 + begin] << 8 | _Package[2 + begin] << 16 | _Package[3 + begin] << 24);
            }
            throw new Exception("Illegal operation");
        }

        private void _SetUint16(int begin, ushort value)
        {
            _Package[begin + 0] = (byte)(value >> 0);
            _Package[begin + 1] = (byte)(value >> 8);
        }

        private ushort _GetUint16(int begin)
        {
            if (_Package.Length - begin > 2)
            {
                return (ushort)(_Package[begin + 0] << 0 | _Package[begin + 1] << 8);
            }
            throw new Exception("Illegal operation");
        }

        private byte _GetUint8(int index)
        {
            if (_Package.Length - index > 1)
            {
                return _Package[index];
            }
            throw new Exception("Illegal operation");
        }

        private void _SetUint8(int begin, byte value)
        {
            if (_Package.Length - begin > 1)
            {
                _Package[begin] = value;
            }
        }

        public IEnumerable<ushort> GetAcks()
        {
            var ack = GetAck();
            var fields = GetAckFields();
            ushort mark = 1;
            for (ushort i = 0; i < 32; i++)
            {
                if ((mark & fields) != 0)
                {
                    yield return (ushort)(ack + i + 1);
                }
            }
        }

        public static int GetHeadSize()
        {
            return HEADSIZE;
        }

        public int ReadPayload(int source_offset, byte[] buffer, int target_offset, int read_count)
        {
            var payloadLength = GetPayloadLength();
            if (payloadLength < source_offset)
                return 0;
            if (buffer.Length < target_offset + read_count)
                return 0;


            int count = read_count;
            var payloadReadCount = payloadLength - source_offset;
            if (payloadReadCount < read_count)
                count = payloadReadCount;

            for (int i = 0; i < count; i++)
            {
                buffer[target_offset + i] = _Package[PAYLOAD_INDEX + i + source_offset];
            }

            return count;
        }

        public byte ReadPayload(int offset)
        {
            return _Package[PAYLOAD_INDEX + offset];
        }

        public byte[] GetBuffer()
        {
            return _Package;
        }

        public void SetError(SocketError error)
        {
            _Instance.Error = error;
        }

        public static int GetPayloadSize()
        {
            return Config.PackageSize - HEADSIZE;
        }

        public bool IsError()
        {
            return _Instance.Error != SocketError.Success;
        }

        public void SetEndPoint(EndPoint end_point)
        {
            _Instance.RemoteEndPoint = end_point;
        }

        public int GetPackageSize()
        {            
            return HEADSIZE + GetPayloadLength();
        }

        public void ClearPayload()
        {
            _SetPayloadLength(0);
        }
    }

  

}