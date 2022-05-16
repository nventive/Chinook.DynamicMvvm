using System;
using System.Collections.Generic;
using System.Text;
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
