using System;

namespace Regulus.Serialization
{
    public interface ITypeDescriber
    {

        Type Type { get; }

        object Default { get; }

        int GetByteCount(object instance);
        int ToBuffer(object instance,Regulus.Memorys.Buffer buffer, int begin);
        int ToObject(Regulus.Memorys.Buffer buffer, int begin, out object instnace);
    }
}