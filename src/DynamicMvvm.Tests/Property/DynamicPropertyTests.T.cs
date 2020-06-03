using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Chinook.DynamicMvvm.Tests.Helpers;
using Xunit;

namespace Chinook.DynamicMvvm.Tests.Property
{
	public class DynamicPropertyTestsT
	{
		private const string DefaultPropertyName = nameof(DefaultPropertyName);

		[Fact]
		public void It_Creates_With_NoValue()
		{
			var property = new DynamicProperty<TestEntity>(DefaultPropertyName);

			property.Value.Should().BeNull();
		}

		[Fact]
		public void It_Creates_With_Value()
		{
			var myValue = new TestEntity();
			var property = new DynamicProperty<TestEntity>(DefaultPropertyName, myValue);

			property.Value.Should().Be(myValue);
		}

		[Fact]
		public void It_Changes_Value()
		{
			var myValue = new TestEntity();
			var property = new DynamicProperty<TestEntity>(DefaultPropertyName)
			{
				Value = myValue
			};

			property.Value.Should().Be(myValue);
		}

		[Fact]
		public void It_Raises_ValueChanged()
		{
			var receivedValues = new List<IDynamicProperty>();
			var property = new DynamicProperty<TestEntity>(DefaultPropertyName);

			property.ValueChanged += OnValueChanged;

			property.Value = new TestEntity();

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
			var myValue = new TestEntity();

			var receivedValues = new List<IDynamicProperty>();
			var property = new DynamicProperty<TestEntity>(DefaultPropertyName, myValue);

			property.ValueChanged += OnValueChanged;

			property.Value = myValue;

			receivedValues.Should().BeEmpty();

			void OnValueChanged(IDynamicProperty p)
			{
				receivedValues.Add(p);
			}
		}

		[Fact]
		public void It_Throws_When_Value_Set_After_Disposed()
		{
			var receivedValues = new List<IDynamicProperty>();
			var property = new DynamicProperty<TestEntity>(DefaultPropertyName);

			property.Dispose();

			Assert.Throws<ObjectDisposedException>(() => property.Value = new TestEntity());
		}

		[Fact]
		public void It_Subscribes_To_ValueChanged()
		{
			var receivedValues = new List<IDynamicProperty>();
			var property = new DynamicProperty<TestEntity>(DefaultPropertyName);

			using (property.Subscribe(OnValueChanged))
			{
				property.Value = new TestEntity();

				receivedValues.Count().Should().Be(1);
				receivedValues[0].Should().Be(property);
			}

			void OnValueChanged(IDynamicProperty<TestEntity> p)
			{
				receivedValues.Add(p);
			}
		}

		[Fact]
		public void It_Subscribes_To_ValueChanged_Disposed()
		{
			var receivedValues = new List<IDynamicProperty>();
			var property = new DynamicProperty<TestEntity>(DefaultPropertyName);

			using (property.Subscribe(OnValueChanged))
			{
				property.Value = new TestEntity();
			}

			property.Value = new TestEntity();

			receivedValues.Count().Should().Be(1);
			receivedValues[0].Should().Be(property);

			void OnValueChanged(IDynamicProperty<TestEntity> p)
			{
				receivedValues.Add(p);
			}
		}
	}
}
