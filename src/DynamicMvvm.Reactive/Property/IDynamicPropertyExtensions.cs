using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// Extensions on <see cref="IDynamicProperty"/>.
	/// </summary>
	public static class IDynamicPropertyExtensions
    {
		/// <summary>
		/// Observes the value of the <paramref name="property"/>.
		/// </summary>
		/// <param name="property"><see cref="IDynamicProperty"/> to observe</param>
		/// <returns>Observable sequence of the values of the property.</returns>
		public static IObservable<object> Observe(this IDynamicProperty property)
		{
			return Observable.FromEvent<DynamicPropertyChangedEventHandler, IDynamicProperty>(
				h => property.ValueChanged += h,
				h => property.ValueChanged -= h
			)
			.Select(p => p.Value);
		}

		/// <summary>
		/// Gets and observes the value of the <paramref name="property"/>.
		/// </summary>
		/// <param name="property"><see cref="IDynamicProperty"/> to observe</param>
		/// <returns>Observable sequence of the values of the property. The sequence will start with the current value of the property.</returns>
		public static IObservable<object> GetAndObserve(this IDynamicProperty property)
		{
			return Observe(property).StartWith(property.Value);
		}

		/// <summary>
		/// Observes the value of the <paramref name="property"/>.
		/// </summary>
		/// <param name="property"><see cref="IDynamicProperty"/> to observe</param>
		/// <returns>Observable sequence of the values of the property.</returns>
		public static IObservable<T> Observe<T>(this IDynamicProperty property)
		{
			return Observe(property).Select(value => (T)value);
		}

		/// <summary>
		/// Gets and observes the value of the <paramref name="property"/>.
		/// </summary>
		/// <param name="property"><see cref="IDynamicProperty"/> to observe</param>
		/// <returns>Observable sequence of the values of the property. The sequence will start with the current value of the property.</returns>
		public static IObservable<T> GetAndObserve<T>(this IDynamicProperty property)
		{
			return GetAndObserve(property).Select(value => (T)value);
		}

		/// <summary>
		/// Observes the value of the <paramref name="property"/>.
		/// </summary>
		/// <param name="property"><see cref="IDynamicProperty"/> to observe</param>
		/// <returns>Observable sequence of the values of the property.</returns>
		public static IObservable<T> Observe<T>(this IDynamicProperty<T> property)
		{
			return ((IDynamicProperty)property).Observe<T>();
		}

		/// <summary>
		/// Gets and observes the value of the <paramref name="property"/>.
		/// </summary>
		/// <param name="property"><see cref="IDynamicProperty"/> to observe</param>
		/// <returns>Observable sequence of the values of the property. The sequence will start with the current value of the property.</returns>
		public static IObservable<T> GetAndObserve<T>(this IDynamicProperty<T> property)
		{
			return Observe(property).StartWith(property.Value);
		}
	}
}
