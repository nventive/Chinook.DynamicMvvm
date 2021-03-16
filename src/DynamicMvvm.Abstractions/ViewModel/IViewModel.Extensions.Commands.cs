﻿using System;
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
		/// <param name="configure">The optional func to configure the command builder.</param>
		/// <param name="name">Command name</param>
		/// <returns><see cref="IDynamicCommand"/></returns>
		public static IDynamicCommand GetCommand(
			this IViewModel viewModel,
			Action execute,
			Func<IDynamicCommandBuilder, IDynamicCommandBuilder> configure = null,
			[CallerMemberName] string name = null
		) => viewModel.GetOrCreateCommand(name, n => viewModel.GetDynamicCommandBuilderFactory().CreateFromAction(n, execute), configure);

		/// <summary>
		/// Gets or creates a <see cref="IDynamicCommand"/> that will execute
		/// the specified <paramref name="execute"/> with a parameter of type <typeparamref name="TParameter"/>.
		/// </summary>
		/// <typeparam name="TParameter">Parameter type</typeparam>
		/// <param name="viewModel"><see cref="IViewModel"/></param>
		/// <param name="execute">Command execution</param>
		/// <param name="configure">The optional func to configure the command builder.</param>
		/// <param name="name">Command name</param>
		/// <returns><see cref="IDynamicCommand"/></returns>
		public static IDynamicCommand GetCommand<TParameter>(
			this IViewModel viewModel,
			Action<TParameter> execute,
			Func<IDynamicCommandBuilder, IDynamicCommandBuilder> configure = null,
			[CallerMemberName] string name = null
		) => viewModel.GetOrCreateCommand(name, n => viewModel.GetDynamicCommandBuilderFactory().CreateFromAction(n, execute), configure);

		/// <summary>
		/// Gets or creates a <see cref="IDynamicCommand"/> that will execute
		/// the specified <paramref name="execute"/> without any parameter.
		/// </summary>
		/// <param name="viewModel"><see cref="IViewModel"/></param>
		/// <param name="execute">Command execution</param>
		/// <param name="configure">The optional func to configure the command builder.</param>
		/// <param name="name">Command name</param>
		/// <returns><see cref="IDynamicCommand"/></returns>
		public static IDynamicCommand GetCommandFromTask(
			this IViewModel viewModel,
			Func<CancellationToken, Task> execute,
			Func<IDynamicCommandBuilder, IDynamicCommandBuilder> configure = null,
			[CallerMemberName] string name = null
		) => viewModel.GetOrCreateCommand(name, n => viewModel.GetDynamicCommandBuilderFactory().CreateFromTask(n, execute), configure);

		/// <summary>
		/// Gets or creates a <see cref="IDynamicCommand"/> that will execute
		/// the specified <paramref name="execute"/> with a parameter of type <typeparamref name="TParameter"/>.
		/// </summary>
		/// <typeparam name="TParameter">Parameter type</typeparam>
		/// <param name="viewModel"><see cref="IViewModel"/></param>
		/// <param name="execute">Command execution</param>
		/// <param name="configure">The optional func to configure the command builder.</param>
		/// <param name="name">Command name</param>
		/// <returns><see cref="IDynamicCommand"/></returns>
		public static IDynamicCommand GetCommandFromTask<TParameter>(
			this IViewModel viewModel,
			Func<CancellationToken, TParameter, Task> execute,
			Func<IDynamicCommandBuilder, IDynamicCommandBuilder> configure = null,
			[CallerMemberName] string name = null
		) => viewModel.GetOrCreateCommand(name, n => viewModel.GetDynamicCommandBuilderFactory().CreateFromTask(n, execute), configure);

		/// <summary>
		/// Gets a different Instance of a command configured for every items in a list.
		/// </summary>
		/// <param name="viewModel">The ViewModel.</param>
		/// <param name="execute">The execute method.</param>
		/// <param name="name">The command name.</param>
		/// <returns><see cref="IDynamicCommand"/></returns>
		public static IDynamicCommand GetItemCommandFromTask(this IViewModel viewModel, Func<CancellationToken, Task> execute, [CallerMemberName] string name = null) => viewModel
			.GetCommandFromTask(execute, name:(name + new Guid()), configure: c => c
			);
		/// <summary>
		/// Gets a different Instance of a command configured for every items in a list.
		/// </summary>
		/// <typeparam name="TParameter">The type of the command parameter.</typeparam>
		/// <param name="viewModel">The ViewModel.</param>
		/// <param name="execute">The execute method.</param>
		/// <param name="name">The command name.</param>
		/// <returns><see cref="IDynamicCommand"/></returns>
		public static IDynamicCommand GetItemCommandFromTask<TParameter>(this IViewModel viewModel, Func<CancellationToken, TParameter, Task> execute, [CallerMemberName] string name = null) => viewModel
			.GetCommandFromTask<TParameter>(execute, name:(name + new Guid()), configure: c => c
			);

		/// <summary>
		/// Gets or creates a <see cref="IDynamicCommand"/> that will be attached to the <paramref name="viewModel"/>.
		/// </summary>
		/// <param name="viewModel"><see cref="IViewModel"/></param>
		/// <param name="name">Command name</param>
		/// <param name="factory">Command factory</param>
		/// <param name="configure">The optional func to configure the command builder.</param>
		/// <returns><see cref="IDynamicCommand"/></returns>
		public static IDynamicCommand GetOrCreateCommand(
			this IViewModel viewModel,
			string name,
			Func<string, IDynamicCommandBuilder> factory,
			Func<IDynamicCommandBuilder, IDynamicCommandBuilder> configure = null
		)
		{
			if (!viewModel.TryGetDisposable<IDynamicCommand>(name, out var command))
			{
				var builder = factory(name);
				if (configure != null)
				{
					builder = configure(builder);
				}
				command = builder.Build();

				viewModel.AddDisposable(command.Name, command);
			}

			return command;
		}

		/// <summary>
		/// Gets the <see cref="IDynamicCommandBuilderFactory"/> for the <paramref name="viewModel"/>.
		/// </summary>
		/// <param name="viewModel"><see cref="IViewModel"/></param>
		/// <returns><see cref="IDynamicCommandBuilderFactory"/></returns>
		private static IDynamicCommandBuilderFactory GetDynamicCommandBuilderFactory(this IViewModel viewModel)
			=> viewModel.ServiceProvider.GetRequiredService<IDynamicCommandBuilderFactory>();
	}
}
