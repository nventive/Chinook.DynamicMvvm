using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Chinook.DynamicMvvm.Tests.Helpers;
using Xunit;

namespace Chinook.DynamicMvvm.Tests.Command.Strategies
{
	public class CanExecuteCommandStrategyTests
	{
		private const string DefaultCommandName = nameof(DefaultCommandName);
		private const string DefaultPropertyName = nameof(DefaultPropertyName);

		[Fact]
		public void It_Raises_CanExecute_When_Property_Changes()
		{
			var canExecute = true;
			var property = new DynamicProperty<bool>(DefaultPropertyName, false);
			var testStrategy = new TestCommandStrategy();

			var strategy = new CanExecuteCommandStrategy(property)
			{
				InnerStrategy = testStrategy
			};

			var command = new DynamicCommand(DefaultCommandName, strategy);

			command.CanExecuteChanged += OnCanExecuteChanged;

			canExecute = command.CanExecute(null);
			canExecute.Should().BeFalse();

			property.Value = true;
			canExecute.Should().BeTrue();

			property.Value = false;
			canExecute.Should().BeFalse();

			void OnCanExecuteChanged(object sender, EventArgs e)
			{
				canExecute = command.CanExecute(null);
			}
		}
	}
}
