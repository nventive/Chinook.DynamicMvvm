﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Chinook.DynamicMvvm
{
	public partial class ViewModelBase
	{
		private readonly Dictionary<string, IDisposable> _disposables = new Dictionary<string, IDisposable>();
		private readonly CancellationTokenSource _cts = new CancellationTokenSource();

		private bool _isDisposing;
		private bool _isDisposed;

		/// <inheritdoc/>
		public bool IsDisposed => _isDisposed;

		/// <summary>
		/// Gets a <see cref="System.Threading.CancellationToken"/> bound to the lifetime of this <see cref="ViewModelBase"/>.
		/// It cancels when this <see cref="ViewModelBase"/> is disposes.
		/// </summary>
		public CancellationToken CancellationToken { get; }

		/// <inheritdoc />
		public IEnumerable<KeyValuePair<string, IDisposable>> Disposables => _disposables;

		/// <inheritdoc />
		public void AddDisposable(IDisposable disposable)
		{
			AddDisposable($"AnonymousDisposable_{Guid.NewGuid()}", disposable);
		}

		/// <inheritdoc />
		public void AddDisposable(string key, IDisposable disposable)
		{
			key = key ?? throw new ArgumentNullException(nameof(key));
			disposable = disposable ?? throw new ArgumentNullException(nameof(disposable));

			ThrowIfDisposed();

			if (_isDisposing)
			{
				_logger.LogDebug("Skipped '{MethodName}' for key '{DisposableKey}' on ViewModel '{ViewModelName}' because it's disposing.", nameof(AddDisposable), key, Name);
				return;
			}

			_disposables.Add(key, disposable);
		}

		/// <inheritdoc />
		public void RemoveDisposable(string key)
		{
			key = key ?? throw new ArgumentNullException(nameof(key));

			if (_isDisposing)
			{
				// No need to remove the disposable while the VM is disposing because the disposables will be cleared.
				return;
			}

			ThrowIfDisposed();

			_disposables.Remove(key);
		}

		/// <inheritdoc />
		public bool TryGetDisposable(string key, out IDisposable disposable)
		{
			key = key ?? throw new ArgumentNullException(nameof(key));

			return _disposables.TryGetValue(key, out disposable);
		}

		private void ThrowIfDisposed()
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException($"The ViewModel {Name} is disposed.");
			}
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
				_logger.LogDebug($"Disposing ViewModel '{Name}'.");
				var toRemove = new List<string>();
				_isDisposing = true;
				_cts.Cancel();
				_cts.Dispose();
				foreach (var pair in _disposables)
				{
					try
					{
						if (pair.Value is not IDynamicProperty)
						{
							toRemove.Add(pair.Key);
						}
						pair.Value.Dispose();
					}
					catch (Exception e)
					{
						_logger.LogError(e, $"Failed to dispose disposable '{pair.Key}' of ViewModel '{Name}'.");
					}
				}

				_dispatcher = null;
				foreach (var key in toRemove)
				{
					_disposables.Remove(key);
				}
				toRemove.Clear();
			}

			_isDisposing = false;
			_isDisposed = true;

			if (_diagnostics.IsEnabled("Disposed"))
			{
				_diagnostics.Write("Disposed", Name);
			}

			_logger.LogInformation($"Disposed ViewModel '{Name}'.");
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
		~ViewModelBase()
		{
			Dispose(isDisposing: false);

			if (_diagnostics.IsEnabled("Destroyed"))
			{
				_diagnostics.Write("Destroyed", Name);
			}

			_logger.LogInformation($"ViewModel '{Name}' destroyed.");
		}
	}
}
