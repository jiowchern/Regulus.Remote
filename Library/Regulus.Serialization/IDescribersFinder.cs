using System;

namespace Regulus.Serialization
{
    public interface IDescribersFinder
    {
        ITypeDescriber Get(Type id);
    }
}