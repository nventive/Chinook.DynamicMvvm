using Windows.UI.Xaml;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// Extensions on <see cref="IViewModel"/> for Uno.
	/// </summary>
	public static class UnoViewModelExtensions
	{
		/// <summary>
		/// Attaches a <see cref="IViewModel"/> to a <paramref name="frameworkElement"/> by setting the <see cref="IViewModel.Dispatcher"/> using the <see cref="CoreDispatcherDispatcher"/> implementation.
		/// </summary>
		/// <param name="viewModel"><see cref="IViewModel"/></param>
		/// <param name="frameworkElement"><see cref="FrameworkElement"/></param>
		public static void AttachToView(this IViewModel viewModel, FrameworkElement frameworkElement)
		{
			viewModel.Dispatcher = new CoreDispatcherDispatcher(frameworkElement);
		}
	}
}
