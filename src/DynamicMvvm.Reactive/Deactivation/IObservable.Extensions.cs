using System;
using System.Collections.Generic;
using System.Text;
using Chinook.DynamicMvvm.Deactivation;

namespace System.Reactive.Linq
{
	/// <summary>
	/// This class exposes extensions methods on <see cref="IObservable{T}"/> in the context of <see cref="Chinook.DynamicMvvm.Deactivation"/>.
	/// </summary>
	public static class ChinookDynamicMvvmDeactivationObservableExtensions
	{
		/// <summary>
		/// Automatically disconnects <paramref name="source"/> when <paramref name="viewModel"/> deactivates.<br/>
		/// Automatically reconnects <paramref name="source"/> when <paramref name="viewModel"/> reactivates.
		/// </summary>
		/// <typeparam name="T">The type of data.</typeparam>
		/// <param name="source">The observable source.</param>
		/// <param name="viewModel">The view model controlling the deactivation and reactivation of the observable.</param>
		/// <returns>
		/// An observable sequence that automatically disconnects and reconnects based on the state of <paramref name="viewModel"/>.
		/// </returns>
		public static IObservable<T> DeactivateWith<T>(this IObservable<T> source, IDeactivatableViewModel viewModel)
		{
			var deactivatableObservable = new DeactivatableObservable<T>(source);
			viewModel.AddDisposable(Guid.NewGuid().ToString(), deactivatableObservable);
			return deactivatableObservable;
		}
	}
}
