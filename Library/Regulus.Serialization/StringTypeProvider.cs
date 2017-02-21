using System;

namespace Regulus.Serialization
{
    public class StringTypeProvider : ITypeProvider
    {
        private readonly int _Id;

        public StringTypeProvider(int id)
        {
            _Id = id;
            
        }

        int ITypeProvider.Id
        {
            get { return _Id; }
        }

        Type ITypeProvider.InstanceType
        {
            get { return typeof(string);  }
        }

        void ITypeProvider.Serialize(object instance, byte[] buffer, int data_index, out int read_count)
        {
            var val = (string) instance;
            var bytes = System.Text.Encoding.UTF8.GetBytes(val);

            for (int i = 0; i < bytes.Length; i++)
            {
                buffer[i + data_index] = bytes[i];
                
            }
            read_count = bytes.Length;
        }

        object ITypeProvider.Deserialize(byte[] buffer, int begin, int end)
        {
            return System.Text.Encoding.UTF8.GetString(buffer, begin, end - begin);
        }

        int ITypeProvider.GetBufferSize(object instance)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes((string)instance);
            return bytes.Length;
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