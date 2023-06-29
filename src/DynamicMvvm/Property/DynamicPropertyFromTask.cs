using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// This is an implementation of a <see cref="IDynamicProperty{T}"/> using a <see cref="Task{T}"/>.
	/// </summary>
	/// <typeparam name="T">Type of value</typeparam>
	public class DynamicPropertyFromTask<T> : DynamicProperty<T>
	{
		private readonly CancellationTokenSource _cancellationTokenSource;
		private bool _isDisposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicPropertyFromTask{T}"/> class.
		/// </summary>
		/// <remarks>
		/// When setting <see cref="IDynamicProperty.Value"/> after being disposed, <see cref="ObjectDisposedException"/> will be thrown.
		/// </remarks>
		/// <param name="name">Name</param>
		/// <param name="source">Source</param>
		/// <param name="initialValue">Initial value</param>
		public DynamicPropertyFromTask(string name, Func<CancellationToken, Task<T>> source, T initialValue = default)
			: base(name, initialValue)
		{
			if (source is null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			_cancellationTokenSource = new CancellationTokenSource();

			_ = SetValueFromSource(_cancellationTokenSource.Token, source);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicPropertyFromTask{T}"/> class.
		/// </summary>
		/// <param name="name">Name</param>
		/// <param name="source">Source</param>
		/// <param name="initialValue">Initial value</param>
		/// <param name="throwOnDisposed">Whether a <see cref="ObjectDisposedException"/> should be thrown when <see cref="IDynamicProperty.Value"/> is changed after being disposed.</param>
		public DynamicPropertyFromTask(string name, Func<CancellationToken, Task<T>> source, bool throwOnDisposed, T initialValue = default)
			: base(name, throwOnDisposed, initialValue)
		{
			if (source is null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			_cancellationTokenSource = new CancellationTokenSource();

			_ = SetValueFromSource(_cancellationTokenSource.Token, source);
		}

		private async Task SetValueFromSource(CancellationToken ct, Func<CancellationToken, Task<T>> source)
		{
			//await Task.Run(async () =>
			//{
			try
			{
				var value = await source(ct);

				Value = value;
			}
			catch (Exception e)
			{
				this.Log().LogError(e, $"Source task failed for property '{Name}'.");
			}
			//});
		}

		/// <inheritdoc />
		protected override void Dispose(bool isDisposing)
		{
			if (_isDisposed)
			{
				return;
			}

			if (isDisposing)
			{
				_cancellationTokenSource.Cancel();
			}

			_isDisposed = true;

			base.Dispose(isDisposing);
		}
	}
}
