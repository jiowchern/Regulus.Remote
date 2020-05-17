using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text.RegularExpressions;

namespace Regulus.Utility
{
	public delegate void Action<in T1, in T2, in T3, in T4, in T5>(T1 t1, T2 t2, T3 t3, T4 t4 , T5 t5);

	public delegate void Action<in T1, in T2, in T3, in T4, in T5 ,  in T6>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);

	//public delegate void Func<in T1, in T2, in T3, in T4, in T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);

	public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, out TResult>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);

	public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, in T6, out TResult>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);
	public partial class Command : ICommand
	{
		public delegate void OnRegister(string command, CommandParameter ret, CommandParameter[] args);

		public delegate void OnUnregister(string command);

		public event OnRegister RegisterEvent;

		public event OnUnregister UnregisterEvent;

		private readonly List<Infomation> _Commands;

		public Command()
		{
			_Commands = new List<Infomation>();
			RegisterEvent += _EmptyRegisterEvent;
			UnregisterEvent += s => { };
		}

		
		public System.Guid Register(string command, Action<string[]> func, Type return_type, Type[] param_types)
		{
			return _Register(command, func, return_type , param_types);
		}
		private System.Guid _Register(string command, Action<string[]> func, Type return_type, Type[] param_types)
		{
			var analysis = new Analysis(command);
			var id = _AddCommand(analysis.Command, func);
			_SendRegister(analysis, return_type, param_types);
			return id;
		}
		public void Register(string command, Action executer)
		{
			Action<string[]> func = args =>
			{
				if(args.Length != 0)
				{
					throw new ArgumentException("命令參數數量為0");
				}

				executer.Method.Invoke(executer.Target, new object[0]);
			};

			_Register(command, func , typeof(void) , new Type[0]);
		}

		

		public void Register<T1>(string command, Action<T1> executer)
		{
			Action<string[]> func = args =>
			{
				if(args.Length != 1)
				{
					throw new ArgumentException("命令參數數量為1");
				}

				object arg0;
				Command.Conversion(args[0], out arg0, typeof(T1));
				T1 val = (T1) arg0;


			    
				executer.Method.Invoke(executer.Target, new [] { arg0 } );
			};


			_Register(command, func, typeof(void), new[]
				{
					typeof(T1)
				});
			
		}

		public void Register<T1, T2>(string command, Action<T1, T2> executer)
		{
			Action<string[]> func = args =>
			{
				if(args.Length != 2)
				{
					throw new ArgumentException("命令參數數量為2");
				}

				object arg0;
				Command.Conversion(args[0], out arg0, typeof(T1));
				object arg1;
				Command.Conversion(args[1], out arg1, typeof(T2));


				executer.Method.Invoke(executer.Target, new[] { arg0 , arg1});
				
			};

			_Register(command, func, typeof(void), new[]
				{
					typeof(T1),
					typeof(T2)
				});            
		}

		public void Register<T1, T2, T3>(string command, Action<T1, T2, T3> executer)
		{
			Action<string[]> func = args =>
			{
				if(args.Length != 3)
				{
					throw new ArgumentException("命令參數數量為3");
				}

				object arg0;
				Command.Conversion(args[0], out arg0, typeof(T1));
				object arg1;
				Command.Conversion(args[1], out arg1, typeof(T2));
				object arg2;
				Command.Conversion(args[2], out arg2, typeof(T3));
				executer.Method.Invoke(executer.Target, new[] { arg0, arg1 ,arg2});
			};


			_Register(command, func, typeof(void), new[]
				{
					typeof(T1),
					typeof(T2),
					typeof(T3)
				});            
		}

		public void Register<T1, T2, T3, T4>(string command, Action<T1, T2, T3, T4> executer)
		{
			Action<string[]> func = args =>
			{
				if(args.Length != 4)
				{
					throw new ArgumentException("命令參數數量為4");
				}

				object arg0;
				Command.Conversion(args[0], out arg0, typeof(T1));
				object arg1;
				Command.Conversion(args[1], out arg1, typeof(T2));
				object arg2;
				Command.Conversion(args[2], out arg2, typeof(T3));
				object arg3;
				Command.Conversion(args[3], out arg3, typeof(T4));
				executer.Method.Invoke(executer.Target, new[] { arg0, arg1, arg2 ,arg3});
			};

			_Register(command, func, typeof(void), new[]
				{
					typeof(T1),
					typeof(T2),
					typeof(T3),
					typeof(T4)
				});            
		}

		public void Register<TR>(string command, Func<TR> executer, Action<TR> value)
		{
			Action<string[]> func = args =>
			{
				if(args.Length != 0)
				{
					throw new ArgumentException("命令參數數量為0");
				}

				var ret = executer.Method.Invoke(executer.Target , new object[0]);
				value.Method.Invoke(value.Target, new object[] {ret});
			};

			_Register(command, func, typeof(TR), new Type[0]);

		}

		public void Register<T1, TR>(string command, Func<T1, TR> executer, Action<TR> value)
		{
			Action<string[]> func = args =>
			{
				if(args.Length != 1)
				{
					throw new ArgumentException("命令參數數量為1");
				}

				object arg0;
				Command.Conversion(args[0], out arg0, typeof(T1));
				var ret = executer.Method.Invoke(executer.Target, new object[] { arg0 });
				value.Method.Invoke(value.Target, new object[] { ret });


			};

			_Register(command, func, typeof(TR), new[]
				{
					typeof(T1)
				});

			
		}

		public void Register<T1, T2, TR>(string command, Func<T1, T2, TR> executer, Action<TR> value)
		{
			Action<string[]> func = args =>
			{
				if(args.Length != 2)
				{
					throw new ArgumentException("命令參數數量為2");
				}

				object arg0;
				Command.Conversion(args[0], out arg0, typeof(T1));
				object arg1;
				Command.Conversion(args[1], out arg1, typeof(T2));

				var ret = executer.Method.Invoke(executer.Target, new object[] { arg0,arg1 });
				value.Method.Invoke(value.Target, new object[] { ret });
			};

			_Register(command, func, typeof(TR), new[]
				{
					typeof(T1),
					typeof(T2)
				});

			
		}

		public void Register<T1, T2, T3, TR>(string command, Func<T1, T2, T3, TR> executer, Action<TR> value)
		{
			Action<string[]> func = args =>
			{
				if(args.Length != 3)
				{
					throw new ArgumentException("命令參數數量為3");
				}

				object arg0;
				Command.Conversion(args[0], out arg0, typeof(T1));
				object arg1;
				Command.Conversion(args[1], out arg1, typeof(T2));
				object arg2;
				Command.Conversion(args[2], out arg2, typeof(T3));

				var ret = executer.Method.Invoke(executer.Target, new[] { arg0, arg1,arg2 });
				value.Method.Invoke(value.Target, new[] { ret });
			};

			_Register(command, func, typeof(TR), new[]
				{
					typeof(T1),
					typeof(T2),
					typeof(T3)
				});
			
		}

		public void Register<T1, T2, T3, T4, TR>(string command, Func<T1, T2, T3, T4, TR> executer, Action<TR> value)
		{
			Action<string[]> func = args =>
			{
				if(args.Length != 4)
				{
					throw new ArgumentException("命令參數數量為4");
				}

				object arg0;
				Command.Conversion(args[0], out arg0, typeof(T1));
				object arg1;
				Command.Conversion(args[1], out arg1, typeof(T2));
				object arg2;
				Command.Conversion(args[2], out arg2, typeof(T3));
				object arg3;
				Command.Conversion(args[3], out arg3, typeof(T4));

				var ret = executer.Method.Invoke(executer.Target, new[] { arg0, arg1, arg2 , arg3 });
				value.Method.Invoke(value.Target, new[] { ret });
			};

			_Register(command, func, typeof(TR), new[]
				{
					typeof(T1),
					typeof(T2),
					typeof(T3),
					typeof(T4)
				});
		}

		public void Unregister(string command)
		{
			var analysis = new Analysis(command);
			if (_Commands.RemoveAll(cmd => cmd.Name == analysis.Command) > 0)
			{
				UnregisterEvent(analysis.Command);
			}
		}

		public void Unregister(System.Guid id)
		{
			var cmd = _Commands.FirstOrDefault( info => info.Id == id);
			if (cmd != null && _Commands.RemoveAll(info  => info.Id == id) > 0)
			{
				UnregisterEvent(cmd.Name);
			}
		}

		public static bool Conversion(string in_string, out object out_value, Type source)
		{
			
			out_value =null;
			if (source == typeof(int))
			{
				var reault = int.MinValue;
				if(int.TryParse(in_string, out reault))
				{
				}

				out_value = reault;

				return true;
			}
			else if(source == typeof(float))
			{
				var reault = float.MinValue;
				if(float.TryParse(in_string, out reault))
				{
				}

				out_value = reault;
				return true;
			}
			else if(source == typeof(byte))
			{
				var reault = byte.MinValue;
				if(byte.TryParse(in_string, out reault))
				{
				}

				out_value = reault;
				return true;
			}
			else if(source == typeof(short))
			{
				var reault = short.MinValue;
				if(short.TryParse(in_string, out reault))
				{
				}

				out_value = reault;
				return true;
			}
			else if(source == typeof(long))
			{
				var reault = long.MinValue;
				if(long.TryParse(in_string, out reault))
				{
				}

				out_value = reault;
				return true;
			}
			else if (source == typeof(bool))
			{
				var reault = false;
				if (bool.TryParse(in_string, out reault))
				{
				}

				out_value = reault;
				return true;
			}
			else if (source == typeof(IPEndPoint))
			{
				try
				{
					var m = Regex.Match(in_string, @"(\d+\.\d+\.\d+\.\d+):(\d+)");
					if (m.Success)
					{
						var address = m.Groups[1].Value;
						var port = m.Groups[2].Value;
						out_value = new IPEndPoint(IPAddress.Parse(address), int.Parse(port));
					}
					else
					{
						out_value = new IPEndPoint(0, 0);
					}
					
				}
				catch(SystemException se)
				{
					out_value = new IPEndPoint(0, 0);
				}
				
				
			}
			else if (source == typeof(string))
			{                
				out_value = in_string;
				return true;
			}
			else if (source.IsEnum)
			{
				
				try
				{
					out_value = Enum.Parse(source, in_string);
					return true;
				}
				catch (Exception)
				{
					
				}             
				
			}
			else
			{
				out_value = Activator.CreateInstance(source);
				return true;
			}

			return false;
		}

		private void _EmptyRegisterEvent(string command, CommandParameter ret, CommandParameter[] args)
		{
			// throw new NotImplementedException();
		}

		private System.Guid _AddCommand(string command, Action<string[]> func)
		{
			var info = new Infomation(command, func);
			_Commands.Add(info);

			return info.Id;
		}

		public int Run(string command, string[] args)
		{
			var commandInfomations = from ci in _Commands where ci.Name.ToLower() == command.ToLower() select ci;
			var infos = new List<Infomation>();

			foreach(var commandInfomation in commandInfomations)
			{
				infos.Add(commandInfomation);
			}

			foreach(var info in infos)
			{
				info.Handler(args);
			}

			return infos.Count();
		}

		private void _SendRegister(Analysis analysis, Type ret, Type[] args)
		{
			var parameterTypes = args.ToArray();
			var parameterDescs = analysis.Parameters.ToArray();
			RegisterEvent(
				analysis.Command,
				new CommandParameter(ret,analysis.Return),
				_BuildCommandParameters(parameterTypes, parameterDescs));
		}

		private CommandParameter[] _BuildCommandParameters(Type[] parameterTypes, string[] parameterDescs)
		{
			var count = parameterTypes.Length;
			var cps = new CommandParameter[count];
			for(var i = 0; i < count; ++i)
			{
				cps[i] = new CommandParameter(
					parameterTypes[i], 
					(i < parameterDescs.Length)
						? parameterDescs[i]
						: string.Empty);
			}

			return cps;
		}

		public void Register(LambdaExpression exp, Action<string[]> func)
		{
			_Register(exp,func);
		}
		private void _Register(LambdaExpression exp , Action<string[]> func)
		{            

			if (exp.Body.NodeType != ExpressionType.Call)
			{
				throw new ArgumentException();
			}

			var methodCall = exp.Body as MethodCallExpression;
			var method = methodCall.Method;
			
			var argNames = (from par in exp.Parameters.Skip(1) select par.Name).ToArray();
			var argTypes = (from par in exp.Parameters.Skip(1) select par.Type).ToArray();

			string commandString = string.Format("{0} [{1}  ] [{2} ]", method.Name, string.Join(",", argNames.ToArray()), "return_" + method.ReturnType.Name);
			_Register(commandString, func, method.ReturnType, argTypes.ToArray());
		}
	}
}
