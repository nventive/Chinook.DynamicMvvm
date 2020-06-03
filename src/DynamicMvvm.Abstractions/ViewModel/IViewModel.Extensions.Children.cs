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
		private const string ParentViewChangedSubscriptionKey = "ParentViewChangedSubscription";
		private const string RemoveSelfFromParentSubscriptionKey = "RemoveSelfFromParentSubscription";

		/// <summary>
		/// Gets the children of a <see cref="IViewModel"/>.
		/// </summary>
		/// <param name="viewModel"><see cref="IViewModel"/></param>
		/// <returns>Childen</returns>
		public static IEnumerable<IViewModel> GetChildren(this IViewModel viewModel) => viewModel
			.Disposables
			.Select(pair => pair.Value as IViewModel)
			.Where(child => child != null);

		/// <summary>
		/// Gets the child of a <see cref="IViewModel"/> for the specified <paramref name="name"/>.
		/// If the child doesn't exist, it is created and attached using <paramref name="childViewModelProvider"/>.
		/// </summary>
		/// <typeparam name="TChildViewModel">Type of child</typeparam>
		/// <param name="viewModel">Parent <see cref="IViewModel"/></param>
		/// <param name="childViewModelProvider">Factory for the child <see cref="IViewModel"/></param>
		/// <param name="name">Child name</param>
		/// <returns>Child <see cref="IViewModel"/></returns>
		public static TChildViewModel GetChild<TChildViewModel>(
			this IViewModel viewModel,
			Func<TChildViewModel> childViewModelProvider,
			[CallerMemberName] string name = null
		) where TChildViewModel : IViewModel
		{
			if (!viewModel.TryGetDisposable<TChildViewModel>(name, out var childViewModel))
			{
				childViewModel = viewModel.AttachChild(childViewModelProvider(), name);
			}

			return childViewModel;
		}

		/// <summary>
		/// Gets the child of a <see cref="IViewModel"/> for the specified <paramref name="name"/>.
		/// If the child doesn't exist, it is created and attached using the default constructor.
		/// </summary>
		/// <typeparam name="TChildViewModel">Type of child</typeparam>
		/// <param name="viewModel">Parent <see cref="IViewModel"/></param>
		/// <param name="name">Child name</param>
		/// <returns>Child <see cref="IViewModel"/></returns>
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
		/// Both also share the same <see cref="IViewModelView"/>.
		/// A child can only be attached once to a single <see cref="IViewModel"/>.
		/// </summary>
		/// <typeparam name="TChildViewModel">Type of child</typeparam>
		/// <param name="viewModel">Parent <see cref="IViewModel"/></param>
		/// <param name="childViewModel">Child <see cref="IViewModel"/></param>
		/// <param name="name">Child name</param>
		/// <returns>Child <see cref="IViewModel"/></returns>
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

			childViewModel.View = viewModel.View;

			var parentViewChangedDisposable = new ParentViewChangedDisposable(viewModel, childViewModel);

			childViewModel.AddDisposable(ParentViewChangedSubscriptionKey, parentViewChangedDisposable);
			childViewModel.AddDisposable(RemoveSelfFromParentSubscriptionKey, new ActionDisposable(() => viewModel.RemoveDisposable(name)));

			return childViewModel;
		}

		/// <summary>
		/// Detaches a child <see cref="IViewModel"/> from its parent <see cref="IViewModel"/>.
		/// By being detached, the child will no longer be disposed when the parent is disposed.
		/// The child <see cref="IViewModelView"/> will also be reset.
		/// </summary>
		/// <param name="viewModel">Parent <see cref="IViewModel"/></param>
		/// <param name="childViewModel">Child <see cref="IViewModel"/></param>
		/// <param name="name">Child name</param>
		/// <returns>Child <see cref="IViewModel"/></returns>
		public static void DetachChild(this IViewModel viewModel, IViewModel childViewModel, string name = null)
		{
			if (childViewModel == null)
			{
				throw new ArgumentNullException(nameof(childViewModel));
			}

			childViewModel.View = null;
			if (childViewModel.TryGetDisposable(ParentViewChangedSubscriptionKey, out var subscription))
			{
				subscription.Dispose();
				childViewModel.RemoveDisposable(ParentViewChangedSubscriptionKey);
			}
			childViewModel.RemoveDisposable(RemoveSelfFromParentSubscriptionKey);

			viewModel.RemoveDisposable(name ?? childViewModel.Name);
		}

		private class ParentViewChangedDisposable : IDisposable
		{
			private IViewModel _parentViewModel;
			private IViewModel _childViewModel;

			public ParentViewChangedDisposable(IViewModel parentViewModel, IViewModel childViewModel)
			{
				_parentViewModel = parentViewModel;
				_childViewModel = childViewModel;

				_parentViewModel.ViewChanged += OnParentViewChanged;
			}

			private void OnParentViewChanged(IViewModelView view)
			{
				_childViewModel.View = view;
			}

			public void Dispose()
			{
				_parentViewModel.ViewChanged -= OnParentViewChanged;
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
