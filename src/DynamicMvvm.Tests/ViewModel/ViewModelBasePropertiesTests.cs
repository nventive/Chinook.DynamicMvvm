using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Chinook.DynamicMvvm.Tests.Helpers;
using Xunit;

namespace Chinook.DynamicMvvm.Tests.ViewModel
{
	public class ViewModelBasePropertiesTests
	{
		private readonly IServiceProvider _serviceProvider;

		public ViewModelBasePropertiesTests()
		{
			var serviceCollection = new ServiceCollection();

			serviceCollection.AddSingleton<IDynamicPropertyFactory, DynamicPropertyFactory>();

			_serviceProvider = serviceCollection.BuildServiceProvider();
		}

		[Fact]
		public void It_Gets_With_NoValue()
		{
			var viewModel = new ViewModelBase(serviceProvider: _serviceProvider);

			var value = viewModel.Get<TestEntity>();

			value.Should().BeNull();
		}

		[Fact]
		public void It_Gets_With_Value()
		{
			var viewModel = new ViewModelBase(serviceProvider: _serviceProvider);

			var myValue = new TestEntity();
			var value = viewModel.Get(myValue);

			value.Should().Be(myValue);
		}

		[Fact]
		public void It_Gets_From_Func()
		{
			var viewModel = new ViewModelBase(serviceProvider: _serviceProvider);

			var myValue = new TestEntity();
			var value = viewModel.Get(() => myValue);

			value.Should().Be(myValue);
		}

		[Fact]
		public void It_Gets_From_Task_With_NoValue()
		{
			var source = new TaskCompletionSource<TestEntity>();
			var viewModel = new ViewModelBase(serviceProvider: _serviceProvider);

			var myValue = new TestEntity();
			var value = viewModel.GetFromTask(_ => source.Task);

			value.Should().BeNull();

			source.TrySetResult(myValue);

			var property = viewModel.GetProperty(nameof(It_Gets_From_Task_With_NoValue));

			property.Value.Should().Be(myValue);
		}

		[Fact]
		public void It_Gets_From_Task_With_Value()
		{
			var source = new TaskCompletionSource<TestEntity>();
			var viewModel = new ViewModelBase(serviceProvider: _serviceProvider);

			var myInitialValue = new TestEntity();
			var myValue = new TestEntity();
			var value = viewModel.GetFromTask(_ => source.Task, myInitialValue);

			value.Should().Be(myInitialValue);

			source.TrySetResult(myValue);

			var property = viewModel.GetProperty(nameof(It_Gets_From_Task_With_Value));

			property.Value.Should().Be(myValue);
		}

		[Fact]
		public void It_Gets_From_Observable_With_NoValue()
		{
			var source = new Subject<TestEntity>();
			var viewModel = new ViewModelBase(serviceProvider: _serviceProvider);

			var myValue = new TestEntity();
			var value = viewModel.GetFromObservable(source);

			value.Should().BeNull();

			source.OnNext(myValue);

			var property = viewModel.GetProperty(nameof(It_Gets_From_Observable_With_NoValue));

			property.Value.Should().Be(myValue);
		}

		[Fact]
		public void It_Gets_From_Observable_With_Value()
		{
			var source = new Subject<TestEntity>();
			var viewModel = new ViewModelBase(serviceProvider: _serviceProvider);

			var myInitialValue = new TestEntity();
			var myValue = new TestEntity();
			var value = viewModel.GetFromObservable(source, myInitialValue);

			value.Should().Be(myInitialValue);

			source.OnNext(myValue);

			var property = viewModel.GetProperty(nameof(It_Gets_From_Observable_With_Value));

			property.Value.Should().Be(myValue);
		}

		[Fact]
		public void It_Raises_PropertyChanged()
		{
			var receivedValues = new List<(object, PropertyChangedEventArgs)>();
			var viewModel = new ViewModelBase(serviceProvider: _serviceProvider);

			viewModel.PropertyChanged += OnPropertyChanged;

			var value = viewModel.Get<TestEntity>();

			var property = viewModel.GetProperty(nameof(It_Raises_PropertyChanged));

			property.Value = new TestEntity();

			receivedValues.Count().Should().Be(1);
			receivedValues[0].Item1.Should().Be(viewModel);
			receivedValues[0].Item2.PropertyName.Should().Be(nameof(It_Raises_PropertyChanged));

			void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
			{
				receivedValues.Add((sender, e));
			}
		}

		[Fact]
		public void It_Gets_Property()
		{
			var viewModel = new ViewModelBase(serviceProvider: _serviceProvider);

			var value = viewModel.Get(new TestEntity());

			var property = viewModel.GetProperty(nameof(It_Gets_Property));

			property.Value.Should().Be(value);
		}

		[Fact]
		public void It_Gets_Unresolved_Property()
		{
			var viewModel = new TestViewModel(serviceProvider: _serviceProvider);

			var property = viewModel.GetProperty(nameof(TestViewModel.ReadWriteEntity));

			property.Should().NotBeNull();
		}

		[Fact]
		public void It_Gets_ReadWrite_Property()
		{
			var viewModel = new TestViewModel(serviceProvider: _serviceProvider);

			viewModel.ReadWriteEntity.Should().BeNull();

			var myValue = new TestEntity();

			viewModel.ReadWriteEntity = myValue;

			viewModel.ReadWriteEntity.Should().Be(myValue);
		}

		[Fact]
		public void It_Sets_Unresolved_ReadWrite_Property()
		{
			var viewModel = new TestViewModel(serviceProvider: _serviceProvider);

			var myValue = new TestEntity();

			viewModel.ReadWriteEntity = myValue;

			viewModel.ReadWriteEntity.Should().Be(myValue);
		}

		[Fact]
		public void It_Gets_Property_With_Expression()
		{
			var viewModel = new TestViewModel(serviceProvider: _serviceProvider);

			var property = viewModel.GetProperty(v => v.ReadEntity);

			property.Should().NotBeNull();
		}

		[Fact]
		public void It_Adds_Disposable()
		{
			var viewModel = new ViewModelBase(serviceProvider: _serviceProvider);

			var myValue = new TestEntity();
			var value = viewModel.Get(() => myValue);

			var isAdded = viewModel.TryGetDisposable(nameof(It_Adds_Disposable), out var _);

			isAdded.Should().BeTrue();
		}

		[Fact]
		public void It_Keeps_Properties_When_Disposed()
		{
			var viewModel = new ViewModelBase(serviceProvider: _serviceProvider);

			var myValue = "test";
			var propertyName = "Prop";
			var value = viewModel.Get(() => myValue, propertyName);

			viewModel.Dispose();

			var property = viewModel.GetProperty(propertyName);

			property.Should().NotBeNull();
			property.Value.Should().Be(myValue);
		}
	}
}
