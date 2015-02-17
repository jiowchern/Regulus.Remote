using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{
    public class EnumDescriptionAttribute : System.Attribute
    {
        public string Message { get; private set; }
        public EnumDescriptionAttribute(string message)
        {
            Message = message;
        }

            
    }
    
}


namespace Regulus.Extension   
{
    public static class EnumDescriptionExtension
    {
        public static string GetEnumDescription<T>(this T enum_instance)
        {
            var memberInfo = typeof(T).GetMember(enum_instance.ToString())
                                              .FirstOrDefault();

            var attribute = memberInfo.GetCustomAttributes(typeof(Regulus.Utility.EnumDescriptionAttribute), false).FirstOrDefault() as Regulus.Utility.EnumDescriptionAttribute;
            return attribute.Message;
        }
    }
}
