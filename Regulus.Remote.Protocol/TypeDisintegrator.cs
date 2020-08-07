using Regulus.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Regulus.Remote.Protocol
{
    public class TypeDisintegrator
    {
        class Trigger
        {
            private bool _Enable;

            public bool Set
            {
                set
                {
                    if (_Enable == false && value == true)
                        _Enable = true;
                }
            }

            public bool Enable { get { return _Enable; } }
        }
        public readonly Type[] Types;


        public TypeDisintegrator(Type type)
        {
            HashSet<Type> types = new HashSet<Type>();
            _Analyze(type, types);

            Types = types.ToArray();
        }

        private void _Analyze(Type type, HashSet<Type> types)
        {

            Trigger trigger = new Trigger();
            if (_IsAtom(type))
            {
                trigger.Set = _Add(new[] { type }, types);
            }
            else if (_IsString(type))
            {
                trigger.Set = _Add(new[] { typeof(char), typeof(char[]), typeof(string) }, types);
            }
            else if (_IsArray(type))
            {
                trigger.Set = _Add(new[] { type, type.GetElementType() }, types);
            }
            else if (_IsType(type))
            {
                if (TypeIdentifier.IsClass(type))
                    trigger.Set = types.Add(type);

                trigger.Set = _Add(_GetEvents(type), types);
                trigger.Set = _Add(_GetPropertys(type), types);
                trigger.Set = _Add(_GetMethods(type), types);
                trigger.Set = _Add(_GetFields(type), types);
            }

            if (trigger.Enable)
            {
                foreach (Type type1 in types.ToArray())
                {
                    _Analyze(type1, types);
                }
            }
        }

        private bool _IsString(Type type)
        {
            return TypeIdentifier.IsString(type);
        }

        private bool _IsArray(Type type)
        {
            return type.IsArray && type != typeof(object[]);
        }

        private static bool _Add(IEnumerable<Type> types1, HashSet<Type> types)
        {
            Trigger trigger = new Trigger();
            foreach (Type eType in types1)
            {
                if (_Valid(eType))
                    trigger.Set = types.Add(eType);
            }

            return trigger.Enable;
        }

        private bool _IsType(Type type)
        {
            return (type.IsInterface || TypeIdentifier.IsClass(type));
        }

        private bool _IsAtom(Type type)
        {
            return TypeIdentifier.IsAtom(type);
        }




        private static bool _Valid(Type type)
        {
            return type.IsVisible == true && type.IsPointer == false && type.IsByRef == false && type.IsInterface == false && type.IsGenericType == false && type.IsAbstract == false && type != typeof(object);
        }

        private IEnumerable<Type> _GetPropertys(Type type)
        {
            foreach (System.Reflection.PropertyInfo propertyInfo in type.GetProperties())
            {
                yield return propertyInfo.PropertyType;
            }

        }

        private IEnumerable<Type> _GetEvents(Type type)
        {
            foreach (System.Reflection.EventInfo eventInfo in type.GetEvents())
            {
                Type handleType = eventInfo.EventHandlerType;
                if (handleType.IsGenericType)
                {
                    Type[] args = handleType.GetGenericArguments();
                    foreach (Type type1 in args)
                    {
                        yield return type1;
                    }
                }
                else
                {
                    foreach (System.Reflection.ParameterInfo parameterInfo in handleType.GetMethod("Invoke").GetParameters())
                    {
                        yield return parameterInfo.ParameterType;
                    }
                }
            }
        }

        private IEnumerable<Type> _GetMethods(Type type)
        {
            List<Type> types = new List<Type>();
            foreach (System.Reflection.MethodInfo methodInfo in type.GetMethods())
            {
                if (methodInfo.IsGenericMethod)
                {
                    continue;
                }

                types.AddRange(methodInfo.GetParameters().Select(m => m.ParameterType));

                Type retType = methodInfo.ReturnType;


                if (retType.IsGenericType)
                {
                    types.AddRange(retType.GetGenericArguments());
                }
            }

            return types;
        }

        private IEnumerable<Type> _GetFields(Type type)
        {
            foreach (System.Reflection.FieldInfo fieldInfo in type.GetFields())
            {
                if (fieldInfo.IsPublic && fieldInfo.IsSpecialName == false)
                {
                    yield return fieldInfo.FieldType;
                }
            }

        }
    }
}
