using System;
using System.Collections.Generic;
using System.Text;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// A <see cref="IDynamicProperty"/> represents a property that will notify its subscribers when its value changes.
	/// It always has a value that can be accessed synchronously.
	/// </summary>
	public interface IDynamicProperty : IDisposable
	{
		/// <summary>
		/// The name of the property.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// The value of the property.
		/// </summary>
		object Value { get; set; }

		/// <summary>
		/// Occurs when the value changes.
		/// </summary>
		event DynamicPropertyChangedEventHandler ValueChanged;
	}

	/// <summary>
	/// A typed version of <see cref="IDynamicProperty"/>.
	/// </summary>
	/// <typeparam name="T">The type of value.</typeparam>
	public interface IDynamicProperty<T> : IDynamicProperty
	{
		/// <summary>
		/// The value of the property.
		/// </summary>
		new T Value { get; set; }
	}

	/// <summary>
	/// Occurs when the value of a <see cref="IDynamicProperty"/> changes.
	/// </summary>
	/// <param name="property"><see cref="IDynamicProperty"/></param>
	public delegate void DynamicPropertyChangedEventHandler(IDynamicProperty property);
}
