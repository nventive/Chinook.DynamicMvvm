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
		/// Will disable the command while it's executing.
		/// </summary>
		/// <param name="innerStrategy"><see cref="IDynamicCommandStrategy"/></param>
		/// <returns><see cref="IDynamicCommandStrategy"/></returns>
		public static IDynamicCommandStrategy DisableWhileExecuting(this IDynamicCommandStrategy innerStrategy)
			=> new DisableWhileExecutingCommandStrategy(innerStrategy);
	}

	/// <summary>
	/// This <see cref="DecoratorCommandStrategy"/> will disable the command while it's executing.
	/// </summary>
	public class DisableWhileExecutingCommandStrategy : DecoratorCommandStrategy
	{
		public int _isExecuting;

		/// <summary>
		/// Initializes a new instance of the <see cref="DisableWhileExecutingCommandStrategy"/> class.
		/// </summary>
		/// <param name="innerStrategy"><see cref="IDynamicCommandStrategy"/></param>
		public DisableWhileExecutingCommandStrategy(IDynamicCommandStrategy innerStrategy)
			: base(innerStrategy)
		{
			innerStrategy.CanExecuteChanged += OnInnerCanExecuteChanged;
		}

		/// <inheritdoc />
		public override event EventHandler CanExecuteChanged;

		/// <inheritdoc />
		public override bool CanExecute(object parameter, IDynamicCommand command)
		{
			var isExecuting = _isExecuting == 1;

			return !isExecuting && InnerStrategy.CanExecute(parameter, command);
		}

		/// <inheritdoc />
		public override async Task Execute(CancellationToken ct, object parameter, IDynamicCommand command)
		{
			if (Interlocked.CompareExchange(ref _isExecuting, 1, 0) == 0)
			{
				try
				{
					RaiseCanExecuteChanged();

					await base.Execute(ct, parameter, command);
				}
				finally
				{
					_isExecuting = 0;

					RaiseCanExecuteChanged();
				}
			}
		}

		private void RaiseCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}

		private void OnInnerCanExecuteChanged(object sender, EventArgs e)
		{
			RaiseCanExecuteChanged();
		}

		/// <inheritdoc />
		public override void Dispose()
		{
			InnerStrategy.CanExecuteChanged -= OnInnerCanExecuteChanged;

			base.Dispose();
		}
	}
}
