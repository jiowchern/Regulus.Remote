using System;

namespace Regulus.Serialization
{
    public interface ITypeDescriberFinder<in TKey>
    {
        ITypeDescriber Get(TKey id);        
    }
}