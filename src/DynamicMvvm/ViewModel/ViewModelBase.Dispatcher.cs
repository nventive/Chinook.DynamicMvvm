using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Chinook.DynamicMvvm
{
	public partial class ViewModelBase
	{
		private readonly WeakReference<IDispatcher> _dispatcher = new WeakReference<IDispatcher>(default);

		/// <inheritdoc />
		public IDispatcher Dispatcher
		{
			get => GetDispatcher();
			set => SetDispatcher(value);
		}

		/// <inheritdoc />
		public event Action<IDispatcher> DispatcherChanged;

		private IDispatcher GetDispatcher()
		{
			return _dispatcher != null && _dispatcher.TryGetTarget(out var dispatcher)
				? dispatcher
				: (default);
		}

		private void SetDispatcher(IDispatcher dispatcher)
		{
			if (dispatcher != null)
			{
				// When the VM is disposed, we don't want to throw when setting a null dispatcher.
				ThrowIfDisposed();
			}

			_dispatcher.SetTarget(dispatcher);

			if (_isDisposing)
			{
				_logger.LogDebug($"Skipped invocation of '{nameof(DispatcherChanged)}' on ViewModel '{Name}' because it's disposing.");
				return;
			}

			DispatcherChanged?.Invoke(dispatcher);
		}
	}
}
