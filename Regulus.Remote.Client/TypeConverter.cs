using System;

namespace Regulus.Remote.Client
{
    public class TypeConverter
    {
        public readonly Type Target;
        public readonly System.Func<string, object> Converter;
        public TypeConverter(Type target, Func<string, object> converter)
        {
            Target = target;
            Converter = converter;
        }
    }
}