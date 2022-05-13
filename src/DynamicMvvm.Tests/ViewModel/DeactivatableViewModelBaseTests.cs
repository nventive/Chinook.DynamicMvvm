using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chinook.DynamicMvvm.Deactivation;
using FluentAssertions;
using Xunit;

namespace Chinook.DynamicMvvm.Tests.ViewModel
{
	public class DeactivatableViewModelBaseTests
	{
		[Fact]
		public void PropertyChanged_events_are_not_raised_while_deactivated_but_raised_when_reactivated()
		{
			var propertyChanges = new List<string>();
			var sut = new DeactivatableViewModelBase();
			sut.PropertyChanged += OnPropertyChanged;
			sut.Deactivate();

			sut.RaisePropertyChanged("Allo");
			sut.RaisePropertyChanged("Hi");
			sut.RaisePropertyChanged("Bonjour");

			propertyChanges.Should().OnlyContain(s => s == "IsDeactivated");

			sut.Reactivate();

			propertyChanges.Should().Contain(new []{ "Allo", "Hi", "Bonjour"});

			void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
			{
				propertyChanges.Add(e.PropertyName);
			}
		}
	}
}
