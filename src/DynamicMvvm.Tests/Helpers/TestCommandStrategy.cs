using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chinook.DynamicMvvm.Tests.Helpers
{
	public class TestCommandStrategy : IDynamicCommandStrategy
	{
		private readonly Func<object, IDynamicCommand, bool> _onCanExecute;
		private readonly Func<CancellationToken, object, IDynamicCommand, Task> _onExecute;
		private readonly Action _onDispose;

		public TestCommandStrategy(
			Func<object, IDynamicCommand, bool> onCanExecute = null,
			Func<CancellationToken, object, IDynamicCommand, Task> onExecute = null,
			Action onDispose = null
		)
		{
			_onCanExecute = onCanExecute;
			_onExecute = onExecute;
			_onDispose = onDispose;
		}

		public bool IsDisposed { get; private set; }

		public bool IsExecuted { get; private set; }

		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter, IDynamicCommand command) => _onCanExecute != null
			? _onCanExecute.Invoke(parameter, command)
			: true;

		public Task Execute(CancellationToken ct, object parameter, IDynamicCommand command) => _onExecute != null
			? _onExecute.Invoke(ct, parameter, command)
			: Task.CompletedTask;

		public void RaiseCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}

		public void Dispose() => _onDispose?.Invoke();
	}
}
