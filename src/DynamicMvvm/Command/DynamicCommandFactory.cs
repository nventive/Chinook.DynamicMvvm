using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// This is a default implementation of <see cref="IDynamicCommandFactory"/>.
	/// </summary>
	[Preserve(AllMembers = true)]
	public class DynamicCommandFactory : IDynamicCommandFactory
	{
		private readonly IDynamicCommandStrategyDecorator _globalDecorator;

		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicCommandFactory"/> class.
		/// </summary>
		/// <param name="globalDecorator">Global <see cref="IDynamicCommandStrategyDecorator"/></param>
		public DynamicCommandFactory(IDynamicCommandStrategyDecorator globalDecorator = null)
		{
			_globalDecorator = globalDecorator;
		}

		/// <summary>
		/// Will create new instance of <see cref="IDynamicCommand"/>.
		/// </summary>
		/// <param name="name">Command name</param>
		/// <param name="strategy"><see cref="IDynamicCommandStrategy"/></param>
		/// <param name="decorator"><see cref="IDynamicCommandStrategyDecorator"/></param>
		/// <returns><see cref="IDynamicCommand"/></returns>
		protected IDynamicCommand CreateCommand(string name, IDynamicCommandStrategy strategy, IDynamicCommandStrategyDecorator decorator)
		{
			// First decorate using the local decorator
			if (decorator != null)
			{
				strategy = decorator.Decorate(strategy);
			}

			// Then decorate using the global decorator
			if (_globalDecorator != null)
			{
				strategy = _globalDecorator.Decorate(strategy);
			}

			return new DynamicCommand(name, strategy);
		}

		/// <inheritdoc />
		public virtual IDynamicCommand CreateFromAction(string name, Action execute, IDynamicCommandStrategyDecorator decorator = null)
			=> CreateCommand(name, new ActionCommandStrategy(execute), decorator);

		/// <inheritdoc />
		public virtual IDynamicCommand CreateFromAction<TParameter>(string name, Action<TParameter> execute, IDynamicCommandStrategyDecorator decorator = null)
			=> CreateCommand(name, new ActionCommandStrategy<TParameter>(execute), decorator);

		/// <inheritdoc />
		public virtual IDynamicCommand CreateFromTask(string name, Func<CancellationToken, Task> execute, IDynamicCommandStrategyDecorator decorator = null)
			=> CreateCommand(name, new TaskCommandStrategy(execute), decorator);

		/// <inheritdoc />
		public virtual IDynamicCommand CreateFromTask<TParameter>(string name, Func<CancellationToken, TParameter, Task> execute, IDynamicCommandStrategyDecorator decorator = null)
			=> CreateCommand(name, new TaskCommandStrategy<TParameter>(execute), decorator);
	}
}
