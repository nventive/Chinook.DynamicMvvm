using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// This is a default implementation of <see cref="IDynamicCommandBuilderFactory"/>.
	/// </summary>
	[Preserve(AllMembers = true)]
	public class DynamicCommandBuilderFactory : IDynamicCommandBuilderFactory
	{
		private readonly Func<IDynamicCommandBuilder, IDynamicCommandBuilder> _defaultConfigure;

		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicCommandBuilderFactory"/> class.
		/// </summary>
		public DynamicCommandBuilderFactory(Func<IDynamicCommandBuilder, IDynamicCommandBuilder> defaultConfigure = null)
		{
			_defaultConfigure = defaultConfigure;
		}

		/// <summary>
		/// Will create new instance of <see cref="IDynamicCommandBuilder"/>.
		/// </summary>
		/// <param name="name">Command name</param>
		/// <param name="strategy"><see cref="IDynamicCommandStrategy"/></param>
		/// <param name="viewModel">The <see cref="IViewModel"/> that will own the newly created command.</param>
		/// <returns><see cref="IDynamicCommandBuilder"/></returns>
		protected IDynamicCommandBuilder CreateBuilder(string name, IDynamicCommandStrategy strategy, IViewModel viewModel)
		{
			IDynamicCommandBuilder builder = new DynamicCommandBuilder(name, strategy, viewModel);

			if (_defaultConfigure != null)
			{
				builder = _defaultConfigure(builder);
			}

			return builder;
		}

		/// <inheritdoc />
		public virtual IDynamicCommandBuilder CreateFromAction(string name, Action execute, IViewModel viewModel = null)
			=> CreateBuilder(name, new ActionCommandStrategy(execute), viewModel);

		/// <inheritdoc />
		public virtual IDynamicCommandBuilder CreateFromAction<TParameter>(string name, Action<TParameter> execute, IViewModel viewModel = null)
			=> CreateBuilder(name, new ActionCommandStrategy<TParameter>(execute), viewModel);

		/// <inheritdoc />
		public virtual IDynamicCommandBuilder CreateFromTask(string name, Func<CancellationToken, Task> execute, IViewModel viewModel = null)
			=> CreateBuilder(name, new TaskCommandStrategy(execute), viewModel);

		/// <inheritdoc />
		public virtual IDynamicCommandBuilder CreateFromTask<TParameter>(string name, Func<CancellationToken, TParameter, Task> execute, IViewModel viewModel = null)
			=> CreateBuilder(name, new TaskCommandStrategy<TParameter>(execute), viewModel);
	}
}
