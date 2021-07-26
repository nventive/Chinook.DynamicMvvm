﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

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
			if (view != null)
			{
				// When the VM is disposed, we don't want to throw when setting a null view.
				ThrowIfDisposed();
			}

			_view.SetTarget(view);

			if (_isDisposing)
			{
				_logger.LogDebug($"Skipped invocation of '{nameof(ViewChanged)}' on ViewModel '{Name}' because it's disposing.");
				return;
			}

			ViewChanged?.Invoke(view);
		}
	}
}
