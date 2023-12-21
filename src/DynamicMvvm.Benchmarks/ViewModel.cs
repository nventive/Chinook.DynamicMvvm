using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chinook.DynamicMvvm;

namespace DynamicMvvm.Benchmarks;

public class ViewModel : ViewModelBase
{
	public ViewModel(string? name, IServiceProvider serviceProvider)
		: base(name, serviceProvider)
	{
		var value = NumberResolved;
	}

	public ViewModel(IServiceProvider serviceProvider)
		: this(name: default, serviceProvider: serviceProvider)
	{
	}

	public int Number
	{
		get => this.Get(initialValue: 42);
		set => this.Set(value);
	}

	public int NumberResolved
	{
		get => this.Get(initialValue: 42);
		set => this.Set(value);
	}
}
