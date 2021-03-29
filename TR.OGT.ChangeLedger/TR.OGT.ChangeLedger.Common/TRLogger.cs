using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Text;
using TR.OGT.Common.Configuration.Application;
using TR.OGT.Common.Logging;

using TRLogger = TR.OGT.Common.Log;

namespace TR.OGT.ChangeLedger.Common
{
	public class TRLogger<T> : ILogger<T>
	{
		private readonly LogType _logType;

		public TRLogger(IOptions<LogConfig> config)
		{
			_logType = config.Value.LogType;
		}

		public IDisposable BeginScope<TState>(TState state)
			=> default;

		public bool IsEnabled(LogLevel logLevel)
			=> true;

		public void Log<TState>(
			LogLevel logLevel,
			EventId eventId,
			TState state,
			Exception exception,
			Func<TState, Exception, string> formatter)
		{
			var sb = new StringBuilder(formatter(state, exception));

			sb.AppendLine();

			if (exception != null)
				sb.AppendLine(JsonConvert.SerializeObject(exception));

			TRLogger.Write(CreateLogDto(_logType, Convert(logLevel), -1, sb.ToString()));
		}

		private LogDataDTO CreateLogDto(LogType logType, LogSeverity severity, int partnerID, string message)
			=> new LogDataDTO
			{
				ApplicationName = typeof(T).Name,
				LogType = logType,
				Severity = severity,
				PartnerID = partnerID,
				Message = message
			};

		private static LogSeverity Convert(LogLevel logLevel)
		{
			switch (logLevel)
			{
				case LogLevel.Trace:
					return LogSeverity.Trace;
				case LogLevel.Debug:
					return LogSeverity.Debug;
				case LogLevel.Information:
					return LogSeverity.Info;
				case LogLevel.Warning:
					return LogSeverity.Warn;
				case LogLevel.Error:
					return LogSeverity.Error;
				case LogLevel.Critical:
					return LogSeverity.Fatal;
				default:
					return LogSeverity.Trace;
			}
		}
	}
}
