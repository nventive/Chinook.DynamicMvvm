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
		/// <remarks>
		/// When setting <see cref="IDynamicProperty.Value"/> after being disposed, <see cref="ObjectDisposedException"/> will be thrown.
		/// </remarks>
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

		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicPropertyFromObservable{T}"/> class.
		/// </summary>
		/// <param name="name">Name</param>
		/// <param name="source">Source</param>
		/// <param name="initialValue">Initial value</param>
		/// <param name="throwOnDisposed">Whether a <see cref="ObjectDisposedException"/> should be thrown when <see cref="IDynamicProperty.Value"/> is changed after being disposed.</param>
		[Obsolete("This constructor is obsolete. The throwOnDisposed parameter is no longer used.", error: false)]
		public DynamicPropertyFromObservable(string name, IObservable<T> source, bool throwOnDisposed, T initialValue = default)
			: this(name, source, initialValue)
		{
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
			private readonly IDynamicProperty<T> _owner;

			public DynamicPropertyObserver(IDynamicProperty<T> owner)
			{
				_owner = owner;
			}

			public void OnCompleted()
			{
			}

			public void OnError(Exception e)
			{
				this.Log().LogDynamicPropertySourceObservableSubscriptionFailed(_owner.Name, e);
			}

			public void OnNext(T value)
			{
				_owner.Value = value;
			}
		}
	}
}
