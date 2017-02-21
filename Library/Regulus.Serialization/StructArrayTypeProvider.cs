using System;
using System.Runtime.InteropServices;
using Regulus.Serialization.Expansion;

namespace Regulus.Serialization
{
    public class StructArrayTypeProvider<T> : ITypeProvider where T : struct
    {
        private readonly int _Id;

        public StructArrayTypeProvider(int id)
        {
            _Id = id;
        }

        int ITypeProvider.Id
        {
            get { return _Id; }
        }

        Type ITypeProvider.InstanceType
        {
            get { return typeof (T[]); }
        }

        

        object ITypeProvider.Deserialize(byte[] buffer, int begin, int end)
        {            
            return buffer.ToStructArray<T>( begin, end);
        }

        

        int ITypeProvider.GetBufferSize(object instance)
        {
            T[] obj = (T[]) instance;
            return obj.Length * Marshal.SizeOf(typeof(T));
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
            T[] obj = (T[])instance;            
            obj.ToBytes(buffer, data_index, out read_count);
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