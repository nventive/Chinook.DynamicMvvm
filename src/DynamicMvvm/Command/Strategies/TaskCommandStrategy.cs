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
		private readonly Func<CancellationToken, object, Task> _execute;

		/// <summary>
		/// Initializes a new instance of the <see cref="TaskCommandStrategy"/> class.
		/// </summary>
		/// <param name="execute">Action to execute</param>
		public TaskCommandStrategy(Func<CancellationToken, object, Task> execute)
		{
			_execute = execute ?? throw new ArgumentNullException(nameof(execute));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TaskCommandStrategy"/> class.
		/// </summary>
		/// <param name="execute">Action to execute</param>
		public TaskCommandStrategy(Func<CancellationToken, Task> execute)
			: this((ct, _) => execute(ct))
		{
			if (execute == null)
			{
				throw new ArgumentNullException(nameof(execute));
			}
		}

		/// <inheritdoc />
		public bool CanExecute(object parameter, IDynamicCommand command) => true;

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
