using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Chinook.DynamicMvvm.Tests.Helpers;
using Xunit;
using System.Threading;

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
