using System;
using System.Collections.Generic;
using System.Text;

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
		/// <param name="name">Name</param>
		/// <param name="value">Initial value</param>
		public DynamicProperty(string name, T value = default)
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
