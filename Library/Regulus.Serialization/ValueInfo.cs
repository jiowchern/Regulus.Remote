using System;
using System.Runtime.InteropServices;

namespace Regulus.Serialization
{
    public struct ValueInfo 
    {
        public readonly int Id;        
        public readonly int Begin;
        public readonly int End;

        public ValueInfo(int id, int begin, int end)
        {
            Id = id;
            Begin = begin;
            End = end;
        }

        public static int Size()
        {
            return Marshal.SizeOf(typeof(ValueInfo));
        }

        public bool IsValid()
        {
            return Id != 0;
        }
    }
}