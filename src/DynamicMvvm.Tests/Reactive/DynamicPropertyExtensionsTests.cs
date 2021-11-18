using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Chinook.DynamicMvvm.Tests.Reactive
{
	public class DynamicPropertyExtensionsTests
	{
		[Fact]
		public async Task The_first_value_of_GetAndObserve_is_the_current_value_of_the_property()
		{
			var property = new DynamicProperty<int>("TestProperty", value: 0);
			var observable = property.GetAndObserve();

			var firstValue = await observable.FirstAsync();
			firstValue.Should().Be(0);

			property.Value = 1;

			var secondValue = await observable.FirstAsync();
			secondValue.Should().Be(1);
		}

		[Fact]
		public async Task Observe_doesnt_yield_until_property_changes()
		{
			var property = new DynamicProperty<int>("TestProperty", value: 0);
			var observable = property.Observe();

			var task = observable.FirstAsync().ToTask();

			Assert.True(task.Status == TaskStatus.Running
				|| task.Status == TaskStatus.WaitingForActivation);

			property.Value = 1;

			var value = await task;
			value.Should().Be(1);
		}
	}
}
