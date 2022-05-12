using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Chinook.DynamicMvvm.Deactivation
{
	/// <summary>
	/// This is an <see cref="IObservable{T}"/> than can be deactivated.
	/// When deactivated, this observable unsubscribes from its inner source.
	/// When reactivated, this observable re-subscribes to its inner source.
	/// </summary>
	/// <typeparam name="T">The type of data.</typeparam>
	public class DeactivatableObservable<T> : IObservable<T>, IDeactivatable, IDisposable
	{
		private readonly IObservable<T> _source;

		private IDisposable _subscription;
		private IObserver<T> _observer;
		private bool _isDisposingOrDisposed;

		/// <summary>
		/// Creates a new instance of <see cref="DeactivatableObservable{T}"/>.
		/// </summary>
		/// <param name="source">The source that will be subscribed to when <see cref="IsDeactivated"/> is false.</param>
		public DeactivatableObservable(IObservable<T> source)
		{
			_source = source ?? throw new ArgumentNullException(paramName: nameof(source));
		}

		/// <inheritdoc/>
		public bool IsDeactivated { get; private set; } = false;

		/// <inheritdoc/>
		public IDisposable Subscribe(IObserver<T> observer)
		{
			if (_observer != null)
			{
				// This is sufficient for our use cases. Supporting more would complexify the code a lot.
				throw new NotSupportedException("DeactivatableObservable only supports 1 subscription.");
			}

			_observer = observer;
			_subscription = _source.Subscribe(observer);
			return new CompositeDisposable(
				_subscription,
				Disposable.Create(() => _observer = null)
			);
		}

		/// <inheritdoc/>
		public void Deactivate()
		{
			if (IsDeactivated)
			{
				return;
			}

			_subscription?.Dispose();

			IsDeactivated = true;

			typeof(IDeactivatable).Log().LogDebug($"Deactivated observable of type '{typeof(T).Name}'.");
		}

		/// <inheritdoc/>
		public void Reactivate()
		{
			if (!IsDeactivated)
			{
				return;
			}

			IsDeactivated = false;

			if (_observer != null)
			{
				_subscription = _source.Subscribe(_observer);
			}

			typeof(IDeactivatable).Log().LogDebug($"Reactivated observable of type '{typeof(T).Name}'.");
		}

		/// <inheritdoc/>
		public void Dispose()
		{
			if (_isDisposingOrDisposed)
			{
				return;
			}

			_isDisposingOrDisposed = true;
			_subscription?.Dispose();
		}
	}
}
