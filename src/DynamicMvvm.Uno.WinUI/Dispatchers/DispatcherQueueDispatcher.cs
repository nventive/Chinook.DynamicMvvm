using System;
using Windows.UI.Core;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// This implementation of <see cref="IDispatcher"/> uses <see cref="DispatcherQueue"/>.
	/// </summary>
	public class DispatcherQueueDispatcher : IDispatcher
	{
		private readonly DispatcherQueue _dispatcherQueue;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="DispatcherQueueDispatcher"/> class.
		/// </summary>
		/// <param name="frameworkElement">The <see cref="FrameworkElement"/> from which to retrieve the <see cref="CoreDispatcher"/>.</param>
		public DispatcherQueueDispatcher(FrameworkElement frameworkElement)
		{
			if (frameworkElement is null)
			{
				throw new ArgumentNullException(nameof(frameworkElement));
			}

			_dispatcherQueue = frameworkElement.DispatcherQueue;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DispatcherQueueDispatcher"/> class.
		/// </summary>
		/// <param name="dispatcherQueue">The <see cref="DispatcherQueue"/>.</param>
		public DispatcherQueueDispatcher(DispatcherQueue dispatcherQueue)
		{
			if (dispatcherQueue is null)
			{
				throw new ArgumentNullException(nameof(dispatcherQueue));
			}

			_dispatcherQueue = dispatcherQueue;
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

			_dispatcherQueue.TryEnqueue(DispatcherQueuePriority.High, () =>
			{
				try
				{
					if (ct.IsCancellationRequested)
					{
						this.Log().LogDebug($"Cancelled 'ExecuteOnDispatcher' because of the cancellation token.");
						return;
					}

					action();
				}
				catch (Exception e)
				{
					this.Log().LogError(e, "Failed 'ExecuteOnDispatcher'.");
				}
			});
		}
	}
}
