using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Chinook.DynamicMvvm
{
	internal static partial class LoggerMessagesReactive
	{
		[LoggerMessage(301, LogLevel.Debug, "Deactivated observable of type '{TypeName}'.")]
		public static partial void LogDeactivatedObservable(this ILogger logger, string typeName);

		[LoggerMessage(302, LogLevel.Debug, "Reactivated observable of type '{TypeName}'.")]
		public static partial void LogReactivatedObservable(this ILogger logger, string typeName);

	}
}
