using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;


using NLog;


using Regulus.Utility;

namespace LogTool
{
	public class LogTool : Singleton<LogTool>
	{
		public Logger Logger = LogManager.GetCurrentClassLogger();

		public void LoadConfigFile()
		{ 
			//string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			//LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration(assemblyFolder + "\\NLog.config", true);
		}

		public void Info(string log_name)
		{
			var eventInfo = new LogEventInfo(LogLevel.Info, string.Empty, log_name);

			eventInfo.Properties["UserGuid"] = new Guid();

			Logger.Log(eventInfo);
		}

		public void SetInfo()
		{
			//var eventInfo = new LogEventInfo(LogLevel.Info, string.Empty, log_name);

			//eventInfo.Properties["UserGuid"] = new Guid();
		}

		public void Debug(string log_message)
		{
			var eventInfo = new LogEventInfo(LogLevel.Debug, string.Empty, log_message);
		}

	}
}
