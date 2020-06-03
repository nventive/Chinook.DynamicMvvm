using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Chinook.DynamicMvvm.Tests.Command
{
	public class DynamicCommandFactoryTests
	{
		private const string DefaultCommandName = nameof(DefaultCommandName);

		[Fact]
		public async Task It_Creates_From_Action()
		{
			var isExecuted = false;
			var factory = new DynamicCommandFactory();

			var command = factory.CreateFromAction(DefaultCommandName, () => isExecuted = true);

			await command.Execute();

			isExecuted.Should().BeTrue();
		}

		[Fact]
		public async Task It_Creates_From_Action_T()
		{
			var receivedParameter = default(TestParameter);
			var factory = new DynamicCommandFactory();

			var command = factory.CreateFromAction<TestParameter>(DefaultCommandName, p => receivedParameter = p);

			var parameter = new TestParameter();
			await command.Execute(parameter);

			receivedParameter.Should().Be(parameter);
		}

		[Fact]
		public async Task It_Creates_From_Task()
		{
			var isExecuted = false;
			var factory = new DynamicCommandFactory();

			var command = factory.CreateFromTask(DefaultCommandName, async ct => isExecuted = true);

			await command.Execute();

			isExecuted.Should().BeTrue();
		}

		[Fact]
		public async Task It_Creates_From_Task_T()
		{
			var receivedParameter = default(TestParameter);
			var factory = new DynamicCommandFactory();

			var command = factory.CreateFromTask<TestParameter>(DefaultCommandName, async (ct, p) => receivedParameter = p);

			var parameter = new TestParameter();
			await command.Execute(parameter);

			receivedParameter.Should().Be(parameter);
		}

		[Fact]
		public async Task It_Decorates_Using_Global()
		{
			var decoratorCalled = false;

			var decorator = new DynamicCommandStrategyDecorator(s =>
			{
				decoratorCalled = true;

				return s;
			});

			var factory = new DynamicCommandFactory(decorator);

			var command = factory.CreateFromAction(DefaultCommandName, () => { });

			await command.Execute();

			decoratorCalled.Should().BeTrue();
		}

		[Fact]
		public async Task It_Decorates_Using_Local()
		{
			var decoratorCalled = false;

			var decorator = new DynamicCommandStrategyDecorator(s =>
			{
				decoratorCalled = true;

				return s;
			});

			var factory = new DynamicCommandFactory();

			var command = factory.CreateFromAction(DefaultCommandName, () => { }, decorator);

			await command.Execute();

			decoratorCalled.Should().BeTrue();
		}

		[Fact]
		public async Task It_Decorates_Using_Local_Then_Global()
		{
			var isGlobalDecoratorCalled = false;
			var isLocalDecoratorCalled = false;
			var isLocalCalledBeforeGlobal = false;

			var globalDecorator = new DynamicCommandStrategyDecorator(s =>
			{
				isGlobalDecoratorCalled = true;

				return s;
			});

			var factory = new DynamicCommandFactory(globalDecorator);

			var localDecorator = new DynamicCommandStrategyDecorator(s =>
			{
				isLocalDecoratorCalled = true;

				if (!isGlobalDecoratorCalled)
				{
					isLocalCalledBeforeGlobal = true;
				}

				return s;
			});

			var command = factory.CreateFromAction(DefaultCommandName, () => { }, localDecorator);

			await command.Execute();

			isGlobalDecoratorCalled.Should().BeTrue();
			isLocalDecoratorCalled.Should().BeTrue();
			isLocalCalledBeforeGlobal.Should().BeTrue();
		}

		private class TestParameter { }
	}
}
