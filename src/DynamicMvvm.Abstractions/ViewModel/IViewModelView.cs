﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// A <see cref="IViewModelView"/> represents the view contract of a <see cref="IViewModel"/>.
	/// </summary>
	public interface IViewModelView
	{
		/// <summary>
		/// Gets whether or not the thread has dispatcher access.
		/// </summary>
		bool GetHasDispatcherAccess();

		/// <summary>
		/// Occurs when the view is loaded.
		/// </summary>
		event EventHandler Loaded;

		/// <summary>
		/// Occurs when the view is unloaded.
		/// </summary>
		event EventHandler Unloaded;

		/// <summary>
		/// Executes the specified action on a dispatcher thread.
		/// </summary>
		/// <param name="action">The action to execute.</param>
		void ExecuteOnDispatcher(Action action);
	}
}
