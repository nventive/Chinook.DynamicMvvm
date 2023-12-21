using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Chinook.DynamicMvvm.Tests.Helpers;
using Xunit;

namespace Chinook.DynamicMvvm.Tests.ViewModel
{
	public class ViewModelBaseChildrenTests
	{
		private const string DefaultChildName = nameof(DefaultChildName);

		[Fact]
		public void It_Creates_And_Attaches_Child()
		{
			var parentViewModel = new ViewModelBase();

			var childViewModel = parentViewModel.GetChild(() => new TestViewModel());

			parentViewModel.GetChildren().Should().Contain(childViewModel);
		}

		[Fact]
		public void It_Creates_And_Attaches_Child_Without_Func()
		{
			var parentViewModel = new ViewModelBase();

			var childViewModel = parentViewModel.GetChild<TestViewModel>();

			parentViewModel.GetChildren().Should().Contain(childViewModel);
		}

		[Fact]
		public void It_Attaches_Child()
		{
			var parentViewModel = new ViewModelBase();
			var childViewModel = new ViewModelBase();

			parentViewModel.AttachChild(childViewModel);

			parentViewModel.GetChildren().Should().Contain(childViewModel);
		}

		[Fact]
		public void It_Detaches_Child()
		{
			var parentViewModel = new ViewModelBase();
			var childViewModel = new ViewModelBase();

			parentViewModel.AttachChild(childViewModel);

			parentViewModel.DetachChild(childViewModel);

			parentViewModel.GetChildren().Should().NotContain(childViewModel);
		}

		[Fact]
		public void It_Doesnt_Throw_When_Calling_Detach_With_A_Disposed_Child()
		{
			var parentViewModel = new ViewModelBase();
			var childViewModel = new ViewModelBase();

			parentViewModel.AttachChild(childViewModel);

			childViewModel.Dispose();
			parentViewModel.DetachChild(childViewModel);

			parentViewModel.GetChildren().Should().NotContain(childViewModel);
		}

		[Fact]
		public void It_Gets_Child_On_Attach()
		{
			var parentViewModel = new ViewModelBase();
			var childViewModel = new TestViewModel();

			var attachedViewModel = parentViewModel.AttachChild(childViewModel);

			attachedViewModel.Should().Be(childViewModel);
			attachedViewModel.Should().BeOfType(typeof(TestViewModel));
		}

		[Fact]
		public void It_Disposes_Child_When_Attached()
		{
			var isChildDisposed = false;
			var parentViewModel = new ViewModelBase();
			var childViewModel = new TestViewModel(onDispose: () => isChildDisposed = true);

			parentViewModel.AttachChild(childViewModel);

			parentViewModel.Dispose();

			isChildDisposed.Should().BeTrue();
		}

		[Fact]
		public void It_Doesnt_Dispose_Child_When_Detached()
		{
			var isChildDisposed = false;
			var parentViewModel = new ViewModelBase();
			var childViewModel = new TestViewModel(onDispose: () => isChildDisposed = true);

			parentViewModel.AttachChild(childViewModel);

			parentViewModel.DetachChild(childViewModel);

			parentViewModel.Dispose();

			isChildDisposed.Should().BeFalse();
		}

		[Fact]
		public void It_Shares_Dispatcher_When_Set_Before_Attached()
		{
			var parentViewModel = new ViewModelBase()
			{
				Dispatcher = new TestDispatcher()
			};

			var childViewModel = new TestViewModel();

			parentViewModel.AttachChild(childViewModel);

			childViewModel.Dispatcher.Should().Be(parentViewModel.Dispatcher);
		}

		[Fact]
		public void It_Shares_Dispatcher_When_Set_After_Attached()
		{
			var parentViewModel = new ViewModelBase();
			var childViewModel = new TestViewModel();

			parentViewModel.AttachChild(childViewModel);

			parentViewModel.Dispatcher = new TestDispatcher();

			childViewModel.Dispatcher.Should().Be(parentViewModel.Dispatcher);
		}

		[Fact]
		public void It_Keeps_The_Dispatcher_When_Detached()
		{
			// We can't assume that the dispatcher is no longer relevant when detaching children.
			// For example, if a view is still subscribed to PropertyChanged, the property changes must still happen on the dispatcher, even if the child is detached.

			var parentViewModel = new ViewModelBase();
			var childViewModel = new TestViewModel();

			parentViewModel.AttachChild(childViewModel);

			parentViewModel.Dispatcher = new TestDispatcher();

			parentViewModel.DetachChild(childViewModel);

			childViewModel.Dispatcher.Should().Be(parentViewModel.Dispatcher);
		}

