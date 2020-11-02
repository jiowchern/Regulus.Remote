using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Regulus.Remote.Protocol
{
    [Serializable]
    internal class TypePropertyNotifierException : Exception
    {
        

        public TypePropertyNotifierException(PropertyInfo propertyInfo, Exception ex) : base($"Wrong Notifier Property. {propertyInfo.DeclaringType}.{propertyInfo.Name}",ex)
        {
        
        }

        
    }
}