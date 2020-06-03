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
		private static readonly Func<TParameter, bool> _defaultCanExecute = _ => true;

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionCommandStrategy{TParameter}"/> class.
		/// </summary>
		/// <param name="execute">Action to execute</param>
		/// <param name="canExecute">Can execute evaluator</param>
		public ActionCommandStrategy(Action<TParameter> execute, Func<TParameter, bool> canExecute)
			: base(p => execute((TParameter)p), p => canExecute((TParameter)p))
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
		/// Initializes a new instance of the <see cref="ActionCommandStrategy{TParameter}"/> class.
		/// </summary>
		/// <param name="execute">Action to execute</param>
		public ActionCommandStrategy(Action<TParameter> execute)
			: this(execute, _defaultCanExecute)
		{
		}
	}
}
