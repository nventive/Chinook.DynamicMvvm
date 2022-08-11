using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Chinook.DynamicMvvm;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// This <see cref="IDynamicCommandStrategy"/> ensures that the <see cref="CanExecuteChanged"/> event is raised using <see cref="IDispatcher.ExecuteOnDispatcher(CancellationToken, Action)"/>.
	/// </summary>
	public class RaiseCanExecuteOnDispatcherCommandStrategy : DecoratorCommandStrategy
	{
		private readonly WeakReference<IViewModel> _viewModel;
		private readonly CancellationTokenSource _cts = new CancellationTokenSource();

		/// <summary>
		/// Creates a new instance of <see cref="RaiseCanExecuteOnDispatcherCommandStrategy"/>.
		/// </summary>
		/// <param name="viewModel">The <see cref="IViewModel"/> from which to access the <see cref="IDispatcher"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="viewModel"/> cannot be null.</exception>
		public RaiseCanExecuteOnDispatcherCommandStrategy(IViewModel viewModel)
		{
			if (viewModel is null)
			{
				throw new ArgumentNullException(nameof(viewModel));
			}

			_viewModel = new WeakReference<IViewModel>(viewModel);
		}

		/// <inheritdoc/>
		public override IDynamicCommandStrategy InnerStrategy
		{
			get => base.InnerStrategy;
			set
			{
				if (base.InnerStrategy != null)
				{
					base.InnerStrategy.CanExecuteChanged -= OnInnerCanExecuteChanged;
				}

				base.InnerStrategy = value;

				if (base.InnerStrategy != null)
				{
					base.InnerStrategy.CanExecuteChanged += OnInnerCanExecuteChanged;
				}
			}
		}

		/// <inheritdoc/>
		public override event EventHandler CanExecuteChanged;

		private void OnInnerCanExecuteChanged(object sender, EventArgs e)
		{
			var hasVM = _viewModel.TryGetTarget(out var viewModel);

			if (!hasVM || viewModel.IsDisposed)
			{
				return;
			}

			// The event should be raised immediately when the view already has dispatcher access OR when there is no view.
			var shouldRaiseImmediately= viewModel.Dispatcher?.GetHasDispatcherAccess() ?? true;

			if (shouldRaiseImmediately)
			{
				RaiseCanExecuteChanged();
			}
			else
			{
				_ = viewModel.Dispatcher.ExecuteOnDispatcher(_cts.Token, RaiseCanExecuteChanged);
			}

			void RaiseCanExecuteChanged()
			{
				CanExecuteChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		/// <inheritdoc/>
		public override void Dispose()
		{
			base.Dispose();

			_cts.Cancel();
			_cts.Dispose();
		}
	}
}
