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
	public class DisableWhileExecutingCommandStrategyTests
	{
		private const string DefaultCommandName = nameof(DefaultCommandName);

		[Fact]
		public async Task It_Disables_While_Executing()
		{
			var taskCompletionSource = new TaskCompletionSource<object>();

			var testStrategy = new TestCommandStrategy(
				onExecute: (_, __, ___) => taskCompletionSource.Task
			);

			var strategy = testStrategy.DisableWhileExecuting();

			var command = new DynamicCommand(DefaultCommandName, strategy);

			command.CanExecuteChanged += OnCanExecuteChanged;

			var canExecute = command.CanExecute(null);

			// The command should be enabled
			canExecute.Should().BeTrue();

			// We execute the command
			var commandExecution = command.Execute();

			// The command should be disabled
			canExecute.Should().BeFalse();

			// The command completes
			taskCompletionSource.TrySetResult(null);

			await commandExecution;

			// The command should be enabled
			canExecute.Should().BeTrue();

			void OnCanExecuteChanged(object sender, EventArgs e)
			{
				canExecute = command.CanExecute(null);
			}
		}
	}
}
