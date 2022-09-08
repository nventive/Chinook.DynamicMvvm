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
	/// This implementation of <see cref="IDispatcher"/> batches its operations to reduce the call count of <see cref="CoreDispatcher.RunAsync"/>.<br/>
	/// When <see cref="ExecuteOnDispatcher"/> is invoked, the dispatcher operation is delayed by a small duration during which other <see cref="ExecuteOnDispatcher"/> invocations that may occur are accumulated.
	/// After that short delay, all accumulated actions are executed within the same <see cref="CoreDispatcher.RunAsync"/>.
	/// </summary>
	public class BatchingCoreDispatcherDispatcher : IDispatcher
	{
		private readonly CoreDispatcher _coreDispatcher;
		private readonly int _throttleDurationMs;
		private readonly object _mutex = new object();
		private readonly Queue<Request> _requests = new Queue<Request>(capacity: 128);

		/// <summary>
		/// Initializes a new instance of the <see cref="BatchingCoreDispatcherDispatcher"/> class.
		/// </summary>
		/// <param name="frameworkElement">The <see cref="FrameworkElement"/> from which to retrieve the <see cref="CoreDispatcher"/>.</param>
		/// <param name="throttleDurationMs">The amount of time during which the batching accumulation occurs.</param>
		public BatchingCoreDispatcherDispatcher(FrameworkElement frameworkElement, int throttleDurationMs = 10)
		{
			if (frameworkElement is null)
			{
				throw new ArgumentNullException(nameof(frameworkElement));
			}

			_coreDispatcher = frameworkElement.Dispatcher;
			_throttleDurationMs = throttleDurationMs;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BatchingCoreDispatcherDispatcher"/> class.
		/// </summary>
		/// <param name="coreDispatcher">The <see cref="CoreDispatcher"/>.</param>
		/// <param name="throttleDurationMs">The amount of time during which the batching accumulation occurs.</param>
		public BatchingCoreDispatcherDispatcher(CoreDispatcher coreDispatcher, int throttleDurationMs = 10)
		{
			_coreDispatcher = coreDispatcher ?? throw new ArgumentNullException(nameof(coreDispatcher));
			_throttleDurationMs = throttleDurationMs;
		}

		/// <inheritdoc />
		public bool GetHasDispatcherAccess() => _coreDispatcher.HasThreadAccess;

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

			await _coreDispatcher.RunAsync(CoreDispatcherPriority.High, () =>
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
		
		private class Request
		{
			public CancellationToken CT { get; set; }

			public Action Action { get; set; }
		}
	}
}
