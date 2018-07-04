using System;

namespace Regulus.Serialization
{
    public interface IDescribersFinder
    {
        IKeyDescriber Get();
        ITypeDescriber Get(Type id);
    }
}