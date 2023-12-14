using System;
using System.Collections.Generic;
using System.Text;
using Chinook.DynamicMvvm.Implementations;
using Microsoft.Extensions.Logging;

namespace Chinook.DynamicMvvm.Deactivation
{
	/// <summary>
	/// This is an implementation of a <see cref="IDynamicProperty{T}"/> using an <see cref="IObservable{T}"/> that
	/// ensures <see cref="IDynamicProperty.ValueChanged"/> is raised on a background thread and its observable source can be deactivated.
	/// </summary>
	/// <typeparam name="T">Type of value</typeparam>
	public class DeactivatableDynamicPropertyFromObservable<T> : ValueChangedOnBackgroundTaskDynamicProperty<T>, IDeactivatable
	{
		private readonly IObservable<T> _source;
		private readonly DynamicPropertyFromObservable<T>.DynamicPropertyObserver _propertyObserver;

		private IDisposable _subscription;
		private bool _isDisposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="DeactivatableDynamicPropertyFromObservable{T}"/> class.
		/// </summary>
		/// <param name="name">The name of the this property.</param>
		/// <param name="source">The observable source.</param>
		/// <param name="viewModel">The <see cref="IViewModel"/> used to determine dispatcher access.</param>
		/// <param name="initialValue">The initial value of this property.</param>
		public DeactivatableDynamicPropertyFromObservable(string name, IObservable<T> source, IViewModel viewModel, T initialValue = default)
			: base(name, viewModel, initialValue)
		{
			if (source is null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			_source = source;
			_propertyObserver = new DynamicPropertyFromObservable<T>.DynamicPropertyObserver(this);
			_subscription = source.Subscribe(_propertyObserver);
		}

		/// <inheritdoc/>
		public bool IsDeactivated { get; private set; } = false;

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

		/// <inheritdoc/>
		public void Deactivate()
		{
			if (IsDeactivated)
			{
				return;
			}

			_subscription.Dispose();

			IsDeactivated = true;

			typeof(IDeactivatable).Log().LogDeactivatedObservableSource(Name);
		}

		/// <inheritdoc/>
		public void Reactivate()
		{
			if (!IsDeactivated)
			{
				return;
			}

			IsDeactivated = false;

			_subscription = _source.Subscribe(_propertyObserver);

			typeof(IDeactivatable).Log().LogReactivatedObservableSource(Name);
		}
	}
}
