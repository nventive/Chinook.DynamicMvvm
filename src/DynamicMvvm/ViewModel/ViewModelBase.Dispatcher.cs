using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Chinook.DynamicMvvm
{
	public partial class ViewModelBase
	{
		private IDispatcher _dispatcher;

		/// <inheritdoc />
		public IDispatcher Dispatcher
		{
			get => _dispatcher;
			set => SetDispatcher(value);
		}

		/// <inheritdoc />
		public event Action<IDispatcher> DispatcherChanged;

		private void SetDispatcher(IDispatcher dispatcher)
		{
			if (dispatcher != null)
			{
				// When the VM is disposed, we don't want to throw when setting a null dispatcher.
				ThrowIfDisposed();
			}

			_dispatcher = dispatcher;

			if (_isDisposing)
			{
				_logger.LogViewModelSkippedMethodBecauseDisposing(nameof(DispatcherChanged), Name);
				return;
			}

			DispatcherChanged?.Invoke(dispatcher);
		}
	}
}
