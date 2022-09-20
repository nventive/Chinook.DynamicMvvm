using System;
using Windows.UI.Xaml;
using Windows.UI.Core;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// This implementation of <see cref="IDispatcher"/> uses <see cref="CoreDispatcher"/>.
	/// </summary>
	public class CoreDispatcherDispatcher : IDispatcher
	{
		private readonly CoreDispatcher _coreDispatcher;

		/// <summary>
		/// Initializes a new instance of the <see cref="CoreDispatcherDispatcher"/> class.
		/// </summary>
		/// <param name="frameworkElement">The <see cref="FrameworkElement"/> from which to retrieve the <see cref="CoreDispatcher"/>.</param>
		public CoreDispatcherDispatcher(FrameworkElement frameworkElement)
		{
			if (frameworkElement is null)
			{
				throw new ArgumentNullException(nameof(frameworkElement));
			}

			_coreDispatcher = frameworkElement.Dispatcher;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CoreDispatcherDispatcher"/> class.
		/// </summary>
		/// <param name="coreDispatcher">The <see cref="CoreDispatcher"/>.</param>
		public CoreDispatcherDispatcher(CoreDispatcher coreDispatcher)
		{
			if (coreDispatcher is null)
			{
				throw new ArgumentNullException(nameof(coreDispatcher));
			}

			_coreDispatcher = coreDispatcher;
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

			await _coreDispatcher.RunAsync(CoreDispatcherPriority.High, () =>
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
