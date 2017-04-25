using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Regulus.Serialization;

namespace Regulus.Protocol
{
    public class TypeDisintegrator
    {
        
        public readonly Type[] Types;
        

        public TypeDisintegrator(Type type)
        {
            var types = new HashSet<Type>();

            var id = new TypeIdentifier(type);

            if ( _IsBase(id.Type) )
            {
                types.Add(type);
                Types = types.ToArray();
                return;
            }


            if (_IsArray(type))
            {
                types.Add(type);
                types.Add(type.GetElementType());
            }
            else if (_IsType(id , type))
            {
                if(id.Type == TypeIdentifier.TYPE.CLASS)
                    types.Add(type);

                _Add(_GetEvents(type), types);
                _Add(_GetPropertys(type), types);
                _Add(_GetMethods(type), types);
                _Add(_GetFields(type), types);
            }

            

            Types = types.ToArray();
        }

        private bool _IsArray(Type type)
        {
            return type.IsArray && type != typeof (object[]);
        }

        private static void _Add(IEnumerable<Type> types1, HashSet<Type> types)
        {
            foreach (var eType in types1.SelectMany(t => new TypeDisintegrator(types, t).Types))
            {
                types.Add(eType);
            }
        }

        private bool _IsType(TypeIdentifier id, Type type)
        {
            return type.IsInterface || id.Type == TypeIdentifier.TYPE.CLASS;
        }

        private bool _IsBase(TypeIdentifier.TYPE type)
        {
            return type == TypeIdentifier.TYPE.STRING || type == TypeIdentifier.TYPE.ENUM ||
                   type == TypeIdentifier.TYPE.BITTABLE || type == TypeIdentifier.TYPE.NUMBER;
        }


        protected TypeDisintegrator(HashSet<Type> types,Type type)
        {
            Types = new Type[0];
            if (types.Contains(type) == false && _Valid(type))
            {
                Types = new TypeDisintegrator(type).Types;
            }            
        }

        private static bool _Valid(Type type)
        {
            return type.IsByRef == false && type.IsInterface == false && type.IsGenericType == false && type.IsAbstract == false && type != typeof(object);
        }

        private IEnumerable<Type> _GetPropertys(Type type)
        {
            foreach (var propertyInfo in type.GetProperties())
            {
                yield return propertyInfo.PropertyType;
            }

        }

        private IEnumerable<Type> _GetEvents(Type type)
        {
            foreach (var eventInfo in type.GetEvents())
            {
                var method = eventInfo.GetRaiseMethod();
                var handleType = eventInfo.EventHandlerType;
                if (handleType.IsGenericType)
                {
                    var args = handleType.GetGenericArguments();
                    foreach (var type1 in args)
                    {
                        yield return type1;
                    }
                }
                else
                {
                    foreach (var parameterInfo in handleType.GetMethod("Invoke").GetParameters())
                    {
                        yield return parameterInfo.ParameterType;
                    }
                }
            }
        }

        private IEnumerable<Type> _GetMethods(Type type)
        {
            List<Type> types = new List<Type>();
            foreach (var methodInfo in type.GetMethods())
            {
                if (methodInfo.IsGenericMethod )
                {
                    continue;
                }
                
                types.AddRange(methodInfo.GetParameters().Select(m => m.ParameterType));

                var retType = methodInfo.ReturnType;

                
                if (retType.IsGenericType  && retType.GetGenericTypeDefinition() == typeof (Regulus.Remoting.Value<>))
                {
                    types.AddRange(retType.GetGenericArguments());
                }
            }

            return types;
        }

        private IEnumerable<Type> _GetFields(Type type)
        {
            foreach (var fieldInfo in type.GetFields())
            {
                if (fieldInfo.IsPublic && fieldInfo.IsSpecialName == false)
                {
                    yield return fieldInfo.FieldType;
                }
            }
            
        }
    }
}
