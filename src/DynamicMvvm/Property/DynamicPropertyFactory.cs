using System;
using System.Threading;
using System.Threading.Tasks;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// This is a default implementation of <see cref="IDynamicPropertyFactory"/>.
	/// </summary>
	/// <remarks>
	/// This implementation doesn't actually require the viewModel parameter.
	/// </remarks>
	[Preserve(AllMembers = true)]
	public class DynamicPropertyFactory : IDynamicPropertyFactory
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicPropertyFactory"/> class.
		/// </summary>
		/// <remarks>
		/// When setting <see cref="IDynamicProperty.Value"/> after being disposed, <see cref="ObjectDisposedException"/> will be thrown.
		/// </remarks>
		public DynamicPropertyFactory()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicPropertyFactory"/> class.
		/// </summary>
		/// <param name="throwOnDisposed">Whether a <see cref="ObjectDisposedException"/> should be thrown when <see cref="IDynamicProperty.Value"/> is changed after being disposed.</param>
		[Obsolete("This constructor is obsolete. The throwOnDisposed parameter is no longer used.", error: false)]
		public DynamicPropertyFactory(bool throwOnDisposed)
		{
		}

		/// <inheritdoc />
		public virtual IDynamicProperty Create<T>(string name, T initialValue = default, IViewModel viewModel = null)
			=> new DynamicProperty<T>(name, initialValue);

		/// <inheritdoc />
		public virtual IDynamicProperty CreateFromTask<T>(string name, Func<CancellationToken, Task<T>> source, T initialValue = default, IViewModel viewModel = null)
			=> new DynamicPropertyFromTask<T>(name, source, initialValue);

		/// <inheritdoc />
		public virtual IDynamicProperty CreateFromObservable<T>(string name, IObservable<T> source, T initialValue = default, IViewModel viewModel = null)
			=> new DynamicPropertyFromObservable<T>(name, source, initialValue);
	}
}
