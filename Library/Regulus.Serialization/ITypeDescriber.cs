using System;

namespace Regulus.Serialization
{
    public interface ITypeDescriber   
    {
        int Id { get;  }
        Type Type { get;  }
        int GetByteCount(object instance);
        int ToBuffer(object instance, byte[] buffer, int begin);
        int ToObject(byte[] buffer, int begin, out object instnace );



        void SetMap(ITypeDescriber[] describer);
    }
}