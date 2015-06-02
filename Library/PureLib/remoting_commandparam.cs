using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting
{
    public class CommandParam
    {                
        public CommandParam()
        {
            Return = new Action(_Empty);
        }

        private void _Empty()
        {            
        }
        public Type[] Types { get; set; }
        public object Callback { get; set; }

        public object Return { get; set; }

        public Type ReturnType { get; set; }
    }

    namespace Extension
    {
        public static class CommandExtension
        {
            public static void Register(this Regulus.Utility.Command command ,  string name , CommandParam param)
            {
                new CommandAdapter(command).Register(name, param);
            }
        }

        class CommandAdapter
        {
            Regulus.Utility.Command _Command;
            public CommandAdapter(Regulus.Utility.Command command)
            {
                _Command = command;
            }
            public void Register(string name,CommandParam param)
            {

                _InvokeRegister(name, param);

                
            }

           

            private void _InvokeRegister(string name, CommandParam param)
            {
                var registerMethod = GetRegister(param.Types , param.ReturnType  );  
              
                if(param.ReturnType != null)
                {

                    var returnValue = registerMethod.Invoke(_Command, new object[] { name, param.Callback , param.Return });                
                }
                else
                {
                    var returnValue = registerMethod.Invoke(_Command, new object[] { name, param.Callback });                
                }
                
            }

            private System.Reflection.MethodInfo _GetReturn(CommandParam param)
            {
                System.Reflection.MethodInfo[] methods = typeof(CommandParam).GetMethods();
                var baseMethod = (from m in methods
                        let parameters = m.GetParameters()
                        where m.Name == "Return"                         
                        select m).SingleOrDefault();

                return baseMethod.MakeGenericMethod(param.ReturnType);
            }

            

            private System.Reflection.MethodInfo GetRegister(Type[] arg_types , Type return_type)
            {

                Type[] genericTypes = return_type != null ? arg_types.Concat(new Type[] { return_type }).ToArray() : arg_types ;
                int paramCount = return_type != null ? 3 : 2;

                System.Reflection.MethodInfo[] methods = typeof(Regulus.Utility.Command).GetMethods();
                var baseMethod = (from m in methods
                                  let genericParameters = m.GetGenericArguments()
                                  let parameters = m.GetParameters()
                                  where m.Name == "Register"
                                  && genericParameters.Length == genericTypes.Length
                                  && parameters.Length == paramCount
                                  select m).SingleOrDefault();

                if(genericTypes .Length > 0)
                {
                    return baseMethod.MakeGenericMethod(genericTypes);
                }
                return baseMethod;
            }
        }


    }
}
