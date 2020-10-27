using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Chinook.DynamicMvvm.Tests.Helpers;
using Xunit;

namespace Chinook.DynamicMvvm.Tests.Command.Strategies
{
	public class CancelPreviousCommandStrategyTests
	{
		private const string DefaultCommandName = nameof(DefaultCommandName);

		[Fact]
		public async Task It_Cancels_Previous()
		{
			var tasks = new List<TaskCompletionSource<object>>();

			var testStrategy = new TestCommandStrategy(onExecute: async (ct, _, __) =>
			{
				var newTask = new TaskCompletionSource<object>();

				tasks.Add(newTask);

				using (ct.Register(() => newTask.TrySetCanceled()))
				{
					await newTask.Task;
				}
			});

			var strategy = new CancelPreviousCommandStrategy()
			{
				InnerStrategy = testStrategy
			};

			var command = new DynamicCommand(DefaultCommandName, strategy);

			// Start a first execution
			var firstExecution = command.Execute();
			var firstTask = tasks.ElementAt(0);

			firstTask.Task.IsCanceled.Should().BeFalse();

			// Start a second execution
			var secondExecution = command.Execute();
			var secondTask = tasks.ElementAt(1);

			firstTask.Task.IsCanceled.Should().BeTrue();
			secondTask.Task.IsCanceled.Should().BeFalse();

			// Start a third execution
			var thirdExecution = command.Execute();
			var thirdTask = tasks.ElementAt(2);

			secondTask.Task.IsCanceled.Should().BeTrue();
			thirdTask.Task.IsCanceled.Should().BeFalse();

			// Complete the third execution
			thirdTask.TrySetResult(null);

			await Task.WhenAll(firstExecution, secondExecution, thirdExecution);
		}
	}
}
