using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Regulus.Serialization.Expansion;

namespace Regulus.Serialization
{
    public class StructTypeProvider<T> : ITypeProvider 
        where T : struct  
    {
        private readonly int _Id;

        public StructTypeProvider(int id)
        {
            _Id = id;
        }

        int ITypeProvider.Id
        {
            get { return _Id; }
        }

       

        Type ITypeProvider.InstanceType
        {
            get { return typeof (T); }
        }

        

        object ITypeProvider.Deserialize(byte[] buffer, int begin, int end)
        {
            int readed;
            return buffer.ToStruct<T>(begin, out readed);
        }

        

        

        int ITypeProvider.GetBufferSize(object instance)
        {
            return Marshal.SizeOf(typeof(T));
        }

        int ITypeProvider.GetFieldId(string name)
        {
            throw new NotImplementedException();
        }

        Type ITypeProvider.GetFieldType(int field)
        {
            throw new NotImplementedException();
        }

        void ITypeProvider.Serialize(object instance, byte[] buffer, int data_index, out int read_count)
        {
            T obj = (T) instance;
            obj.ToBytes(buffer , data_index , out read_count );
        }

        void ITypeProvider.SetField(int field_id, object owner, object field_value)
        {
            throw new NotImplementedException();
        }

        bool ITypeProvider.NeedSerializerData()
        {
            return true;
        }

       
    }
}
