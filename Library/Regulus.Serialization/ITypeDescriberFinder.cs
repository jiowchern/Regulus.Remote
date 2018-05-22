using System;

namespace Regulus.Serialization
{
    public interface ITypeDescriberFinder
    {
        ITypeDescriber GetById(int id);
        ITypeDescriber GetByType(Type type);
    }
}