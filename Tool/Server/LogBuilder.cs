// Decompiled with JetBrains decompiler
// Type: NLog.Fluent.LogBuilder
// Assembly: NLog, Version=4.0.0.0, Culture=neutral, PublicKeyToken=5120e14c03d0593c
// MVID: B43B3E78-0EB1-44DA-A722-4BB6F7A9ABF9
// Assembly location: D:\Projects\C#\Test\packages\NLog.4.1.2\lib\net45\NLog.dll

using NLog;
using NLog.Time;
using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;




// Decompiled with JetBrains decompiler
// Type: NLog.Fluent.LoggerExtensions
// Assembly: NLog, Version=4.0.0.0, Culture=neutral, PublicKeyToken=5120e14c03d0593c
// MVID: B43B3E78-0EB1-44DA-A722-4BB6F7A9ABF9
// Assembly location: D:\Projects\Regulus\packages\NLog.4.1.2\lib\net45\NLog.dll

using NLog;
using System;

namespace NLog.Fluent_2
{
	/// <summary>
	/// Extension methods for NLog <see cref="T:NLog.Logger"/>.
	/// 
	/// </summary>
	public static class LoggerExtensions
	{
		/// <summary>
		/// Starts building a log event with the specified <see cref="T:NLog.LogLevel"/>.
		/// 
		/// </summary>
		/// <param name="logger">The logger to write the log event to.</param><param name="logLevel">The log level.</param>
		/// <returns/>
		[CLSCompliant(false)]
		public static LogBuilder Log(this ILogger logger, NLog.LogLevel logLevel)
		{
			return new LogBuilder(logger, logLevel);
		}

		/// <summary>
		/// Starts building a log event at the <c>Trace</c> level.
		/// 
		/// </summary>
		/// <param name="logger">The logger to write the log event to.</param>
		/// <returns/>
		[CLSCompliant(false)]
		public static LogBuilder Trace(this ILogger logger)
		{
			return new LogBuilder(logger, NLog.LogLevel.Trace);
		}

		/// <summary>
		/// Starts building a log event at the <c>Debug</c> level.
		/// 
		/// </summary>
		/// <param name="logger">The logger to write the log event to.</param>
		/// <returns/>
		[CLSCompliant(false)]
		public static LogBuilder Debug(this ILogger logger)
		{
			return new LogBuilder(logger, NLog.LogLevel.Debug);
		}

		/// <summary>
		/// Starts building a log event at the <c>Info</c> level.
		/// 
		/// </summary>
		/// <param name="logger">The logger to write the log event to.</param>
		/// <returns/>
		[CLSCompliant(false)]
		public static LogBuilder Info(this ILogger logger)
		{
			return new LogBuilder(logger, NLog.LogLevel.Info);
		}

		/// <summary>
		/// Starts building a log event at the <c>Warn</c> level.
		/// 
		/// </summary>
		/// <param name="logger">The logger to write the log event to.</param>
		/// <returns/>
		[CLSCompliant(false)]
		public static LogBuilder Warn(this ILogger logger)
		{
			return new LogBuilder(logger, NLog.LogLevel.Warn);
		}

		/// <summary>
		/// Starts building a log event at the <c>Error</c> level.
		/// 
		/// </summary>
		/// <param name="logger">The logger to write the log event to.</param>
		/// <returns/>
		[CLSCompliant(false)]
		public static LogBuilder Error(this ILogger logger)
		{
			return new LogBuilder(logger, NLog.LogLevel.Error);
		}

		/// <summary>
		/// Starts building a log event at the <c>Fatal</c> level.
		/// 
		/// </summary>
		/// <param name="logger">The logger to write the log event to.</param>
		/// <returns/>
		[CLSCompliant(false)]
		public static LogBuilder Fatal(this ILogger logger)
		{
			return new LogBuilder(logger, NLog.LogLevel.Fatal);
		}
	}
}

namespace NLog.Fluent_2
{
	/// <summary>
	/// A fluent class to build log events for NLog.
	/// 
	/// </summary>
	public class LogBuilder
	{
		private readonly LogEventInfo _logEvent;
		private readonly ILogger _logger;

