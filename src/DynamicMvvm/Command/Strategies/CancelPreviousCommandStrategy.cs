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
		/// <param name="builder">The builder.</param>
		/// <returns><see cref="IDynamicCommandBuilder"/></returns>
		public static IDynamicCommandBuilder CancelPrevious(this IDynamicCommandBuilder builder)
			=> builder.WithStrategy(new CancelPreviousCommandStrategy());
	}

	/// <summary>
	/// This <see cref="DelegatingCommandStrategy"/> will cancel the previous command execution when executing the command.
	/// </summary>
	public class CancelPreviousCommandStrategy : DelegatingCommandStrategy
	{
		private CancellationTokenSource _cancellationTokenSource;

		/// <summary>
		/// Initializes a new instance of the <see cref="CancelPreviousCommandStrategy"/> class.
		/// </summary>
		public CancelPreviousCommandStrategy()
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
