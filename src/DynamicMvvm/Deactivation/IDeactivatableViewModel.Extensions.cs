using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Chinook.DynamicMvvm.Deactivation;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// This class exposes extensions methods on <see cref="IDeactivatableViewModel"/>.
	/// </summary>
	public static class DeactivatableViewModelExtensions
	{
		/// <summary>
		/// Gets or creates a <see cref="IDynamicProperty"/> attached to this <see cref="IViewModel"/>.<br/>
		/// The underlying <see cref="IDynamicProperty"/> implements <see cref="IDeactivatable"/> so the observation can be deactivated.
		/// </summary>
		/// <typeparam name="T">The property type.</typeparam>
		/// <param name="viewModel">The <see cref="IViewModel"/> owning the property.</param>
		/// <param name="source">The observable of values that feeds the property.</param>
		/// <param name="initialValue">The property's initial value.</param>
		/// <param name="name">The property's name.</param>
		/// <returns>The property's value.</returns>
		public static T GetFromDeactivatableObservable<T>(this IDeactivatableViewModel viewModel, IObservable<T> source, T initialValue = default, [CallerMemberName] string name = null)
		{
			return viewModel.Get<T>(viewModel.GetOrCreateDynamicProperty(name, n => new DeactivatableDynamicPropertyFromObservable<T>(name, source, viewModel, initialValue)));
		}
	}
}
