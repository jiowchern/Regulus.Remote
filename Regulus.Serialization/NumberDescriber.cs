using System;
using System.Runtime.InteropServices;

namespace Regulus.Serialization
{

   
    public class NumberDescriber<T> : NumberDescriber
    {
        public NumberDescriber() : base(typeof(T)) { }
    }
    public class NumberDescriber : ITypeDescriber
    {

        private readonly Type _Type;
        private readonly object _Default;
        private readonly int _Size;


        public NumberDescriber(Type type)
        {

            var t = new System.Tuple<int>(0);
            _Default = Activator.CreateInstance(type);

            _Size = Marshal.SizeOf(type);


            _Type = type;
        }



        public Type Type { get { return _Type; } }

        int ITypeDescriber.GetByteCount(object instance)
        {
            ulong instanceVal = _GetUInt64(instance);
            return Varint.GetByteCount(instanceVal);
        }
        public object Default { get { return _Default; } }
        int ITypeDescriber.ToBuffer(object instance, Regulus.Memorys.Buffer buffer, int begin)
        {
            var bytes = buffer.Bytes;
            ulong instanceVal = _GetUInt64(instance);
            return Varint.NumberToBuffer(bytes.Array, bytes.Offset + begin, instanceVal);
        }


        private ulong _GetUInt64(object instance)
        {
            byte[] bytes = new byte[_Size];

            IntPtr ptr = Marshal.AllocHGlobal(_Size);

            Marshal.StructureToPtr(instance, ptr, false);

            Marshal.Copy(ptr, bytes, 0, _Size);

            Marshal.FreeHGlobal(ptr);

            ulong val = 0ul;
            for (int i = 0; i < _Size; i++)
            {
                ulong b = (ulong)bytes[i];
                b <<= i * 8;
                val |= b;
            }
            return val;
        }


        int ITypeDescriber.ToObject(Regulus.Memorys.Buffer buffer, int begin, out object instance)
        {
            ulong value;
            int readed = Varint.BufferToNumber(buffer, begin, out value);

            if (Type == typeof(byte))
            {
                instance = (byte)value;
            }
            else if (Type == typeof(short))
            {
                instance = (short)value;
            }
            else if (Type == typeof(ushort))
            {
                instance = (ushort)value;
            }
            else if (Type == typeof(int))
            {
                instance = (int)value;
            }
            else if (Type == typeof(uint))
            {
                instance = (uint)value;
            }
            else if (Type == typeof(long))
            {
                instance = (long)value;
            }
            else if (Type == typeof(char))
            {
                instance = (char)value;
            }
            else if (Type == typeof(bool))
            {
                instance = value != 0;
            }
            else
            {
                instance = value;
            }


            return readed;
        }


        private static T _Cast<T>(object o)
        {
            return (T)o;
        }


    }
}