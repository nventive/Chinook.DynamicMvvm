using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// This implementation of <see cref="IViewModelView"/> batches its operations to reduce the call count of <see cref="CoreDispatcher.RunAsync"/>.<br/>
	/// When <see cref="ExecuteOnDispatcher"/> is invoked, the dispatcher operation is delayed by a small duration during which other <see cref="ExecuteOnDispatcher"/> invocations that may occur are accumulated.
	/// After that short delay, all accumulated actions are executed within the same <see cref="CoreDispatcher.RunAsync"/>.
	/// </summary>
	public class BatchingViewModelView : IViewModelView
	{
		private readonly FrameworkElement _frameworkElement;
		private readonly int _throttleDurationMs;
		private readonly object _mutex = new object();
		private readonly Queue<Request> _requests = new Queue<Request>(capacity: 128);

		/// <summary>
		/// Initializes a new instance of the <see cref="BatchingViewModelView"/> class.
		/// </summary>
		/// <param name="frameworkElement">The <see cref="FrameworkElement"/>.</param>
		/// <param name="throttleDurationMs">The amount of time during which the batching accumulation occurs.</param>
		public BatchingViewModelView(FrameworkElement frameworkElement, int throttleDurationMs = 10)
		{
			_frameworkElement = frameworkElement ?? throw new ArgumentNullException(nameof(frameworkElement));
			_throttleDurationMs = throttleDurationMs;

			_frameworkElement.Loaded += OnLoaded;
			_frameworkElement.Unloaded += OnUnloaded;
		}

		/// <inheritdoc />
		public bool GetHasDispatcherAccess() => _frameworkElement.Dispatcher.HasThreadAccess;

		/// <inheritdoc />
		public event EventHandler Loaded;

		/// <inheritdoc />
		public event EventHandler Unloaded;

		/// <inheritdoc />
		public async Task ExecuteOnDispatcher(CancellationToken ct, Action action)
		{
			if (ct.IsCancellationRequested)
			{
				this.Log().LogDebug($"Cancelled 'ExecuteOnDispatcher' because of the cancellation token.");
				return;
			}

			if (GetHasDispatcherAccess())
			{
				this.Log().LogDebug($"Executed action immediately because already on dispatcher.");
				action();
				return;
			}

			lock (_mutex)
			{
				_requests.Enqueue(new Request { CT = ct, Action = action });
			}

			await Task.Delay(_throttleDurationMs, ct);

			Request[] requests;
			lock (_mutex)
			{
				requests = _requests.ToArray();
				_requests.Clear();
			}

			if (requests.Length == 0)
			{
				return;
			}

			await _frameworkElement.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
			{
				foreach (var request in requests)
				{
					try
					{
						if (request.CT.IsCancellationRequested)
						{
							this.Log().LogDebug($"Cancelled 'ExecuteOnDispatcher' because of the cancellation token.");
							continue;
						}

						request.Action();
					}
					catch (Exception e)
					{
						this.Log().LogError(e, "Failed 'ExecuteOnDispatcher'.");
					}
				}
			});

			this.Log().LogDebug($"Batched {requests.Length} dispatcher requests.");
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			Loaded?.Invoke(sender, EventArgs.Empty);
		}

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			Unloaded?.Invoke(sender, EventArgs.Empty);
		}

		private class Request
		{
			public CancellationToken CT { get; set; }

			public Action Action { get; set; }
		}
	}
}
