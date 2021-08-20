using System;
using System.ComponentModel;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Chinook.DynamicMvvm
{
	public partial class ViewModelBase
	{
		/// <inheritdoc />
		public event PropertyChangedEventHandler PropertyChanged;

		/// <inheritdoc />
		public virtual void RaisePropertyChanged(string propertyName)
		{
			ThrowIfDisposed();

			if (_isDisposing)
			{
				_logger.LogDebug($"Skipped '{nameof(RaisePropertyChanged)}' for '{GetType().Name}.{propertyName}' on ViewModel '{Name}' because it's disposing.");
				return;
			}

			if (PropertyChanged == null)
			{
				return;
			}

			var viewModelView = GetView();

			if (viewModelView != null && !viewModelView.GetHasDispatcherAccess())
			{
				_ = viewModelView.ExecuteOnDispatcher(CancellationToken, () => RaisePropertyChangedInner(propertyName));
			}
			else
			{
				RaisePropertyChangedInner(propertyName);
			}
		}

		private void RaisePropertyChangedInner(string propertyName)
		{
			if (_isDisposing)
			{
				_logger.LogDebug($"Skipped '{nameof(RaisePropertyChangedInner)}' for '{GetType().Name}.{propertyName}' on ViewModel '{Name}' because it's disposing.");
				return;
			}

			if (_isDisposed)
			{
				_logger.LogDebug($"Skipped '{nameof(RaisePropertyChangedInner)}' for '{GetType().Name}.{propertyName}' on ViewModel '{Name}' because it's disposed.");
				return;
			}

			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

			_logger.LogDebug($"Raised property changed for '{propertyName}' from ViewModel '{Name}'.");
		}
	}
}
