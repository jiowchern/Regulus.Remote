using System;
using System.Linq;

namespace Regulus.Serialization
{
    public class TypeIdentifier
    {

        public readonly System.Collections.Generic.IEnumerable<ITypeDescriber> Describers;
        public TypeIdentifier(Type type, IDescribersFinder finder)
        {

            if (_IsPoint(type))
            {
                Describers = new ITypeDescriber[0];
            }
            else if (_IsDelegate(type))
            {
                Describers = new ITypeDescriber[0];
            }
            else if (_IsEnum(type))
            {
                Describers = new[] { new EnumDescriber(type) };
            }
            else if (_IsNumber(type))
            {
                Describers = new[] { new NumberDescriber(type) };
            }
            else if (_IsByteArray(type))
            {
                NumberDescriber byteDescriber = new NumberDescriber(typeof(int));
                Describers = new ITypeDescriber[] { new ByteArrayDescriber(byteDescriber), byteDescriber };
            }
            else if (_IsBuffer(type))
            {
                Describers = new ITypeDescriber[] { new BufferDescriber(type) };
            }
            else if (_IsBittable(type))
            {
                Describers = new ITypeDescriber[] { new BlittableDescriber(type) };
            }
            else if (_IsString(type))
            {
                BufferDescriber chars = new BufferDescriber(typeof(char[]));
                Describers = new ITypeDescriber[] { new StringDescriber(chars), chars };
            }
            else if (_IsArray(type))
            {
                Describers = new ITypeDescriber[] { new ArrayDescriber(type, finder) };
            }
            else if (_IsClass(type))
            {
                Describers = new ITypeDescriber[] { new ClassDescriber(type, finder) };
            }
            else
                throw new Exception("not supported type " + type);
        }

        private bool _IsPoint(Type type)
        {
            return type.IsPointer;
        }

        private bool _IsDelegate(Type type)
        {
            return type.IsSubclassOf(typeof(Delegate));
        }

        private static bool _IsByteArray(Type type)
        {
            return type == typeof(byte[]);
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

            return type.IsByRef == false && type.IsAbstract == false && type.IsInterface == false && type.IsCOMObject == false && type.IsSpecialName == false && type.IsSubclassOf(typeof(Delegate)) == false && type.IsPointer == false;
        }

        private bool _IsArray(Type type)
        {
            return type.GetInterfaces().Any(i => i == typeof(System.Collections.IList));
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