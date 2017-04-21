using System;
using System.Reflection;

namespace Regulus.Serialization
{


    public interface ITypeProvider 
    {
        int Id { get; }
        Type InstanceType { get; }
        
        void Serialize(object instance, byte[] buffer, int data_index, out int read_count);
        object Deserialize(byte[] buffer, int begin , int end);
        
        int GetBufferSize(object instance);
        int GetFieldId(string name);

        Type GetFieldType(int field);

        
        void SetField(int field_id , object owner , object field_value);
        bool NeedSerializerData();
    }
}