using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// This <see cref="IDynamicCommandStrategy"/> will execute a task.
	/// </summary>
	public class TaskCommandStrategy : IDynamicCommandStrategy
	{
		private static readonly Func<object, bool> _defaultCanExecute = _ => true;

		private readonly Func<object, bool> _canExecute;
		private readonly Func<CancellationToken, object, Task> _execute;

		/// <summary>
		/// Initializes a new instance of the <see cref="TaskCommandStrategy"/> class.
		/// </summary>
		/// <param name="execute">Action to execute</param>
		/// <param name="canExecute">Can execute evaluator</param>
		public TaskCommandStrategy(Func<CancellationToken, object, Task> execute, Func<object, bool> canExecute)
		{
			_execute = execute ?? throw new ArgumentNullException(nameof(execute));
			_canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TaskCommandStrategy"/> class.
		/// </summary>
		/// <param name="execute">Action to execute</param>
		/// <param name="canExecute">Can execute evaluator</param>
		public TaskCommandStrategy(Func<CancellationToken, Task> execute, Func<object, bool> canExecute)
			: this((ct, _) => execute(ct), canExecute)
		{
			if (execute == null)
			{
				throw new ArgumentNullException(nameof(execute));
			}

			if (canExecute == null)
			{
				throw new ArgumentNullException(nameof(canExecute));
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TaskCommandStrategy"/> class.
		/// </summary>
		/// <param name="execute">Action to execute</param>
		public TaskCommandStrategy(Func<CancellationToken, object, Task> execute)
		   : this(execute, _defaultCanExecute)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TaskCommandStrategy"/> class.
		/// </summary>
		/// <param name="execute">Action to execute</param>
		public TaskCommandStrategy(Func<CancellationToken, Task> execute)
			: this((ct, _) => execute(ct), _defaultCanExecute)
		{
			if (execute == null)
			{
				throw new ArgumentNullException(nameof(execute));
			}
		}

		/// <inheritdoc />
		public bool CanExecute(object parameter, IDynamicCommand command)
			=> _canExecute(parameter);

		/// <inheritdoc />
		public Task Execute(CancellationToken ct, object parameter, IDynamicCommand command)
			=> _execute(ct, parameter);

		/// <inheritdoc />
		public event EventHandler CanExecuteChanged;

		/// <inheritdoc />
		public void Dispose()
		{
			CanExecuteChanged = null;
		}
	}
}
