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
	public interface IDynamicCommandBuilderFactory
	{
		/// <summary>
		/// Creates a <see cref="IDynamicCommandBuilder"/> with the specified <paramref name="name"/>
		/// that will execute the specified <paramref name="execute"/> action.
		/// </summary>
		/// <param name="name">The command name.</param>
		/// <param name="execute">The action to execute.</param>
		/// <returns>The created <see cref="IDynamicCommandBuilder"/>.</returns>
		IDynamicCommandBuilder CreateFromAction(
			string name,
			Action execute
		);

		/// <summary>
		/// Creates a <see cref="IDynamicCommandBuilder"/> with the specified <paramref name="name"/>
		/// that will execute the specified <paramref name="execute"/> action with
		/// a parameter of type <typeparamref name="TParameter"/>.
		/// </summary>
		/// <param name="name">The command name.</param>
		/// <param name="execute">The action to execute.</param>
		/// <returns>The created <see cref="IDynamicCommandBuilder"/>.</returns>
		IDynamicCommandBuilder CreateFromAction<TParameter>(
			string name,
			Action<TParameter> execute
		);

		/// <summary>
		/// Creates a <see cref="IDynamicCommandBuilder"/> with the specified <paramref name="name"/>
		/// that will execute the specified <paramref name="execute"/> task.
		/// </summary>
		/// <param name="name">The command name.</param>
		/// <param name="execute">The task to execute.</param>
		/// <returns>The created <see cref="IDynamicCommandBuilder"/>.</returns>
		IDynamicCommandBuilder CreateFromTask(
			string name,
			Func<CancellationToken, Task> execute
		);

		/// <summary>
		/// Creates a <see cref="IDynamicCommandBuilder"/> with the specified <paramref name="name"/>
		/// that will execute the specified <paramref name="execute"/> task with
		/// a parameter of type <typeparamref name="TParameter"/>.
		/// </summary>
		/// <param name="name">The command name.</param>
		/// <param name="execute">The task to execute.</param>
		/// <returns>The created <see cref="IDynamicCommandBuilder"/>.</returns>
		IDynamicCommandBuilder CreateFromTask<TParameter>(
			string name,
			Func<CancellationToken, TParameter, Task> execute
		);
	}
}
