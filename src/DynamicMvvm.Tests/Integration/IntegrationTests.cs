﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Chinook.DynamicMvvm.Tests.Integration
{
	public class IntegrationTests
	{
		private readonly IServiceProvider _serviceProvider;

		public IntegrationTests()
		{
			var serviceCollection = new ServiceCollection();

			serviceCollection.AddSingleton<IDynamicPropertyFactory, DynamicPropertyFactory>();
			serviceCollection.AddSingleton<IDynamicCommandFactory, DynamicCommandFactory>();

			_serviceProvider = serviceCollection.BuildServiceProvider();
		}

		[Fact]
		public async Task It_Updates_Property_With_Projection()
		{
			var viewModel = new MyViewModel(_serviceProvider);

			viewModel.Counter.Should().Be(0);
			viewModel.CounterString.Should().Be("0");

			viewModel.Counter++;

			viewModel.Counter.Should().Be(1);
			viewModel.CounterString.Should().Be("1");

			await viewModel.IncrementCounter.Execute();

			viewModel.Counter.Should().Be(2);
			viewModel.CounterString.Should().Be("2");

			viewModel.CounterString = "3";

			viewModel.Counter.Should().Be(2);
			viewModel.CounterString.Should().Be("3");

			await viewModel.IncrementCounterAsync.Execute();

			viewModel.Counter.Should().Be(3);
			viewModel.CounterString.Should().Be("3");
		}

		[Fact]
		public async Task It_Completes_Command_When_Disposed()
		{
			var viewModel = new MyViewModel(_serviceProvider);

			var longRunningOperation = viewModel.LongRunningOperation.Execute();

			viewModel.Dispose();

			await longRunningOperation;

			longRunningOperation.Status.Should().Be(TaskStatus.RanToCompletion);
		}

		[Fact]
		public async Task It_Catches_Command_Errors()
		{
			var viewModel = new MyViewModel(_serviceProvider);

			await viewModel.IncrementCounterOrThrow.Execute();
			viewModel.ErrorMessage.Should().BeNull();

			await viewModel.IncrementCounterOrThrow.Execute();
			viewModel.ErrorMessage.Should().NotBeNullOrEmpty();
		}

		[Fact]
		public async Task It_Supports_Multiple_CanExecute()
		{
			var viewModel = new MyViewModel(_serviceProvider);

			await viewModel.MyCommand.Execute();
			viewModel.Counter.Should().Be(0);

			viewModel.CanExecuteMyCommand = true;

			await viewModel.MyCommand.Execute();
			viewModel.Counter.Should().Be(1);
		}

		private class MyViewModel : ViewModelBase
		{
			public MyViewModel(IServiceProvider serviceProvider)
				: base(serviceProvider: serviceProvider)
			{
				// TODO #174135 Property from Projection?
				var updateCounterStringWhenCounterChanges = this.GetProperty(v => v.Counter)
					.Subscribe(p => CounterString = p.Value.ToString());

				AddDisposable(updateCounterStringWhenCounterChanges);
			}

			public int Counter
			{
				get => this.Get<int>();
				set => this.Set(value);
			}

			public string CounterString
			{
				get => this.Get(Counter.ToString());
				set => this.Set(value);
			}

			public string ErrorMessage
			{
				get => this.Get<string>();
				set => this.Set(value);
			}

			public bool CanExecuteMyCommand
			{
				get => this.Get<bool>();
				set => this.Set(value);
			}

			public IDynamicCommand IncrementCounter => this.GetCommand(() =>
			{
				Counter++;
			});

			public IDynamicCommand IncrementCounterAsync => this.GetCommandFromTask(async ct =>
			{
				await Task.Delay(TimeSpan.FromMilliseconds(100), ct);

				Counter++;
			});

			public IDynamicCommand LongRunningOperation => this.GetCommandFromTask(async ct =>
			{
				await Task.Delay(TimeSpan.FromHours(1), ct);
			});

			public IDynamicCommand IncrementCounterOrThrow => this.GetCommand(() =>
			{
				if (Counter > 0)
				{
					throw new Exception("Counter was already incremented.");
				}

				Counter++;
			}, c => c.CatchErrors(OnError));

			public IDynamicCommand MyCommand => this.GetCommand(
				() => Counter++,
				c => c
					.WithCanExecute(this.GetProperty(v => v.CanExecuteMyCommand))
					.CatchErrors(OnError)
					.DisableWhileExecuting()
			);

			private Task OnError(CancellationToken ct, IDynamicCommand command, Exception e)
			{
				ErrorMessage = e.Message;

				return Task.CompletedTask;
			}
		}
	}
}
