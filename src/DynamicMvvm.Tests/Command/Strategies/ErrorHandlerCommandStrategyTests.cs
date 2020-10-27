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
	public class ErrorHandlerCommandStrategyTests
	{
		private const string DefaultCommandName = nameof(DefaultCommandName);

		[Fact]
		public async Task It_Catches_Errors_If_Any()
		{
			var exception = new MyCustomException();
			var receivedCommand = default(IDynamicCommand);
			var receivedException = default(Exception);

			var testStrategy = new TestCommandStrategy(
				onExecute: (_, __, ___) => throw exception
			);

			var strategy = new ErrorHandlerCommandStrategy(new DynamicCommandErrorHandler((ct, c, e) =>
			{
				receivedCommand = c;
				receivedException = e;

				return Task.CompletedTask;
			}))
			{
				InnerStrategy = testStrategy
			};

			var command = new DynamicCommand(DefaultCommandName, strategy);

			await command.Execute();

			receivedCommand.Should().Be(command);
			receivedException.Should().Be(exception);
		}

		[Fact]
		public async Task It_Doesnt_Catch_Errors_If_None()
		{
			var catchedErrors = false;

			var testStrategy = new TestCommandStrategy();

			var strategy = new ErrorHandlerCommandStrategy(new DynamicCommandErrorHandler((ct, c, e) =>
			{
				catchedErrors = true;

				return Task.CompletedTask;
			}))
			{
				InnerStrategy = testStrategy
			};

			var command = new DynamicCommand(DefaultCommandName, strategy);

			await command.Execute();

			catchedErrors.Should().BeFalse();
		}

		private class MyCustomException : Exception { }
	}
}
