using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// This <see cref="IDynamicCommandStrategy"/> will execute an action.
	/// </summary>
	public class ActionCommandStrategy : IDynamicCommandStrategy
	{
		private static readonly Func<object, bool> _defaultCanExecute = _ => true;

		private readonly Func<object, bool> _canExecute;
		private readonly Action<object> _execute;

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionCommandStrategy"/> class.
		/// </summary>
		/// <param name="execute">Action to execute</param>
		/// <param name="canExecute">Can execute evaluator</param>
		public ActionCommandStrategy(Action<object> execute, Func<object, bool> canExecute)
		{
			_execute = execute ?? throw new ArgumentNullException(nameof(execute));
			_canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionCommandStrategy"/> class.
		/// </summary>
		/// <param name="execute">Action to execute</param>
		/// <param name="canExecute">Can execute evaluator</param>
		public ActionCommandStrategy(Action execute, Func<object, bool> canExecute)
			: this(_ => execute(), canExecute)
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
		/// Initializes a new instance of the <see cref="ActionCommandStrategy"/> class.
		/// </summary>
		/// <param name="execute">Action to execute</param>
		public ActionCommandStrategy(Action<object> execute)
			: this(execute, _defaultCanExecute)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionCommandStrategy"/> class.
		/// </summary>
		/// <param name="execute">Action to execute</param>
		public ActionCommandStrategy(Action execute)
			: this(execute, _defaultCanExecute)
		{
		}

		/// <inheritdoc />
		public event EventHandler CanExecuteChanged;

		/// <inheritdoc />
		public bool CanExecute(object parameter, IDynamicCommand command)
			=> _canExecute(parameter);

		/// <inheritdoc />
		public Task Execute(CancellationToken ct, object parameter, IDynamicCommand command)
		{
			_execute(parameter);

			return Task.CompletedTask;
		}

		/// <inheritdoc />
		public void Dispose()
		{
			CanExecuteChanged = null;
		}
	}
}
