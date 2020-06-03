using System;
using System.Collections.Generic;
using System.Text;
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
