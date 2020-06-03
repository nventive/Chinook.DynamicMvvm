using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chinook.DynamicMvvm
{
	public static partial class DynamicCommandStrategyExtensions
	{
		/// <summary>
		/// Will catch any exception thrown by the execution of the command and delegate it to the specified error handler.
		/// </summary>
		/// <param name="innerStrategy"><see cref="IDynamicCommandStrategy"/></param>
		/// <param name="errorHandler">Error handler</param>
		/// <returns><see cref="IDynamicCommandStrategy"/></returns>
		public static IDynamicCommandStrategy CatchErrors(this IDynamicCommandStrategy innerStrategy, IDynamicCommandErrorHandler errorHandler)
			=> new ErrorHandlerCommandStrategy(innerStrategy, errorHandler);

		/// <summary>
		/// Will catch any exception thrown by the execution of the command and delegate it to the specified error handler.
		/// </summary>
		/// <param name="innerStrategy"><see cref="IDynamicCommandStrategy"/></param>
		/// <param name="errorHandler">Error handler</param>
		/// <returns><see cref="IDynamicCommandStrategy"/></returns>
		public static IDynamicCommandStrategy CatchErrors(this IDynamicCommandStrategy innerStrategy, Func<CancellationToken, IDynamicCommand, Exception, Task> errorHandler)
			=> new ErrorHandlerCommandStrategy(innerStrategy, new DynamicCommandErrorHandler(errorHandler));
	}

	/// <summary>
	/// This <see cref="DecoratorCommandStrategy"/> will catch any exception
	/// that may be thrown during its execution and delegate the exception to an error handler.
	/// </summary>
	public class ErrorHandlerCommandStrategy : DecoratorCommandStrategy
	{
		private readonly IDynamicCommandErrorHandler _errorHandler;

		/// <summary>
		/// Initializes a new instance of the <see cref="ErrorHandlerCommandStrategy"/> class.
		/// </summary>
		/// <param name="innerStrategy"><see cref="IDynamicCommandStrategy"/></param>
		/// <param name="errorHandler">Error handler</param>
		public ErrorHandlerCommandStrategy(IDynamicCommandStrategy innerStrategy, IDynamicCommandErrorHandler errorHandler)
			: base(innerStrategy)
		{
			_errorHandler = errorHandler;
		}

		/// <inheritdoc/>
		public override async Task Execute(CancellationToken ct, object parameter, IDynamicCommand command)
		{
			try
			{
				await base.Execute(ct, parameter, command);
			}
			catch (Exception e)
			{
				await _errorHandler.HandleError(ct, command, e);
			}
		}
	}

	public interface IDynamicCommandErrorHandler
	{
		/// <summary>
		/// Handles an error from a <see cref="IDynamicCommand"/>.
		/// </summary>
		/// <param name="ct"><see cref="CancellationToken"/></param>
		/// <param name="command"><see cref="IDynamicCommand"/></param>
		/// <param name="exception"><see cref="Exception"/></param>
		/// <returns><see cref="Task"/></returns>
		Task HandleError(CancellationToken ct, IDynamicCommand command, Exception exception);
	}

	public class DynamicCommandErrorHandler : IDynamicCommandErrorHandler
	{
		private readonly Func<CancellationToken, IDynamicCommand, Exception, Task> _handlerFunction;

		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicCommandErrorHandler"/> class.
		/// </summary>
		/// <param name="handlerFunction">Error handler function</param>
		public DynamicCommandErrorHandler(Func<CancellationToken, IDynamicCommand, Exception, Task> handlerFunction)
		{
			_handlerFunction = handlerFunction ?? throw new ArgumentNullException(nameof(handlerFunction));
		}

		/// <inheritdoc/>
		public Task HandleError(CancellationToken ct, IDynamicCommand command, Exception exception)
			=> _handlerFunction.Invoke(ct, command, exception);
	}
}
