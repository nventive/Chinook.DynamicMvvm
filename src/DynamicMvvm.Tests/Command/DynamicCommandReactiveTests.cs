using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Chinook.DynamicMvvm.Tests.Helpers;
using Xunit;

namespace Chinook.DynamicMvvm.Tests.Command
{
	public class DynamicCommandReactiveTests
	{
		private const string DefaultCommandName = nameof(DefaultCommandName);

		[Fact]
		public async Task It_Observes_IsExecuting()
		{
			var receivedValues = new List<bool>();
			var strategy = new TestCommandStrategy();

			var command = new DynamicCommand(DefaultCommandName, strategy);

			var testSubscriber = new TestSubscriber<bool>(onNext: t => receivedValues.Add(t));

			var subscription = command
				.ObserveIsExecuting()
				.Subscribe(testSubscriber);

			using (subscription)
			{
				await command.Execute();

				receivedValues.Count().Should().Be(2);
				receivedValues[0].Should().BeTrue();
				receivedValues[1].Should().BeFalse();
			}
		}

		[Fact]
		public async Task It_Gets_And_Observes_IsExecuting()
		{
			var receivedValues = new List<bool>();
			var strategy = new TestCommandStrategy();

			var command = new DynamicCommand(DefaultCommandName, strategy);

			var testSubscriber = new TestSubscriber<bool>(onNext: t => receivedValues.Add(t));

			var subscription = command
				.GetAndObserveIsExecuting()
				.Subscribe(testSubscriber);

			using (subscription)
			{
				await command.Execute();

				receivedValues.Count().Should().Be(3);
				receivedValues[0].Should().BeFalse();
				receivedValues[1].Should().BeTrue();
				receivedValues[2].Should().BeFalse();
			}
		}
	}
}
