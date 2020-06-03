using System;
using System.Text;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// Extensions on <see cref="IDynamicCommand"/>.
	/// </summary>
	public static class IDynamicCommandExtensions
	{
		/// <summary>
		/// Observes if the <see cref="IDynamicCommand"/> is currently executing.
		/// </summary>
		/// <param name="command"><see cref="IDynamicCommand"/></param>
		/// <returns>Observable of boolean</returns>
		public static IObservable<bool> ObserveIsExecuting(this IDynamicCommand command)
		{
			return Observable.FromEventPattern<EventHandler, EventArgs>(
				h => command.IsExecutingChanged += h,
				h => command.IsExecutingChanged -= h
			)
			.Select(p => command.IsExecuting);
		}

		/// <summary>
		/// Gets and observes if the <see cref="IDynamicCommand"/> is currently executing.
		/// </summary>
		/// <param name="command"><see cref="IDynamicCommand"/></param>
		/// <returns>Observable of boolean</returns>
		public static IObservable<bool> GetAndObserveIsExecuting(this IDynamicCommand command)
		{
			return ObserveIsExecuting(command).StartWith(command.IsExecuting);
		}
	}
}