		[Fact]
		public void It_Attaches_Multiple_Children()
		{
			var parentViewModel = new ViewModelBase();
			var childViewModel1 = new ViewModelBase();
			var childViewModel2 = new ViewModelBase();

			parentViewModel.AttachChild(childViewModel1, "Child1");
			parentViewModel.AttachChild(childViewModel2, "Child2");

			parentViewModel.GetChildren().Should().Contain(childViewModel1);
			parentViewModel.GetChildren().Should().Contain(childViewModel2);
		}

		[Fact]
		public void It_Doesnt_Attach_Children_With_Same_Name()
		{
			var parentViewModel = new ViewModelBase();
			var childViewModel1 = new ViewModelBase();
			var childViewModel2 = new ViewModelBase();

			parentViewModel.AttachChild(childViewModel1, "Child1");

			Assert.Throws<InvalidOperationException>(() => parentViewModel.AttachChild(childViewModel2, "Child1"));
		}

		[Fact]
		public void It_Attaches_Multiple_Children_With_Name_From_ViewModel()
		{
			var parentViewModel = new ViewModelBase();
			var childViewModel1 = new ViewModelBase("Child1");
			var childViewModel2 = new ViewModelBase("Child2");

			parentViewModel.AttachChild(childViewModel1);
			parentViewModel.AttachChild(childViewModel2);

			parentViewModel.GetChildren().Should().Contain(childViewModel1);
			parentViewModel.GetChildren().Should().Contain(childViewModel2);
		}

		[Fact]
		public void It_Doesnt_Attach_Children_With_Same_Name_From_ViewModel()
		{
			var parentViewModel = new ViewModelBase();
			var childViewModel1 = new ViewModelBase("Child1");
			var childViewModel2 = new ViewModelBase("Child1");

			parentViewModel.AttachChild(childViewModel1);

			Assert.Throws<InvalidOperationException>(() => parentViewModel.AttachChild(childViewModel2));
		}

		[Fact]
		public void It_Attaches_Child_When_AttachOrReplaceChild()
		{
			var parentViewModel = new ViewModelBase();
			var childViewModel = new ViewModelBase();

			parentViewModel.AttachOrReplaceChild(childViewModel);

			parentViewModel.GetChildren().Should().Contain(childViewModel);
		}

		[Fact]
		public void It_Replaces_PreviousChild_And_Attaches_NewChild_When_AttachOrReplaceChild_With_Same_Name()
		{
			var parentViewModel = new ViewModelBase();
			var childViewModel1 = new ViewModelBase();
			var childViewModel2 = new ViewModelBase();

			parentViewModel.AttachOrReplaceChild(childViewModel1, "Child1");
			parentViewModel.AttachOrReplaceChild(childViewModel2, "Child1");

			parentViewModel.GetChildren().Should().NotContain(childViewModel1);
			parentViewModel.GetChildren().Should().Contain(childViewModel2);
		}

		[Fact]
		public void It_Replaces_PreviousChild_And_Attaches_NewChild_When_AttachOrReplaceChild_With_Same_Name_From_ViewModel()
		{
			var parentViewModel = new ViewModelBase();
			var childViewModel1 = new ViewModelBase("Child1");
			var childViewModel2 = new ViewModelBase("Child1");

			parentViewModel.AttachOrReplaceChild(childViewModel1);
			parentViewModel.AttachOrReplaceChild(childViewModel2);

			parentViewModel.GetChildren().Should().NotContain(childViewModel1);
			parentViewModel.GetChildren().Should().Contain(childViewModel2);
		}

		[Fact]
		public void It_Dispose_PreviousChild_When_AttachOrReplaceChild_With_Same_Name()
		{
			var parentViewModel = new ViewModelBase();
			var childViewModel1 = new ViewModelBase();
			var childViewModel2 = new ViewModelBase();

			parentViewModel.AttachOrReplaceChild(childViewModel1, "Child1");
			parentViewModel.AttachOrReplaceChild(childViewModel2, "Child1");

			Assert.Throws<ObjectDisposedException>(() => childViewModel1.SetErrors(string.Empty, Array.Empty<object>()));
		}

		[Fact]
		public void It_Dispose_PreviousChild_When_AttachOrReplaceChild_With_Same_Name_From_ViewModel()
		{
			var parentViewModel = new ViewModelBase();
			var childViewModel1 = new ViewModelBase("Child1");
			var childViewModel2 = new ViewModelBase("Child1");

			parentViewModel.AttachOrReplaceChild(childViewModel1);
			parentViewModel.AttachOrReplaceChild(childViewModel2);

			Assert.Throws<ObjectDisposedException>(() => childViewModel1.SetErrors(string.Empty, Array.Empty<object>()));
		}
	}
}
