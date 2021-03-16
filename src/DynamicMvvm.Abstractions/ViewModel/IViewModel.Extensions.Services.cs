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
		/// <typeparam name="T">The desired type of service.</typeparam>
		/// <param name="viewModel">The <see cref="IViewModel"/> providing the <see cref="IServiceProvider"/>.</param>
		/// <returns>The registered service.</returns>
		public static T GetService<T>(this IViewModel viewModel)
		{
			return viewModel.ServiceProvider.GetRequiredService<T>();
		}

		/// <summary>
		/// Returns the registered service of the specified  <paramref name="type"/>.
		/// </summary>
		/// <param name="viewModel">The <see cref="IViewModel"/> providing the <see cref="IServiceProvider"/>.</param>
		/// <param name="type">The desired type of service.</param>
		/// <returns>The registered service.</returns>
		public static object GetService(this IViewModel viewModel, Type type)
		{
			return viewModel.ServiceProvider.GetRequiredService(type);
		}
	}
}
