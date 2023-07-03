using System;

namespace Chinook.DynamicMvvm.Implementations
{
	/// <summary>
	/// This is an implementation of a <see cref="IDynamicProperty{T}"/> using an <see cref="IObservable{T}"/> that ensures <see cref="IDynamicProperty.ValueChanged"/> is raised on a background thread.
	/// </summary>
	/// <typeparam name="T">Type of value</typeparam>
	public class ValueChangedOnBackgroundTaskDynamicPropertyFromObservable<T> : ValueChangedOnBackgroundTaskDynamicProperty<T>
	{
		private readonly DynamicPropertyFromObservable<T>.DynamicPropertyObserver _propertyObserver;
		private readonly IDisposable _subscription;
		private bool _isDisposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="ValueChangedOnBackgroundTaskDynamicPropertyFromObservable{T}"/> class.
		/// </summary>
		/// <remarks>
		/// When setting <see cref="IDynamicProperty.Value"/> after being disposed, <see cref="ObjectDisposedException"/> will be thrown.
		/// </remarks>
		/// <param name="name">The name of the this property.</param>
		/// <param name="source">Source</param>
		/// <param name="viewModel">The <see cref="IViewModel"/> used to determine dispatcher access.</param>
		/// <param name="initialValue">The initial value of this property.</param>
		public ValueChangedOnBackgroundTaskDynamicPropertyFromObservable(string name, IObservable<T> source, IViewModel viewModel, T initialValue = default)
			: base(name, viewModel, initialValue)
		{
			if (source is null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			_propertyObserver = new DynamicPropertyFromObservable<T>.DynamicPropertyObserver(this);
			_subscription = source.Subscribe(_propertyObserver);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValueChangedOnBackgroundTaskDynamicPropertyFromObservable{T}"/> class.
		/// </summary>
		/// <param name="name">The name of the this property.</param>
		/// <param name="source">Source</param>
		/// <param name="viewModel">The <see cref="IViewModel"/> used to determine dispatcher access.</param>
		/// <param name="initialValue">The initial value of this property.</param>
		/// <param name="throwOnDisposed">Whether a <see cref="ObjectDisposedException"/> should be thrown when <see cref="IDynamicProperty.Value"/> is changed after being disposed.</param>
		public ValueChangedOnBackgroundTaskDynamicPropertyFromObservable(string name, IObservable<T> source, IViewModel viewModel, bool throwOnDisposed, T initialValue = default)
			: base(name, viewModel, throwOnDisposed, initialValue)
		{
			if (source is null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			_propertyObserver = new DynamicPropertyFromObservable<T>.DynamicPropertyObserver(this);
			_subscription = source.Subscribe(_propertyObserver);
		}

		/// <inheritdoc />
		protected override void Dispose(bool isDisposing)
		{
			if (_isDisposed)
			{
				return;
			}

			if (isDisposing && _subscription != null)
			{
				_subscription.Dispose();
			}

			_isDisposed = true;

			base.Dispose(isDisposing);
		}
	}
}
