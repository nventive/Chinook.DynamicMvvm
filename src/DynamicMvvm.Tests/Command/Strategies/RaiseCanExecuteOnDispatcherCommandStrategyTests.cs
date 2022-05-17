using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chinook.DynamicMvvm.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace Chinook.DynamicMvvm.Tests.Command.Strategies
{
	public class RaiseCanExecuteOnDispatcherCommandStrategyTests
	{
		[Fact]
		public void Requires_a_non_null_ViewModel()
		{
			Assert.Throws<ArgumentNullException>(() => new RaiseCanExecuteOnDispatcherCommandStrategy(viewModel: null));
		}

		[Fact]
		public void Dispatches_to_view_when_not_already_on_dispatcher()
		{
			var vm = new ViewModelBase();
			var sut = new RaiseCanExecuteOnDispatcherCommandStrategy(vm);
			var inner = new TestCommandStrategy();
			var dispatched = false;
			sut.InnerStrategy = inner;

			vm.View = new TestViewModelView(hasDispatcherAccess: false, OnExecuteOnDispatcher);

			inner.RaiseCanExecuteChanged();

			dispatched.Should().BeTrue();
			
			void OnExecuteOnDispatcher(Action action)
			{
				dispatched = true;
				action();
			}
		}

		[Fact]
		public void Does_not_dispatch_to_view_when_already_on_dispatcher()
		{
			var vm = new ViewModelBase();
			var sut = new RaiseCanExecuteOnDispatcherCommandStrategy(vm);
			var inner = new TestCommandStrategy();
			var dispatched = false;
			sut.InnerStrategy = inner;

			vm.View = new TestViewModelView(hasDispatcherAccess: true, OnExecuteOnDispatcher);

			inner.RaiseCanExecuteChanged();

			dispatched.Should().BeFalse();

			void OnExecuteOnDispatcher(Action action)
			{
				dispatched = true;
				action();
			}
		}

		[Fact]
		public void Raises_even_if_view_is_null()
		{
			var vm = new ViewModelBase();
			var sut = new RaiseCanExecuteOnDispatcherCommandStrategy(vm);
			var inner = new TestCommandStrategy();
			var raised = false;
			sut.InnerStrategy = inner;
			sut.CanExecuteChanged += OnCanExecuteChanged;

			inner.RaiseCanExecuteChanged();

			raised.Should().BeTrue();

			void OnCanExecuteChanged(object sender, EventArgs e)
			{
				raised = true;
			}
		}

	}
}
