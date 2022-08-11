using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// An <see cref="IDispatcher"/> allows to execute code on a specific thread.
	/// This is useful when the <see cref="INotifyPropertyChanged.PropertyChanged"/> event needs to be raised on a specific thread.
	/// </summary>
	public interface IDispatcher
	{
		/// <summary>
		/// Gets whether or not the thread has dispatcher access.
		/// </summary>
		bool GetHasDispatcherAccess();

		/// <summary>
		/// Executes the specified action on a dispatcher thread.
		/// </summary>
		/// <param name="ct">The cancellation token.</param>
		/// <param name="action">The action to execute.</param>
		Task ExecuteOnDispatcher(CancellationToken ct, Action action);
	}
}
