using System;

namespace Regulus.Serialization
{
    public interface IKeyDescriber
    {
        int GetByteCount(Type type);
        int ToBuffer(Type type, Regulus.Memorys.Buffer buffer, int begin);
        int ToObject(Regulus.Memorys.Buffer buffer, int begin, out Type type);
    }
}