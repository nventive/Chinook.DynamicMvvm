using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// Extensions on <see cref="IViewModel"/> to create <see cref="IDynamicProperty"/>.
	/// </summary>
	public static partial class IViewModelExtensions
	{
		/// <summary>
		/// Gets the value of a <see cref="IDynamicProperty"/>.
		/// </summary>
		/// <typeparam name="T">Property type</typeparam>
		/// <param name="viewModel"><see cref="IViewModel"/></param>
		/// <param name="property">Property</param>
		/// <returns>Property value</returns>
		public static T Get<T>(
			this IViewModel viewModel,
			IDynamicProperty property
		) => (T)property.Value;

		/// <summary>
		/// Gets or creates a <see cref="IDynamicProperty"/> attached to this <see cref="IViewModel"/>.
		/// </summary>
		/// <typeparam name="T">Property type</typeparam>
		/// <param name="viewModel"><see cref="IViewModel"/></param>
		/// <param name="initialValue">Property initial value</param>
		/// <param name="name">Property name</param>
		/// <returns>Property value</returns>
		public static T Get<T>(
			this IViewModel viewModel,
			T initialValue = default,
			[CallerMemberName] string name = null
		) => viewModel.Get<T>(viewModel.GetOrCreateDynamicProperty(name, n => viewModel.GetDynamicPropertyFactory().Create(n, initialValue)));

		/// <summary>
		/// Gets or creates a <see cref="IDynamicProperty"/> attached to this <see cref="IViewModel"/>.
		/// </summary>
		/// <typeparam name="T">Property type</typeparam>
		/// <param name="viewModel"><see cref="IViewModel"/></param>
		/// <param name="initialValue">Property initial value</param>
		/// <param name="name">Property name</param>
		/// <returns>Property value</returns>
		public static T Get<T>(
			this IViewModel viewModel,
			Func<T> initialValue,
			[CallerMemberName] string name = null
		) => viewModel.Get<T>(viewModel.GetOrCreateDynamicProperty(name, n => viewModel.GetDynamicPropertyFactory().Create(n, initialValue())));

		/// <summary>
		/// Gets or creates a <see cref="IDynamicProperty"/> attached to this <see cref="IViewModel"/>.
		/// </summary>
		/// <typeparam name="T">Property type</typeparam>
		/// <param name="viewModel"><see cref="IViewModel"/></param>
		/// <param name="source">Property source</param>
		/// <param name="initialValue">Property initial value</param>
		/// <param name="name">Property name</param>
		/// <returns>Property value</returns>
		public static T GetFromTask<T>(
			this IViewModel viewModel,
			Func<CancellationToken, Task<T>> source,
			T initialValue = default,
			[CallerMemberName] string name = null
		) => viewModel.Get<T>(viewModel.GetOrCreateDynamicProperty(name, n => viewModel.GetDynamicPropertyFactory().CreateFromTask(n, source, initialValue)));

		/// <summary>
		/// Gets or creates a <see cref="IDynamicProperty"/> attached to this <see cref="IViewModel"/>.
		/// </summary>
		/// <typeparam name="T">Property type</typeparam>
		/// <param name="viewModel"><see cref="IViewModel"/></param>
		/// <param name="source">Property source</param>
		/// <param name="initialValue">Property initial value</param>
		/// <param name="name">Property name</param>
		/// <returns>Property value</returns>
		public static T GetFromObservable<T>(
			this IViewModel viewModel,
			IObservable<T> source,
			T initialValue = default,
			[CallerMemberName] string name = null
		) => viewModel.Get<T>(viewModel.GetOrCreateDynamicProperty(name, n => viewModel.GetDynamicPropertyFactory().CreateFromObservable(n, source, initialValue)));

		/// <summary>
		/// Sets the value of a property.
		/// </summary>
		/// <typeparam name="T">Property type</typeparam>
		/// <param name="viewModel"><see cref="IViewModel"/></param>
		/// <param name="value">Value to set</param>
		/// <param name="property">Property to set</param>
		public static void Set<T>(
			this IViewModel viewModel,
			T value,
			IDynamicProperty property
		) => property.Value = value;

		/// <summary>
		/// Sets the value of a property.
		/// If the property doesn't exist, it creates it.
		/// </summary>
		/// <typeparam name="T">Property type</typeparam>
		/// <param name="viewModel"><see cref="IViewModel"/></param>
		/// <param name="value">Value to set</param>
		/// <param name="name">Property name</param>
		public static void Set<T>(this IViewModel viewModel, T value, [CallerMemberName] string name = null)
			=> viewModel.Set(value, viewModel.GetOrResolveProperty(name));

		/// <summary>
		/// Gets or creates a property of the specified <paramref name="name"/>.
		/// </summary>
		/// <param name="viewModel"><see cref="IViewModel"/></param>
		/// <param name="name">Property name</param>
		/// <returns><see cref="IDynamicProperty"/></returns>
		public static IDynamicProperty GetProperty(this IViewModel viewModel, string name)
			=> viewModel.GetOrResolveProperty(name);

		/// <summary>
		/// Gets or creates a property of the specified <paramref name="name"/>.
		/// </summary>
		/// <typeparam name="T">Property type</typeparam>
		/// <param name="viewModel"><see cref="IViewModel"/></param>
		/// <param name="name">Property name</param>
		/// <returns><see cref="IDynamicProperty"/></returns>
		public static IDynamicProperty<T> GetProperty<T>(this IViewModel viewModel, string name)
			=> (IDynamicProperty<T>)viewModel.GetOrResolveProperty(name);

		/// <summary>
		/// Gets or creates a <see cref="IDynamicProperty"/> attached to this <see cref="IViewModel"/>.
		/// </summary>
		/// <typeparam name="TViewModel">ViewModel type</typeparam>
		/// <typeparam name="TProperty">Property type</typeparam>
		/// <param name="viewModel"><see cref="IViewModel"/></param>
		/// <param name="expression">Property</param>
		/// <returns><see cref="IDynamicProperty"/></returns>
		public static IDynamicProperty<TProperty> GetProperty<TViewModel, TProperty>(this TViewModel viewModel, Expression<Func<TViewModel, TProperty>> expression)
			where TViewModel : IViewModel
		{
			var viewModelType = typeof(TViewModel);

			if (!(expression.Body is MemberExpression member))
			{
				throw new ArgumentException($"Expression '{expression}' refers to a method, not a property.");
			}

			var propInfo = member.Member as PropertyInfo;
			if (propInfo == null)
			{
				throw new ArgumentException($"Expression '{expression}' refers to a field, not a property.");
			}

			if (viewModelType != propInfo.ReflectedType && !viewModelType.IsSubclassOf(propInfo.ReflectedType))
			{
				throw new ArgumentException($"Expression '{expression}' refers to a property that is not from type '{viewModelType}'.");
			}

			return (IDynamicProperty<TProperty>)viewModel.GetOrResolveProperty(propInfo.Name);
		}

		/// <summary>
		/// Gets or creates a property of the specified <paramref name="name"/>.
		/// </summary>
		/// <param name="viewModel"><see cref="IViewModel"/></param>
		/// <param name="name">Property name</param>
		/// <param name="factory">Property factory</param>
		/// <returns>IDynamicProperty</returns>
		public static IDynamicProperty GetOrCreateDynamicProperty(this IViewModel viewModel, string name, Func<string, IDynamicProperty> factory)
		{
			if (!viewModel.TryGetDisposable<IDynamicProperty>(name, out var property))
			{
				property = factory(name);
				property.ValueChanged += OnDynamicPropertyChanged;

				viewModel.AddDisposable(name, property);
			}

			return property;

			void OnDynamicPropertyChanged(IDynamicProperty dynamicProperty)
			{
				viewModel.RaisePropertyChanged(dynamicProperty.Name);
			}
		}

		/// <summary>
		/// Gets or resolves a property of the specified <paramref name="name"/>.
		/// </summary>
		/// <param name="viewModel"><see cref="IViewModel"/></param>
		/// <param name="name">Property name</param>
		/// <returns>IDynamicProperty</returns>
		public static IDynamicProperty GetOrResolveProperty(this IViewModel viewModel, string name)
		{
			if (!viewModel.TryGetDisposable<IDynamicProperty>(name, out var property))
			{
				typeof(IViewModel).Log().LogWarning($"Resolving property {viewModel.Name}.{name} using reflection.");

				// This is a rare case where the property was resolved before being created.
				// We simply resolve it manually on the type.
				viewModel.GetType().GetProperty(name).GetValue(viewModel);

				// This will now set the property variable.
				viewModel.TryGetDisposable(name, out property);
			}

			return property;
		}

		/// <summary>
		/// Gets the <see cref="IDynamicPropertyFactory"/> from the <paramref name="viewModel"/>.
		/// </summary>
		/// <param name="viewModel"><see cref="IViewModel"/></param>
		/// <returns>IDynamicPropertyFactory</returns>
		public static IDynamicPropertyFactory GetDynamicPropertyFactory(this IViewModel viewModel)
			=> viewModel.ServiceProvider.GetRequiredService<IDynamicPropertyFactory>();
	}
}
