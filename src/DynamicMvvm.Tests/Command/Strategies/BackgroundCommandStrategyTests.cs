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
	public class BackgroundCommandStrategyTests
	{
		private const string DefaultCommandName = nameof(DefaultCommandName);

		[Fact]
		public async Task It_Executes_On_Background_Thread()
		{
			var isRunningOnThreadPool = false;

			var testStrategy = new TestCommandStrategy(
				onExecute: (_, __, ___) =>
				{
					isRunningOnThreadPool = Thread.CurrentThread.IsThreadPoolThread;

					return Task.CompletedTask;
				}
			);

			var strategy = testStrategy.OnBackgroundThread();

			var command = new DynamicCommand(DefaultCommandName, strategy);

			await command.Execute();

			isRunningOnThreadPool.Should().BeTrue();
		}
	}
}
