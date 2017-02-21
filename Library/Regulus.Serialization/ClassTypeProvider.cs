using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace Regulus.Serialization
{
    public class ClassTypeProvider<T> : ITypeProvider where T : new()
    {
        struct Field
        {
            public int Id;
            public FieldInfo Info;
        }
        private readonly int _Id;

        private readonly Field[] _Fields;

        public ClassTypeProvider(int id )
        {
            _Id = id;
            int fieldId = 0;
            _Fields = (from info in typeof(T).GetFields() where info.IsStatic == false && info.IsPublic select new Field {Id = ++fieldId , Info = info }).ToArray();            
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
            return new T();
        }

        

        int ITypeProvider.GetBufferSize(object instance)
        {
            return 0;
        }

        int ITypeProvider.GetFieldId(string name)
        {
            return (from field in _Fields where field.Info.Name == name select field.Id).Single();
        }

        Type ITypeProvider.GetFieldType(int id)
        {
            return (from field in _Fields where field.Id == id select field.Info.FieldType ).Single();
        }

        void ITypeProvider.Serialize(object instance, byte[] buffer, int data_index, out int read_count)
        {
            read_count = 0;
        }

        void ITypeProvider.SetField(int field_id, object owner, object field_value)
        {
            var info = (from field in _Fields where field.Id == field_id select field.Info).Single();
            info.SetValue(owner , field_value);
        }

        bool ITypeProvider.NeedSerializerData()
        {
            return false;
        }
    }
}