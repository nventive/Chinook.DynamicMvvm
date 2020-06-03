﻿using System;
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
		public void It_Shares_View_When_Set_Before_Attached()
		{
			var parentViewModel = new ViewModelBase()
			{
				View = new TestViewModelView()
			};

			var childViewModel = new TestViewModel();

			parentViewModel.AttachChild(childViewModel);

			childViewModel.View.Should().Be(parentViewModel.View);
		}

		[Fact]
		public void It_Shares_View_When_Set_After_Attached()
		{
			var parentViewModel = new ViewModelBase();
			var childViewModel = new TestViewModel();

			parentViewModel.AttachChild(childViewModel);

			parentViewModel.View = new TestViewModelView();

			childViewModel.View.Should().Be(parentViewModel.View);
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
	}
}
