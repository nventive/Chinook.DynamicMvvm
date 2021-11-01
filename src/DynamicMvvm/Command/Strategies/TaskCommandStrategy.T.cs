using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// This <see cref="IDynamicCommandStrategy"/> will execute a task with
	/// a parameter of type <typeparamref name="TParameter"/>.
	/// </summary>
	public class TaskCommandStrategy<TParameter> : TaskCommandStrategy
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TaskCommandStrategy{TParameter}"/> class.
		/// </summary>
		/// <param name="execute">Action to execute</param>
		public TaskCommandStrategy(Func<CancellationToken, TParameter, Task> execute)
			: base((ct, p) => execute(ct, (TParameter)p))
		{
			if (execute == null)
			{
				throw new ArgumentNullException(nameof(execute));
			}
		}
	}
}
