using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// Extensions on <see cref="IDynamicProperty"/> to observe its value.
	/// </summary>
	public static class IDynamicPropertyExtensions
	{
		/// <summary>
		/// Observes the value of the <paramref name="property"/> and optionaly starts the sequence with the current value.
		/// </summary>
		/// <typeparam name="T">The type of the values.</typeparam>
		/// <param name="property">The property to observe.</param>
		/// <param name="startWithCurrent">Whether the sequence should start with the current value of the property.</param>
		/// <returns>An observable sequence of the values of the property.</returns>
		private static IObservable<T> ObserveValue<T>(IDynamicProperty property, bool startWithCurrent)
		{
			return Observable.Create<T>(Subscribe);

			IDisposable Subscribe(IObserver<T> observer)
			{
				if (startWithCurrent)
				{
					observer.OnNext((T)property.Value);
				}

				property.ValueChanged += OnPropertyValueChanged;

				return Disposable.Create(() => property.ValueChanged -= OnPropertyValueChanged);

				void OnPropertyValueChanged(IDynamicProperty p)
				{
					observer.OnNext((T)p.Value);
				}
			}
		}

		/// <summary>
		/// Observes the value of the <paramref name="property"/>.
		/// </summary>
		/// <param name="property"><see cref="IDynamicProperty"/> to observe.</param>
		/// <returns>Observable sequence of the values of the property.</returns>
		public static IObservable<object> Observe(this IDynamicProperty property)
		{
			return ObserveValue<object>(property, startWithCurrent: false);
		}

		/// <summary>
		/// Gets and observes the value of the <paramref name="property"/>.
		/// </summary>
		/// <param name="property"><see cref="IDynamicProperty"/> to observe.</param>
		/// <returns>Observable sequence of the values of the property. The sequence will start with the current value of the property.</returns>
		public static IObservable<object> GetAndObserve(this IDynamicProperty property)
		{
			return ObserveValue<object>(property, startWithCurrent: true);
		}

		/// <summary>
		/// Observes the value of the <paramref name="property"/>.
		/// </summary>
		/// <typeparam name="T">The property type.</typeparam>
		/// <param name="property"><see cref="IDynamicProperty"/> to observe.</param>
		/// <returns>Observable sequence of the values of the property.</returns>
		public static IObservable<T> Observe<T>(this IDynamicProperty property)
		{
			return ObserveValue<T>(property, startWithCurrent: false);
		}

		/// <summary>
		/// Gets and observes the value of the <paramref name="property"/>.
		/// </summary>
		/// <typeparam name="T">The property type.</typeparam>
		/// <param name="property"><see cref="IDynamicProperty"/> to observe.</param>
		/// <returns>Observable sequence of the values of the property. The sequence will start with the current value of the property.</returns>
		public static IObservable<T> GetAndObserve<T>(this IDynamicProperty property)
		{
			return ObserveValue<T>(property, startWithCurrent: true);
		}

		/// <summary>
		/// Observes the value of the <paramref name="property"/>.
		/// </summary>
		/// <typeparam name="T">The property type.</typeparam>
		/// <param name="property"><see cref="IDynamicProperty"/> to observe.</param>
		/// <returns>Observable sequence of the values of the property.</returns>
		public static IObservable<T> Observe<T>(this IDynamicProperty<T> property)
		{
			return ((IDynamicProperty)property).Observe<T>();
		}

		/// <summary>
		/// Gets and observes the value of the <paramref name="property"/>.
		/// </summary>
		/// <typeparam name="T">The property type.</typeparam>
		/// <param name="property"><see cref="IDynamicProperty"/> to observe.</param>
		/// <returns>Observable sequence of the values of the property. The sequence will start with the current value of the property.</returns>
		public static IObservable<T> GetAndObserve<T>(this IDynamicProperty<T> property)
		{
			return ObserveValue<T>(property, startWithCurrent: true);
		}
	}
}
