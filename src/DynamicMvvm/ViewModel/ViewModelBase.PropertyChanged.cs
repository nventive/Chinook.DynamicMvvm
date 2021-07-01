using System;
using System.ComponentModel;
using Microsoft.Extensions.Logging;

namespace Chinook.DynamicMvvm
{
	public partial class ViewModelBase
	{
		/// <inheritdoc />
		public event PropertyChangedEventHandler PropertyChanged;

		/// <inheritdoc />
		public void RaisePropertyChanged(string propertyName)
		{
			ThrowIfDisposed();

			if (PropertyChanged == null)
			{
				return;
			}

			var viewModelView = GetView();

			if (viewModelView != null && !viewModelView.GetHasDispatcherAccess())
			{
				viewModelView.ExecuteOnDispatcher(() => RaisePropertyChangedInner(propertyName));
			}
			else
			{
				RaisePropertyChangedInner(propertyName);
			}
		}

		private void RaisePropertyChangedInner(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

			_logger.LogDebug($"Raised property changed for '{propertyName}' from ViewModel '{Name}'.");
		}
	}
}
