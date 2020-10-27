using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Chinook.DynamicMvvm.Tests.Helpers;
using Xunit;

namespace Chinook.DynamicMvvm.Tests.ViewModel
{
	public class ViewModelBaseCommandsTest
	{
		private readonly IServiceProvider _serviceProvider;

		public ViewModelBaseCommandsTest()
		{
			var serviceCollection = new ServiceCollection();

			serviceCollection.AddSingleton<IDynamicCommandBuilderFactory, DynamicCommandBuilderFactory>();

			_serviceProvider = serviceCollection.BuildServiceProvider();
		}

		[Fact]
		public async Task It_Gets_From_Action()
		{
			var isExecuted = false;
			var viewModel = new ViewModelBase(serviceProvider: _serviceProvider);

			var command = viewModel.GetCommand(() => isExecuted = true);
			await command.Execute();

			isExecuted.Should().BeTrue();
		}

		[Fact]
		public async Task It_Gets_From_Action_T()
		{
			var receivedParameter = default(TestEntity);
			var viewModel = new ViewModelBase(serviceProvider: _serviceProvider);

			var command = viewModel.GetCommand<TestEntity>(p => receivedParameter = p);

			var parameter = new TestEntity();
			await command.Execute(parameter);

			receivedParameter.Should().Be(parameter);
		}

		[Fact]
		public async Task It_Gets_From_Task()
		{
			var isExecuted = false;
			var viewModel = new ViewModelBase(serviceProvider: _serviceProvider);

			var command = viewModel.GetCommandFromTask(async ct => isExecuted = true);

			await command.Execute();

			isExecuted.Should().BeTrue();
		}

		[Fact]
		public async Task It_Gets_From_Task_T()
		{
			var receivedParameter = default(TestEntity);
			var viewModel = new ViewModelBase(serviceProvider: _serviceProvider);

			var command = viewModel.GetCommandFromTask<TestEntity>(async (ct, p) => receivedParameter = p);

			var parameter = new TestEntity();
			await command.Execute(parameter);

			receivedParameter.Should().Be(parameter);
		}

		[Fact]
		public void It_Adds_Disposable()
		{
			var viewModel = new ViewModelBase(serviceProvider: _serviceProvider);

			var command = viewModel.GetCommand(() => { });

			var isAdded = viewModel.TryGetDisposable(nameof(It_Adds_Disposable), out var _);

			isAdded.Should().BeTrue();
		}
	}
}
