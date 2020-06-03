using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// This interface is used to create <see cref="IDynamicCommand"/>.
	/// </summary>
	public interface IDynamicCommandFactory
	{
		/// <summary>
		/// Creates a <see cref="IDynamicCommand"/> with the specified <paramref name="name"/>
		/// that will execute the specified <paramref name="execute"/> action.
		/// </summary>
		/// <param name="name">Command name</param>
		/// <param name="execute">Action to execute</param>
		/// <param name="decorator">Command decorator</param>
		/// <returns><see cref="IDynamicCommand"/></returns>
		IDynamicCommand CreateFromAction(
			string name,
			Action execute,
			IDynamicCommandStrategyDecorator decorator = null
		);

		/// <summary>
		/// Creates a <see cref="IDynamicCommand"/> with the specified <paramref name="name"/>
		/// that will execute the specified <paramref name="execute"/> action with
		/// a parameter of type <typeparamref name="TParameter"/>.
		/// </summary>
		/// <param name="name">Command name</param>
		/// <param name="execute">Action to execute</param>
		/// <param name="decorator">Command decorator</param>
		/// <returns><see cref="IDynamicCommand"/></returns>
		IDynamicCommand CreateFromAction<TParameter>(
			string name,
			Action<TParameter> execute,
			IDynamicCommandStrategyDecorator decorator = null
		);

		/// <summary>
		/// Creates a <see cref="IDynamicCommand"/> with the specified <paramref name="name"/>
		/// that will execute the specified <paramref name="execute"/> task.
		/// </summary>
		/// <param name="name">Command name</param>
		/// <param name="execute">Task to execute</param>
		/// <param name="decorator">Command decorator</param>
		/// <returns><see cref="IDynamicCommand"/></returns>
		IDynamicCommand CreateFromTask(
			string name,
			Func<CancellationToken, Task> execute,
			IDynamicCommandStrategyDecorator decorator = null
		);

		/// <summary>
		/// Creates a <see cref="IDynamicCommand"/> with the specified <paramref name="name"/>
		/// that will execute the specified <paramref name="execute"/> task with
		/// a parameter of type <typeparamref name="TParameter"/>.
		/// </summary>
		/// <param name="name">Command name</param>
		/// <param name="execute">Task to execute</param>
		/// <param name="decorator">Command decorator</param>
		/// <returns><see cref="IDynamicCommand"/></returns>
		IDynamicCommand CreateFromTask<TParameter>(
			string name,
			Func<CancellationToken, TParameter, Task> execute,
			IDynamicCommandStrategyDecorator decorator = null
		);
	}
}
