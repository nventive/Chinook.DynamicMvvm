using System;
using System.Collections.Generic;
using System.Text;

namespace Chinook.DynamicMvvm
{
	public static class IDynamicPropertyExtensions
	{
		/// <summary>
		/// Subscribes to the changes in the value of the <paramref name="property"/> and invokes the
		/// <paramref name="onValueChanged"/> callback.
		/// </summary>
		/// <param name="property">The property to subscribe to.</param>
		/// <param name="onValueChanged">The callback.</param>
		/// <returns><see cref="IDisposable"/></returns>
		public static IDisposable Subscribe(this IDynamicProperty property, Action<IDynamicProperty> onValueChanged)
		{
			if (onValueChanged is null)
			{
				throw new ArgumentNullException(nameof(onValueChanged));
			}

			return new ValueChangedDisposable(property, () => onValueChanged.Invoke(property));
		}

		/// <summary>
		/// Subscribes to the changes in the value of the <paramref name="property"/> and invokes the
		/// <paramref name="onValueChanged"/> callback.
		/// </summary>
		/// <typeparam name="T">The type of the property.</typeparam>
		/// <param name="property">The property to subscribe to.</param>
		/// <param name="onValueChanged">The callback.</param>
		/// <returns><see cref="IDisposable"/></returns>
		public static IDisposable Subscribe<T>(this IDynamicProperty<T> property, Action<IDynamicProperty<T>> onValueChanged)
		{
			if (onValueChanged is null)
			{
				throw new ArgumentNullException(nameof(onValueChanged));
			}

			return new ValueChangedDisposable(property, () => onValueChanged.Invoke(property));
		}

		private class ValueChangedDisposable : IDisposable
		{
			private IDynamicProperty _property;
			private readonly Action _onValueChanged;

			public ValueChangedDisposable(IDynamicProperty property, Action onValueChanged)
			{
				_property = property ?? throw new ArgumentNullException(nameof(property));
				_onValueChanged = onValueChanged ?? throw new ArgumentNullException(nameof(onValueChanged));

				_property.ValueChanged += OnValueChanged;
			}

			private void OnValueChanged(IDynamicProperty property)
			{
				_onValueChanged.Invoke();
			}

			public void Dispose()
			{
				_property.ValueChanged -= OnValueChanged;
				_property = null;
			}
		}
	}
}
