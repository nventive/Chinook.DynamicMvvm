using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// Extensions on <see cref="IViewModel"/> to get disposables.
	/// </summary>
	public static partial class IViewModelExtensions
	{
		/// <summary>
		/// Gets the typed <see cref="IDisposable"/> if it exists or default of <typeparamref name="TDisposable"/> otherwise.
		/// </summary>
		/// <typeparam name="TDisposable">The type of <see cref="IDisposable"/>.</typeparam>
		/// <param name="viewModel">This <see cref="IViewModel"/>.</param>
		/// <param name="key">The key associated with the desired disposable.</param>
		/// <param name="disposable">The disposable associated with <paramref name="key"/>.</param>
		/// <returns>The typed disposable when the <paramref name="key"/> was found, or default of <typeparamref name="TDisposable"/> otherwise.</returns>
		public static bool TryGetDisposable<TDisposable>(this IViewModel viewModel, string key, out TDisposable disposable)
		{
			if (viewModel.TryGetDisposable(key, out var untypedDisposable))
			{
				disposable = (TDisposable)untypedDisposable;
				return true;
			}
			else
			{
				disposable = default;
				return false;
			}
		}

		/// <summary>
		/// Gets the typed <see cref="IDisposable"/> associated with <paramref name="key"/>.
		/// </summary>
		/// <typeparam name="TDisposable">The type of <see cref="IDisposable"/>.</typeparam>
		/// <param name="viewModel">This <see cref="IViewModel"/>.</param>
		/// <param name="key">The key associated with the desired disposable.</param>
		/// <param name="create">The func used to create the <see cref="IDisposable"/>.</param>
		/// <returns>The typed disposable associated with <paramref name="key"/>.</returns>
		/// <exception cref="ArgumentNullException">When any of the parameters is null.</exception>
		public static TDisposable GetOrCreateDisposable<TDisposable>(this IViewModel viewModel, string key, Func<TDisposable> create)
			where TDisposable : IDisposable
		{
			if (viewModel is null)
			{
				throw new ArgumentNullException(nameof(viewModel));
			}

			if (key is null)
			{
				throw new ArgumentNullException(nameof(key));
			}

			if (create is null)
			{
				throw new ArgumentNullException(nameof(create));
			}

			if (viewModel.TryGetDisposable(key, out var existingDisposable))
			{
				return (TDisposable)existingDisposable;
			}
			else
			{
				var disposable = create();
				viewModel.AddDisposable(key, disposable);
				return disposable;
			}
		}
	}
}
