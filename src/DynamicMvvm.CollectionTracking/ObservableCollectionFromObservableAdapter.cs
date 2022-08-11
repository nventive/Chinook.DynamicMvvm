using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using Chinook.DynamicMvvm;
using CollectionTracking;

namespace Chinook.DynamicMvvm.CollectionTracking
{
	/// <summary>
	/// This class adapts an observable of lists into a <see cref="ReadOnlyObservableCollection{T}"/> exposed via <see cref="ReadOnlyCollection"/> or <see cref="Collection"/>.
	/// It also ensures that <see cref="INotifyCollectionChanged.CollectionChanged"/> is raised using the provided <see cref="IViewModel.Dispatcher"/>.
	/// </summary>
	/// <typeparam name="T">The collection item type.</typeparam>
	public class ObservableCollectionFromObservableAdapter<T> : IDisposable
	{
		private readonly IViewModel _viewModel;
		private readonly IObservable<IEnumerable<T>> _source;
		private readonly CancellationTokenSource _cts = new CancellationTokenSource();
		private readonly IDisposable _subscription;
		private readonly object _referenceListMutex = new object();
		private readonly Queue<IEnumerable<CollectionOperation<T>>> _operationsQueue = new Queue<IEnumerable<CollectionOperation<T>>>();

		private IEnumerable<T> _referenceList;

		public ObservableCollectionFromObservableAdapter(IViewModel viewModel, IObservable<IEnumerable<T>> source, IEnumerable<T> initialValue)
		{
			Collection = new ObservableCollection<T>(initialValue);
			_referenceList = initialValue.ToList();
			_viewModel = viewModel;
			_source = source;

			ReadOnlyCollection = new ReadOnlyObservableCollection<T>(Collection);
			_subscription = _source.Subscribe(new BasicObserver(OnNext));

			Collection.CollectionChanged += OnCollectionChanged;
		}

		public ReadOnlyObservableCollection<T> ReadOnlyCollection { get; }

		public ObservableCollection<T> Collection { get; }

		private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			// We need to change the reference list to the latest one when we are done manipulating the Collection.
			// Otherwise, the reference list will keep giving the operation it needs to reach the final result we want, and it will loop infinitely, because it never changes.
			lock (_referenceListMutex)
			{
				// This ensures that the reference list only changes when the changes come from the manipulation of the Collection (e.g. ListView reorder) instead of the observable.
				if (_operationsQueue.Count == 0)
				{
					_referenceList = Collection.ToList();
				}
			}
		}

		private void OnNext(IEnumerable<T> list)
		{
			// Here, we add the operation to a queue to keep track of operations applied to the list.
			QueueUpdate(list);

			if (_viewModel.Dispatcher == null)
			{
				ApplyOperations();
			}
			else
			{
				_ = _viewModel.Dispatcher.ExecuteOnDispatcher(_cts.Token, ApplyOperations);
			}

			void ApplyOperations()
			{
				// Here, we Dequeue the operation after it was executed, so that the operation remains in the queue until it is applied.
				Collection.ApplyOperations(_operationsQueue.Peek());
				_operationsQueue.Dequeue();
			}
		}

		/// <summary>
		/// Atomically updates <see cref="_referenceList"/> and returns the operations.
		/// </summary>
		/// <remarks>
		/// We need a separate list than <see cref="Collection"/> to compute the operations because <see cref="Collection"/> is modified on a separate thread most of the time.
		/// It's important because when updates happen really fast, comparing with <see cref="Collection"/> (and not <see cref="_referenceList"/>) would end up applying the same operations multiple times, causing inconsistencies.
		/// </remarks>
		/// <param name="list">The new target list.</param>
		private void QueueUpdate(IEnumerable<T> list)
		{
			lock (_referenceListMutex)
			{
				var operations = _referenceList.GetOperations(list);
				_referenceList = list.ToList();

				_operationsQueue.Enqueue(operations);
			}
		}

		public void Dispose()
		{
			_subscription.Dispose();
			_cts.Cancel();
			_cts.Dispose();
			Collection.CollectionChanged -= OnCollectionChanged;
		}

		private class BasicObserver : IObserver<IEnumerable<T>>
		{
			private readonly Action<IEnumerable<T>> _onNext;

			public BasicObserver(Action<IEnumerable<T>> onNext)
			{
				_onNext = onNext;
			}
			public void OnCompleted()
			{
			}

			public void OnError(Exception error)
			{
			}

			public void OnNext(IEnumerable<T> value)
			{
				_onNext(value);
			}
		}
	}
}
