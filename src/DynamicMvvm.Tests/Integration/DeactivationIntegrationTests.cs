using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Chinook.DynamicMvvm;
using Chinook.DynamicMvvm.Deactivation;
using FluentAssertions;
using Xunit;

namespace Chinook.DynamicMvvm.Tests.Integration
{
	public class DeactivationIntegrationTests
	{
		[Fact]
		public void DeactivatableViewModelBase_works_with_DeactivatablePropertyFromObservable()
		{
			const string propertyName = nameof(TestVM.Count);
			var countChanged = false;
			var sut = new TestVM();

			sut.PropertyChanged += OnPropertyChanged;
			sut.Count.Should().Be(0);

			// Update while activated. Updates should happen.
			sut.CountSubject.OnNext(1);
			ReadCountChanged().Should().BeTrue();
			sut.CountSubject.HasObservers.Should().BeTrue();
			sut.Count.Should().Be(1);

			// Deactivate. Updates should not happen and observable should not be subscribed to.
			sut.Deactivate();
			sut.CountSubject.HasObservers.Should().BeFalse();

			// Update while deactivated. 
			sut.CountSubject.OnNext(2);
			sut.Count.Should().Be(1);
			ReadCountChanged().Should().BeFalse();

			// Reactivate. The replay subject should be subscribed to and push an update.
			sut.Reactivate();
			ReadCountChanged().Should().BeTrue();
			sut.CountSubject.HasObservers.Should().BeTrue();
			sut.Count.Should().Be(2);

			void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
			{
				if (e.PropertyName == propertyName)
				{
					countChanged = true;
				}
			}

			bool ReadCountChanged()
			{
				var value = countChanged;
				countChanged = false;
				return value;
			}
		}

		public class TestVM : DeactivatableViewModelBase
		{
			public ReplaySubject<int> CountSubject = new ReplaySubject<int>(bufferSize: 1);

			public int Count => this.GetFromDeactivatableObservable<int>(CountSubject, initialValue: 0);
		}
	}
}
