using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// A <see cref="IDynamicCommand"/> is a <see cref="ICommand"/> that will notify its subscribers when it is executing.
	/// It adds support of async execution using the <see cref="Execute(object)"/> method.
	/// </summary>
	public interface IDynamicCommand : ICommand, INotifyPropertyChanged, IDisposable
    {
		/// <summary>
		/// Gets the name of the command.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets whether or not the command is currently executing.
		/// </summary>
		bool IsExecuting { get; }

		/// <summary>
		/// Occurs when changes occur that affect whether or not the command is executing.
		/// </summary>
		event EventHandler IsExecutingChanged;

		/// <summary>
		/// Task based version of the <see cref="ICommand.Execute(object)"/> method.
		/// </summary>
		new Task Execute(object parameter);
	}
}
