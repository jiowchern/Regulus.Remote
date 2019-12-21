using System;

namespace Regulus.Serialization.Dynamic
{
    public class CustomFinder : ITypeFinder
    {
        private readonly Func<string, Type> _Find;
        public CustomFinder(Func<string, Type> finder)
        {
            _Find = finder;
        }
        Type ITypeFinder.Find(string type)
        {
            return _Find(type);
        }
    }
}