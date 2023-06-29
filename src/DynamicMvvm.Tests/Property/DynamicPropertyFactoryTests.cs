using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Chinook.DynamicMvvm.Tests.Helpers;
using Xunit;

namespace Chinook.DynamicMvvm.Tests.Property
{
	public class DynamicPropertyFactoryTests
	{
		private const string DefaultPropertyName = nameof(DefaultPropertyName);

		[Fact]
		public void It_Creates_With_No_Value()
		{
			var factory = new DynamicPropertyFactory();

			var property = factory.Create<TestEntity>(DefaultPropertyName);

			property.Value.Should().BeNull();
		}

		[Fact]
		public void It_Creates_With_Value()
		{
			var myValue = new TestEntity();
			var factory = new DynamicPropertyFactory();

			var property = factory.Create(DefaultPropertyName, myValue);

			property.Value.Should().Be(myValue);
		}

		[Fact]
		public void It_Creates_From_Observable_With_No_Value()
		{
			var source = new Subject<TestEntity>();
			var factory = new DynamicPropertyFactory();

			var property = factory.CreateFromObservable(DefaultPropertyName, source);

			property.Value.Should().BeNull();
		}

		[Fact]
		public void It_Creates_From_Observable_With_Value()
		{
			var source = new Subject<TestEntity>();
			var myValue = new TestEntity();
			var factory = new DynamicPropertyFactory();

			var property = factory.CreateFromObservable(DefaultPropertyName, source, myValue);

			property.Value.Should().Be(myValue);
		}

		[Fact]
		public void It_Creates_From_Task_With_No_Value()
		{
			var source = new TaskCompletionSource<TestEntity>();
			var factory = new DynamicPropertyFactory();

			var property = factory.CreateFromTask(DefaultPropertyName, _ => source.Task);

			property.Value.Should().BeNull();
		}

		[Fact]
		public void It_Creates_From_Task_With_Value()
		{
			var source = new TaskCompletionSource<TestEntity>();
			var myValue = new TestEntity();
			var factory = new DynamicPropertyFactory();

			var property = factory.CreateFromTask(DefaultPropertyName, _ => source.Task, myValue);

			property.Value.Should().Be(myValue);
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void It_Creates_Property_That_Throws_On_Set_When_Disposed_Only_If_ThrowOnDisposed_Is_True(bool throwOnDisposed)
		{
			// Arrange
			var factory = new DynamicPropertyFactory(throwOnDisposed);

			var taskProperty = factory.CreateFromTask(DefaultPropertyName, _ => Task.FromResult(new TestEntity()), new TestEntity());
			var obProperty = factory.CreateFromObservable(DefaultPropertyName, new Subject<TestEntity>(), new TestEntity());
			var property = factory.Create(DefaultPropertyName, new TestEntity());

			// Act
			taskProperty.Dispose();
			obProperty.Dispose();
			property.Dispose();

			Action actTask = () =>
			{
				taskProperty.Value = new TestEntity();
			};
			Action actOb = () =>
			{
				obProperty.Value = new TestEntity();
			};
			Action actP = () =>
			{
				property.Value = new TestEntity();
			};

			// Assert
			if (throwOnDisposed)
			{
				actTask.Should().Throw<InvalidOperationException>();
				actOb.Should().Throw<InvalidOperationException>();
				actP.Should().Throw<InvalidOperationException>();
			}
			else
			{
				actTask.Should().NotThrow();
				actOb.Should().NotThrow();
				actP.Should().NotThrow();
			}
		}
	}
}
