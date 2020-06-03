using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Chinook.DynamicMvvm.Tests.ViewModel
{
	public class ViewModelBaseServicesTests : IDisposable
	{
		[Fact]
		public void It_Resolves_Service_When_ServiceProvider_From_Constructor()
		{
			var service = new MyService();

			var serviceCollection = new ServiceCollection();
			serviceCollection.AddSingleton(s => service);

			var serviceProvider = serviceCollection.BuildServiceProvider();

			var viewModel = new ViewModelBase(serviceProvider: serviceProvider);

			var receivedService = viewModel.GetService<MyService>();

			receivedService.Should().Be(service);
		}

		[Fact]
		public void It_Resolves_Service_When_ServiceProvider_From_Default()
		{
			var service = new MyService();

			var serviceCollection = new ServiceCollection();
			serviceCollection.AddSingleton(s => service);

			var serviceProvider = serviceCollection.BuildServiceProvider();

			ViewModelBase.DefaultServiceProvider = serviceProvider;

			var viewModel = new ViewModelBase();

			var receivedService = viewModel.GetService<MyService>();

			receivedService.Should().Be(service);
		}

		[Fact]
		public void It_Doesnt_Resolve_If_No_ServiceProvider()
		{
			var viewModel = new ViewModelBase();

			Assert.ThrowsAny<Exception>(() => viewModel.GetService<MyService>());
		}

		[Fact]
		public void It_Resolves_Using_Parameter()
		{
			var service = new MyService();

			var serviceCollection = new ServiceCollection();
			serviceCollection.AddSingleton(s => service);

			var serviceProvider = serviceCollection.BuildServiceProvider();

			var viewModel = new ViewModelBase(serviceProvider: serviceProvider);

			var receivedService = viewModel.GetService(typeof(MyService));

			receivedService.Should().Be(service);
		}

		public void Dispose()
		{
			ViewModelBase.DefaultServiceProvider = null;
		}

		private class MyService { }
	}
}
