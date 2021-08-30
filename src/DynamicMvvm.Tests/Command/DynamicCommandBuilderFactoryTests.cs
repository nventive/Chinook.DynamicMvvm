using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Chinook.DynamicMvvm.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace Chinook.DynamicMvvm.Tests.Command
{
	public class DynamicCommandBuilderFactoryTests
	{
		private const string DefaultCommandName = nameof(DefaultCommandName);

		[Fact]
		public async Task It_Creates_From_Action()
		{
			var isExecuted = false;
			var factory = new DynamicCommandBuilderFactory();

			var command = factory.CreateFromAction(DefaultCommandName, () => isExecuted = true).Build();

			await command.Execute();

			isExecuted.Should().BeTrue();
		}

		[Fact]
		public async Task It_Creates_From_Action_T()
		{
			var receivedParameter = default(TestParameter);
			var factory = new DynamicCommandBuilderFactory();

			var command = factory.CreateFromAction<TestParameter>(DefaultCommandName, p => receivedParameter = p).Build();

			var parameter = new TestParameter();
			await command.Execute(parameter);

			receivedParameter.Should().Be(parameter);
		}

		[Fact]
		public async Task It_Creates_From_Task()
		{
			var isExecuted = false;
			var factory = new DynamicCommandBuilderFactory();

			var command = factory.CreateFromTask(DefaultCommandName, async ct => isExecuted = true).Build();

			await command.Execute();

			isExecuted.Should().BeTrue();
		}

		[Fact]
		public async Task It_Creates_From_Task_T()
		{
			var receivedParameter = default(TestParameter);
			var factory = new DynamicCommandBuilderFactory();

			var command = factory.CreateFromTask<TestParameter>(DefaultCommandName, async (ct, p) => receivedParameter = p).Build();

			var parameter = new TestParameter();
			await command.Execute(parameter);

			receivedParameter.Should().Be(parameter);
		}

		[Fact]
		public async Task It_Decorates_Using_Global()
		{
			var decoratorCalled = false;

			var factory = new DynamicCommandBuilderFactory(Configure);

			var command = factory.CreateFromAction(DefaultCommandName, () => { }).Build();

			await command.Execute();

			decoratorCalled.Should().BeTrue();

			IDynamicCommandBuilder Configure(IDynamicCommandBuilder builder)
			{
				decoratorCalled = true;
				return builder;
			}
		}

		[Fact]
		public async Task It_Decorates_Using_Local()
		{
			var testString = "";

			var s1 = new TestStrategy(() => testString += "1");
			var s2 = new TestStrategy(() => testString += "2");
			var s3 = new TestStrategy(() => testString += "3");

			var factory = new DynamicCommandBuilderFactory();

			var command = factory.CreateFromAction(DefaultCommandName, () => { })
				.WithStrategy(s1)
				.WithStrategy(s2)
				.WithStrategy(s3)
				.Build();

			await command.Execute();

			testString.Should().Be("123");
		}

		[Fact]
		public async Task It_applies_strategies_in_order()
		{
			var testString = "";

			var s1 = new TestStrategy(() => testString += "1");
			var s2 = new TestStrategy(() => testString += "2");
			var s3 = new TestStrategy(() => testString += "3");

			var factory = new DynamicCommandBuilderFactory(b => b
				.WithStrategy(s1)
			);

			var command = factory.CreateFromAction(DefaultCommandName, () => { })
				.WithStrategy(s2)
				.WithStrategy(s3)
				.Build();

			await command.Execute();

			testString.Should().Be("123");
		}

		[Fact]
		public async Task It_applies_strategies_in_order2()
		{
			var testString = "";

			var s1 = new TestStrategy(() => testString += "1");
			var s2 = new TestStrategy(() => testString += "2");
			var s3 = new TestStrategy(() => testString += "3");

			var factory = new DynamicCommandBuilderFactory(b => b
				.WithStrategy(s2)
			);

			var command = factory.CreateFromAction(DefaultCommandName, () => { })
				.WithStrategy(s3)
				.WithStrategy(s1, wrapExisting: true)
				.Build();

			await command.Execute();

			testString.Should().Be("123");
		}

		[Fact]
		public void It_passes_ViewModel_owner_correctly()
		{
			var owner = new TestViewModel();

			var builder = new DynamicCommandBuilder("myCommand", new ActionCommandStrategy(() => { }), owner); ;

			builder.ViewModel.Should().Be(owner);
		}

		[Fact]
		public void It_passes_ViewModel_owner_correctly_when_null()
		{
			var owner = default(IViewModel);

			var builder = new DynamicCommandBuilder("myCommand", new ActionCommandStrategy(() => { }), owner); ;

			builder.ViewModel.Should().BeNull();
		}

		private class TestParameter { }

		private class TestStrategy : DecoratorCommandStrategy
		{
			private readonly Action _testAction;

			public TestStrategy(Action testAction)
			{
				_testAction = testAction;
			}

			public override Task Execute(CancellationToken ct, object parameter, IDynamicCommand command)
			{
				_testAction();

				return base.Execute(ct, parameter, command);
			}
		}
	}
}
