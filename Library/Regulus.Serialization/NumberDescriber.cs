using System;

using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Regulus.Serialization
{


    public class NumberDescriber<T> : NumberDescriber
    {
        public NumberDescriber(int id) : base(id , typeof(T)) { }
    }
    public class NumberDescriber : ITypeDescriber 
    {
        private readonly int _Id     ;
        private readonly Type _Type;
        private readonly object _Default;
        private readonly int _Size;


        public NumberDescriber(int id , Type type)
        {

            _Default = Activator.CreateInstance(type);

            _Size = Marshal.SizeOf(type);
            
            _Id = id;
            _Type = type;
        }

        int ITypeDescriber.Id
        {
            get { return _Id; }
        }

        public Type Type { get { return _Type; } }

        int ITypeDescriber.GetByteCount(object instance)
        {
            var instanceVal = _GetUInt64(instance);
            return Varint.GetByteCount(instanceVal);
        }
        public object Default { get { return _Default; } }
        int ITypeDescriber.ToBuffer(object instance, byte[] buffer, int begin)
        {            
            var instanceVal = _GetUInt64(instance);
            return Varint.NumberToBuffer(buffer, begin, instanceVal);
        }
        

        private ulong _GetUInt64(object instance)
        {
            var bytes = new byte[_Size];

            var ptr = Marshal.AllocHGlobal(_Size);
            
            Marshal.StructureToPtr(instance, ptr, false);
            
            Marshal.Copy(ptr, bytes, 0, _Size);            

            Marshal.FreeHGlobal(ptr);

            var val = 0ul;
            for (int i = 0; i < _Size; i++)
            {
                var b =(ulong) bytes[i];
                b <<= i * 8;
                val |= b;
            }
            return val;
        }


        int ITypeDescriber.ToObject(byte[] buffer, int begin, out object instance)
        {
            ulong value;            
            var readed = Varint.BufferToNumber(buffer, begin, out value);

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
        public void SetMap(TypeSet type_set)
        {
 
        }
    }
}