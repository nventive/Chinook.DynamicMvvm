using System;
using System.Diagnostics;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// This is a default implementation of <see cref="IDynamicProperty"/>.
	/// </summary>
	public class DynamicProperty : IDynamicProperty
	{
		private static readonly DiagnosticSource _diagnostics = new DiagnosticListener("Chinook.DynamicMvvm.IDynamicProperty");
		private object _value;
		private bool _isDisposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicProperty"/> class.
		/// </summary>
		/// <param name="name">Name</param>
		/// <param name="value">Initial value</param>
		public DynamicProperty(string name, object value = default)
		{
			Name = name;
			_value = value;

			if (_diagnostics.IsEnabled("Created"))
			{
				_diagnostics.Write("Created", Name);
			}
		}

		/// <inheritdoc />
		public string Name { get; }

		/// <inheritdoc />
		public object Value 
		{ 
			get => _value;
			set
			{
				if (_isDisposed)
				{
					throw new ObjectDisposedException(Name);
				}

				if (!Equals(value, _value))
				{
					_value = value;

					ValueChanged?.Invoke(this);
				}
			}
		}

		/// <inheritdoc />
		public event DynamicPropertyChangedEventHandler ValueChanged;

		/// <inheritdoc />
		public override string ToString()
		{
			return Name + " " + Value;
		}

		/// <inheritdoc />
		protected virtual void Dispose(bool isDisposing)
		{
			if (_isDisposed)
			{
				return;
			}

			if (isDisposing)
			{
				ValueChanged = null;
			}

			_isDisposed = true;

			if (_diagnostics.IsEnabled("Disposed"))
			{
				_diagnostics.Write("Disposed", Name);
			}
		}

		/// <inheritdoc />
		public void Dispose()
		{
			Dispose(isDisposing: true);

			// If diagnostics are enabled, don't suppress the finalizer.
			// This allows the differentiation between disposed and destroyed instances. 
			if (!_diagnostics.IsEnabled("Destroyed"))
			{
				GC.SuppressFinalize(this);
			}
		}

		/// <inheritdoc />
		~DynamicProperty()
		{
			Dispose(isDisposing: false);

			if (_diagnostics.IsEnabled("Destroyed"))
			{
				_diagnostics.Write("Destroyed", Name);
			}
		}
	}
}
