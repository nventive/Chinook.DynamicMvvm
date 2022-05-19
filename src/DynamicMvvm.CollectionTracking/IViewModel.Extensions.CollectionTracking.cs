using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text;
using Chinook.DynamicMvvm.CollectionTracking;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// This class exposes extensions on <see cref="IViewModel"/> related to collection tracking.
	/// </summary>
	public static class CollectionTrackingViewModelExtensions
	{
		/// <summary>
		/// Gets a <see cref="ReadOnlyObservableCollection{T}"/> from an observable of list.
		/// </summary>
		/// <typeparam name="T">The collection item type.</typeparam>
		/// <param name="viewModel">The <see cref="IViewModel"/> owning the underlying adapter.</param>
		/// <param name="source">The observable source.</param>
		/// <param name="initialValue">The initial value for the collection.</param>
		/// <param name="name">The key for the disposable.</param>
		/// <returns>A <see cref="ReadOnlyObservableCollection{T}"/> updating every time <paramref name="source"/> changes.</returns>
		public static ReadOnlyObservableCollection<T> GetReadOnlyCollectionFromObservable<T>(this IViewModel viewModel, IObservable<IEnumerable<T>> source, IEnumerable<T> initialValue, [CallerMemberName] string name = null)
		{
			if (viewModel.IsDisposed)
			{
				return null;
			}

			var adapter = viewModel.GetOrCreateDisposable(name, () => new ObservableCollectionFromObservableAdapter<T>(viewModel, source, initialValue));
			return adapter.ReadOnlyCollection;
		}

		/// <summary>
		/// Gets a <see cref="ReadOnlyObservableCollection{T}"/> from an observable of list.
		/// </summary>
		/// <typeparam name="T">The collection item type.</typeparam>
		/// <param name="viewModel">The <see cref="IViewModel"/> owning the underlying adapter.</param>
		/// <param name="source">The observable source.</param>
		/// <param name="initialValue">The initial value for the collection.</param>
		/// <param name="name">The key for the disposable.</param>
		/// <returns>A <see cref="ObservableCollection{T}"/> updating every time <paramref name="source"/> changes.</returns>
		public static ObservableCollection<T> GetObservableCollectionFromObservable<T>(this IViewModel viewModel, IObservable<IEnumerable<T>> source, IEnumerable<T> initialValue, [CallerMemberName] string name = null)
		{
			if (viewModel.IsDisposed)
			{
				return null;
			}

			var adapter = viewModel.GetOrCreateDisposable(name, () => new ObservableCollectionFromObservableAdapter<T>(viewModel, source, initialValue));
			return adapter.Collection;
		}
	}
}
