using System;
using Regulus.Serialization.Expansion;

namespace Regulus.Serialization
{
    public class EnumTypeProvider<T> : ITypeProvider 
    {
        private readonly int _Id;

        public EnumTypeProvider(int id)
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

        void ITypeProvider.Serialize(object instance, byte[] buffer, int data_index, out int read_count)
        {
            var val = (int)instance;
            val.ToBytes(buffer ,data_index , out read_count);
        }

        object ITypeProvider.Deserialize(byte[] buffer, int begin, int end)
        {

            var enums = Enum.GetValues(typeof (T)) as System.Collections.IList;            
            int readed;
            var index = buffer.ToStruct<int>(begin, out readed);
            return enums[index];
        }

        int ITypeProvider.GetBufferSize(object instance)
        {
            return 4;
        }

        int ITypeProvider.GetFieldId(string name)
        {
            throw new NotImplementedException();
        }

        Type ITypeProvider.GetFieldType(int field)
        {
            throw new NotImplementedException();
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