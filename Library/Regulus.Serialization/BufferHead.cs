using System;
using System.Runtime.InteropServices;

namespace Regulus.Serialization
{    
    public struct BufferHead
    {
        public readonly int Type;
        public readonly int InstanceAmount;
        public readonly int ValueAmount;
        public BufferHead( int type , int instance_amount, int value_amount)
        {
            Type = type;
            InstanceAmount = instance_amount;
            ValueAmount = value_amount;
        }

        public static int Size()
        {
            return System.Runtime.InteropServices.Marshal.SizeOf(typeof (BufferHead));
        }


    }
}