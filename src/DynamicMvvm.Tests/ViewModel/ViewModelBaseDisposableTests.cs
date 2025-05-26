using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chinook.DynamicMvvm.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace Chinook.DynamicMvvm.Tests.ViewModel
{
	public class ViewModelBaseDisposableTests
	{
		private const string DefaultDisposableKey = nameof(DefaultDisposableKey);

		[Fact]
		public void It_Reports_IsDisposed_Correctly()
		{
			var viewModel = new ViewModelBase();

			viewModel.IsDisposed.Should().BeFalse();
			viewModel.Dispose();
			viewModel.IsDisposed.Should().BeTrue();
		}

		[Fact]
		public void It_Adds_Anonymous_Disposable()
		{
			var viewModel = new ViewModelBase();
			var disposable = new TestDisposable();

			viewModel.AddDisposable(disposable);

			var storedDisposable = viewModel.Disposables.SingleOrDefault(s => s.Value == disposable);

			storedDisposable.Key.Should().NotBeNull();
			storedDisposable.Value.Should().NotBeNull();
		}

		[Fact]
		public void It_Adds_Disposable()
		{
			var viewModel = new ViewModelBase();
			var disposable = new TestDisposable();

			viewModel.AddDisposable(DefaultDisposableKey, disposable);

			var storedDisposable = viewModel.Disposables.SingleOrDefault(s => s.Key == DefaultDisposableKey);

			storedDisposable.Key.Should().NotBeNull();
			storedDisposable.Value.Should().NotBeNull();
		}

		[Fact]
		public void It_Adds_Disposables_With_Same_Key_Only_Once()
		{
			// Arrange
			var viewModel = new ViewModelBase();
			var disposable1 = new TestDisposable();
			var disposable2 = new TestDisposable();

			// Act
			viewModel.AddDisposable(DefaultDisposableKey, disposable1);
			viewModel.AddDisposable(DefaultDisposableKey, disposable2);

			// Assert
			var firstDisposable = viewModel.Disposables.SingleOrDefault(s => s.Key == DefaultDisposableKey);
			firstDisposable.Value.Should().Be(disposable1);
			viewModel.Disposables.Should().HaveCount(1);
		}

		[Fact]
		public async Task It_Adds_Disposables_In_A_Multithreaded_Context()
		{
			// Arrange
			var viewModel = new ViewModelBase();
			const int itemCount = 10;

			// Act
			var addTask = Task.Run(async () =>
			{
				for (var i = 0; i < itemCount; i++)
				{
					viewModel.AddDisposable($"Disposable_{i}", new TestDisposable());
					await Task.Delay(1);
				}
			});

			// Act
			var enumerationTask = Task.Run(async () =>
			{
				await Task.Delay(5);
				foreach (var disposable in viewModel.Disposables)
				{
					await Task.Delay(1);
				}
			});

			await Task.WhenAll(addTask, enumerationTask);

			// Assert
			viewModel.Disposables.Count().Should().Be(itemCount);
		}

		[Fact]
		public void It_Removes_Disposable()
		{
			var viewModel = new ViewModelBase();
			var disposable = new TestDisposable();

			viewModel.AddDisposable(DefaultDisposableKey, disposable);

			viewModel.RemoveDisposable(DefaultDisposableKey);

			var storedDisposable = viewModel.Disposables.SingleOrDefault(s => s.Key == DefaultDisposableKey);

			storedDisposable.Key.Should().BeNull();
			storedDisposable.Value.Should().BeNull();
		}

		[Fact]
		public async Task It_Removes_Disposables_In_A_Multithreaded_Context()
		{
			// Arrange
			var viewModel = new ViewModelBase();

			for (var i = 0; i < 10; i++)
			{
				viewModel.AddDisposable($"Disposable_{i}", new TestDisposable());
			}

			// Act
			var enumerationTask = Task.Run(async () =>
			{
				foreach (var disposable in viewModel.Disposables)
				{
					await Task.Delay(1);
				}
			});

			var removalTask = Task.Run(async () =>
			{
				foreach (var disposable in viewModel.Disposables)
				{
					viewModel.RemoveDisposable(disposable.Key);
					await Task.Delay(1);
				}
			});

			await Task.WhenAll(enumerationTask, removalTask);

			// Assert
			viewModel.Disposables.Should().BeEmpty();
		}

		[Fact]
		public void It_Gets_Disposable_If_Added()
		{
			var viewModel = new ViewModelBase();
			var disposable = new TestDisposable();

			viewModel.AddDisposable(DefaultDisposableKey, disposable);

			var disposableFound = viewModel.TryGetDisposable(DefaultDisposableKey, out var storedDisposable);

			disposableFound.Should().BeTrue();
			storedDisposable.Should().NotBeNull();
		}

		[Fact]
		public void It_Doesnt_Get_Disposable_If_Not_Added()
		{
			var viewModel = new ViewModelBase();

			var disposableFound = viewModel.TryGetDisposable(DefaultDisposableKey, out var storedDisposable);

			disposableFound.Should().BeFalse();
			storedDisposable.Should().BeNull();
		}

		[Fact]
		public void It_Disposes_Disposable_When_Disposed()
		{
			var firstDisposed = false;
			var secondDisposed = false;

			var viewModel = new ViewModelBase();

			viewModel.AddDisposable(new TestDisposable(() => firstDisposed = true));
			viewModel.AddDisposable(new TestDisposable(() => secondDisposed = true));

			viewModel.Dispose();

			firstDisposed.Should().BeTrue();
			secondDisposed.Should().BeTrue();
		}

		[Fact]
		public void It_Disposes_Disposable_When_Disposed_And_Exception()
		{
			var isDisposed = false;

			var viewModel = new ViewModelBase();

			viewModel.AddDisposable(new TestDisposable(() => throw new Exception()));
			viewModel.AddDisposable(new TestDisposable(() => isDisposed = true));

			viewModel.Dispose();

			isDisposed.Should().BeTrue();
		}

		[Fact]
		public void It_Clears_Disposables_When_Disposed()
		{
			var viewModel = new ViewModelBase();

			viewModel.AddDisposable(new TestDisposable());
			viewModel.AddDisposable(new TestDisposable());

			viewModel.Dispose();

			viewModel.Disposables.Should().BeEmpty();
		}

		[Fact]
		public void It_Exposes_A_Not_Cancelled_Token_When_Not_Disposed()
		{
			var viewModel = new ViewModelBase();
			viewModel.CancellationToken.IsCancellationRequested.Should().BeFalse();
		}

		[Fact]
		public void It_Exposes_A_Cancelled_Token_When_Disposed()
		{
			var viewModel = new ViewModelBase();

			viewModel.Dispose();
			viewModel.CancellationToken.IsCancellationRequested.Should().BeTrue();
		}

		[Fact]
		public async void It_doesnt_throw_when_adding_disposables_while_disposing()
		{
			// Here we test a concurrency issue.
			// When disposing, we iterate over the disposables so we want to make sure that trying to change those disposables don't create the following exception.
			// System.InvalidOperationException: Collection was modified; enumeration operation may not execute.
			// To validate this scenario, this test forces the race condition by manipulating the viewModel from two threads:
			// - Dispose the viewModel slowly (using disposables that take time to dispose).
			// - Add more disposables after the dispose operation has been started.

			// Arrange
			var viewModel = new ViewModelBase();
			for (int j = 0; j < 10; j++)
			{
				viewModel.AddDisposable(new TestDisposable(() =>
				{
					Thread.Sleep(10);
				}));
			}

			// Act
			var task = Task.Run(async () =>
			{
				await Task.Yield();
				viewModel.Dispose();
				await Task.Yield();
			});

			for (int j = 0; j < 100; j++)
			{
				Thread.Sleep(1);
				try
				{
					viewModel.AddDisposable(new TestDisposable());
				}
				catch (ObjectDisposedException)
				{
					// When the VM is fully disposed, adding a disposable should throw an ObjectDisposedException.
					// This is the correct behavior.
				}
			}

			// Assert
			await task;
		}
	}
}
