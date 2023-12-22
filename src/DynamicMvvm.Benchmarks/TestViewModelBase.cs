using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chinook.DynamicMvvm;
using Microsoft.Extensions.DependencyInjection;

namespace DynamicMvvm.Benchmarks
{
	/// <summary>
	/// This implementation of IViewModel is used for testing the extension methods of IViewModel.
	/// It's not a valid implementation of IViewModel.
	/// </summary>
	public class TestViewModelBase : IViewModel
	{
		public TestViewModelBase(IServiceProvider? serviceProvider = null)
		{
			ServiceProvider = serviceProvider;
		}

		public string Name => "TestViewModelBase";

		public virtual IEnumerable<KeyValuePair<string, IDisposable>> Disposables => Enumerable.Empty<KeyValuePair<string, IDisposable>>();

		public IDispatcher? Dispatcher { get; set; }

		public IServiceProvider? ServiceProvider { get; set; }

		public bool IsDisposed { get; set; }

		public bool HasErrors => false;

		public event Action<IDispatcher>? DispatcherChanged;
		public event PropertyChangedEventHandler? PropertyChanged;
		public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

		public virtual void AddDisposable(string key, IDisposable disposable)
		{
		}

		public void ClearErrors(string? propertyName = null)
		{
		}

		public void Dispose()
		{
		}

		public IEnumerable GetErrors(string? propertyName)
		{
			return Enumerable.Empty<object>();
		}

		public void RaisePropertyChanged(string propertyName)
		{
		}

		public void RemoveDisposable(string key)
		{
		}

		public void SetErrors(IDictionary<string, IEnumerable<object>> errors)
		{
		}

		public void SetErrors(string propertyName, IEnumerable<object> errors)
		{
		}

		public virtual bool TryGetDisposable(string key, out IDisposable? disposable)
		{
			disposable = default;
			return false;
		}
	}
}
