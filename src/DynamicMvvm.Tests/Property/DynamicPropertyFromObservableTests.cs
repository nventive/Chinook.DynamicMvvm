using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Chinook.DynamicMvvm.Tests.Helpers;
using Xunit;

namespace Chinook.DynamicMvvm.Tests.Property
{
	public class DynamicPropertyFromObservableTests
	{
		private const string DefaultPropertyName = nameof(DefaultPropertyName);

		[Fact]
		public void It_Creates_With_NoValue()
		{
			var source = new Subject<TestEntity>();

			var property = new DynamicPropertyFromObservable<TestEntity>(DefaultPropertyName, source);

			property.Value.Should().BeNull();
		}

		[Fact]
		public void It_Creates_With_Value()
		{
			var source = new Subject<TestEntity>();
			var value = new TestEntity();

			var property = new DynamicPropertyFromObservable<TestEntity>(DefaultPropertyName, source, value);

			property.Value.Should().Be(value);
		}

		[Fact]
		public void It_Changes_Value()
		{
			var source = new Subject<TestEntity>();
			var value = new TestEntity();

			var property = new DynamicPropertyFromObservable<TestEntity>(DefaultPropertyName, source)
			{
				Value = value
			};

			property.Value.Should().Be(value);
		}

		[Fact]
		public void It_Changes_Value_From_Source()
		{ 
			var source = new Subject<TestEntity>();
			var value = new TestEntity();

			var property = new DynamicPropertyFromObservable<TestEntity>(DefaultPropertyName, source);

			source.OnNext(value);

			property.Value.Should().Be(value);
		}

		[Fact]
		public void It_Changes_Value_From_Source_Then_Set()
		{
			var source = new Subject<TestEntity>();
			var value = new TestEntity();

			var property = new DynamicPropertyFromObservable<TestEntity>(DefaultPropertyName, source);

			source.OnNext(new TestEntity());

			property.Value = value;

			property.Value.Should().Be(value);
		}

		[Fact]
		public void It_Raises_ValueChanged()
		{
			var source = new Subject<TestEntity>();
			var receivedValues = new List<IDynamicProperty>();
			var property = new DynamicPropertyFromObservable<TestEntity>(DefaultPropertyName, source);

			property.ValueChanged += OnValueChanged;

			source.OnNext(new TestEntity());

			receivedValues.Count().Should().Be(1);
			receivedValues[0].Should().Be(property);

			void OnValueChanged(IDynamicProperty p)
			{
				receivedValues.Add(p);
			}
		}

		[Fact]
		public void It_Doesnt_Raise_ValueChanged_For_SameValue()
		{
			var source = new Subject<TestEntity>();
			var value = new TestEntity();

			var receivedValues = new List<IDynamicProperty>();
			var property = new DynamicPropertyFromObservable<TestEntity>(DefaultPropertyName, source, value);

			property.ValueChanged += OnValueChanged;

			source.OnNext(value);

			receivedValues.Should().BeEmpty();

			void OnValueChanged(IDynamicProperty p)
			{
				receivedValues.Add(p);
			}
		}

		[Fact]
		public void It_Doesnt_Set_Value_After_Disposed()
		{
			var source = new Subject<TestEntity>();
			var value = new TestEntity();

			var property = new DynamicPropertyFromObservable<TestEntity>(DefaultPropertyName, source);

			property.Dispose();

			source.OnNext(value);

			property.Value.Should().NotBe(value);
		}
	}
}
