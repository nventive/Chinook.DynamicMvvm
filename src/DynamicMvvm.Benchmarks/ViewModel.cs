using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Chinook.DynamicMvvm;

namespace DynamicMvvm.Benchmarks;

public class ViewModel : ViewModelBase
{
	public ViewModel(string? name, IServiceProvider serviceProvider)
		: base(name, serviceProvider)
	{
	}

	public ViewModel(IServiceProvider serviceProvider)
		: this(name: default, serviceProvider: serviceProvider)
	{
	}
}
