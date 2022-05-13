using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chinook.DynamicMvvm;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chinook.DynamicMvvm.Deactivation
{
	/// <summary>
	/// This is a default implementation of <see cref="IDeactivatableViewModel"/>.<br/>
	/// When <see cref="IsDeactivated"/> is true, property changes are suppressed and buffered until <see cref="IDeactivatable.Reactivate"/> is called.
	/// </summary>
	public class DeactivatableViewModelBase : ViewModelBase, IDeactivatableViewModel
	{
		// This mutex protects the modifications on _changedProperties when IsDeactivated changes.
		private readonly object _isDeactivatedMutex = new object();
		private readonly HashSet<string> _changedProperties = new HashSet<string>();
		private bool _isDeactivated = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="DeactivatableViewModelBase"/> class.
		/// </summary>
		/// <param name="name">The name of the ViewModel.</param>
		/// <param name="serviceProvider">The service provider.</param>
		public DeactivatableViewModelBase(string name = null, IServiceProvider serviceProvider = null)
			: base(name, serviceProvider)
		{

		}

		/// <inheritdoc/>
		public bool IsDeactivated
		{
			get => _isDeactivated;
			private set
			{
				_isDeactivated = value;
				if (!IsDisposed)
				{
					// Bypass the overridden implementation of RaisePropertyChanged so we can guarantee that the view will always be updated (e.g. Update even if IsDeactivated == true).
					base.RaisePropertyChanged(nameof(IsDeactivated));
				}
			}
		}

		/// <inheritdoc />
		/// <remarks>
		/// When <see cref="IsDeactivated"/> is true, property changes are suppressed and buffered until <see cref="IDeactivatable.Reactivate"/> is called.
		/// </remarks>
		public override void RaisePropertyChanged(string propertyName)
		{
			if (IsDeactivated)
			{
				lock (_isDeactivatedMutex)
				{
					_changedProperties.Add(propertyName);
				}
			}
			else
			{
				base.RaisePropertyChanged(propertyName);
			}
		}

		/// <summary>
		/// Deactivates this ViewModel and all other <see cref="IDeactivatable"/> objects contained in <see cref="IViewModel.Disposables"/>.
		/// </summary>
		public virtual void Deactivate()
		{
			if (IsDeactivated)
			{
				return;
			}

			IsDeactivated = true;

			var children = Disposables.Select(pair => pair.Value).OfType<IDeactivatable>();
			foreach (var child in children)
			{
				child.Deactivate();
			}

			typeof(IDeactivatable).Log().LogDebug($"Deactivated ViewModel '{Name}'.");
		}

		/// <summary>
		/// Reactivates this ViewModel and all other <see cref="IDeactivatable"/> objects contained in <see cref="IViewModel.Disposables"/>.
		/// </summary>
		public virtual void Reactivate()
		{
			if (!IsDeactivated)
			{
				return;
			}

			string[] toRaise;
			lock (_isDeactivatedMutex)
			{
				IsDeactivated = false;
				toRaise = _changedProperties.ToArray();
				_changedProperties.Clear();
			}

			foreach (var propertyName in toRaise)
			{
				RaisePropertyChanged(propertyName);
			}

			var children = Disposables.Select(pair => pair.Value).OfType<IDeactivatable>().ToList();
			foreach (var child in children)
			{
				child.Reactivate();
			}

			typeof(IDeactivatable).Log().LogDebug($"Reactivated ViewModel '{Name}' and raised {toRaise.Length} property changes.");
		}
	}
}
