using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Regulus.Utility
{
	public delegate void Action<in T1, in T2, in T3, in T4, in T5>(T1 t1, T2 t2, T3 t3, T4 t4 , T5 t5);

	public delegate void Action<in T1, in T2, in T3, in T4, in T5 ,  in T6>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);

	//public delegate void Func<in T1, in T2, in T3, in T4, in T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);

	public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, out TResult>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);

	public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, in T6, out TResult>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);




	public class Command : ICommand
	{
		public delegate void OnRegister(string command, CommandParameter ret, CommandParameter[] args);

		public delegate void OnUnregister(string command);

		public event OnRegister RegisterEvent;

		public event OnUnregister UnregisterEvent;

		private class Infomation
		{
			public Action<string[]> Handler;

			public string Name;
		}

		public class CommandParameter
		{
			public Type Param { get; private set; }

			public string Description { get; private set; }

			public CommandParameter(Type p, string description)
			{
				Param = p;
				Description = description;
			}

			public static implicit operator CommandParameter(Type type)
			{
				return new CommandParameter(type, string.Empty);
			}
		}

		public class Analysis
		{
			public string Command { get; private set; }

			public string[] Parameters { get; private set; }

			public Analysis(string message)
			{
				_Analyze(message);
			}

			private void _Analyze(string message)
			{
				var expansion = @"^\s*(?<command>\w+)\s*\[\s*(?<args>.+?)\]\s*\[\s*(?<ret>.+?)\s*\]|^\s*(?<command>\w+)\s*\[\s*(?<args>.+?)\]|^\s*(?<command>\w+)\s*";
				var regex = new Regex(expansion);
				var match = regex.Match(message);
				if(match.Success)
				{
					var command = match.Groups["command"];
					Command = command.Value;
					var args = match.Groups["args"];
					var ret = match.Groups["ret"];
					_SetParameters(_AnalyzeArgs(args.Value) , _AnalyzeReturn(ret.Value) );
				}
			}

			


			private void _SetParameters(string[] parameters , string return_parameter)
			{
				Parameters = parameters;
				Return = return_parameter;

			}

			public string Return { get; private set; }

			private string _AnalyzeReturn(string value)
			{
				return value;
			}

			private string[] _AnalyzeArgs(string message)
			{
				var args = new List<string>();

				// \s*(\w+)\s*,?
				// ^\s*(?<command>\w+)\s*\[\s*(?<args>.+)\]|^\s*(?<command>\w+)\s*
				const string expansion = @"\s*(?<Arg>\w+)\s*,?";
				var regex = new Regex(expansion);
				var matchs = regex.Matches(message);
				foreach(Match match in  matchs)
				{
					args.Add(match.Groups["Arg"].Value);
				}

				return args.ToArray();
			}
		}

		private readonly List<Infomation> _Commands;

		public Command()
		{
			_Commands = new List<Infomation>();
			RegisterEvent += _EmptyRegisterEvent;
			UnregisterEvent += s => { };
		}

		public void RegisterLambda<TThis,TR>(TThis instance, Expression<Func<TThis,TR>> exp , Action<TR> return_value)
		{
			Action<string[]> func = (args) =>
			{
				if (args.Length != 0)
				{
					throw new ArgumentException("命令參數數量為0");
				}

				return_value(exp.Compile().Invoke(instance));
			};
			_Register(exp, func);
		}


		public void RegisterLambda<TThis, T0 , TR>(TThis instance, Expression<Func<TThis, T0,TR>> exp, Action<TR> return_value)
		{
			Action<string[]> func = (args) =>
			{
				if (args.Length != 1)
				{
					throw new ArgumentException("命令參數數量為1");
				}

				object arg0;
				Command.Conversion(args[0], out arg0, typeof(T0));

				return_value(exp.Compile().Invoke(instance ,(T0)arg0 ));
			};
			_Register(exp, func);
		}

		public void RegisterLambda<TThis, T0 , T1, TR>(TThis instance, Expression<Func<TThis, T0 , T1, TR>> exp, Action<TR> return_value)
		{
			Action<string[]> func = (args) =>
			{
				if (args.Length != 2)
				{
					throw new ArgumentException("命令參數數量為2");
				}

				object arg0;
				Command.Conversion(args[0], out arg0, typeof(T0));

				object arg1;
				Command.Conversion(args[1], out arg1, typeof(T1));



				return_value(exp.Compile().Invoke(instance, (T0)arg0, (T1)arg1));
			};
			_Register(exp, func);
		}

		public void RegisterLambda<TThis, T0, T1 , T2, TR>(TThis instance, Expression<Func<TThis, T0, T1 , T2, TR>> exp, Action<TR> return_value)
		{
			Action<string[]> func = (args) =>
			{
				if (args.Length != 3)
				{
					throw new ArgumentException("命令參數數量為3");
				}

				object arg0;
				Command.Conversion(args[0], out arg0, typeof(T0));

				object arg1;
				Command.Conversion(args[1], out arg1, typeof(T1));

				object arg2;
				Command.Conversion(args[2], out arg2, typeof(T2));

				return_value(exp.Compile().Invoke(instance, (T0)arg0, (T1)arg1 , (T2)arg2));
			};
			_Register(exp, func);
		}

		public void RegisterLambda<TThis, T0, T1, T2 , T3, TR>(TThis instance, Expression<Func<TThis, T0, T1, T2 , T3, TR>> exp, Action<TR> return_value)
		{
			Action<string[]> func = (args) =>
			{
				if (args.Length != 4)
				{
					throw new ArgumentException("命令參數數量為4");
				}

				object arg0;
				Command.Conversion(args[0], out arg0, typeof(T0));

				object arg1;
				Command.Conversion(args[1], out arg1, typeof(T1));

				object arg2;
				Command.Conversion(args[2], out arg2, typeof(T2));

				object arg3;
				Command.Conversion(args[3], out arg3, typeof(T3));

				return_value(exp.Compile().Invoke(instance, (T0)arg0, (T1)arg1, (T2)arg2, (T3)arg3));
			};
			_Register(exp, func);
		}

		public void RegisterLambda<TThis, T0, T1, T2 , T3, T4, TR>(TThis instance, Expression<Func<TThis, T0, T1, T2 , T3 , T4, TR>> exp, Action<TR> return_value)
		{
			Action<string[]> func = (args) =>
			{
				if (args.Length != 5)
				{
					throw new ArgumentException("命令參數數量為5");
				}

				object arg0;
				Command.Conversion(args[0], out arg0, typeof(T0));

				object arg1;
				Command.Conversion(args[1], out arg1, typeof(T1));

				object arg2;
				Command.Conversion(args[2], out arg2, typeof(T2));

				object arg3;
				Command.Conversion(args[3], out arg3, typeof(T3));

				object arg4;
				Command.Conversion(args[4], out arg4, typeof(T4));

				return_value(exp.Compile().Invoke(instance, (T0)arg0, (T1)arg1, (T2)arg2, (T3)arg3 , (T4)arg4));
			};
			_Register(exp, func);
		}


		public void RegisterLambda<TThis>(TThis instance, Expression<Action<TThis>> exp)
		{
			Action<string[]> func = (args) =>
			{
				if (args.Length != 0)
				{
					throw new ArgumentException("命令參數數量為0");
				}


				exp.Compile().Invoke(instance);
			};
			_Register(exp, func);
		}

		public void RegisterLambda<TThis,T0>(TThis instance, Expression<Action<TThis , T0>> exp)
		{
			Action<string[]> func = (args) =>
			{
				if (args.Length != 1)
				{
					throw new ArgumentException("命令參數數量為1");
				}
				object arg0;
				Command.Conversion(args[0], out arg0, typeof(T0));
				
				exp.Compile().Invoke(instance , (T0)arg0);
			};
			_Register(exp, func);
		}

		public void RegisterLambda<TThis, T0 , T1 >(TThis instance, Expression<Action<TThis, T0,T1>> exp)
		{
			Action<string[]> func = (args) =>
			{
				if (args.Length != 2)
				{
					throw new ArgumentException("命令參數數量為2");
				}
				object arg0;
				Command.Conversion(args[0], out arg0, typeof(T0));

				object arg1;
				Command.Conversion(args[1], out arg1, typeof(T1));

				exp.Compile().Invoke(instance, (T0)arg0 , (T1)arg1);
			};
			_Register(exp, func);
		}

		public void RegisterLambda<TThis, T0, T1 , T2>(TThis instance, Expression<Action<TThis, T0, T1, T2>> exp)
		{
			Action<string[]> func = (args) =>
			{
				if (args.Length != 3)
				{
					throw new ArgumentException("命令參數數量為3");
				}
				object arg0;
				Command.Conversion(args[0], out arg0, typeof(T0));

				object arg1;
				Command.Conversion(args[1], out arg1, typeof(T1));

				object arg2;
				Command.Conversion(args[2], out arg2, typeof(T2));

				exp.Compile().Invoke(instance, (T0)arg0, (T1)arg1 , (T2)arg2);
			};
			_Register(exp, func);
		}

		public void RegisterLambda<TThis, T0, T1, T2 , T3>(TThis instance, Expression<Action<TThis, T0, T1, T2,T3>> exp)
		{
			Action<string[]> func = (args) =>
			{
				if (args.Length != 4)
				{
					throw new ArgumentException("命令參數數量為4");
				}
				object arg0;
				Command.Conversion(args[0], out arg0, typeof(T0));

				object arg1;
				Command.Conversion(args[1], out arg1, typeof(T1));

				object arg2;
				Command.Conversion(args[2], out arg2, typeof(T2));

				object arg3;
				Command.Conversion(args[3], out arg3, typeof(T3));

				exp.Compile().Invoke(instance, (T0)arg0, (T1)arg1, (T2)arg2, (T3)arg3);
			};
			_Register(exp, func);
		}

		public void RegisterLambda<TThis, T0, T1, T2, T3 ,T4>(TThis instance, Expression<Action<TThis, T0, T1, T2, T3 , T4>> exp)
		{
			Action<string[]> func = (args) =>
			{
				if (args.Length != 5)
				{
					throw new ArgumentException("命令參數數量為5");
				}
				object arg0;
				Command.Conversion(args[0], out arg0, typeof(T0));

				object arg1;
				Command.Conversion(args[1], out arg1, typeof(T1));

				object arg2;
				Command.Conversion(args[2], out arg2, typeof(T2));

				object arg3;
				Command.Conversion(args[3], out arg3, typeof(T3));

				object arg4;
				Command.Conversion(args[4], out arg4, typeof(T4));

				exp.Compile().Invoke(instance, (T0)arg0, (T1)arg1, (T2)arg2, (T3)arg3 , (T4)arg4);
			};
			_Register(exp, func);
		}

		private void _Register(string command, Action<string[]> func, Type return_type, Type[] param_types)
		{
			var analysis = new Analysis(command);
			_AddCommand(analysis.Command, func);
			_SendRegister(analysis, return_type, param_types);
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
                value.Method.Invoke(value, new[] { ret });
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
                value.Method.Invoke(value, new[] { ret });
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
			if(_Commands.RemoveAll(cmd => cmd.Name == command) > 0)
			{
				UnregisterEvent(command);
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

		private void _AddCommand(string command, Action<string[]> func)
		{
			_Commands.Add(
				new Infomation
				{
					Name = command, 
					Handler = func
				});
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
