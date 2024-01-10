﻿using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

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
		/// <typeparam name="T">The property type.</typeparam>
		/// <param name="viewModel">The <see cref="IViewModel"/> owning the property.</param>
		/// <param name="property">The property.</param>
		/// <returns>The property's value. Default of <typeparamref name="T"/> is returned if the <see cref="IViewModel"/> is disposed.</returns>
		public static T Get<T>(
			this IViewModel viewModel,
			IDynamicProperty property)
		{
			if (viewModel.IsDisposed)
			{
				return default(T);
			}

			return (T)property?.Value;
		}

		/// <summary>
		/// Gets the value of a <see cref="IDynamicProperty{T}"/>.
		/// </summary>
		/// <typeparam name="T">The property type.</typeparam>
		/// <param name="viewModel">The <see cref="IViewModel"/> owning the property.</param>
		/// <param name="property">The property.</param>
		/// <returns>The property's value. Default of <typeparamref name="T"/> is returned if the <see cref="IViewModel"/> is disposed or if <paramref name="property"/> is null.</returns>
		public static T Get<T>(
			this IViewModel viewModel,
			IDynamicProperty<T> property)
		{
			if (viewModel.IsDisposed || property == null)
			{
				return default(T);
			}

			return property.Value;
		}

		/// <summary>
		/// Gets or creates a <see cref="IDynamicProperty"/> attached to this <see cref="IViewModel"/>.
		/// </summary>
		/// <typeparam name="T">The property type.</typeparam>
		/// <param name="viewModel">The <see cref="IViewModel"/> owning the property.</param>
		/// <param name="initialValue">The property's initial value.</param>
		/// <param name="name">The property's name.</param>
		/// <returns>The property's value.</returns>
		public static T Get<T>(
			this IViewModel viewModel,
			T initialValue = default,
			[CallerMemberName] string name = null
		)
		{
			// We don't use GetOrCreateDynamicProperty internally to avoid the performance costs of the lambda and closure.
			if (viewModel.IsDisposed)
			{
				return default(T);
			}

			if (viewModel.TryGetDisposable<IDynamicProperty>(name, out var property))
			{
				return viewModel.Get<T>(property);
			}
			else
			{
				property = AddDynamicPropertyFromValue(viewModel, initialValue, name);

				return (T)property.Value;
			}
		}

		private static IDynamicProperty AddDynamicPropertyFromValue<T>(IViewModel viewModel, T initialValue, string name)
		{
			var property = viewModel.GetDynamicPropertyFactory().Create(name, initialValue, viewModel);
			property.ValueChanged += OnDynamicPropertyChanged;

			viewModel.AddDisposable(name, property);

			return property;

			void OnDynamicPropertyChanged(IDynamicProperty dynamicProperty)
			{
				viewModel.RaisePropertyChanged(dynamicProperty.Name);
			}
		}

		/// <summary>
		/// Gets or creates a <see cref="IDynamicProperty"/> attached to this <see cref="IViewModel"/>.
		/// </summary>
		/// <typeparam name="T">The property type.</typeparam>
		/// <param name="viewModel">The <see cref="IViewModel"/> owning the property.</param>
		/// <param name="initialValue">The property's initial value.</param>
		/// <param name="name">The property's name.</param>
		/// <returns>The property's value.</returns>
		public static T Get<T>(
			this IViewModel viewModel,
			Func<T> initialValue,
			[CallerMemberName] string name = null
		)
		{
			// We don't use GetOrCreateDynamicProperty internally to avoid the performance costs of the lambda and closure.
			if (viewModel.IsDisposed)
			{
				return default(T);
			}

			if (viewModel.TryGetDisposable<IDynamicProperty>(name, out var property))
			{
				return viewModel.Get<T>(property);
			}
			else
			{
				property = AddDynamicPropertyFromValue(viewModel, initialValue, name);

				return (T)property.Value;
			}
		}

		private static IDynamicProperty AddDynamicPropertyFromValue<T>(IViewModel viewModel, Func<T> initialValue, string name)
		{
			var property = viewModel.GetDynamicPropertyFactory().Create(name, initialValue(), viewModel);
			property.ValueChanged += OnDynamicPropertyChanged;

			viewModel.AddDisposable(name, property);

			return property;

			void OnDynamicPropertyChanged(IDynamicProperty dynamicProperty)
			{
				viewModel.RaisePropertyChanged(dynamicProperty.Name);
			}
		}

		/// <summary>
		/// Gets or creates a <see cref="IDynamicProperty"/> attached to this <see cref="IViewModel"/>.
		/// </summary>
		/// <typeparam name="T">The property type.</typeparam>
		/// <param name="viewModel">The <see cref="IViewModel"/> owning the property.</param>
		/// <param name="source">The asynchronous value source of the property.</param>
		/// <param name="initialValue">The property's initial value.</param>
		/// <param name="name">The property's name.</param>
		/// <returns>The property's value.</returns>
		public static T GetFromTask<T>(
			this IViewModel viewModel,
			Func<CancellationToken, Task<T>> source,
			T initialValue = default,
			[CallerMemberName] string name = null
		)
		{
			// We don't use GetOrCreateDynamicProperty internally to avoid the performance costs of the lambda and closure.
			if (viewModel.IsDisposed)
			{
				return default(T);
			}

			if (viewModel.TryGetDisposable<IDynamicProperty>(name, out var property))
			{
				return viewModel.Get<T>(property);
			}
			else
			{
				property = AddDynamicPropertyTask(viewModel, source, initialValue, name);

				return (T)property.Value;
			}
		}

		private static IDynamicProperty AddDynamicPropertyTask<T>(IViewModel viewModel, Func<CancellationToken, Task<T>> source, T initialValue, string name)
		{
			var property = viewModel.GetDynamicPropertyFactory().CreateFromTask(name, source, initialValue, viewModel);
			property.ValueChanged += OnDynamicPropertyChanged;

			viewModel.AddDisposable(name, property);

			return property;

			void OnDynamicPropertyChanged(IDynamicProperty dynamicProperty)
			{
				viewModel.RaisePropertyChanged(dynamicProperty.Name);
			}
		}

		/// <summary>
		/// Gets or creates a <see cref="IDynamicProperty"/> attached to this <see cref="IViewModel"/>.
		/// </summary>
		/// <typeparam name="T">The property type.</typeparam>
		/// <param name="viewModel">The <see cref="IViewModel"/> owning the property.</param>
		/// <param name="source">The observable of values that feeds the property.</param>
		/// <param name="initialValue">The property's initial value.</param>
		/// <param name="name">The property's name.</param>
		/// <returns>The property's value.</returns>
		public static T GetFromObservable<T>(
			this IViewModel viewModel,
			IObservable<T> source,
			T initialValue = default,
			[CallerMemberName] string name = null
		)
		{
			// We don't use GetOrCreateDynamicProperty internally to avoid the performance costs of the lambda and closure.
			if (viewModel.IsDisposed)
			{
				return default(T);
			}

			if (viewModel.TryGetDisposable<IDynamicProperty>(name, out var property))
			{
				return viewModel.Get<T>(property);
			}
			else
			{
				property = AddDynamicPropertyFromObservable(viewModel, source, initialValue, name);

				return (T)property.Value;
			}
		}

		private static IDynamicProperty AddDynamicPropertyFromObservable<T>(IViewModel viewModel, IObservable<T> source, T initialValue, string name)
		{
			var property = viewModel.GetDynamicPropertyFactory().CreateFromObservable(name, source, initialValue, viewModel);
			property.ValueChanged += OnDynamicPropertyChanged;

			viewModel.AddDisposable(name, property);

			return property;

			void OnDynamicPropertyChanged(IDynamicProperty dynamicProperty)
			{
				viewModel.RaisePropertyChanged(dynamicProperty.Name);
			}
		}

		/// <summary>
		/// Sets the value of a property.
		/// </summary>
		/// <remarks>
		/// Nothing happens if the <see cref="IViewModel"/> is disposed.
		/// </remarks>
		/// <typeparam name="T">The property type.</typeparam>
		/// <param name="viewModel">The <see cref="IViewModel"/> owning the property.</param>
		/// <param name="value">The value to set.</param>
		/// <param name="property">The property to set.</param>
		public static void Set<T>(
			this IViewModel viewModel,
			T value,
			IDynamicProperty property)
		{
			if (viewModel.IsDisposed)
			{
				return;
			}

			property.Value = value;
		}

		/// <summary>
		/// Sets the value of a property.
		/// If the property doesn't exist, it creates it.
		/// </summary>
		/// <typeparam name="T">The property type.</typeparam>
		/// <param name="viewModel">The <see cref="IViewModel"/> owning the property.</param>
		/// <param name="value">The value to set.</param>
		/// <param name="name">The property's name.</param>
		public static void Set<T>(this IViewModel viewModel, T value, [CallerMemberName] string name = null)
			=> viewModel.Set(value, viewModel.GetOrResolveProperty(name));

		/// <summary>
		/// Gets or creates a property of the specified <paramref name="name"/>.
		/// </summary>
		/// <param name="viewModel">The <see cref="IViewModel"/> owning the property.</param>
		/// <param name="name">The property's name.</param>
		/// <returns>The <see cref="IDynamicProperty"/> with the specified <paramref name="name"/>.</returns>
		public static IDynamicProperty GetProperty(this IViewModel viewModel, string name)
			=> viewModel.GetOrResolveProperty(name);

		/// <summary>
		/// Gets or creates a property of the specified <paramref name="name"/>.
		/// </summary>
		/// <typeparam name="T">The property type.</typeparam>
		/// <param name="viewModel">The <see cref="IViewModel"/> owning the property.</param>
		/// <param name="name">The property's name.</param>
		/// <returns>The <see cref="IDynamicProperty{T}"/> with the specified <paramref name="name"/>.</returns>
		public static IDynamicProperty<T> GetProperty<T>(this IViewModel viewModel, string name)
			=> (IDynamicProperty<T>)viewModel.GetOrResolveProperty(name);

		/// <summary>
		/// Gets or creates a <see cref="IDynamicProperty"/> attached to this <see cref="IViewModel"/>.
		/// </summary>
		/// <typeparam name="TViewModel">The ViewModel type.</typeparam>
		/// <typeparam name="TProperty">The property type.</typeparam>
		/// <param name="viewModel">The <see cref="IViewModel"/> owning the property.</param>
		/// <param name="expression">The expression resolving the property.</param>
		/// <returns>The <see cref="IDynamicProperty"/> obtained via the <paramref name="expression"/>.</returns>
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
		/// <param name="viewModel">The <see cref="IViewModel"/> owning the property.</param>
		/// <param name="name">The property's name.</param>
		/// <param name="factory">The property factory.</param>
		/// <returns>The <see cref="IDynamicProperty"/> matching the specified <paramref name="name"/>. Null is returned if the <see cref="IViewModel"/> is disposed.</returns>
		public static IDynamicProperty GetOrCreateDynamicProperty(this IViewModel viewModel, string name, Func<string, IDynamicProperty> factory)
		{
			if (viewModel.IsDisposed)
			{
				return null;
			}

			if (!viewModel.TryGetDisposable<IDynamicProperty>(name, out var property))
			{
				property = factory(name);
				property.ValueChanged += OnDynamicPropertyChanged;

				// We check the same condition twice because it's possible that the factory invocation already added the property.
				// This can happen when the property implements IViewModel and was added using AttachChild.
				if (!viewModel.TryGetDisposable<IDynamicProperty>(name, out var _))
				{
					viewModel.AddDisposable(name, property);
				}
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
		/// <param name="viewModel">The <see cref="IViewModel"/> owning the property.</param>
		/// <param name="name">The property's name.</param>
		/// <returns>The <see cref="IDynamicProperty"/> matching the specified <paramref name="name"/>. Null is returned if the <see cref="IViewModel"/> is disposed.</returns>
		public static IDynamicProperty GetOrResolveProperty(this IViewModel viewModel, string name)
		{
			if (viewModel.IsDisposed)
			{
				return null;
			}

			if (!viewModel.TryGetDisposable<IDynamicProperty>(name, out var property))
			{
				typeof(IViewModel).Log().LogViewModelResolvingPropertyUsingReflection(viewModel.GetType().Name, name, viewModel.Name);

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
		/// <param name="viewModel">The <see cref="IViewModel"/> providing the factory.</param>
		/// <returns>The <see cref="IDynamicPropertyFactory"/>.</returns>
		public static IDynamicPropertyFactory GetDynamicPropertyFactory(this IViewModel viewModel)
			=> viewModel.ServiceProvider.GetRequiredService<IDynamicPropertyFactory>();
	}
}
