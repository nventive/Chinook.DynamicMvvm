using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Chinook.DynamicMvvm.Tests.Helpers;
using Xunit;

namespace Chinook.DynamicMvvm.Tests.ViewModel
{
	public class ViewModelBasePropertyChangedTests
	{
		private const string DefaultPropertyName = nameof(DefaultPropertyName);

		[Fact]
		public void It_Raises_PropertyChanged()
		{
			var receivedValues = new List<(object, PropertyChangedEventArgs)>();
			var viewModel = new ViewModelBase();

			viewModel.PropertyChanged += OnPropertyChanged;

			viewModel.RaisePropertyChanged(DefaultPropertyName);

			receivedValues.Count().Should().Be(1);
			receivedValues[0].Item1.Should().Be(viewModel);
			receivedValues[0].Item2.PropertyName.Should().Be(DefaultPropertyName);

			void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
			{
				receivedValues.Add((sender, e));
			}
		}

		[Fact]
		public void It_Raises_PropertyChanged_OnDispatcher_If_Not_OnDispatcher()
		{
			var executedOnDispatcher = false;
			var receivedValues = new List<(object, PropertyChangedEventArgs)>();

			var viewModel = new ViewModelBase
			{
				Dispatcher = new TestDispatcher(
					hasDispatcherAccess: false,
					onExecuteOnDispatcher: a =>
					{
						executedOnDispatcher = true;

						a();
					}
				)
			};

			viewModel.PropertyChanged += OnPropertyChanged;

			viewModel.RaisePropertyChanged(DefaultPropertyName);

			executedOnDispatcher.Should().BeTrue();

			receivedValues.Count().Should().Be(1);
			receivedValues[0].Item1.Should().Be(viewModel);
			receivedValues[0].Item2.PropertyName.Should().Be(DefaultPropertyName);

			void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
			{
				receivedValues.Add((sender, e));
			}
		}

		[Fact]
		public void It_Doesnt_Raise_PropertyChanged_OnDispatcher_If_OnDispatcher()
		{
			var executedOnDispatcher = false;
			var receivedValues = new List<(object, PropertyChangedEventArgs)>();

			var viewModel = new ViewModelBase
			{
				Dispatcher = new TestDispatcher(
					hasDispatcherAccess: true,
					onExecuteOnDispatcher: a =>
					{
						executedOnDispatcher = true;

						a();
					}
				)
			};

			viewModel.PropertyChanged += OnPropertyChanged;

			viewModel.RaisePropertyChanged(DefaultPropertyName);

			executedOnDispatcher.Should().BeFalse();

			receivedValues.Count().Should().Be(1);
			receivedValues[0].Item1.Should().Be(viewModel);
			receivedValues[0].Item2.PropertyName.Should().Be(DefaultPropertyName);

			void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
			{
				receivedValues.Add((sender, e));
			}
		}
	}
}
