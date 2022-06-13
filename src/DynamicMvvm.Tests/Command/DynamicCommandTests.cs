using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using FluentAssertions;
using Chinook.DynamicMvvm.Tests.Command.Strategies;
using Chinook.DynamicMvvm.Tests.Helpers;
using Xunit;

namespace Chinook.DynamicMvvm.Tests.Command
{
	public class DynamicCommandTests
	{
		private const string DefaultCommandName = nameof(DefaultCommandName);

		[Fact]
		public async Task It_Executes_Strategy()
		{
			var isExecuted = false;

			var strategy = new TestCommandStrategy(
				onExecute: async (_, __, ___) => isExecuted = true
			);

			var command = new DynamicCommand(DefaultCommandName, strategy);

			await command.Execute();

			isExecuted.Should().BeTrue();
		}

		[Fact]
		public async Task It_Doesnt_Execute_Strategy_When_CantExecute()
		{
			var isExecuted = false;

			var strategy = new TestCommandStrategy(
				onCanExecute: (_, __) => false,
				onExecute: async (_, __, ___) => isExecuted = true
			);

			var command = new DynamicCommand(DefaultCommandName, strategy);

			await command.Execute();

			isExecuted.Should().BeFalse();
		}

		[Fact]
		public async Task It_Sends_Parameter_To_Strategy()
		{
			var canExecuteStrategyParameter = default(object);
			var executeStrategyParameter = default(object);

			var strategy = new TestCommandStrategy(
				onCanExecute: (p, __) => { canExecuteStrategyParameter = p; return true; },
				onExecute: async (_, p, ___) => executeStrategyParameter = p
			);

			var command = new DynamicCommand(DefaultCommandName, strategy);

			var parameter = new object();
			await command.Execute(parameter);

			canExecuteStrategyParameter.Should().Be(parameter);
			executeStrategyParameter.Should().Be(parameter);
		}

		[Fact]
		public async Task It_Raises_IsExecutingChanged()
		{
			var receivedValues = new List<(object, string, bool)>();

			var strategy = new TestCommandStrategy();
			var command = new DynamicCommand(DefaultCommandName, strategy);

			command.PropertyChanged += OnPropertyChanged;

			await command.Execute();

			receivedValues.Count().Should().Be(2);

			receivedValues[0].Item1.Should().Be(command);
			receivedValues[0].Item2.Should().Be(nameof(DynamicCommand.IsExecuting));
			receivedValues[0].Item3.Should().Be(true);

			receivedValues[1].Item1.Should().Be(command);
			receivedValues[1].Item2.Should().Be(nameof(DynamicCommand.IsExecuting));
			receivedValues[1].Item3.Should().Be(false);

			void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
			{
				receivedValues.Add((sender, e.PropertyName, command.IsExecuting));
			}
		}

		[Fact]
		public void It_Raises_CanExecuteChanged()
		{
			var canExecuteChanged = false;
			var strategy = new TestCommandStrategy();
			var command = new DynamicCommand(DefaultCommandName, strategy);

			command.CanExecuteChanged += OnCanExecuteChanged;

			strategy.RaiseCanExecuteChanged();

			canExecuteChanged.Should().BeTrue();

			void OnCanExecuteChanged(object sender, EventArgs e)
			{
				canExecuteChanged = true;
			}
		}

		[Fact]
		public async Task It_Awaits_Execution()
		{
			var taskCompletionSource = new TaskCompletionSource<object>();

			var strategy = new TestCommandStrategy(
				onExecute: async (_, __, ___) =>
				{
					await taskCompletionSource.Task;
				}
			);

			var command = new DynamicCommand(DefaultCommandName, strategy);

			var commandExecution = command.Execute();

			taskCompletionSource.TrySetResult(null);

			await commandExecution;

			taskCompletionSource.Task.IsCompleted.Should().BeTrue();
		}

