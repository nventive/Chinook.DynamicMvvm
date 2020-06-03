using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// A <see cref="IDynamicCommandStrategy"/> is an execution delegate for a <see cref="IDynamicCommand"/>.
	/// It can be used to delegate the execution of the <see cref="IDynamicCommand"/> to different implementations (e.g. action, task, etc.)
	/// </summary>
	public interface IDynamicCommandStrategy : IDisposable
	{
		/// <summary>
		/// Determines if the strategy can be executed. 
		/// </summary>
		/// <param name="parameter">Command parameter</param>
		/// <param name="command">Calling command</param>
		/// <returns>True if the command can be executed; false otherwise.</returns>
		bool CanExecute(object parameter, IDynamicCommand command);

		/// <summary>
		/// Executes the strategy.
		/// </summary>
		/// <param name="ct">Cancellation token</param>
		/// <param name="parameter">Command parameter</param>
		/// <param name="command">Calling command</param>
		/// <returns>Task</returns>
		Task Execute(CancellationToken ct, object parameter, IDynamicCommand command);

		/// <summary>
		/// Occurs when changes occur that affect whether or not the command should execute.
		/// </summary>
		event EventHandler CanExecuteChanged;
	}
}
