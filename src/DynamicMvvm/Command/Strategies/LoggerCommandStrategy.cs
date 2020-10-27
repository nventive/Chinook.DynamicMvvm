using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Chinook.DynamicMvvm
{
	public static partial class DynamicCommandStrategyExtensions
	{
		/// <summary>
		/// Will add logs to the command execution.
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <param name="logger">Optional; the desired logger. If null is passed, a new one will be created using <see cref="DynamicMvvmConfiguration.LoggerFactory"/>.</param>
		/// <returns><see cref="IDynamicCommandBuilder"/></returns>
		public static IDynamicCommandBuilder WithLogs(this IDynamicCommandBuilder builder, ILogger logger = null)
			=> builder.WithStrategy(new LoggerCommandStrategy(logger ?? typeof(IDynamicCommand).Log()));
	}

	/// <summary>
	/// This <see cref="DecoratorCommandStrategy"/> will log the execution of the command.
	/// </summary>
	public class LoggerCommandStrategy : DecoratorCommandStrategy
	{
		private readonly ILogger _logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="LoggerCommandStrategy"/> class.
		/// </summary>
		/// <param name="logger"><see cref="ILogger"/></param>
		public LoggerCommandStrategy(ILogger logger)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <inheritdoc />
		public override async Task Execute(CancellationToken ct, object parameter, IDynamicCommand command)
		{
			try
			{
				_logger.LogDebug($"Executing command '{command.Name}'.");

				await base.Execute(ct, parameter, command);

				_logger.LogInformation($"Executed command '{command.Name}'.");
			}
			catch (Exception e)
			{
				_logger.LogError(e, $"Failed to execute command '{command.Name}'.");

				throw;
			}
		}
	}
}
