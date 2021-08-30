using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Chinook.DynamicMvvm.Tests.Command
{
	public class DynamicCommandBuilderExtensionsTests
	{
		[Fact]
		public void WithStrategy_properly_adds()
		{
			var builder = new DynamicCommandBuilder("myCommand", new ActionCommandStrategy(() => { }), null);

			builder.Strategies.Should().BeEmpty();

			builder.WithStrategy(new BackgroundCommandStrategy());

			builder.Strategies.Should().ContainSingle();
		}

		[Fact]
		public void WithoutStrategy_properly_removes()
		{
			var builder = new DynamicCommandBuilder("myCommand", new ActionCommandStrategy(() => { }), null)
				.OnBackgroundThread()
				.Locked()
				.DisableWhileExecuting();

			builder.Strategies.Should().HaveCount(3);

			builder.WithoutStrategy<BackgroundCommandStrategy>();

			builder.Strategies.Should().HaveCount(2);
		}

		[Fact]
		public void ClearStrategies_properly_clears()
		{
			var builder = new DynamicCommandBuilder("myCommand", new ActionCommandStrategy(() => { }), null)
				.OnBackgroundThread()
				.Locked()
				.DisableWhileExecuting();


			builder.Strategies.Should().NotBeEmpty();

			builder.ClearStrategies();

			builder.Strategies.Should().BeEmpty();
		}		
	}
}
