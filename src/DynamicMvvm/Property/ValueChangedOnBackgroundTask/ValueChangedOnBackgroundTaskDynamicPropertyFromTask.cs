using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Chinook.DynamicMvvm.Implementations
{
	/// <summary>
	/// This is an implementation of a <see cref="IDynamicProperty{T}"/> using a <see cref="Task{T}"/> that ensures <see cref="IDynamicProperty.ValueChanged"/> is raised on a background thread.
	/// </summary>
	/// <typeparam name="T">Type of value</typeparam>
	public class ValueChangedOnBackgroundTaskDynamicPropertyFromTask<T> : ValueChangedOnBackgroundTaskDynamicProperty<T>
	{
		private readonly CancellationTokenSource _cancellationTokenSource;
		private bool _isDisposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="ValueChangedOnBackgroundTaskDynamicPropertyFromTask{T}"/> class.
		/// </summary>
		/// <remarks>
		/// When setting <see cref="IDynamicProperty.Value"/> after being disposed, <see cref="ObjectDisposedException"/> will be thrown.
		/// </remarks>
		/// <param name="name">The name of the this property.</param>
		/// <param name="source">The task source for this property.</param>
		/// <param name="viewModel">The <see cref="IViewModel"/> used to determine dispatcher access.</param>
		/// <param name="initialValue">The initial value of this property.</param>
		public ValueChangedOnBackgroundTaskDynamicPropertyFromTask(string name, Func<CancellationToken, Task<T>> source, IViewModel viewModel, T initialValue = default)
			: base(name, viewModel, initialValue)
		{
			if (source is null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			_cancellationTokenSource = new CancellationTokenSource();

			_ = SetValueFromSource(_cancellationTokenSource.Token, source);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValueChangedOnBackgroundTaskDynamicPropertyFromTask{T}"/> class.
		/// </summary>
		/// <param name="name">The name of the this property.</param>
		/// <param name="source">The task source for this property.</param>
		/// <param name="viewModel">The <see cref="IViewModel"/> used to determine dispatcher access.</param>
		/// <param name="initialValue">The initial value of this property.</param>
		/// <param name="throwOnDisposed">Whether a <see cref="ObjectDisposedException"/> should be thrown when <see cref="IDynamicProperty.Value"/> is changed after being disposed.</param>
		[Obsolete("This constructor is obsolete. The throwOnDisposed parameter is no longer used.", error: false)]
		public ValueChangedOnBackgroundTaskDynamicPropertyFromTask(string name, Func<CancellationToken, Task<T>> source, IViewModel viewModel, bool throwOnDisposed, T initialValue = default)
			: this(name, source, viewModel, initialValue)
		{
		}

		private async Task SetValueFromSource(CancellationToken ct, Func<CancellationToken, Task<T>> source)
		{
			try
			{
				var value = await source(ct);

				Value = value;
			}
			catch (Exception e)
			{
				this.Log().LogDynamicPropertySourceTaskFailed(Name, e);
			}
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
				_cancellationTokenSource.Dispose();
			}

			_isDisposed = true;

			base.Dispose(isDisposing);
		}
	}
}
