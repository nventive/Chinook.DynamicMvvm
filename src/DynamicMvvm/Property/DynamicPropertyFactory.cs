using System;
using System.Collections.Generic;
using System.Text;
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
