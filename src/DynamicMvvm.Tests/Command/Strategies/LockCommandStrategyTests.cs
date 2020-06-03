using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using FluentAssertions;
using Chinook.DynamicMvvm.Tests.Helpers;
using Xunit;

namespace Chinook.DynamicMvvm.Tests.Command.Strategies
{
	public class LockCommandStrategyTests
	{
		private const string DefaultCommandName = nameof(DefaultCommandName);

		[Fact]
		public async Task It_Has_Single_Execution()
		{
			var concurrentExecutions = 0;
			var hadConcurrentExecutions = false;

			var tasks = new[]
			{
				new TaskCompletionSource<object>(),
				new TaskCompletionSource<object>(),
				new TaskCompletionSource<object>(),
				new TaskCompletionSource<object>(),
			};

			var testStrategy = new TestCommandStrategy(onExecute: async (_, i, ___) =>
			{
				if (Interlocked.Increment(ref concurrentExecutions) > 1)
				{
					hadConcurrentExecutions = true;
				}

				await tasks[(int)i].Task;

				Interlocked.Decrement(ref concurrentExecutions);
			});

			var strategy = testStrategy.Locked();

			var command = new DynamicCommand(DefaultCommandName, strategy);

			var executions = tasks
				.Select((t, i) => command.Execute(i))
				.ToArray();

			Array.ForEach(tasks, t => t.TrySetResult(null));

			await Task.WhenAll(executions);

			hadConcurrentExecutions.Should().BeFalse();
		}
	}
}
