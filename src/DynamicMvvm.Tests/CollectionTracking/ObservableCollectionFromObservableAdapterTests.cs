using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Chinook.DynamicMvvm.CollectionTracking;
using FluentAssertions;
using Xunit;

namespace Chinook.DynamicMvvm.Tests.CollectionTracking
{
	public class ObservableCollectionFromObservableAdapterTests
	{
		public static IEnumerable<object[]> InitialValues =>
			new[]
			{
				new []{ Enumerable.Empty<string>() },
				new []{ new string[] { "Hello", "Bonjour" } },
				new []{ Enumerable.Range(0,10).Select(i => i.ToString()) },
			};

		[Theory]
		[MemberData(nameof(InitialValues))]
		public void Initializes_with_initial_value(IEnumerable<string> initialValue)
		{
			var vm = new ViewModelBase();
			var subject = new Subject<IEnumerable<string>>();

			var sut = new ObservableCollectionFromObservableAdapter<string>(vm, subject, initialValue);

			sut.Collection.Should().BeEquivalentTo(initialValue);
			sut.ReadOnlyCollection.Should().BeEquivalentTo(initialValue);
		}

		[Fact]
		public void Updates_when_observable_pushes()
		{
			// Arrange
			var vm = new ViewModelBase();
			var subject = new Subject<IEnumerable<string>>();
			var sut = new ObservableCollectionFromObservableAdapter<string>(vm, subject, initialValue: Enumerable.Empty<string>());

			// Act
			subject.OnNext(new[] { "Hello" });

			// Assert
			var expected = new [] { "Hello" };
			sut.Collection.Should().BeEquivalentTo(expected);
			sut.ReadOnlyCollection.Should().BeEquivalentTo(expected);
		}

		[Fact]
		public void Pushing_a_new_list_with_an_additional_item_is_reported_as_a_single_add()
		{
			// Arrange
			var initialValue = Enumerable.Range(0,10).Select(i => i.ToString()).ToImmutableList();
			var vm = new ViewModelBase();
			var subject = new Subject<IEnumerable<string>>();
			var sut = new ObservableCollectionFromObservableAdapter<string>(vm, subject, initialValue);
			var incc = sut.ReadOnlyCollection as INotifyCollectionChanged;
			var args = default(NotifyCollectionChangedEventArgs);
			incc.CollectionChanged += OnCollectionChanged;

			// Act
			subject.OnNext(initialValue.Insert(0, "-1"));

			// Assert
			args.Action.Should().Be(NotifyCollectionChangedAction.Add);
			args.NewStartingIndex.Should().Be(0);
			args.NewItems.Should().ContainEquivalentOf("-1");

			void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
			{
				args = e;
			}
		}		
	}
}
