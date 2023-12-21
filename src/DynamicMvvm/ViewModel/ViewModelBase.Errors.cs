using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Chinook.DynamicMvvm
{
	public partial class ViewModelBase
	{
		private Dictionary<string, IEnumerable<object>> _errors;

		/// <inheritdoc />
		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		/// <inheritdoc />
		public bool HasErrors => _errors is null ? false : _errors.Values.SelectMany(x => x).Any();

		/// <inheritdoc />
		public IEnumerable GetErrors(string propertyName)
		{
			var errors = Enumerable.Empty<object>();

			if (string.IsNullOrEmpty(propertyName))
			{
				// Entity level errors.
				errors = _errors?.Values.SelectMany(s => s);
			}
			else
			{
				// Property level errors.
				_errors?.TryGetValue(propertyName, out errors);
			}

			return errors ?? Enumerable.Empty<object>();
		}

		/// <inheritdoc />
		public void SetErrors(string propertyName, IEnumerable<object> errors)
		{
			ThrowIfDisposed();

			if (_isDisposing)
			{
				_logger.LogViewModelSkippedMethodBecauseDisposing_PropertyName(nameof(SetErrors), GetType().Name, propertyName, Name);
				return;
			}

			EnsureErrorsAreInitialized();
			_errors[propertyName] = errors;

			ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
		}

		/// <inheritdoc />
		public void SetErrors(IDictionary<string, IEnumerable<object>> errors)
		{
			ThrowIfDisposed();

			if (_isDisposing)
			{
				_logger.LogViewModelSkippedMethodBecauseDisposing(nameof(SetErrors), Name);
				return;
			}

			_errors = new Dictionary<string, IEnumerable<object>>(errors);

			ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName: null));
		}

		/// <inheritdoc />
		public void ClearErrors(string propertyName = null)
		{
			ThrowIfDisposed();

			if (_isDisposing)
			{
				_logger.LogViewModelSkippedMethodBecauseDisposing_PropertyName(nameof(ClearErrors), GetType().Name, propertyName, Name);
				return;
			}

			if (_errors is null)
			{
				// No errors to clear.
				return;
			}

			if (string.IsNullOrEmpty(propertyName))
			{
				_errors.Clear();
			}
			else
			{
				_errors[propertyName] = Enumerable.Empty<object>();
			}

			ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
		}

		private void EnsureErrorsAreInitialized()
		{
			if (_errors is null)
			{
				_errors = new Dictionary<string, IEnumerable<object>>();
			}
		}
	}
}
