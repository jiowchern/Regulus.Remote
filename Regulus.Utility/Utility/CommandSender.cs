using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

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

		private readonly List<CommandInvoker> _CommandInvoker = new List<CommandInvoker>();

		private CommandParamParser[] _CommandParamParsers;

		// 填入解析參數 , 命令參數皆由string 傳入 , 匹配對應的傳出值 
		// P.S 最少要實做 int float string
		public void Initialize(CommandParamParser[] parsers)
		{
			_CommandParamParsers = parsers;
		}

		public void AddInvoker(string name, object invoker, Type type)
		{
			_CommandInvoker.Add(
				new CommandInvoker
				{
					Name = name, 
					Handler = invoker, 
					Type = type
				});
		}

		public void RemoveInvoker(string name)
		{
			_CommandInvoker.RemoveAll(invoker => invoker.Name == name);
		}

		public void Send(string command_feedstock)
		{
			var pattern = @"(\w+)\.(\w+)([\w\s\.]*)";
			var rgx = new Regex(pattern, RegexOptions.IgnoreCase);
			var matches = rgx.Matches(command_feedstock);
			var firstGroup = (from Match m in matches select m.Groups).FirstOrDefault();
			if(firstGroup != null)
			{
				var commandAndArgs = (from Group value in firstGroup select value.Value).ToArray();
				if(commandAndArgs.Count() > 1)
				{
					var name = commandAndArgs[1];

					var first = (from ch in _CommandInvoker
					             where ch.Name == name
					             select new
					             {
						             handler = ch.Handler, 
						             type = ch.Type
					             }).FirstOrDefault();

					if(first != null)
					{
						var command = commandAndArgs[2];
						var args = CommandSender._GetParams(first.type, command, commandAndArgs[3], _CommandParamParsers);
						try
						{
							first.type.InvokeMember(
								command, 
								BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod, 
								null, 
								first.handler, 
								args);
						}
						catch(Exception e)
						{
							throw new SystemException("沒有可呼叫命令 命令類型:" + name + "命令名稱:" + command + "參數:" + args + "\n" + e);
						}
					}
				}
				else
				{
					throw new SystemException("命令解析失敗");
				}
			}
		}

		private static object[] _GetParams(
			Type type, 
			string command, 
			string args, 
			CommandParamParser[] command_param_parsers)
		{
			var pattern = @"[\w\.]+";
			var rgx = new Regex(pattern, RegexOptions.IgnoreCase);
			var matches = rgx.Matches(args);

			var method = type.GetMethod(command, BindingFlags.Public | BindingFlags.Instance);
			if(method != null)
			{
				var parameterInfos = method.GetParameters().ToArray();
				var argments = (from Match match in matches select match.Value).ToArray();

				var retArgs = new object[parameterInfos.Count()];
				for(var i = 0; i < parameterInfos.Count(); ++i)
				{
					var parser =
						(from cpp in command_param_parsers where cpp.Type == parameterInfos[i].ParameterType select cpp.Parser)
							.FirstOrDefault();
					if(parser != null)
					{
						try
						{
							retArgs[i] = parser.Invoke(argments[i]);
						}
						catch(SystemException e)
						{
							throw new SystemException("參數解析失敗" + e);
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