		/// <summary>
		/// Gets the <see cref="P:NLog.Fluent.LogBuilder.LogEventInfo"/> created by the builder.
		/// 
		/// </summary>
		public LogEventInfo LogEventInfo
		{
			get
			{
				return this._logEvent;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:NLog.Fluent.LogBuilder"/> class.
		/// 
		/// </summary>
		/// <param name="logger">The <see cref="T:NLog.Logger"/> to send the log event.</param>
		[CLSCompliant(false)]
		public LogBuilder(ILogger logger)
		  : this(logger, NLog.LogLevel.Debug)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:NLog.Fluent.LogBuilder"/> class.
		/// 
		/// </summary>
		/// <param name="logger">The <see cref="T:NLog.Logger"/> to send the log event.</param><param name="logLevel">The <see cref="T:NLog.LogLevel"/> for the log event.</param>
		[CLSCompliant(false)]
		public LogBuilder(ILogger logger, NLog.LogLevel logLevel)
		{
			if (logger == null)
				throw new ArgumentNullException("logger");
			if (logLevel == (NLog.LogLevel)null)
				throw new ArgumentNullException("logLevel");
			this._logger = logger;
			this._logEvent = new LogEventInfo()
			{
				Level = logLevel,
				LoggerName = logger.Name,
				TimeStamp = TimeSource.Current.Time
			};
		}

		/// <summary>
		/// Sets the <paramref name="exception"/> information of the logging event.
		/// 
		/// </summary>
		/// <param name="exception">The exception information of the logging event.</param>
		/// <returns/>
		public LogBuilder Exception(Exception exception)
		{
			this._logEvent.Exception = exception;
			return this;
		}

		/// <summary>
		/// Sets the level of the logging event.
		/// 
		/// </summary>
		/// <param name="logLevel">The level of the logging event.</param>
		/// <returns/>
		public LogBuilder Level(NLog.LogLevel logLevel)
		{
			if (logLevel == (NLog.LogLevel)null)
				throw new ArgumentNullException("logLevel");
			this._logEvent.Level = logLevel;
			return this;
		}

		/// <summary>
		/// Sets the logger name of the logging event.
		/// 
		/// </summary>
		/// <param name="loggerName">The logger name of the logging event.</param>
		/// <returns/>
		public LogBuilder LoggerName(string loggerName)
		{
			this._logEvent.LoggerName = loggerName;
			return this;
		}

		/// <summary>
		/// Sets the log message on the logging event.
		/// 
		/// </summary>
		/// <param name="message">The log message for the logging event.</param>
		/// <returns/>
		public LogBuilder Message(string message)
		{
			this._logEvent.Message = message;
			return this;
		}

		/// <summary>
		/// Sets the log message and parameters for formatting on the logging event.
		/// 
		/// </summary>
		/// <param name="format">A composite format string.</param><param name="arg0">The object to format.</param>
		/// <returns/>
		public LogBuilder Message(string format, object arg0)
		{
			this._logEvent.Message = format;
			this._logEvent.Parameters = new object[1]
			{
		arg0
			};
			return this;
		}

		/// <summary>
		/// Sets the log message and parameters for formatting on the logging event.
		/// 
		/// </summary>
		/// <param name="format">A composite format string.</param><param name="arg0">The first object to format.</param><param name="arg1">The second object to format.</param>
		/// <returns/>
		public LogBuilder Message(string format, object arg0, object arg1)
		{
			this._logEvent.Message = format;
			this._logEvent.Parameters = new object[2]
			{
		arg0,
		arg1
			};
			return this;
		}

		/// <summary>
		/// Sets the log message and parameters for formatting on the logging event.
		/// 
		/// </summary>
		/// <param name="format">A composite format string.</param><param name="arg0">The first object to format.</param><param name="arg1">The second object to format.</param><param name="arg2">The third object to format.</param>
		/// <returns/>
		public LogBuilder Message(string format, object arg0, object arg1, object arg2)
		{
			this._logEvent.Message = format;
			this._logEvent.Parameters = new object[3]
			{
		arg0,
		arg1,
		arg2
			};
			return this;
		}

		/// <summary>
		/// Sets the log message and parameters for formatting on the logging event.
		/// 
		/// </summary>
		/// <param name="format">A composite format string.</param><param name="arg0">The first object to format.</param><param name="arg1">The second object to format.</param><param name="arg2">The third object to format.</param><param name="arg3">The fourth object to format.</param>
		/// <returns/>
		public LogBuilder Message(string format, object arg0, object arg1, object arg2, object arg3)
		{
			this._logEvent.Message = format;
			this._logEvent.Parameters = new object[4]
			{
		arg0,
		arg1,
		arg2,
		arg3
			};
			return this;
		}

		/// <summary>
		/// Sets the log message and parameters for formatting on the logging event.
		/// 
		/// </summary>
		/// <param name="format">A composite format string.</param><param name="args">An object array that contains zero or more objects to format.</param>
		/// <returns/>
		public LogBuilder Message(string format, params object[] args)
		{
			this._logEvent.Message = format;
			this._logEvent.Parameters = args;
			return this;
		}

		/// <summary>
		/// Sets the log message and parameters for formatting on the logging event.
		/// 
		/// </summary>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param><param name="format">A composite format string.</param><param name="args">An object array that contains zero or more objects to format.</param>
		/// <returns/>
		public LogBuilder Message(IFormatProvider provider, string format, params object[] args)
		{
			this._logEvent.FormatProvider = provider;
			this._logEvent.Message = format;
			this._logEvent.Parameters = args;
			return this;
		}

		/// <summary>
		/// Sets a per-event context property on the logging event.
		/// 
		/// </summary>
		/// <param name="name">The name of the context property.</param><param name="value">The value of the context property.</param>
		/// <returns/>
		public LogBuilder Property(object name, object value)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			this._logEvent.Properties[name] = value;
			return this;
		}

		/// <summary>
		/// Sets multiple per-event context properties on the logging event.
		/// 
		/// </summary>
		/// <param name="properties">The properties to set.</param>
		/// <returns/>
		public LogBuilder Properties(IDictionary properties)
		{
			if (properties == null)
				throw new ArgumentNullException("properties");
			foreach (object index in (IEnumerable)properties.Keys)
				this._logEvent.Properties[index] = properties[index];
			return this;
		}

		/// <summary>
		/// Sets the timestamp of the logging event.
		/// 
		/// </summary>
		/// <param name="timeStamp">The timestamp of the logging event.</param>
		/// <returns/>
		public LogBuilder TimeStamp(DateTime timeStamp)
		{
			this._logEvent.TimeStamp = timeStamp;
			return this;
		}

		/// <summary>
		/// Sets the stack trace for the event info.
		/// 
		/// </summary>
		/// <param name="stackTrace">The stack trace.</param><param name="userStackFrame">Index of the first user stack frame within the stack trace.</param>
		/// <returns/>
		public LogBuilder StackTrace(StackTrace stackTrace, int userStackFrame)
		{
			this._logEvent.SetStackTrace(stackTrace, userStackFrame);
			return this;
		}

		/// <summary>
		/// Writes the log event to the underlying logger.
		/// 
		/// </summary>
		/// <param name="callerMemberName">The method or property name of the caller to the method. This is set at by the compiler.</param><param name="callerFilePath">The full path of the source file that contains the caller. This is set at by the compiler.</param><param name="callerLineNumber">The line number in the source file at which the method is called. This is set at by the compiler.</param>
		public void Write([CallerMemberName] string callerMemberName = null, [CallerFilePath] string callerFilePath = null, [CallerLineNumber] int callerLineNumber = 0)
		{
			if (callerMemberName != null)
				this.Property((object)"CallerMemberName", (object)callerMemberName);
			if (callerFilePath != null)
				this.Property((object)"CallerFilePath", (object)callerFilePath);
			if (callerLineNumber != 0)
				this.Property((object)"CallerLineNumber", (object)callerLineNumber);
			this._logger.Log(this._logEvent);
		}

		/// <summary>
		/// Writes the log event to the underlying logger if the condition delegate is true.
		/// 
		/// </summary>
		/// <param name="condition">If condition is true, write log event; otherwise ignore event.</param><param name="callerMemberName">The method or property name of the caller to the method. This is set at by the compiler.</param><param name="callerFilePath">The full path of the source file that contains the caller. This is set at by the compiler.</param><param name="callerLineNumber">The line number in the source file at which the method is called. This is set at by the compiler.</param>
		public void WriteIf(Func<bool> condition, [CallerMemberName] string callerMemberName = null, [CallerFilePath] string callerFilePath = null, [CallerLineNumber] int callerLineNumber = 0)
		{
			if (condition == null || !condition())
				return;
			if (callerMemberName != null)
				this.Property((object)"CallerMemberName", (object)callerMemberName);
			if (callerFilePath != null)
				this.Property((object)"CallerFilePath", (object)callerFilePath);
			if (callerLineNumber != 0)
				this.Property((object)"CallerLineNumber", (object)callerLineNumber);
			this._logger.Log(this._logEvent);
		}

		/// <summary>
		/// Writes the log event to the underlying logger if the condition is true.
		/// 
		/// </summary>
		/// <param name="condition">If condition is true, write log event; otherwise ignore event.</param><param name="callerMemberName">The method or property name of the caller to the method. This is set at by the compiler.</param><param name="callerFilePath">The full path of the source file that contains the caller. This is set at by the compiler.</param><param name="callerLineNumber">The line number in the source file at which the method is called. This is set at by the compiler.</param>
		public void WriteIf(bool condition, [CallerMemberName] string callerMemberName = null, [CallerFilePath] string callerFilePath = null, [CallerLineNumber] int callerLineNumber = 0)
		{
			if (!condition)
				return;
			if (callerMemberName != null)
				this.Property((object)"CallerMemberName", (object)callerMemberName);
			if (callerFilePath != null)
				this.Property((object)"CallerFilePath", (object)callerFilePath);
			if (callerLineNumber != 0)
				this.Property((object)"CallerLineNumber", (object)callerLineNumber);
			this._logger.Log(this._logEvent);
		}
	}
}
