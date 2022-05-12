using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Chinook.DynamicMvvm.Implementations;
using FluentAssertions;
using Xunit;

namespace Chinook.DynamicMvvm.Tests.Property
{
	public class ValueChangedOnBackgroundTaskDynamicPropertyTests
	{
		[Fact]
		public async Task ValueChanged_is_raised_on_a_different_task_when_dispatcher_access_is_true()
		{
			var vm = new ViewModelBase
			{
				View = new TestViewModelView()
				{
					HasDispatcherAccess = true
				}
			};

			var syncContext = new TaskCompletionSource<SynchronizationContext>();
			var thread = new TaskCompletionSource<Thread>();
			var mainContext = SynchronizationContext.Current;
			var mainThread = Thread.CurrentThread;
			var sut = new ValueChangedOnBackgroundTaskDynamicProperty("sut", vm);

			sut.ValueChanged += OnValueChanged;
			sut.Value = 1;

			var valueChangedContext = await syncContext.Task;
			var valueChangedThread = await thread.Task;

			valueChangedContext.Should().NotBe(mainContext);
			valueChangedThread.Should().NotBe(mainThread);

			void OnValueChanged(IDynamicProperty property)
			{
				syncContext.SetResult(SynchronizationContext.Current);
				thread.SetResult(Thread.CurrentThread);
			}
		}

		[Fact]
		public async Task ValueChanged_is_raised_on_the_same_task_when_dispatcher_access_is_false()
		{
			var vm = new ViewModelBase
			{
				View = new TestViewModelView()
				{
					HasDispatcherAccess = false
				}
			};

			var syncContext = new TaskCompletionSource<SynchronizationContext>();
			var thread = new TaskCompletionSource<Thread>();
			var mainContext = SynchronizationContext.Current;
			var mainThread = Thread.CurrentThread;
			var sut = new ValueChangedOnBackgroundTaskDynamicProperty("sut", vm);

			sut.ValueChanged += OnValueChanged;
			sut.Value = 1;

			var valueChangedContext = await syncContext.Task;
			var valueChangedThread = await thread.Task;

			valueChangedContext.Should().Be(mainContext);
			valueChangedThread.Should().Be(mainThread);

			void OnValueChanged(IDynamicProperty property)
			{
				syncContext.SetResult(SynchronizationContext.Current);
				thread.SetResult(Thread.CurrentThread);
			}
		}

		private class TestViewModelView : IViewModelView
		{
			public bool HasDispatcherAccess { get; set; }

			public event EventHandler Loaded;
			public event EventHandler Unloaded;

			public Task ExecuteOnDispatcher(CancellationToken ct, Action action)
			{
				action();
				return Task.CompletedTask;
			}

			public bool GetHasDispatcherAccess()
			{
				return HasDispatcherAccess;
			}
		}
	}
}
