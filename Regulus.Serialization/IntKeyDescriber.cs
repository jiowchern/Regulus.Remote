using System;
using System.Collections.Generic;

namespace Regulus.Serialization
{
    public class IntKeyDescriber : IKeyDescriber
    {
        private readonly Dictionary<Type, int> _Ids;
        private readonly Dictionary<int, Type> _Types;

        public IntKeyDescriber(ITypeDescriber[] describers)
        {

            _Types = new Dictionary<int, Type>();
            _Ids = new Dictionary<Type, int>();
            for (int i = 0; i < describers.Length; i++)
            {
                int id = i + 1;
                ITypeDescriber des = describers[i];
                _Ids.Add(des.Type, id);
                _Types.Add(id, des.Type);
            }
        }


        int _Get(Type type)
        {
            int id;
            _Ids.TryGetValue(type, out id);
            return id;
        }

        Type _Get(int id)
        {
            Type type;
            _Types.TryGetValue(id, out type);
            return type;
        }

        int IKeyDescriber.GetByteCount(Type type)
        {
            int id = _Get(type);
            int idCount = Varint.GetByteCount(id);
            return idCount;
        }

        int IKeyDescriber.ToBuffer(Type type, Regulus.Memorys.Buffer buffer, int begin)
        {
            var bytes = buffer.Bytes;
            int id = _Get(type);
            return Varint.NumberToBuffer(bytes.Array, bytes.Offset + begin, id);
        }

        int IKeyDescriber.ToObject(Regulus.Memorys.Buffer buffer, int begin, out Type type)
        {
            int id;
            int count = Varint.BufferToNumber(buffer, begin, out id);
            type = _Get(id);
            return count;
        }
    }


}