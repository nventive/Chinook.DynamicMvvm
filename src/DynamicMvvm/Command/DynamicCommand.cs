using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// This is a default implementation of <see cref="IDynamicCommand"/>.
	/// </summary>
	[Preserve(AllMembers = true)]
	public class DynamicCommand : IDynamicCommand
	{
		private static readonly DiagnosticSource _diagnostics = new DiagnosticListener("Chinook.DynamicMvvm.IDynamicCommand");

		private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
		private readonly IDynamicCommandStrategy _strategy;
		private int _executions;
		private bool _isDisposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicCommand"/> class.
		/// </summary>
		/// <param name="name">Command name</param>
		/// <param name="strategy"><see cref="IDynamicCommandStrategy"/></param>
		public DynamicCommand(string name, IDynamicCommandStrategy strategy)
		{
			Name = name;

			_strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));

			_strategy.CanExecuteChanged += OnCanExecuteChanged;

			if (_diagnostics.IsEnabled("Created"))
			{
				_diagnostics.Write("Created", Name);
			}
		}

		/// <inheritdoc />
		public string Name { get; }

		/// <inheritdoc />
		public bool IsExecuting => _executions > 0;

		/// <inheritdoc />
		public event EventHandler CanExecuteChanged;

		/// <inheritdoc />
		public event EventHandler IsExecutingChanged;

		/// <inheritdoc />
		public event PropertyChangedEventHandler PropertyChanged;

		/// <inheritdoc />
		public bool CanExecute(object parameter)
		{
			return _strategy.CanExecute(parameter, this);
		}

		/// <inheritdoc />
		void ICommand.Execute(object parameter)
		{
			_ = Execute(parameter);
		}

		/// <inheritdoc />
		public async Task Execute(object parameter)
		{
			try
			{
				if (!CanExecute(parameter))
				{
					return;
				}

				try
				{
					IncrementExecutions();

					await _strategy.Execute(_cancellationTokenSource.Token, parameter, this);
				}
				finally
				{
					DecrementExecutions();
				}
			}
			catch (Exception e)
			{
				// This will run on a UI thread, so we want to make sure
				// this task doesn't throw otherwise it could lead to a crash.
				this.Log().LogError(e, $"Command execution of '{Name}' failed. Consider using {nameof(ErrorHandlerCommandStrategy)}.");
			}
		}

		private void RaisePropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void OnCanExecuteChanged(object sender, EventArgs e)
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}

		private void IncrementExecutions()
		{
			// Raise the IsExecutingChanged only when the counter is at 1.
			if (Interlocked.Increment(ref _executions) == 1)
			{
				IsExecutingChanged?.Invoke(this, EventArgs.Empty);

				RaisePropertyChanged(nameof(IsExecuting));
			}
		}

		private void DecrementExecutions()
		{
			// Raise the IsExecutingChanged only when the counter is at 0.
			if (Interlocked.Decrement(ref _executions) == 0)
			{
				IsExecutingChanged?.Invoke(this, EventArgs.Empty);

				RaisePropertyChanged(nameof(IsExecuting));
			}
		}

		/// <inheritdoc />
		public override string ToString() => Name;

		/// <inheritdoc />
		protected virtual void Dispose(bool isDisposing)
		{
			if (_isDisposed)
			{
				return;
			}

			if (isDisposing)
			{
				_cancellationTokenSource.Cancel();
				_cancellationTokenSource.Dispose();

				_strategy.CanExecuteChanged -= OnCanExecuteChanged;
				_strategy.Dispose();
			}

			_isDisposed = true;

			if (_diagnostics.IsEnabled("Disposed"))
			{
				_diagnostics.Write("Disposed", Name);
			}
		}

		/// <inheritdoc />
		public void Dispose()
		{
			Dispose(isDisposing: true);

			// If diagnostics are enabled, don't suppress the finalizer.
			// This allows the differentiation between disposed and destroyed instances. 
			if (!_diagnostics.IsEnabled("Destroyed"))
			{
				GC.SuppressFinalize(this);
			}
		}

		/// <inheritdoc />
		~DynamicCommand()
		{
			Dispose(isDisposing: false);

			if (_diagnostics.IsEnabled("Destroyed"))
			{
				_diagnostics.Write("Destroyed", Name);
			}
		}
	}
}
