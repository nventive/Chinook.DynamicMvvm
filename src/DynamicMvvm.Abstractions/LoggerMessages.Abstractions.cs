using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Chinook.DynamicMvvm
{
	internal static partial class LoggerMessagesAbstractions
	{
		[LoggerMessage(101, LogLevel.Warning, "Resolving property '{ViewModelTypeName}.{PropertyName}' using reflection on '{ViewModelName}'.")]
		public static partial void LogViewModelResolvingPropertyUsingReflection(this ILogger logger, string viewModelTypeName, string propertyName, string viewModelName);
	}
}
