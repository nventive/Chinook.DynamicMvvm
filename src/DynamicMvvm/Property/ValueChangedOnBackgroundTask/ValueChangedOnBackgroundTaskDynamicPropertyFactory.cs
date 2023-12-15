using System;
using System.Threading;
using System.Threading.Tasks;

namespace Chinook.DynamicMvvm.Implementations
{
	/// <summary>
	/// This implementation of <see cref="IDynamicPropertyFactory"/> uses the <see cref="ValueChangedOnBackgroundTaskDynamicProperty"/> base class for all methods.
	/// </summary>
	[Preserve(AllMembers = true)]
	public class ValueChangedOnBackgroundTaskDynamicPropertyFactory : IDynamicPropertyFactory
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicPropertyFactory"/> class.
		/// </summary>
		/// <remarks>
		/// When setting <see cref="IDynamicProperty.Value"/> after being disposed, <see cref="ObjectDisposedException"/> will be thrown.
		/// </remarks>
		public ValueChangedOnBackgroundTaskDynamicPropertyFactory()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicPropertyFactory"/> class.
		/// </summary>
		/// <param name="throwOnDisposed">Whether a <see cref="ValueChangedOnBackgroundTaskDynamicPropertyFactory"/> should be thrown when <see cref="IDynamicProperty.Value"/> is changed after being disposed.</param>
		[Obsolete("This constructor is obsolete. The throwOnDisposed parameter is no longer used.", error: false)]
		public ValueChangedOnBackgroundTaskDynamicPropertyFactory(bool throwOnDisposed = true)
		{
		}

		/// <inheritdoc/>
		public virtual IDynamicProperty Create<T>(string name, T initialValue = default, IViewModel viewModel = null)
		{
			return new ValueChangedOnBackgroundTaskDynamicProperty<T>(name, viewModel, initialValue);
		}

		/// <inheritdoc/>
		public virtual IDynamicProperty CreateFromObservable<T>(string name, IObservable<T> source, T initialValue = default, IViewModel viewModel = null)
		{
			return new ValueChangedOnBackgroundTaskDynamicPropertyFromObservable<T>(name, source, viewModel, initialValue);
		}

		/// <inheritdoc/>
		public virtual IDynamicProperty CreateFromTask<T>(string name, Func<CancellationToken, Task<T>> source, T initialValue = default, IViewModel viewModel = null)
		{
			return new ValueChangedOnBackgroundTaskDynamicPropertyFromTask<T>(name, source, viewModel, initialValue);
		}
	}
}
