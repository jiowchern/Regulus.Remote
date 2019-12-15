using System;

namespace Regulus.Serialization
{
    public interface IKeyDescriber
    {
        int GetByteCount(Type type);
        int ToBuffer(Type type, byte[] buffer, int begin);
        int ToObject(byte[] buffer, int begin, out Type type);
    }
}