using System;
using System.Linq;

namespace Regulus.Remote.Client
{
    public class TypeConverterSet
    {
        private readonly TypeConverter[] converters;

        public TypeConverterSet(params TypeConverter[] converters)
        {
            this.converters = converters;
        }
        internal bool Convert(string inArg, out object val, Type parameterType)
        {
            TypeConverter converter = converters.FirstOrDefault((c) => c.Target == parameterType);
            if (converter != null)
                val = converter.Converter(inArg);
            val = null;
            return converter != null;
        }
    }
}