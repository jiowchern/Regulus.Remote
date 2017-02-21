using System;

namespace Regulus.Serialization
{
    public class TypeInfo
    {
        public readonly int TypeId;
        public readonly int Count;


        private TypeInfo(int type_id, int count)
        {
            TypeId = type_id;
            Count = count;
        }


        public static TypeInfo Create(byte[] buffer, int point)
        {
            var typeId = BitConverter.ToInt32(buffer, point + 0);
            var count = BitConverter.ToInt32(buffer, point + 4);

            return new TypeInfo(typeId, count);
        }
    }
}