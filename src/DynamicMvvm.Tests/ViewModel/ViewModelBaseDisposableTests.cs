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
	}
}
