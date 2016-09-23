using System;
using System.Linq;
using System.Reflection;


using Regulus.Utility;

namespace Regulus.Remoting
{
	public class CommandParam
	{
		public Type[] Types { get; set; }

		public object Callback { get; set; }

		public object Return { get; set; }

		public Type ReturnType { get; set; }

		public CommandParam()
		{
			Return = new Action(_Empty);
		}

		private void _Empty()
		{
		}
	}

	namespace Extension
	{
		public static class CommandExtension
		{
			public static void Register(this Command command, string name, CommandParam param)
			{
				new CommandAdapter(command).Register(name, param);
			}
		}

		internal class CommandAdapter
		{
			private readonly Command _Command;

			public CommandAdapter(Command command)
			{
				_Command = command;
			}

			public void Register(string name, CommandParam param)
			{
				_InvokeRegister(name, param);
			}

			private void _InvokeRegister(string name, CommandParam param)
			{
				var registerMethod = GetRegister(param.Types, param.ReturnType);

				if(param.ReturnType != null)
				{
					var returnValue = registerMethod.Invoke(
						_Command, 
						new[]
						{
							name, 
							param.Callback, 
							param.Return
						});
				}
				else
				{
					var returnValue = registerMethod.Invoke(
						_Command, 
						new[]
						{
							name, 
							param.Callback
						});
				}
			}

			private MethodInfo _GetReturn(CommandParam param)
			{
				var methods = typeof(CommandParam).GetMethods();
				var baseMethod = (from m in methods
				                  let parameters = m.GetParameters()
				                  where m.Name == "Return"
				                  select m).SingleOrDefault();

				return baseMethod.MakeGenericMethod(param.ReturnType);
			}

			private MethodInfo GetRegister(Type[] arg_types, Type return_type)
			{
				var genericTypes = return_type != null
					                   ? arg_types.Concat(
						                   new[]
						                   {
							                   return_type
						                   }).ToArray()
					                   : arg_types;
				var paramCount = return_type != null
					                 ? 3
					                 : 2;

				var methods = typeof(Command).GetMethods();
				var baseMethod = (from m in methods
				                  let genericParameters = m.GetGenericArguments()
				                  let parameters = m.GetParameters()
				                  where m.Name == "Register"
				                        && genericParameters.Length == genericTypes.Length
                                        
				                        && parameters.Length == paramCount
				                  select m).SingleOrDefault();

				if(genericTypes.Length > 0)
				{
					return baseMethod.MakeGenericMethod(genericTypes);
				}

				return baseMethod;
			}

		   
		}
	}
}
