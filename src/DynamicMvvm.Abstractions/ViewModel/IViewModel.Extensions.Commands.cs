using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// Extensions on <see cref="IViewModel"/> to create <see cref="IDynamicCommand"/>.
	/// </summary>
	public static partial class IViewModelExtensions
	{
		/// <summary>
		/// Gets or creates a <see cref="IDynamicCommand"/> that will execute
		/// the specified <paramref name="execute"/> without any parameter.
		/// </summary>
		/// <param name="viewModel"><see cref="IViewModel"/></param>
		/// <param name="execute">Command execution</param>
		/// <param name="decorator">Command decorator</param>
		/// <param name="name">Command name</param>
		/// <returns><see cref="IDynamicCommand"/></returns>
		public static IDynamicCommand GetCommand(
			this IViewModel viewModel,
			Action execute,
			Func<IDynamicCommandStrategy, IDynamicCommandStrategy> decorator = null,
			[CallerMemberName] string name = null
		) => viewModel.GetOrCreateCommand(name, n => viewModel.GetDynamicCommandFactory().CreateFromAction(n, execute, new DynamicCommandStrategyDecorator(decorator)));

		/// <summary>
		/// Gets or creates a <see cref="IDynamicCommand"/> that will execute
		/// the specified <paramref name="execute"/> with a parameter of type <typeparamref name="TParameter"/>.
		/// </summary>
		/// <typeparam name="TParameter">Parameter type</typeparam>
		/// <param name="viewModel"><see cref="IViewModel"/></param>
		/// <param name="execute">Command execution</param>
		/// <param name="decorator">Command decorator</param>
		/// <param name="name">Command name</param>
		/// <returns><see cref="IDynamicCommand"/></returns>
		public static IDynamicCommand GetCommand<TParameter>(
			this IViewModel viewModel,
			Action<TParameter> execute,
			Func<IDynamicCommandStrategy, IDynamicCommandStrategy> decorator = null,
			[CallerMemberName] string name = null
		) => viewModel.GetOrCreateCommand(name, n => viewModel.GetDynamicCommandFactory().CreateFromAction(n, execute, new DynamicCommandStrategyDecorator(decorator)));

		/// <summary>
		/// Gets or creates a <see cref="IDynamicCommand"/> that will execute
		/// the specified <paramref name="execute"/> without any parameter.
		/// </summary>
		/// <param name="viewModel"><see cref="IViewModel"/></param>
		/// <param name="execute">Command execution</param>
		/// <param name="decorator">Command decorator</param>
		/// <param name="name">Command name</param>
		/// <returns><see cref="IDynamicCommand"/></returns>
		public static IDynamicCommand GetCommandFromTask(
			this IViewModel viewModel,
			Func<CancellationToken, Task> execute,
			Func<IDynamicCommandStrategy, IDynamicCommandStrategy> decorator = null,
			[CallerMemberName] string name = null
		) => viewModel.GetOrCreateCommand(name, n => viewModel.GetDynamicCommandFactory().CreateFromTask(n, execute, new DynamicCommandStrategyDecorator(decorator)));

		/// <summary>
		/// Gets or creates a <see cref="IDynamicCommand"/> that will execute
		/// the specified <paramref name="execute"/> with a parameter of type <typeparamref name="TParameter"/>.
		/// </summary>
		/// <typeparam name="TParameter">Parameter type</typeparam>
		/// <param name="viewModel"><see cref="IViewModel"/></param>
		/// <param name="execute">Command execution</param>
		/// <param name="decorator">Command decorator</param>
		/// <param name="name">Command name</param>
		/// <returns><see cref="IDynamicCommand"/></returns>
		public static IDynamicCommand GetCommandFromTask<TParameter>(
			this IViewModel viewModel,
			Func<CancellationToken, TParameter, Task> execute,
			Func<IDynamicCommandStrategy, IDynamicCommandStrategy> decorator = null,
			[CallerMemberName] string name = null
		) => viewModel.GetOrCreateCommand(name, n => viewModel.GetDynamicCommandFactory().CreateFromTask(n, execute, new DynamicCommandStrategyDecorator(decorator)));

		/// <summary>
		/// Gets or creates a <see cref="IDynamicCommand"/> that will be attached to the <paramref name="viewModel"/>.
		/// </summary>
		/// <param name="viewModel"><see cref="IViewModel"/></param>
		/// <param name="name">Command name</param>
		/// <param name="factory">Command factory</param>
		/// <returns><see cref="IDynamicCommand"/></returns>
		public static IDynamicCommand GetOrCreateCommand(
			this IViewModel viewModel,
			string name,
			Func<string, IDynamicCommand> factory
		)
		{
			if (!viewModel.TryGetDisposable<IDynamicCommand>(name, out var command))
			{
				command = factory(name);

				viewModel.AddDisposable(command.Name, command);
			}

			return command;
		}

		/// <summary>
		/// Gets the <see cref="IDynamicCommandFactory"/> for the <paramref name="viewModel"/>.
		/// </summary>
		/// <param name="viewModel"><see cref="IViewModel"/></param>
		/// <returns><see cref="IDynamicCommandFactory"/></returns>
		private static IDynamicCommandFactory GetDynamicCommandFactory(this IViewModel viewModel)
			=> viewModel.ServiceProvider.GetRequiredService<IDynamicCommandFactory>();
	}
}
