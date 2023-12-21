using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Chinook.DynamicMvvm
{
	internal static partial class LoggerMessages
	{
		[LoggerMessage(201, LogLevel.Error, "Validation failed for property '{PropertyName}'.")]
		public static partial void LogValidationFailed(this ILogger logger, string propertyName, Exception exception);
	}
}
