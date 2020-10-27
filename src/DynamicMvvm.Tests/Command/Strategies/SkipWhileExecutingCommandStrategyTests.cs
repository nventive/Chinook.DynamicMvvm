using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FluentAssertions;
using Chinook.DynamicMvvm.Tests.Helpers;
using Xunit;

namespace Chinook.DynamicMvvm.Tests.Command.Strategies
{
	public class SkipWhileExecutingCommandStrategyTests
	{
		private const string DefaultCommandName = nameof(DefaultCommandName);

		[Fact]
		public async Task It_Skips_While_Executing()
		{
			var actualExecutions = 0;

			var tasks = new[]
			{
				new TaskCompletionSource<object>(),
				new TaskCompletionSource<object>(),
				new TaskCompletionSource<object>(),
				new TaskCompletionSource<object>(),
			};

			var testStrategy = new TestCommandStrategy(
				onExecute: async (_, i, ___) =>
				{
					actualExecutions++;
					
					await tasks[(int)i].Task;
				}
			);


			var strategy = new SkipWhileExecutingCommandStrategy()
			{
				InnerStrategy = testStrategy
			};

			var command = new DynamicCommand(DefaultCommandName, strategy);

			var executions = tasks
				.Select((t, i) => command.Execute(i))
				.ToArray();

			Array.ForEach(tasks, t => t.TrySetResult(null));

			await Task.WhenAll(executions);

			actualExecutions.Should().Be(1);
		}
	}
}
