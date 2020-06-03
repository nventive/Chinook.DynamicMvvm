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
		/// Will cancel the previous command execution when executing the command.
		/// </summary>
		/// <param name="innerStrategy"><see cref="IDynamicCommandStrategy"/></param>
		/// <returns><see cref="IDynamicCommandStrategy"/></returns>
		public static IDynamicCommandStrategy CancelPrevious(this IDynamicCommandStrategy innerStrategy)
			=> new CancelPreviousCommandStrategy(innerStrategy);
	}

	/// <summary>
	/// This <see cref="DecoratorCommandStrategy"/> will cancel the previous command execution when executing the command.
	/// </summary>
	public class CancelPreviousCommandStrategy : DecoratorCommandStrategy
	{
		private CancellationTokenSource _cancellationTokenSource;

		/// <summary>
		/// Initializes a new instance of the <see cref="CancelPreviousCommandStrategy"/> class.
		/// </summary>
		/// <param name="innerStrategy"><see cref="IDynamicCommandStrategy"/></param>
		public CancelPreviousCommandStrategy(IDynamicCommandStrategy innerStrategy)
			: base(innerStrategy)
		{
		}

		/// <inheritdoc />
		public override async Task Execute(CancellationToken ct, object parameter, IDynamicCommand command)
		{
			TryCancelExecution();

			_cancellationTokenSource = new CancellationTokenSource();

			using (ct.Register(TryCancelExecution))
			{
				await base.Execute(_cancellationTokenSource.Token, parameter, command);
			}
		}

		/// <summary>
		/// Will cancel the current execution if any.
		/// </summary>
		private void TryCancelExecution()
		{
			if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
			{
				_cancellationTokenSource.Cancel();
				_cancellationTokenSource.Dispose();
			}
		}

		/// <inheritdoc />
		public override void Dispose()
		{
			TryCancelExecution();

			base.Dispose();
		}
	}
}
