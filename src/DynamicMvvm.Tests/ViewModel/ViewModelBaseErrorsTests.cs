using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Chinook.DynamicMvvm.Tests.ViewModel
{
	public class ViewModelBaseErrorsTests
	{
		[Fact]
		public void It_Has_No_Errors_By_Default()
		{
			// Arrange
			var sut = new ViewModelBase();

			// Assert
			sut.HasErrors.Should().BeFalse();
			sut.GetErrors(null).Should().BeEmpty();
			sut.GetErrors("FakeProperty").Should().BeEmpty();
		}

		[Fact]
		public void It_Can_Set_Errors_For_A_Property()
		{
			// Arrange
			var sut = new ViewModelBase();

			// Act
			sut.SetErrors("Foo", new[] { "Bar" });

			// Assert
			sut.HasErrors.Should().BeTrue();
			sut.GetErrors("Foo").Should().ContainEquivalentOf("Bar");
		}

		[Fact]
		public void It_Can_Set_Errors_Using_Dictionary()
		{
			// Arrange
			var sut = new ViewModelBase();

			// Act
			sut.SetErrors(new Dictionary<string, IEnumerable<object>>()
			{
				{ "Foo", new[] { "Bar" } }
			});

			// Assert
			sut.HasErrors.Should().BeTrue();
			sut.GetErrors(null).Should().ContainEquivalentOf("Bar");
		}

		[Fact]
		public void It_Can_Clear_Errors_For_A_Property()
		{
			// Arrange
			var sut = new ViewModelBase();

			sut.SetErrors("Prop1", new[] { "Error1" });
			sut.SetErrors("Prop2", new[] { "Error2" });

			// Act
			sut.ClearErrors("Prop1");

			// Assert
			sut.HasErrors.Should().BeTrue();
			sut.GetErrors(null).Should().ContainEquivalentOf("Error2");
		}

		[Fact]
		public void It_Can_Clear_Errors_For_All_Properties()
		{
			// Arrange
			var sut = new ViewModelBase();

			sut.SetErrors("Prop1", new[] { "Error1" });
			sut.SetErrors("Prop2", new[] { "Error2" });

			// Act
			sut.ClearErrors();

			// Assert
			sut.HasErrors.Should().BeFalse();
			sut.GetErrors(null).Should().BeEmpty();
		}

		[Fact]
		public void It_Raises_ErrorsChanged_When_Errors_Are_Set()
		{
			// Arrange
			var sut = new ViewModelBase();
			var receivedValues = new List<(object, DataErrorsChangedEventArgs)>();

			sut.ErrorsChanged += OnErrorsChanged;

			// Act
			sut.SetErrors("Foo", new[] { "Bar" });

			// Assert
			receivedValues.Count().Should().Be(1);
			receivedValues[0].Item1.Should().Be(sut);
			receivedValues[0].Item2.PropertyName.Should().Be("Foo");

			void OnErrorsChanged(object sender, DataErrorsChangedEventArgs e)
			{
				receivedValues.Add((sender, e));
			}
		}

		[Fact]
		public void It_Raises_ErrorsChanged_When_Errors_Are_Cleared()
		{
			// Arrange
			var sut = new ViewModelBase();
			var receivedValues = new List<(object, DataErrorsChangedEventArgs)>();

			sut.SetErrors("Foo", new[] { "Bar" });
			sut.ErrorsChanged += OnErrorsChanged;

			// Act
			sut.ClearErrors("Foo");

			// Assert
			receivedValues.Count().Should().Be(1);
			receivedValues[0].Item1.Should().Be(sut);
			receivedValues[0].Item2.PropertyName.Should().Be("Foo");

			void OnErrorsChanged(object sender, DataErrorsChangedEventArgs e)
			{
				receivedValues.Add((sender, e));
			}
		}

		[Fact]
		public void It_Raises_ErrorsChanged_When_Errors_Are_Cleared_For_All_Properties()
		{
			// Arrange
			var sut = new ViewModelBase();
			var receivedValues = new List<(object, DataErrorsChangedEventArgs)>();

			sut.SetErrors("Foo", new[] { "Bar" });
			sut.ErrorsChanged += OnErrorsChanged;

			// Act
			sut.ClearErrors();

			// Assert
			receivedValues.Count().Should().Be(1);
			receivedValues[0].Item1.Should().Be(sut);
			receivedValues[0].Item2.PropertyName.Should().BeNull();

			void OnErrorsChanged(object sender, DataErrorsChangedEventArgs e)
			{
				receivedValues.Add((sender, e));
			}
		}

		[Fact]
		public void Clear_Doesnt_Raise_ErrorsChanged_There_Are_No_Errors()
		{
			// Arrange
			var sut = new ViewModelBase();
			var receivedValues = new List<(object, DataErrorsChangedEventArgs)>();

			sut.ErrorsChanged += OnErrorsChanged;

			// Act
			sut.ClearErrors();

			// Assert
			receivedValues.Should().BeEmpty();

			void OnErrorsChanged(object sender, DataErrorsChangedEventArgs e)
			{
				receivedValues.Add((sender, e));
			}
		}
	}
}
