using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Regulus.Serialization.Expansion
{
    public static class Expansion
    {
        public static void ToBytes<T>(this T source, byte[] buffer ,int begin, out int read_count) where T : struct 
        {
            GCHandle pinStructure = GCHandle.Alloc(source, GCHandleType.Pinned);
            try
            {
                read_count = Marshal.SizeOf(typeof(T));
                Marshal.Copy(pinStructure.AddrOfPinnedObject(), buffer, begin, read_count);
            }
            finally
            {
                if(pinStructure.IsAllocated)
                    pinStructure.Free();
            }
        }
        public static void ToBytes<T>(this T[] source, byte[] buffer, int begin, out int read_count) where T : struct
        {
            var size= Marshal.SizeOf(typeof (T));
            read_count = 0;
            for (int i = 0; i < source.Length; i++)
            {
                int readed;
                source[i].ToBytes(buffer , begin + i * size , out readed);
                read_count += readed;
            }           
        }



        public static T[] ToStructArray<T>(this byte[] source, int begin , int end ) where T : struct
        {
            var size = Marshal.SizeOf(typeof (T));
            var byteCount = end - begin;
            var len = byteCount/size;
            T[] target = new T[len];

            for (int i = 0; i < len; i++)
            {
                int readed;
                target[i] = ToStruct<T>(source , begin + i * size , out readed);
            }

            return target;
            
        }

        public static T ToStruct<T>(this byte[] source, int begin, out int read_count) where T : struct
        {

            int size = Marshal.SizeOf(typeof(T));
            read_count = size;
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(source, begin, ptr, size);

            T data = (T)Marshal.PtrToStructure(ptr, typeof(T));
            Marshal.FreeHGlobal(ptr);

            return data;
        }
    }
}
