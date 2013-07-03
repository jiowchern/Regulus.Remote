using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{
	public class CommandSender
	{
		public class CommandInvoker
		{
			public string Name { get; set; }
			public object Handler { get; set; }
			public Type Type { get; set; }
		}

        
		public class CommandParamParser
		{
			public Type Type { get; set; }
			public Func<string, object> Parser { get; set; }
		}

		CommandParamParser[] _CommandParamParsers;
		System.Collections.Generic.List<CommandInvoker> _CommandInvoker = new List<CommandInvoker>();

        // 填入解析參數 , 命令參數皆由string 傳入 , 匹配對應的傳出值 
        // P.S 最少要實做 int float string
		public void Initialize(CommandParamParser[] parsers)
		{			
			_CommandParamParsers = parsers;
		}

		public void AddInvoker(string name , object invoker , Type type)
		{
			_CommandInvoker.Add(new CommandInvoker() { Name = name, Handler = invoker, Type = type });
		}
		public void RemoveInvoker(string name)
		{
			_CommandInvoker.RemoveAll( invoker => invoker.Name == name );
		}

		public void Send(string command_feedstock)
		{
			string pattern = @"(\w+)\.(\w+)([\w\s\.]*)";
			var rgx = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
			var matches = rgx.Matches(command_feedstock);
			var firstGroup = (from System.Text.RegularExpressions.Match m in matches select m.Groups).FirstOrDefault();
			if (firstGroup != null)
			{
				var commandAndArgs = (from System.Text.RegularExpressions.Group value in firstGroup select value.Value).ToArray();
				if (commandAndArgs.Count() > 1)
				{
					string name = commandAndArgs[1];

					var first = (from ch in _CommandInvoker where ch.Name == name select new { handler = ch.Handler, type = ch.Type }).FirstOrDefault();

					if (first != null)
					{
						string command = commandAndArgs[2];
						object[] args = _GetParams(first.type, command, commandAndArgs[3], _CommandParamParsers);
						try
						{
							first.type.InvokeMember(command, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.InvokeMethod, null, first.handler, args);

						}
						catch (System.Exception e)
						{
							throw new SystemException("沒有可呼叫命令 命令類型:" + name + "命令名稱:" + command + "參數:" + args.ToString() + "\n" + e.ToString());							
						}
					}
				}
				else
				{
					throw new SystemException("命令解析失敗");					
				}

			}
		}
		private static object[] _GetParams(Type type, string command, string args, CommandParamParser[] command_param_parsers)
		{
			string pattern = @"[\w\.]+";
			var rgx = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
			var matches = rgx.Matches(args);

			var method = type.GetMethod(command, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
			if (method != null)
			{
				var parameterInfos = method.GetParameters().ToArray();
				var argments = (from System.Text.RegularExpressions.Match match in matches select match.Value).ToArray();

				object[] retArgs = new object[parameterInfos.Count()];
				for (int i = 0; i < parameterInfos.Count(); ++i)
				{
					var parser = (from cpp in command_param_parsers where cpp.Type == parameterInfos[i].ParameterType select cpp.Parser).FirstOrDefault();
					if (parser != null)
					{
						try
						{
							retArgs[i] = parser.Invoke(argments[i]);							
						}					
						catch(SystemException e)
						{
                            throw new SystemException("參數解析失敗" + e.ToString());							
						}
						
						
					}
					else
					{
						retArgs[i] = parameterInfos[i].DefaultValue;
					}
				}

				return retArgs;
			}
			return null;
		}

		public void Release()
		{

		}
	}
}
