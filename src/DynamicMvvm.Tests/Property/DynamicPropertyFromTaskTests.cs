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
	public class DynamicPropertyFromTaskTests
	{
		private const string DefaultPropertyName = nameof(DefaultPropertyName);

		[Fact]
		public void It_Creates_With_NoValue()
		{
			var source = new TaskCompletionSource<TestEntity>();

			var property = new DynamicPropertyFromTask<TestEntity>(DefaultPropertyName, _ => source.Task);

			property.Value.Should().BeNull();
		}

		[Fact]
		public void It_Creates_With_Value()
		{
			var source = new TaskCompletionSource<TestEntity>();
			var value = new TestEntity();

			var property = new DynamicPropertyFromTask<TestEntity>(DefaultPropertyName, _ => source.Task, value);

			property.Value.Should().Be(value);
		}

		[Fact]
		public void It_Changes_Value()
		{
			var source = new TaskCompletionSource<TestEntity>();
			var value = new TestEntity();

			var property = new DynamicPropertyFromTask<TestEntity>(DefaultPropertyName, _ => source.Task)
			{
				Value = value
			};

			property.Value.Should().Be(value);
		}

		[Fact]
		public void It_Changes_Value_From_Source()
		{
			var source = new TaskCompletionSource<TestEntity>();
			var value = new TestEntity();

			var property = new DynamicPropertyFromTask<TestEntity>(DefaultPropertyName, _ => source.Task);

			source.TrySetResult(value);

			property.Value.Should().Be(value);
		}

		[Fact]
		public void It_Changes_Value_From_Source_Then_Set()
		{
			var source = new TaskCompletionSource<TestEntity>();
			var value = new TestEntity();

			var property = new DynamicPropertyFromTask<TestEntity>(DefaultPropertyName, _ => source.Task);

			source.TrySetResult(new TestEntity());

			property.Value = value;

			property.Value.Should().Be(value);
		}

		[Fact]
		public void It_Raises_ValueChanged()
		{
			var source = new TaskCompletionSource<TestEntity>();
			var receivedValues = new List<IDynamicProperty>();
			var property = new DynamicPropertyFromTask<TestEntity>(DefaultPropertyName, _ => source.Task);

			property.ValueChanged += OnValueChanged;

			source.TrySetResult(new TestEntity());

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
			var source = new TaskCompletionSource<TestEntity>();
			var value = new TestEntity();

			var receivedValues = new List<IDynamicProperty>();
			var property = new DynamicPropertyFromTask<TestEntity>(DefaultPropertyName, _ => source.Task, value);

			property.ValueChanged += OnValueChanged;

			source.TrySetResult(value);

			receivedValues.Should().BeEmpty();

			void OnValueChanged(IDynamicProperty p)
			{
				receivedValues.Add(p);
			}
		}

		[Fact]
		public void It_Doesnt_Set_Value_After_Disposed()
		{
			var source = new TaskCompletionSource<TestEntity>();
			var value = new TestEntity();

			var property = new DynamicPropertyFromTask<TestEntity>(DefaultPropertyName, _ => source.Task);

			property.Dispose();

			source.TrySetResult(value);

			property.Value.Should().NotBe(value);
		}
	}
}
