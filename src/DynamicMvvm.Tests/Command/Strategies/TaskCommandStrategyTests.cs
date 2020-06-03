using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using FluentAssertions;
using Xunit;

namespace Chinook.DynamicMvvm.Tests.Command.Strategies
{
	public class TaskCommandStrategyTests
	{
		private const string DefaultCommandName = nameof(DefaultCommandName);

		[Fact]
		public async Task It_Executes_Without_Parameter()
		{
			var isExecuted = false;

			var strategy = new TaskCommandStrategy(ct =>
			{
				isExecuted = true;

				return Task.CompletedTask;
			});

			var command = new DynamicCommand(DefaultCommandName, strategy);

			await command.Execute();

			isExecuted.Should().BeTrue();
		}

		[Fact]
		public async Task It_Executes_With_Parameter()
		{
			var canExecuteStrategyParameter = default(object);
			var executeStrategyParameter = default(object);

			var strategy = new TaskCommandStrategy(
				execute: (ct, p) =>
				{
					executeStrategyParameter = p;

					return Task.CompletedTask;
				},
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

			var strategy = new TaskCommandStrategy<TestParameter>(
				execute: (ct, p) =>
				{
					executeStrategyParameter = p;

					return Task.CompletedTask;
				},
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

			var strategy = new TaskCommandStrategy(
				execute: ct =>
				{
					isExecuted = true;

					return Task.CompletedTask;
				},
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

			var strategy = new TaskCommandStrategy<TestParameter>(
				execute: (ct, p) =>
				{
					isExecuted = true;

					return Task.CompletedTask;
				},
				canExecute: _ => false
			);

			var command = new DynamicCommand(DefaultCommandName, strategy);

			await command.Execute();

			isExecuted.Should().BeFalse();
		}

		private class TestParameter { }
	}
}
