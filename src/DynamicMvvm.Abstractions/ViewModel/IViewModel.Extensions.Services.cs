using System;
using Microsoft.Extensions.DependencyInjection;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// Extensions on <see cref="IViewModel"/> to resolve services.
	/// </summary>
	public static partial class IViewModelExtensions
	{
		/// <summary>
		/// Returns the registered service of the specified type <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">Type of service</typeparam>
		/// <param name="viewModel">ViewModel</param>
		/// <returns>Registered service</returns>
		public static T GetService<T>(this IViewModel viewModel)
		{
			return viewModel.ServiceProvider.GetRequiredService<T>();
		}

		/// <summary>
		/// Returns the registered service of the specified type <paramref name="type"/>.
		/// </summary>
		/// <param name="viewModel">ViewModel</param>
		/// <param name="type">Type of service</param>
		/// <returns>Registered service</returns>
		public static object GetService(this IViewModel viewModel, Type type)
		{
			return viewModel.ServiceProvider.GetRequiredService(type);
		}
	}
}
