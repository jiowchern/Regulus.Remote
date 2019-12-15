using System;

namespace Regulus.Serialization.Dynamic
{
    public interface ITypeFinder
    {
        Type Find(string type);
    }
}