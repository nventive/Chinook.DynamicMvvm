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
				_logger.LogViewModelSkippedMethodBecauseDisposing_PropertyName(nameof(RaisePropertyChanged), GetType().Name, propertyName, Name);
				return;
			}

			if (PropertyChanged == null)
			{
				return;
			}

			if (Dispatcher != null && !Dispatcher.GetHasDispatcherAccess())
			{
				_ = Dispatcher.ExecuteOnDispatcher(CancellationToken, () => RaisePropertyChangedInner(propertyName));
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
				_logger.LogViewModelSkippedMethodBecauseDisposing_PropertyName(nameof(RaisePropertyChangedInner), GetType().Name, propertyName, Name);
				return;
			}

			if (_isDisposed)
			{
				_logger.LogViewModelSkippedMethodBecauseDisposed_PropertyName(nameof(RaisePropertyChangedInner), GetType().Name, propertyName, Name);
				return;
			}

			try
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			}
			catch (Exception exception) when (Dispatcher is null)
			{
				// Give some details and tips on how to fix the issue.
				_logger.LogViewModelFailedToRaisePropertyChangedWhenDispatcherIsNull(exception);
				throw;
			}

			_logger.LogViewModelRaisedPropertyChanged(propertyName, Name);
		}
	}
}
