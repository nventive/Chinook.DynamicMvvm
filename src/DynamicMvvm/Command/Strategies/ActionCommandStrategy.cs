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
		private readonly Action<object> _execute;

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionCommandStrategy"/> class.
		/// </summary>
		/// <param name="execute">Action to execute</param>
		public ActionCommandStrategy(Action<object> execute)
		{
			_execute = execute ?? throw new ArgumentNullException(nameof(execute));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionCommandStrategy"/> class.
		/// </summary>
		/// <param name="execute">Action to execute</param>
		public ActionCommandStrategy(Action execute)
			: this(_ => execute())
		{
			if (execute == null)
			{
				throw new ArgumentNullException(nameof(execute));
			}
		}

		/// <inheritdoc />
		public event EventHandler CanExecuteChanged;

		/// <inheritdoc />
		public bool CanExecute(object parameter, IDynamicCommand command) => true;

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
