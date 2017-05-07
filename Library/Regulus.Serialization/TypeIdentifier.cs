using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Regulus.Serialization
{
    public class TypeIdentifier
    {
        public enum TYPE
        {
            UNKNOWN,
            STRING,
            NUMBER,
            ARRAY,
            CLASS,
            ENUM,
            BITTABLE
        }


        public readonly TYPE Type;
        public TypeIdentifier(Type type)
        {

            Type = TYPE.UNKNOWN;
            
            if (_IsString(type))
            {
                Type = TYPE.STRING;
            }
            else if (_IsNumber(type))
            {
                Type = TYPE.NUMBER;
            }
            else if (_IsArray(type))
            {
                Type = TYPE.ARRAY;
            }
            
            else if (_IsEnum(type))
            {
                Type = TYPE.ENUM;
            }
            else if (_IsBittable(type))
            {
                Type = TYPE.BITTABLE;
            }
            else if (_IsClass(type))
            {
                Type = TYPE.CLASS;
            }
        }



        private bool _IsBittable(Type type)
        {

            return _BittableTypes.Any(t => t == type);

            
        }

        private bool _IsString(Type type)
        {
            return type == typeof(string);
        }

        private bool _IsEnum(Type type)
        {
            return type.IsEnum;
        }

        private bool _IsClass(Type type)
        {
            return type.IsByRef == false && type.IsAbstract == false && type.IsInterface == false && type.IsCOMObject == false && type.IsSpecialName == false && type.IsSubclassOf(typeof(Delegate)) == false;
        }

        private bool _IsArray(Type type)
        {
            return type.IsArray;
        }

        private readonly Type[] _NumberTypes = new[]
        {
            typeof (short),
            typeof (ushort),
            typeof (int),
            typeof (uint),
            typeof (char),
            typeof (bool),
            typeof (long),
            typeof (ulong),
        };


        private readonly Type[] _BittableTypes = new[]
        {
            typeof (float),
            typeof (decimal),
            typeof (double),
            typeof (Guid),
            typeof (char),
            typeof (byte),
        };



        private bool _IsNumber(Type type)
        {
            return _NumberTypes.Any(t => t == type);
        }
    }
}