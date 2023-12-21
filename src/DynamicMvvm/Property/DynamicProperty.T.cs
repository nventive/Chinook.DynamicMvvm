using System;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// This is a default implementation of <see cref="IDynamicProperty{T}"/>.
	/// </summary>
	public class DynamicProperty<T> : DynamicProperty, IDynamicProperty<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicProperty{T}"/> class.
		/// </summary>
		/// <remarks>
		/// When setting <see cref="Value"/> after being disposed, <see cref="ObjectDisposedException"/> will be thrown.
		/// </remarks>
		/// <param name="name">Name</param>
		/// <param name="value">Initial value</param>
		public DynamicProperty(string name, T value = default)
			: base(name, value)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicProperty{T}"/> class.
		/// </summary>
		/// <param name="name">Name</param>
		/// <param name="value">Initial value</param>
		/// <param name="throwOnDisposed">Whether a <see cref="ObjectDisposedException"/> should be thrown when <see cref="Value"/> is changed after being disposed.</param>
		[Obsolete("This constructor is obsolete. The throwOnDisposed parameter is no longer used.", error: false)]
		public DynamicProperty(string name, bool throwOnDisposed, T value = default)
			: base(name, value)
		{
		}

		/// <inheritdoc />
		public new T Value
		{
			get => (T)((DynamicProperty)this).Value;
			set => ((DynamicProperty)this).Value = value;
		}
	}
}
