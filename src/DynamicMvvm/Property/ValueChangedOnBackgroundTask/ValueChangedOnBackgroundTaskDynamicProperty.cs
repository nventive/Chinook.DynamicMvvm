using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Chinook.DynamicMvvm.Implementations
{
	/// <summary>
	/// This implementation of <see cref="IDynamicProperty"/> ensures that <see cref="IDynamicProperty.ValueChanged"/> is raised on a background thread.
	/// </summary>
	public class ValueChangedOnBackgroundTaskDynamicProperty : IDynamicProperty
	{
		private static readonly DiagnosticSource _diagnostics = new DiagnosticListener("Chinook.DynamicMvvm.IDynamicProperty");
		private readonly WeakReference<IViewModel> _viewModel;
		private readonly bool _throwOnDisposed;

		private object _value;
		private bool _isDisposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="ValueChangedOnBackgroundTaskDynamicProperty"/> class.
		/// </summary>
		/// <remarks>
		/// When setting <see cref="Value"/> after being disposed, <see cref="ObjectDisposedException"/> will be thrown.
		/// </remarks>
		/// <param name="name">The name of the this property.</param>
		/// <param name="viewModel">The <see cref="IViewModel"/> used to determine dispatcher access.</param>
		/// <param name="value">The initial value of this property.</param>
		public ValueChangedOnBackgroundTaskDynamicProperty(string name, IViewModel viewModel, object value = default)
		{
			Name = name;
			_viewModel = new WeakReference<IViewModel>(viewModel);
			_value = value;

			if (_diagnostics.IsEnabled("Created"))
			{
				_diagnostics.Write("Created", Name);
			}
			_throwOnDisposed = true;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValueChangedOnBackgroundTaskDynamicProperty"/> class.
		/// </summary>
		/// <param name="name">The name of the this property.</param>
		/// <param name="viewModel">The <see cref="IViewModel"/> used to determine dispatcher access.</param>
		/// <param name="value">The initial value of this property.</param>
		/// <param name="throwOnDisposed">Whether a <see cref="ObjectDisposedException"/> should be thrown when <see cref="Value"/> is changed after being disposed.</param>
		public ValueChangedOnBackgroundTaskDynamicProperty(string name, IViewModel viewModel, bool throwOnDisposed, object value = default)
		{
			Name = name;
			_viewModel = new WeakReference<IViewModel>(viewModel);
			_value = value;

			if (_diagnostics.IsEnabled("Created"))
			{
				_diagnostics.Write("Created", Name);
			}
			_throwOnDisposed = throwOnDisposed;
		}

		protected bool IsOnDispatcher => _viewModel.TryGetTarget(out var vm) && (vm.Dispatcher?.GetHasDispatcherAccess() ?? false);

		/// <inheritdoc />
		public string Name { get; }

		/// <inheritdoc />
		public virtual object Value
		{
			get => _value;
			set
			{
				if (_isDisposed)
				{
					if (_throwOnDisposed)
					{
						throw new ObjectDisposedException(Name);
					}
					else
					{
						return;
					}
				}

				if (!Equals(value, _value))
				{
					_value = value;

					if (IsOnDispatcher)
					{
						_ = Task.Run(() => ValueChanged?.Invoke(this));
					}
					else
					{
						ValueChanged?.Invoke(this);
					}
				}
			}
		}

		/// <inheritdoc />
		public virtual event DynamicPropertyChangedEventHandler ValueChanged;

		/// <inheritdoc />
		public override string ToString()
		{
			return Name + " " + Value;
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (_isDisposed)
			{
				return;
			}

			if (isDisposing)
			{
				_viewModel.SetTarget(null);
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

		~ValueChangedOnBackgroundTaskDynamicProperty()
		{
			Dispose(isDisposing: false);

			if (_diagnostics.IsEnabled("Destroyed"))
			{
				_diagnostics.Write("Destroyed", Name);
			}
		}
	}

	/// <summary>
	/// This implementation of <see cref="IDynamicProperty{T}"/> ensures that <see cref="IDynamicProperty.ValueChanged"/> is raised on a background thread.
	/// </summary>
	public class ValueChangedOnBackgroundTaskDynamicProperty<T> : ValueChangedOnBackgroundTaskDynamicProperty, IDynamicProperty<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ValueChangedOnBackgroundTaskDynamicProperty{T}"/> class.
		/// </summary>
		/// <param name="name">The name of the this property.</param>
		/// <param name="viewModel">The <see cref="IViewModel"/> used to determine dispatcher access.</param>
		/// <param name="value">The initial value of this property.</param>
		public ValueChangedOnBackgroundTaskDynamicProperty(string name, IViewModel viewModel, T value = default)
			: base(name, viewModel, value)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValueChangedOnBackgroundTaskDynamicProperty{T}"/> class.
		/// </summary>
		/// <param name="name">The name of the this property.</param>
		/// <param name="viewModel">The <see cref="IViewModel"/> used to determine dispatcher access.</param>
		/// <param name="value">The initial value of this property.</param>
		/// <param name="throwOnDisposed">Whether a <see cref="ObjectDisposedException"/> should be thrown when <see cref="Value"/> is changed after being disposed.</param>
		public ValueChangedOnBackgroundTaskDynamicProperty(string name, IViewModel viewModel, bool throwOnDisposed, T value = default)
			: base(name, viewModel, throwOnDisposed, value)
		{
		}

		/// <inheritdoc />
		public new T Value
		{
			get => (T)base.Value;
			set => base.Value = value;
		}
	}
}
