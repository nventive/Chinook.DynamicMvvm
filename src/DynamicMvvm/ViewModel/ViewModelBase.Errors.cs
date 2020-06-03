using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Chinook.DynamicMvvm
{
	public partial class ViewModelBase
	{
		private ConcurrentDictionary<string, IEnumerable<object>> _errors = new ConcurrentDictionary<string, IEnumerable<object>>();

		/// <inheritdoc />
		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		/// <inheritdoc />
		public bool HasErrors => _errors.Any();

		/// <inheritdoc />
		public IEnumerable GetErrors(string propertyName)
		{
			var errors = Enumerable.Empty<object>();

			if (string.IsNullOrEmpty(propertyName))
			{
				// Entity level errors.
				errors = _errors.Values.SelectMany(s => s);
			}
			else
			{
				// Property level errors.
				_errors.TryGetValue(propertyName, out errors);
			}

			return errors ?? Enumerable.Empty<object>();
		}

		/// <inheritdoc />
		public void SetErrors(string propertyName, IEnumerable<object> errors)
		{
			_errors[propertyName] = errors;

			ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
		}

		/// <inheritdoc />
		public void SetErrors(IDictionary<string, IEnumerable<object>> errors)
		{
			_errors = new ConcurrentDictionary<string, IEnumerable<object>>(errors);

			ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName: null));
		}

		/// <inheritdoc />
		public void ClearErrors(string propertyName = null)
		{
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
	}
}
