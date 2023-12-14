using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Chinook.DynamicMvvm
{
	internal static partial class LoggerMessagesUnoWinUI
	{
		[LoggerMessage(401, LogLevel.Debug, "Cancelled 'ExecuteOnDispatcher' because of the cancellation token.")]
		public static partial void LogCancelledExecuteOnDispatcherBecauseOfCancellationToken(this ILogger logger);

		[LoggerMessage(402, LogLevel.Debug, "Executed action immediately because already on dispatcher.")]
		public static partial void LogExecutedActionImmediatelyBecauseAlreadyOnDispatcher(this ILogger logger);

		[LoggerMessage(403, LogLevel.Debug, "Batched {RequestCount} dispatcher requests.")]
		public static partial void LogBatchedDispatcherRequests(this ILogger logger, int requestCount);

		[LoggerMessage(404, LogLevel.Error, "Failed 'ExecuteOnDispatcher'.")]
		public static partial void LogFailedExecuteOnDispatcher(this ILogger logger, Exception exception);
	}
}