		[Fact]
		public async Task It_Executes_Multiple_In_Parallel()
		{
			var actualExecutions = 0;

			var tasks = new[]
			{
				new TaskCompletionSource<object>(),
				new TaskCompletionSource<object>(),
				new TaskCompletionSource<object>(),
				new TaskCompletionSource<object>(),
			};

			var strategy = new TestCommandStrategy(
				onExecute: async (_, i, ___) =>
				{
					actualExecutions++;

					await tasks[(int)i].Task;
				}
			);

			var command = new DynamicCommand(DefaultCommandName, strategy);

			var executions = tasks
				.Select((t, i) => command.Execute(i))
				.ToArray();

			Array.ForEach(tasks, t => t.TrySetResult(null));

			await Task.WhenAll(executions);

			actualExecutions.Should().Be(tasks.Length);
		}

		[Fact]
		public async Task It_Stays_Executing_When_Multiple_Executions()
		{
			var receivedValues = new List<(object, bool)>();

			var tasks = new[]
			{
				new TaskCompletionSource<object>(),
				new TaskCompletionSource<object>(),
				new TaskCompletionSource<object>(),
				new TaskCompletionSource<object>(),
			};

			var strategy = new TestCommandStrategy(
				onExecute: async (_, i, ___) =>
				{
					await tasks[(int)i].Task;
				}
			);

			var command = new DynamicCommand(DefaultCommandName, strategy);

			command.IsExecutingChanged += OnIsExecutingChanged;

			var executions = tasks
				.Select((t, i) => command.Execute(i))
				.ToArray();

			Array.ForEach(tasks, t => t.TrySetResult(null));

			await Task.WhenAll(executions);

			receivedValues.Count().Should().Be(2);

			receivedValues[0].Item1.Should().Be(command);
			receivedValues[0].Item2.Should().Be(true);

			receivedValues[1].Item1.Should().Be(command);
			receivedValues[1].Item2.Should().Be(false);

			void OnIsExecutingChanged(object sender, EventArgs e)
			{
				receivedValues.Add((sender, command.IsExecuting));
			}
		}

		[Fact]
		public void It_Supports_ICommand()
		{
			var isExecuted = false;
			var taskCompletionSource = new TaskCompletionSource<object>();

			var strategy = new TestCommandStrategy(
				onExecute: async (_, __, ___) =>
				{
					isExecuted = true;

					await taskCompletionSource.Task;
				}
			);

			var command = new DynamicCommand(DefaultCommandName, strategy);

			((ICommand)command).Execute(null);

			taskCompletionSource.TrySetResult(null);

			isExecuted.Should().BeTrue();
			taskCompletionSource.Task.IsCompleted.Should().BeTrue();
		}

		[Fact]
		public void It_Disposes_Strategy_When_Disposed()
		{
			var isDisposed = false;

			var strategy = new TestCommandStrategy(onDispose: () => isDisposed = true);
			var command = new DynamicCommand(DefaultCommandName, strategy);

			command.Dispose();

			isDisposed.Should().BeTrue();
		}

		[Fact]
		public async Task It_Cancels_CancellationToken_When_Disposed()
		{
			var taskCompletionSource = new TaskCompletionSource<object>();

			var strategy = new TestCommandStrategy(
				onExecute: async (ct, _, __) =>
				{
					using (ct.Register(() => taskCompletionSource.TrySetCanceled()))
					{
						await taskCompletionSource.Task;
					}
				}
			);

			var command = new DynamicCommand(DefaultCommandName, strategy);

			var commandExecution = command.Execute();

			command.Dispose();

			await commandExecution;

			taskCompletionSource.Task.IsCanceled.Should().BeTrue();
		}

		[Fact]
		public async Task It_Doesnt_Execute_When_Disposed()
		{
			var didExecute = false;
			var didIsExecutingChanged = false;
			var strategy = new TestCommandStrategy(
				onExecute: async (ct, _, __) =>
				{
					didExecute = true;
				}
			);

			var command = new DynamicCommand(DefaultCommandName, strategy);
			command.IsExecutingChanged += OnIsExecutingChanged;
			
			command.Dispose();
			
			var commandExecution = command.Execute();

			await commandExecution;

			didExecute.Should().BeFalse();
			didIsExecutingChanged.Should().BeFalse();
			
			void OnIsExecutingChanged(object sender, EventArgs e)
			{
				didIsExecutingChanged = true;
			}
		}
	}
}
