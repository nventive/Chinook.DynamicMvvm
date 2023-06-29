using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Chinook.DynamicMvvm.Tests.Property
{
	public class DynamicPropertyTests
	{
		private const string DefaultPropertyName = nameof(DefaultPropertyName);

		[Fact]
		public void It_Creates_With_NoValue()
		{
			var property = new DynamicProperty(DefaultPropertyName);

			property.Value.Should().BeNull();
		}

		[Fact]
		public void It_Creates_With_Value()
		{
			var myValue = new object();
			var property = new DynamicProperty(DefaultPropertyName, myValue);

			property.Value.Should().Be(myValue);
		}

		[Fact]
		public void It_Changes_Value()
		{
			var myValue = new object();
			var property = new DynamicProperty(DefaultPropertyName)
			{
				Value = myValue
			};

			property.Value.Should().Be(myValue);
		}

		[Fact]
		public void It_Raises_ValueChanged()
		{
			var receivedValues = new List<IDynamicProperty>();
			var property = new DynamicProperty(DefaultPropertyName);

			property.ValueChanged += OnValueChanged;

			property.Value = new object();

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
			var myValue = new object();

			var receivedValues = new List<IDynamicProperty>();
			var property = new DynamicProperty(DefaultPropertyName, myValue);

			property.ValueChanged += OnValueChanged;

			property.Value = myValue;

			receivedValues.Should().BeEmpty();

			void OnValueChanged(IDynamicProperty p)
			{
				receivedValues.Add(p);
			}
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void It_Throws_When_Value_Set_After_Disposed_Only_If_ThrowOnDisposed_Is_True(bool throwOnDisposed)
		{
			// Arrange
			var property = new DynamicProperty(DefaultPropertyName, throwOnDisposed: throwOnDisposed);

			// Act
			property.Dispose();
			Action act = () =>
			{
				property.Value = new object();
			};

			// Assert
			if (throwOnDisposed)
			{
				act.Should().Throw<ObjectDisposedException>();
			}
			else
			{
				act.Should().NotThrow();
			}
		}

		[Fact]
		public void It_Throws_When_Value_Set_After_Disposed()
		{
			var property = new DynamicProperty(DefaultPropertyName);

			property.Dispose();

			Assert.Throws<ObjectDisposedException>(() => property.Value = new object());
		}

		[Fact]
		public void It_Subscribes_To_ValueChanged()
		{
			var receivedValues = new List<IDynamicProperty>();
			var property = new DynamicProperty(DefaultPropertyName);

			using (property.Subscribe(OnValueChanged))
			{
				property.Value = new object();

				receivedValues.Count().Should().Be(1);
				receivedValues[0].Should().Be(property);
			}

			void OnValueChanged(IDynamicProperty p)
			{
				receivedValues.Add(p);
			}
		}

		[Fact]
		public void It_Subscribes_To_ValueChanged_Disposed()
		{
			var receivedValues = new List<IDynamicProperty>();
			var property = new DynamicProperty(DefaultPropertyName);

			using (property.Subscribe(OnValueChanged))
			{
				property.Value = new object();
			}

			property.Value = new object();

			receivedValues.Count().Should().Be(1);
			receivedValues[0].Should().Be(property);

			void OnValueChanged(IDynamicProperty p)
			{
				receivedValues.Add(p);
			}
		}
	}
}
