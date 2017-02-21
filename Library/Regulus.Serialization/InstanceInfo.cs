using System;
using System.Runtime.InteropServices;
using Regulus.Serialization.Expansion;
namespace Regulus.Serialization
{
    
    public struct InstanceInfo
    {
        public readonly int Id;
        
        public readonly int Parent;
        public readonly int ParentField;                
        

        public InstanceInfo(int id  , int parent, int parent_field  )
        {            
            Parent = parent;
            ParentField = parent_field;
            Id = id;                    
        }

        public static InstanceInfo FromBuffer(byte[] buffer, int point, out int read_count)
        {
            return buffer.ToStruct<InstanceInfo>(point , out read_count);
        }

        public static int Size()
        {
            return System.Runtime.InteropServices.Marshal.SizeOf(typeof(InstanceInfo));
        }


        public void ToBuffer(byte[] buffer, int buffer_index, out int read_count)
        {
            this.ToBytes(buffer , buffer_index ,out read_count);            
        }
    }
}