using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Chinook.DynamicMvvm.Tests.Command.Strategies
{
	public class ActionCommandStrategyTests
	{
		private const string DefaultCommandName = nameof(DefaultCommandName);

		[Fact]
		public async Task It_Executes_Without_Parameter()
		{
			var isExecuted = false;

			var strategy = new ActionCommandStrategy(() => isExecuted = true);
			var command = new DynamicCommand(DefaultCommandName, strategy);

			await command.Execute();

			isExecuted.Should().BeTrue();
		}

		[Fact]
		public async Task It_Executes_With_Parameter()
		{
			var canExecuteStrategyParameter = default(object);
			var executeStrategyParameter = default(object);

			var strategy = new ActionCommandStrategy(
				execute: p => executeStrategyParameter = p,
				canExecute: p => { canExecuteStrategyParameter = p; return true; }
			);

			var command = new DynamicCommand(DefaultCommandName, strategy);

			var parameter = new object();
			await command.Execute(parameter);

			canExecuteStrategyParameter.Should().Be(parameter);
			executeStrategyParameter.Should().Be(parameter);
		}

		[Fact]
		public async Task It_Executes_With_Parameter_T()
		{
			var canExecuteStrategyParameter = default(TestParameter);
			var executeStrategyParameter = default(TestParameter);

			var strategy = new ActionCommandStrategy<TestParameter>(
				execute: p => executeStrategyParameter = p,
				canExecute: p => { canExecuteStrategyParameter = p; return true; }
			);

			var command = new DynamicCommand(DefaultCommandName, strategy);

			var parameter = new TestParameter();
			await command.Execute(parameter);

			canExecuteStrategyParameter.Should().Be(parameter);
			executeStrategyParameter.Should().Be(parameter);
		}

		[Fact]
		public async Task It_Doesnt_Execute_When_CantExecute()
		{
			var isExecuted = false;

			var strategy = new ActionCommandStrategy(
				execute: () => isExecuted = true,
				canExecute: _ => false
			);

			var command = new DynamicCommand(DefaultCommandName, strategy);

			await command.Execute();

			isExecuted.Should().BeFalse();
		}

		[Fact]
		public async Task It_Doesnt_Execute_When_CantExecute_T()
		{
			var isExecuted = false;

			var strategy = new ActionCommandStrategy<TestParameter>(
				execute: p => isExecuted = true,
				canExecute: _ => false
			);

			var command = new DynamicCommand(DefaultCommandName, strategy);

			await command.Execute();

			isExecuted.Should().BeFalse();
		}

		private class TestParameter {	}
	}
}
