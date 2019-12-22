using System;

using System.Reflection;

namespace Regulus.Remote.Unity
{
}

namespace Regulus.Remote.Unity
{


    public class AdsorberAttribute : System.Attribute
    {
        public AdsorberAttribute(Type adsorber, string target)
        {
            
            Adsorber = adsorber;
            
            Target = target;
        }

        

        public readonly Type Adsorber;        
        public readonly string Target;



        public enum MATCH
        {
            OK,
            LOST_METHOD,
            DIFFERENT_METHOD_PARAMS_LENGTH,
            DIFFERENT_METHOD_PARAMS_TYPE
        }
        public MATCH Match(MethodInfo method)
        {
            var field = Adsorber.GetField(Target);
            if (field == null)
            {
                return MATCH.LOST_METHOD;
            }            

            var adsbuterParams = field.FieldType.GetMethod("Invoke").GetParameters();
            var scriptParams = method.GetParameters();

            if (adsbuterParams.Length != scriptParams.Length)
            {
                return MATCH.DIFFERENT_METHOD_PARAMS_LENGTH;
            }

            for (int i = 0; i < adsbuterParams.Length; i++)
            {
                if(adsbuterParams[i].ParameterType != scriptParams[i].ParameterType)
                    return MATCH.DIFFERENT_METHOD_PARAMS_TYPE;
            }
            
            return MATCH.OK;
        }
    }
}
