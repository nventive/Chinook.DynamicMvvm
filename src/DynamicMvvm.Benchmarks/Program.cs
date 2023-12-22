using BenchmarkDotNet.Running;
using DynamicMvvm.Benchmarks;
using Chinook.DynamicMvvm;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;

BenchmarkRunner.Run(new[]
	{
		typeof(ViewModelBaseBenchmark),
		typeof(ViewModelExtensionsBenchmark),
	},
	ManualConfig
		.Create(DefaultConfig.Instance)
		.WithOptions(ConfigOptions.JoinSummary)
		.WithOrderer(new DefaultOrderer(SummaryOrderPolicy.Declared, MethodOrderPolicy.Declared))
		.HideColumns("Type", "Job", "InvocationCount", "UnrollFactor", "Error", "StdDev", "MaxIterationCount", "MaxWarmupIterationCount")
);

// The following section is to profile manually using Visual Studio's debugger.

//Console.ReadKey();

//var serviceProvider = new HostBuilder()
//	.ConfigureServices(serviceCollection => serviceCollection
//		.AddSingleton<IDynamicCommandBuilderFactory, DynamicCommandBuilderFactory>()
//		.AddSingleton<IDynamicPropertyFactory, DynamicPropertyFactory>()
//	)
//	.Build()
//	.Services;

//var vm = new InitiatedViewModel();
//vm.Number = 1;

//var vm1 = new ViewModel("ViewModel", serviceProvider);
//var vm2 = new ViewModel("ViewModel", serviceProvider);
//var value = vm1.NumberResolved;
//value = vm1.Number;
//Console.WriteLine(value);

//Console.Read();
