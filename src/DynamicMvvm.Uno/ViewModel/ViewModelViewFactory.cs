using Windows.UI.Xaml;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// This implementation of <see cref="IViewModelViewFactory"/> uses the <see cref="ViewModelView"/> implementation.
	/// </summary>
	[Preserve(AllMembers = true)]
	public class ViewModelViewFactory : IViewModelViewFactory
	{
		public IViewModelView Create(object view)
		{
			return new ViewModelView((FrameworkElement)view);
		}
	}
}
