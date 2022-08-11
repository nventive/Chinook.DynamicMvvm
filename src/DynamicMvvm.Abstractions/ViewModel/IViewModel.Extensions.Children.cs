using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// Extensions on <see cref="IViewModel"/> to attach children.
	/// </summary>
	public static partial class IViewModelExtensions
	{
		private const string ParentDispatcherChangedSubscriptionKey = "ParentDispatcherChangedSubscription";
		private const string RemoveSelfFromParentSubscriptionKey = "RemoveSelfFromParentSubscription";

		/// <summary>
		/// Gets the children of this <see cref="IViewModel"/>.
		/// </summary>
		/// <param name="viewModel">This <see cref="IViewModel"/>.</param>
		/// <returns>A IEnumerable of child <see cref="IViewModel"/>s of this ViewModel.</returns>
		public static IEnumerable<IViewModel> GetChildren(this IViewModel viewModel) => viewModel
			.Disposables
			.Select(pair => pair.Value as IViewModel)
			.Where(child => child != null);

		/// <summary>
		/// Gets the child of this <see cref="IViewModel"/> for the specified <paramref name="name"/>.
		/// If the child doesn't exist, it is created and attached using <paramref name="childViewModelProvider"/>.
		/// </summary>
		/// <typeparam name="TChildViewModel">The type of child ViewModel.</typeparam>
		/// <param name="viewModel">The parent <see cref="IViewModel"/>.</param>
		/// <param name="childViewModelProvider">The factory to the child <see cref="IViewModel"/>.</param>
		/// <param name="name">The child ViewModel's name.</param>
		/// <returns>The attached child <see cref="IViewModel"/>. Default of <typeparamref name="TChildViewModel"/> is returned if the <see cref="IViewModel"/> is disposed.</returns>
		public static TChildViewModel GetChild<TChildViewModel>(
			this IViewModel viewModel,
			Func<TChildViewModel> childViewModelProvider,
			[CallerMemberName] string name = null
		) where TChildViewModel : IViewModel
		{
			if (viewModel.IsDisposed)
			{
				return default(TChildViewModel);
			}

			if (!viewModel.TryGetDisposable<TChildViewModel>(name, out var childViewModel))
			{
				childViewModel = viewModel.AttachChild(childViewModelProvider(), name);
			}

			return childViewModel;
		}

		/// <summary>
		/// Gets the child of this <see cref="IViewModel"/> for the specified <paramref name="name"/>.
		/// If the child doesn't exist, it is created and attached using the default constructor.
		/// </summary>
		/// <typeparam name="TChildViewModel">The type of child viewmodel.</typeparam>
		/// <param name="viewModel">The parent <see cref="IViewModel"/>.</param>
		/// <param name="name">The child ViewModel's name.</param>
		/// <returns>The attached child <see cref="IViewModel"/>.</returns>
		public static TChildViewModel GetChild<TChildViewModel>(
			this IViewModel viewModel,
			[CallerMemberName] string name = null
		) where TChildViewModel : IViewModel, new()
		{
			return viewModel.GetChild(() => new TChildViewModel(), name);
		}

		/// <summary>
		/// Attaches a child <see cref="IViewModel"/> to a parent <see cref="IViewModel"/>.
		/// By being attached, the child will be disposed when the parent is disposed.
		/// Both also share the same <see cref="IDispatcher"/>.
		/// A child can only be attached once to a single <see cref="IViewModel"/>.
		/// </summary>
		/// <typeparam name="TChildViewModel">The type of child viewmodel.</typeparam>
		/// <param name="viewModel">The parent <see cref="IViewModel"/>.</param>
		/// <param name="childViewModel">The child <see cref="IViewModel"/> to attach.</param>
		/// <param name="name">The child ViewModel's name. This defaults to <paramref name="childViewModel"/>.Name when not provided.</param>
		/// <returns>The attached child <see cref="IViewModel"/>.</returns>
		public static TChildViewModel AttachChild<TChildViewModel>(this IViewModel viewModel, TChildViewModel childViewModel, string name = null)
			where TChildViewModel : IViewModel
		{
			if (childViewModel == null)
			{
				throw new ArgumentNullException(nameof(childViewModel));
			}

			name = name ?? childViewModel.Name;

			if (viewModel.TryGetDisposable(name, out var _))
			{
				throw new InvalidOperationException($"A child ViewModel with the name '{name}' is already attached to this ViewModel.");
			}

			viewModel.AddDisposable(name, childViewModel);

			childViewModel.Dispatcher = viewModel.Dispatcher;

			var parentViewChangedDisposable = new ParentDispatcherChangedDisposable(viewModel, childViewModel);

			childViewModel.AddDisposable(ParentDispatcherChangedSubscriptionKey, parentViewChangedDisposable);
			childViewModel.AddDisposable(RemoveSelfFromParentSubscriptionKey, new ActionDisposable(() => viewModel.RemoveDisposable(name)));

			return childViewModel;
		}

		/// <summary>
		/// Detaches a child <see cref="IViewModel"/> from its parent <see cref="IViewModel"/>.
		/// By being detached, the child will no longer be disposed when the parent is disposed.
		/// The child's <see cref="IDispatcher"/> will also be removed.
		/// </summary>
		/// <param name="viewModel">The parent <see cref="IViewModel"/>.</param>
		/// <param name="childViewModel">The child <see cref="IViewModel"/> to detach.</param>
		/// <param name="name">The child ViewModel's name. This defaults to <paramref name="childViewModel"/>.Name when not provided.</param>
		public static void DetachChild(this IViewModel viewModel, IViewModel childViewModel, string name = null)
		{
			if (childViewModel == null)
			{
				throw new ArgumentNullException(nameof(childViewModel));
			}

			childViewModel.Dispatcher = null;
			if (childViewModel.TryGetDisposable(ParentDispatcherChangedSubscriptionKey, out var subscription))
			{
				subscription.Dispose();
				childViewModel.RemoveDisposable(ParentDispatcherChangedSubscriptionKey);
			}
			childViewModel.RemoveDisposable(RemoveSelfFromParentSubscriptionKey);

			viewModel.RemoveDisposable(name ?? childViewModel.Name);
		}

		/// <summary>
		/// Attaches a child <see cref="IViewModel"/> to a parent <see cref="IViewModel"/>.
		/// If the child has already been attached, the newer instance will be attached instead of the previous instance.
		/// By being attached, the child will be disposed when the parent is disposed.
		/// Both also share the same <see cref="IDispatcher"/>.
		/// </summary>
		/// <remarks>The previous instance will be disposed when replaced.</remarks>
		/// <param name="viewModel">The parent <see cref="IViewModel"/>.</param>
		/// <param name="childViewModel">The child <see cref="IViewModel"/> to detach.</param>
		/// <param name="name">The child ViewModel's name. This defaults to <paramref name="childViewModel"/>.Name when not provided.</param>
		public static TChildViewModel AttachOrReplaceChild<TChildViewModel>(this IViewModel viewModel, TChildViewModel childViewModel, string name = null)
			where TChildViewModel : IViewModel
		{
			name = name ?? childViewModel.Name;

			if (viewModel.TryGetDisposable(name, out var instance))
			{
				viewModel.DetachChild((IViewModel)instance, name);
				instance.Dispose();
			}
			return viewModel.AttachChild(childViewModel, name);
		}

		private class ParentDispatcherChangedDisposable : IDisposable
		{
			private IViewModel _parentViewModel;
			private IViewModel _childViewModel;

			public ParentDispatcherChangedDisposable(IViewModel parentViewModel, IViewModel childViewModel)
			{
				_parentViewModel = parentViewModel;
				_childViewModel = childViewModel;

				_parentViewModel.DispatcherChanged += OnParentDispatcherChanged;
			}

			private void OnParentDispatcherChanged(IDispatcher view)
			{
				_childViewModel.Dispatcher = view;
			}

			public void Dispose()
			{
				_parentViewModel.DispatcherChanged -= OnParentDispatcherChanged;
				_parentViewModel = null;
				_childViewModel = null;
			}
		}

		private class ActionDisposable : IDisposable
		{
			private readonly Action _action;
			private bool _isDisposed;

			public ActionDisposable(Action action)
			{
				_action = action;
			}

			public void Dispose()
			{
				if (!_isDisposed)
				{
					_isDisposed = true;
					_action();
				}
			}
		}
	}
}
