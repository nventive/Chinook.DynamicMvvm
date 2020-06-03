using System;
using System.Collections.Generic;
using System.Text;

namespace Chinook.DynamicMvvm
{
	public partial class ViewModelBase
	{
		private readonly WeakReference<IViewModelView> _view = new WeakReference<IViewModelView>(default);

		/// <inheritdoc />
		public IViewModelView View
		{
			get => GetView();
			set => SetView(value);
		}

		/// <inheritdoc />
		public event Action<IViewModelView> ViewChanged;

		private IViewModelView GetView()
		{
			return _view != null && _view.TryGetTarget(out var view)
				? view
				: (default);
		}

		private void SetView(IViewModelView view)
		{
			_view.SetTarget(view);

			ViewChanged?.Invoke(view);
		}
	}
}
