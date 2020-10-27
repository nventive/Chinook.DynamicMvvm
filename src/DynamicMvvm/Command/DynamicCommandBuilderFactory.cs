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
		/// <returns><see cref="IDynamicCommandBuilder"/></returns>
		protected IDynamicCommandBuilder CreateBuilder(string name, IDynamicCommandStrategy strategy)
		{
			IDynamicCommandBuilder builder = new DynamicCommandBuilder(name, strategy);

			if (_defaultConfigure != null)
			{
				builder = _defaultConfigure(builder);
			}

			return builder;
		}

		/// <inheritdoc />
		public virtual IDynamicCommandBuilder CreateFromAction(string name, Action execute)
			=> CreateBuilder(name, new ActionCommandStrategy(execute));

		/// <inheritdoc />
		public virtual IDynamicCommandBuilder CreateFromAction<TParameter>(string name, Action<TParameter> execute)
			=> CreateBuilder(name, new ActionCommandStrategy<TParameter>(execute));

		/// <inheritdoc />
		public virtual IDynamicCommandBuilder CreateFromTask(string name, Func<CancellationToken, Task> execute)
			=> CreateBuilder(name, new TaskCommandStrategy(execute));

		/// <inheritdoc />
		public virtual IDynamicCommandBuilder CreateFromTask<TParameter>(string name, Func<CancellationToken, TParameter, Task> execute)
			=> CreateBuilder(name, new TaskCommandStrategy<TParameter>(execute));
	}
}
