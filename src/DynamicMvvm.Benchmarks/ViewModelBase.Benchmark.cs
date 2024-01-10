using BenchmarkDotNet.Attributes;
using Chinook.DynamicMvvm;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DynamicMvvm.Benchmarks;

[MemoryDiagnoser]
[MaxIterationCount(36)]
[MaxWarmupCount(16)]
public class ViewModelBaseBenchmark
{
	private readonly IServiceProvider _serviceProvider = new HostBuilder()
		.ConfigureServices(serviceCollection => serviceCollection
			.AddSingleton<IDynamicCommandBuilderFactory, DynamicCommandBuilderFactory>()
			.AddSingleton<IDynamicPropertyFactory, DynamicPropertyFactory>()
		)
		.Build()
		.Services;

	private const int ViewModelCount = 2500000;
	private ViewModel[]? _viewModelsToDispose;

	[Benchmark]
	public IViewModel CreateViewModel()
	{
		return new ViewModel(_serviceProvider);
	}

	[Benchmark]
	public IViewModel CreateViewModel_WithExplicitName()
	{
		return new ViewModel("ViewModel", _serviceProvider);
	}

	[IterationSetup(Targets = new[]
	{
		nameof(DisposeViewModel),
	})]
	public void SetupViewModel()
	{
		_viewModelsToDispose = Enumerable
			.Range(0, ViewModelCount)
			.Select(i => new ViewModel("ViewModel", _serviceProvider))
			.ToArray();
	}

	[Benchmark(OperationsPerInvoke = ViewModelCount)]
	[MaxIterationCount(16)]
	public void DisposeViewModel()
	{
		for (var i = 0; i < ViewModelCount; i++)
		{
			_viewModelsToDispose![i].Dispose();
		}
	}
}
