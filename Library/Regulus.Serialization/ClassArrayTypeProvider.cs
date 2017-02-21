using System;
using System.Runtime.InteropServices;
using Regulus.Serialization.Expansion;

namespace Regulus.Serialization
{
    public class ClassArrayTypeProvider<T> : ITypeProvider
    {
        private readonly int _Id;

        public ClassArrayTypeProvider(int id)
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
            int readed;
            var count = buffer.ToStruct<int>(begin, out readed);
            return new T[count];
        }


        int ITypeProvider.GetBufferSize(object instance)
        {
            return Marshal.SizeOf(typeof(int));
        }

        int ITypeProvider.GetFieldId(string name)
        {
            throw new NotImplementedException();
        }

        Type ITypeProvider.GetFieldType(int field)
        {
            return typeof (T);
        }

        void ITypeProvider.Serialize(object instance, byte[] buffer, int data_index, out int read_count)
        {
            var array = (T[]) instance;
            int length = array.Length;            
            length.ToBytes(buffer, data_index , out read_count);
        }

        void ITypeProvider.SetField(int field_id, object owner, object field_value)
        {
            var array = (T[]) owner;
            array[field_id] = (T)field_value;
        }

        bool ITypeProvider.NeedSerializerData()
        {
            return true;            
        }
    }
}