using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Chinook.DynamicMvvm;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DynamicMvvm.Benchmarks;

[MemoryDiagnoser]
public class Benchmark
{
	private readonly IServiceProvider _serviceProvider = new HostBuilder()
		.ConfigureServices(serviceCollection => serviceCollection
			.AddSingleton<IDynamicCommandBuilderFactory, DynamicCommandBuilderFactory>()
			.AddSingleton<IDynamicPropertyFactory, DynamicPropertyFactory>()
		)
		.Build()
		.Services;

	private ViewModel? _viewModel1;
	private ViewModel? _viewModel2;

	[Benchmark]
	public void CreateViewModel()
	{
		var vm = new ViewModel(_serviceProvider);
	}

	[Benchmark]
	public void CreateViewModel_WithExplicitName()
	{
		var vm = new ViewModel("ViewModel", _serviceProvider);
	}

	[IterationSetup(Targets = new[]
	{
		nameof(ReadProperty_Unresolved),
		nameof(ReadProperty_Resolved),
		nameof(SetProperty_Unresolved),
		nameof(SetProperty_Resolved),
		nameof(DisposeViewModel),
	})]
	public void SetupViewModel()
	{
		_viewModel1 = new ViewModel("ViewModel", _serviceProvider);
	}

	[IterationSetup(Targets = new[]
	{
		nameof(SetProperty_Unresolved_WithListener),
		nameof(SetProperty_Resolved_WithListener),
		nameof(DisposeViewModel_WithListener),
	})]
	public void SetupViewModelWithListener()
	{
		_viewModel2 = new ViewModel("ViewModel", _serviceProvider);
		_viewModel2.PropertyChanged += (s, e) => { };
	}

	[Benchmark]
	public void ReadProperty_Unresolved()
	{
		var value = _viewModel1!.Number;
	}

	[Benchmark]
	public void ReadProperty_Resolved()
	{
		var value = _viewModel1!.NumberResolved;
	}

	[Benchmark]
	public void SetProperty_Unresolved()
	{
		_viewModel1!.Number = 1;
	}

	[Benchmark]
	public void SetProperty_Resolved()
	{
		_viewModel1!.NumberResolved = 1;
	}

	[Benchmark]
	public void DisposeViewModel()
	{
		_viewModel1!.Dispose();
	}

	[Benchmark]
	public void SetProperty_Unresolved_WithListener()
	{
		_viewModel2!.Number = 1;
	}

	[Benchmark]
	public void SetProperty_Resolved_WithListener()
	{
		_viewModel2!.NumberResolved = 1;
	}

	[Benchmark]
	public void DisposeViewModel_WithListener()
	{
		_viewModel2!.Dispose();
	}
}
