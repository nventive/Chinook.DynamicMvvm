using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// This implementation of <see cref="IDispatcher"/> batches its operations to reduce the call count of <see cref="DispatcherQueue.TryEnqueue"/>.<br/>
	/// When <see cref="ExecuteOnDispatcher"/> is invoked, the dispatcher operation is delayed by a small duration during which other <see cref="ExecuteOnDispatcher"/> invocations that may occur are accumulated.
	/// After that short delay, all accumulated actions are executed within the same <see cref="DispatcherQueue.TryEnqueue"/>.
	/// </summary>
	public class BatchingDispatcherQueueDispatcher : IDispatcher
	{
		private readonly DispatcherQueue _dispatcherQueue;
		private readonly int _throttleDurationMs;
		private readonly object _mutex = new object();
		private readonly Queue<Request> _requests = new Queue<Request>(capacity: 128);

		/// <summary>
		/// Initializes a new instance of the <see cref="BatchingDispatcherQueueDispatcher"/> class.
		/// </summary>
		/// <param name="frameworkElement">The <see cref="FrameworkElement"/> from which to retrieve the <see cref="DispatcherQueue"/>.</param>
		/// <param name="throttleDurationMs">The amount of time during which the batching accumulation occurs.</param>
		public BatchingDispatcherQueueDispatcher(FrameworkElement frameworkElement, int throttleDurationMs = 10)
		{
			if (frameworkElement is null)
			{
				throw new ArgumentNullException(nameof(frameworkElement));
			}

			_dispatcherQueue = frameworkElement.DispatcherQueue;
			_throttleDurationMs = throttleDurationMs;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BatchingDispatcherQueueDispatcher"/> class.
		/// </summary>
		/// <param name="dispatcherQueue">The <see cref="DispatcherQueue"/>.</param>
		/// <param name="throttleDurationMs">The amount of time during which the batching accumulation occurs.</param>
		public BatchingDispatcherQueueDispatcher(DispatcherQueue dispatcherQueue, int throttleDurationMs = 10)
		{
			_dispatcherQueue = dispatcherQueue ?? throw new ArgumentNullException(nameof(dispatcherQueue));
			_throttleDurationMs = throttleDurationMs;
		}

		/// <inheritdoc />
		public bool GetHasDispatcherAccess() => _dispatcherQueue.HasThreadAccess;

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
		

			_dispatcherQueue.TryEnqueue(DispatcherQueuePriority.High, () =>
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
