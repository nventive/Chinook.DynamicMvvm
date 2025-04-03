﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Chinook.DynamicMvvm
{
	public partial class ViewModelBase
	{
		private readonly ConcurrentDictionary<string, IDisposable> _disposables = new();
		private readonly CancellationTokenSource _cts = new();

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
				_logger.LogViewModelSkippedMethodBecauseDisposing_DisposableKey(nameof(AddDisposable), key, Name);
				return;
			}

			_disposables.TryAdd(key, disposable);
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

			_disposables.TryRemove(key, out _);
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
				_logger.LogViewModelDisposing(Name);

				_isDisposing = true;
				_cts.Cancel();
				_cts.Dispose();
				foreach (var pair in _disposables)
				{
					try
					{
						pair.Value.Dispose();
					}
					catch (Exception e)
					{
						_logger.LogViewModelFailedToDisposeDisposable(pair.Key, Name, e);
					}
				}

				_dispatcher = null;
				_disposables.Clear();
			}

			_isDisposing = false;
			_isDisposed = true;

			if (_diagnostics.IsEnabled("Disposed"))
			{
				_diagnostics.Write("Disposed", Name);
			}

			_logger.LogViewModelDisposed(Name);
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

			_logger.LogViewModelDestroyed(Name);
		}
	}
}
