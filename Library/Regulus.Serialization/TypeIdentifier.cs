using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;

namespace Regulus.Serialization
{
    public class TypeIdentifier
    {
        public readonly ITypeDescriber Describer;
        public TypeIdentifier(Type type , int id)
        {
            
            if (_IsEnum(type))
            {
                Describer = new EnumDescriber(id , type);
            }
            else if (_IsNumber(type))
            {
                Describer = new NumberDescriber(id , type);
            }
            else if (_IsByteArray(type))
            {
                Describer = new ByteArrayDescriber(id);
            }
            else if (_IsBuffer(type))
            {
                Describer = new BufferDescriber(id, type);
            }
            else if (_IsBittable(type))
            {
                Describer = new BlittableDescriber(id, type);
            }
            else if (_IsString(type))
            {
                Describer = new StringDescriber(id);
            }
            else if (_IsArray(type))
            {
                Describer = new ArrayDescriber(id , type);
            }            
            else if (_IsClass(type))
            {
                Describer = new ClassDescriber(id , type);
            }
            else 
                throw new Exception("Unrecognized type " + type.FullName );

        }

        

        private static bool _IsByteArray(Type type)
        {
            return type == typeof (byte[]);
        }

        private static bool _IsBuffer(Type type)
        {
            return _BufferTypes.Any(t => t == type);
        }


        private static bool _IsBittable(Type type)
        {

            return _BittableTypes.Any(t => t == type);

            
        }

        private static bool _IsString(Type type)
        {
            return type == typeof(string);
        }

        private static bool _IsEnum(Type type)
        {
            return type.IsEnum;
        }

        public static bool IsClass(Type type)
        {
            return _IsClass(type);
        }

        private static bool _IsClass(Type type)
        {
            return type.IsByRef == false && type.IsAbstract == false && type.IsInterface == false && type.IsCOMObject == false && type.IsSpecialName == false && type.IsSubclassOf(typeof(Delegate)) == false;
        }

        private bool _IsArray(Type type)
        {
            return type.IsArray;
        }

        private static readonly Type[] _NumberTypes = new[]
        {
            typeof (short),
            typeof (ushort),
            typeof (int),
            typeof (uint),            
            typeof (bool),
            typeof (long),
            typeof (ulong),
        };


        private static readonly Type[] _BufferTypes = new[]
        {            
            typeof (char[]),
        };

        private static readonly Type[] _BittableTypes = new[]
        {            
            typeof (float),
            typeof (decimal),
            typeof (double),
            typeof (Guid),
            typeof (char),
            typeof (byte),            
        };



        private static bool _IsNumber(Type type)
        {
            return _NumberTypes.Any(t => t == type);
        }


        public static bool IsAtom(Type type)
        {
            return _IsNumber(type) || _IsBittable(type) || _IsBuffer(type) || _IsByteArray(type) || _IsEnum(type);
        }

        public static bool IsString(Type type)
        {
            return _IsString(type);
        }
    }
}