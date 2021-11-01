using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// This <see cref="IDynamicCommandStrategy"/> will execute an action with
	/// a parameter of type <typeparamref name="TParameter"/>.
	/// </summary>
	public class ActionCommandStrategy<TParameter> : ActionCommandStrategy
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ActionCommandStrategy{TParameter}"/> class.
		/// </summary>
		/// <param name="execute">Action to execute</param>
		public ActionCommandStrategy(Action<TParameter> execute)
			: base(p => execute((TParameter)p))
		{
			if (execute == null)
			{
				throw new ArgumentNullException(nameof(execute));
			}
		}
	}
}
