using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
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
            if(memberInfo != null)
            {
                var attribute = memberInfo.GetCustomAttributes(typeof(Regulus.Utility.EnumDescriptionAttribute), false).FirstOrDefault() as Regulus.Utility.EnumDescriptionAttribute;
                if (attribute != null)
                    return attribute.Message;
                return "";
            }
            return null;
        }

        
    }

    public static class EnumHelper
    {
        public static IEnumerable<Enum> GetFlags(this Enum enum_instance) 
        {
            var ienum = Convert.ToUInt64(enum_instance);
            var availableSuits = Enum.GetValues(enum_instance.GetType()).Cast<Enum>();
            foreach (var suit in availableSuits)
            {
                var iflag = Convert.ToUInt64(suit);
                if ((ienum & iflag) > 0)
                    yield return suit;
            }
            yield break;
        }
    }
}
