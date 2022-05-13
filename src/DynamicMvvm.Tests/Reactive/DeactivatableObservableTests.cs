using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Chinook.DynamicMvvm.Deactivation;
using FluentAssertions;
using Xunit;

namespace Chinook.DynamicMvvm.Tests.Reactive
{
	public class DeactivatableObservableTests
	{
		[Fact]
		public void Updates_are_paused_when_deactivated()
		{
			var updateCount = 0;
			var subject = new ReplaySubject<int>(bufferSize: 1);
			var sut = new DeactivatableObservable<int>(subject);

			sut.Subscribe(OnNext);

			updateCount.Should().Be(0);
			subject.OnNext(0);
			updateCount.Should().Be(1);

			sut.Deactivate();

			subject.OnNext(0);
			// The previous update should not go through because the observable is deactivated.
			updateCount.Should().Be(1);

			sut.Reactivate();
			// When we reactivate to the ReplaySubject, we should get an update (because it replays).
			updateCount.Should().Be(2);

			void OnNext(int obj)
			{
				++updateCount;
			}
		}
	}
}
