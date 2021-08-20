using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// This is an implementation of a <see cref="IDynamicProperty{T}"/> using an <see cref="IObservable{T}"/>.
	/// </summary>
	/// <typeparam name="T">Type of value</typeparam>
	public class DynamicPropertyFromObservable<T> : DynamicProperty<T>
	{
		private readonly DynamicPropertyObserver _propertyObserver;
		private readonly IDisposable _subscription;
		private bool _isDisposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicPropertyFromObservable{T}"/> class.
		/// </summary>
		/// <param name="name">Name</param>
		/// <param name="source">Source</param>
		/// <param name="initialValue">Initial value</param>
		public DynamicPropertyFromObservable(string name, IObservable<T> source, T initialValue = default)
			: base(name, initialValue)
		{
			if (source is null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			_propertyObserver = new DynamicPropertyObserver(this);
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

		public class DynamicPropertyObserver : IObserver<T>
		{
			private readonly DynamicPropertyFromObservable<T> _owner;

			public DynamicPropertyObserver(DynamicPropertyFromObservable<T> owner)
			{
				_owner = owner;
			}

			public void OnCompleted()
			{
			}

			public void OnError(Exception e)
			{
				this.Log().LogError(e, $"Source observable subscription failed for property '{_owner.Name}'. The property will no longer be updated by the observable.");
			}

			public void OnNext(T value)
			{
				_owner.Value = value;
			}
		}
	}
}
