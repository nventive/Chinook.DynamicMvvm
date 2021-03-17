using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// A <see cref="IViewModel"/> is a container that you can use for data bindings.
	/// Other objects can be attached to the <see cref="IViewModel"/> by registering <see cref="IDisposable"/> objects.
	/// The <see cref="IViewModel"/> has a life cycle. Disposing it will dispose all its attached objects.
	/// It implements <see cref="INotifyPropertyChanged"/> to support property data binding.
	/// It implements <see cref="INotifyDataErrorInfo"/> to support error data binding.
	/// A <see cref="IViewModel"/> is coupled to a <see cref="IViewModelView"/> to raise its events on the right threads.
	/// </summary>
	public interface IViewModel : INotifyPropertyChanged, INotifyDataErrorInfo, IDisposable
	{
		/// <summary>
		/// Gets the name of the <see cref="IViewModel"/>.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Invokes <see cref="INotifyPropertyChanged.PropertyChanged"/> on the correct thread.
		/// </summary>
		/// <param name="propertyName">The name of the property.</param>
		void RaisePropertyChanged(string propertyName);

		/// <summary>
		/// Adds a disposable to this ViewModel.
		/// The <paramref name="disposable"/> disposes when <see cref="IDisposable.Dispose"/> is called.
		/// You can retrieve <paramref name="disposable"/> using <see cref="Disposables"/> and <see cref="TryGetDisposable(string, out IDisposable)"/>.
		/// </summary>
		/// <param name="key">The key associated with the disposable.</param>
		/// <param name="disposable">The disposable to dispose with this ViewModel.</param>
		void AddDisposable(string key, IDisposable disposable);

		/// <summary>
		/// Removes a previously added (with <see cref="AddDisposable(string, IDisposable)"/>) disposable from this ViewModel.
		/// </summary>
		/// <param name="key">The key used to add the disposable to this ViewModel.</param>
		void RemoveDisposable(string key);

		/// <summary>
		/// Gets a disposable using its associated key.
		/// </summary>
		/// <param name="key">The key associated to a disposable.</param>
		/// <param name="disposable">When found, the disposable associated with the key. Null otherwise.</param>
		/// <returns>True if the key was found. False otherwise.</returns>
		bool TryGetDisposable(string key, out IDisposable disposable);

		/// <summary>
		/// Gets an enumerable of key-value pair containing each pair created via <see cref="AddDisposable(string, IDisposable)"/>.
		/// </summary>
		IEnumerable<KeyValuePair<string, IDisposable>> Disposables { get; }

		/// <summary>
		/// Gets or sets the view attached to this <see cref="IViewModel"/>.
		/// </summary>
		IViewModelView View { get; set; }

		/// <summary>
		/// Notifies when the <see cref="View"/> property changes.
		/// </summary>
		event Action<IViewModelView> ViewChanged;

		/// <summary>
		/// Gets the services available for this <see cref="IViewModel"/>.
		/// </summary>
		IServiceProvider ServiceProvider { get; }

		/// <summary>
		/// Sets the errors for the <see cref="IViewModel"/>.
		/// </summary>
		/// <param name="errors">
		/// The errors.
		/// The dictionary maps property names to their associated errors.
		/// </param>
		void SetErrors(IDictionary<string, IEnumerable<object>> errors);

		/// <summary>
		/// Sets the errors for a given property of the <see cref="IViewModel"/>.
		/// </summary>
		/// <param name="propertyName">The property name.</param>
		/// <param name="errors">The errors associated with the property.</param>
		void SetErrors(string propertyName, IEnumerable<object> errors);

		/// <summary>
		/// Clears the errors for the property specified with <paramref name="propertyName"/>.
		/// If no <paramref name="propertyName"/> is provided, clears all errors.
		/// </summary>
		/// <param name="propertyName">The property name.</param>
		void ClearErrors(string propertyName = null);
	}
}
