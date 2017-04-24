using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

using Regulus.Extension;
using Regulus.Serialization.Expansion;

namespace Regulus.Serialization
{
    public class Serializer
    {
        private readonly ITypeDescriber[] _Describers;

        public Serializer(params ITypeDescriber[] describers)
        {
            _Describers = describers;

            foreach (var typeDescriber in _Describers)
            {
                typeDescriber.SetMap(_Describers);
            }          
        }


        public class ZigZag
        {
            public static uint Encode(int number)
            {
                return (uint)((number << 1) ^ (number >> 31));
            }


            public static ulong Encode(long number)
            {
                return (ulong)((number << 1) ^ (number >> 63));
            }


            public static int Decode(uint number)
            {
                return (int)(number >> 1) ^ -(int)(number & 1);
            }

            public static long Decode(ulong number)
            {
                return (long)(number >> 1) ^ -(long)(number & 1);
            }
        }

        public class Varint
        {

            public static int NumberToBuffer(byte[] buffer, int offset, int value)
            {
                return NumberToBuffer(buffer , offset , (ulong)value);
            }
            public static int NumberToBuffer(byte[] buffer, int offset, ulong value)
            {
                int i = 0;
                while (value >= 0x80)
                {
                    buffer[offset + i] = (byte)(value | 0x80);
                    value >>= 7;
                    i++;
                }
                buffer[offset + i] = (byte)value;
                return i + 1;
            }

            public static int GetByteCount(int value)
            { 
                return GetByteCount((ulong)value);
            }
            public static int GetByteCount(ulong value)
            {
                int i;
                for (i = 0; i < 9 && value >= 0x80; i++, value >>= 7) { }
                return i + 1;
            }
            

            public static int BufferToNumber(byte[] buffer, int offset, out ulong value)
            {
                value = 0;
                int s = 0;
                for (var i = 0; i < buffer.Length - offset; i++)
                {
                    if (buffer[offset + i] < 0x80)
                    {
                        if (i > 9 || i == 9 && buffer[offset + i] > 1)
                        {
                            value = 0;
                            return -(i + 1); // overflow
                        }
                        value |= (ulong)(buffer[offset + i] << s);
                        return i + 1;
                    }
                    value |= (ulong)(buffer[offset + i] & 0x7f) << s;
                    s += 7;
                }
                value = 0;
                return 0;
            }
        }


        
        public byte[] ObjectToBuffer(object instance)
        {
            var type = instance.GetType();
           
            return ObjectToBuffer(instance , type);
        }

        public byte[] ObjectToBuffer<T>(T instance)
        {
            return ObjectToBuffer(instance, typeof(T));
        }
        public byte[] ObjectToBuffer(object instance , Type type)
        {
            var describer = _GetDescriber(type);
            var id = describer.Id;
            var idCount = Varint.GetByteCount((ulong)id);
            var bufferCount = describer.GetByteCount(instance);
            var buffer = new byte[idCount + bufferCount];
            var readCount = Varint.NumberToBuffer(buffer, 0, id);
            describer.ToBuffer(instance, buffer, readCount);
            return buffer;
        }


        public T BufferToObject<T>(byte[] buffer)
        {
            return (T)BufferToObject(buffer);
        }
        public object BufferToObject(byte[] buffer)
        {
            ulong id;
            var readIdCount = Varint.BufferToNumber(buffer, 0, out id);
            var describer = _GetDescriber((int)id);
            object instance;
            describer.ToObject(buffer, readIdCount , out instance);

            return instance;
        }

        private ITypeDescriber _GetDescriber(int id)
        {
            return _Describers.First(describer => describer.Id == id);
        }

        private ITypeDescriber _GetDescriber(Type type)
        {
            return _Describers.First(describer => describer.Type == type);
        }

        
    }
}


