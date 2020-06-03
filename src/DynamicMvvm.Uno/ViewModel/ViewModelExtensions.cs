using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// Extensions on <see cref="IViewModel"/> for Uno.
	/// </summary>
	public static class ViewModelExtensions
	{
		/// <summary>
		/// Attaches a <see cref="IViewModel"/> to a <paramref name="frameworkElement"/>.
		/// </summary>
		/// <param name="viewModel"><see cref="IViewModel"/></param>
		/// <param name="frameworkElement"><see cref="FrameworkElement"/></param>
		public static void AttachToView(this IViewModel viewModel, FrameworkElement frameworkElement)
		{
			viewModel.View = new ViewModelView(frameworkElement);
		}
	}
}
