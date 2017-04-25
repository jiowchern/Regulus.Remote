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
                Type = TYPE.NUMBER;
            }
            else if (_IsClass(type))
            {
                Type = TYPE.CLASS;
            }
            else if (_IsEnum(type))
            {
                Type = TYPE.ENUM;
            }
            else if (_IsBittable(type))
            {
                Type = TYPE.BITTABLE;
            }
        }



        private bool _IsBittable(Type type)
        {

            try
            {
                var val = Activator.CreateInstance(type);
                GCHandle.Alloc(val, GCHandleType.Pinned).Free();
                return true;
            }
            catch (Exception)
            {


            }

            return false;
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
            return type.IsClass;
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



        private bool _IsNumber(Type type)
        {
            return _NumberTypes.Any(t => t == type);
        }
    }
}