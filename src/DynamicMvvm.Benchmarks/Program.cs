using BenchmarkDotNet.Running;
using DynamicMvvm.Benchmarks;
using Chinook.DynamicMvvm;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

BenchmarkRunner.Run<Benchmark>();

// The following section is to profile manually using Visual Studio's debugger.

//var serviceProvider = new HostBuilder()
//	.ConfigureServices(serviceCollection => serviceCollection
//		.AddSingleton<IDynamicCommandBuilderFactory, DynamicCommandBuilderFactory>()
//		.AddSingleton<IDynamicPropertyFactory, DynamicPropertyFactory>()
//	)
//	.Build()
//	.Services;

//var vm1 = new ViewModel("ViewModel", serviceProvider);
//var vm2 = new ViewModel("ViewModel", serviceProvider);

//Console.Read();
