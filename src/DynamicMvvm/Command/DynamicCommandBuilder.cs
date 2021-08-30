using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// The default implementation of <see cref="IDynamicCommandBuilder"/>.
	/// </summary>
	public class DynamicCommandBuilder : IDynamicCommandBuilder
	{
		private IDynamicCommand _command;

		/// <summary>
		/// Creates a new instance of <see cref="DynamicCommandBuilder"/>.
		/// </summary>
		/// <param name="name">The name of the command.</param>
		/// <param name="baseStrategy">The base strategy to use for the command.</param>
		/// <param name="viewModel">The <see cref="IViewModel"/> that will own the newly created <see cref="IDynamicCommand"/>.</param>
		public DynamicCommandBuilder(string name, IDynamicCommandStrategy baseStrategy, IViewModel viewModel = null)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException($"'{nameof(name)}' cannot be null or empty", nameof(name));
			}

			Name = name;
			ViewModel = viewModel;
			BaseStrategy = baseStrategy ?? throw new ArgumentNullException(nameof(baseStrategy));
		}

		/// <inheritdoc/>
		public string Name { get; }

		/// <inheritdoc/>
		public IViewModel ViewModel { get; }

		/// <inheritdoc/>
		public IDynamicCommandStrategy BaseStrategy { get; }

		/// <inheritdoc/>
		public IList<DecoratorCommandStrategy> Strategies { get; set; } = new List<DecoratorCommandStrategy>();

		/// <inheritdoc/>
		public IDynamicCommand Build()
		{
			if (_command != null)
			{
				throw new InvalidOperationException("This builder already built a command. A DynamicCommandBuilder can only build a single command because strategy instances cannot be shared.");
			}

			var strategy = GetStrategy(BaseStrategy, Strategies);
			_command = new DynamicCommand(Name, strategy);

			return _command;
		}

		private static IDynamicCommandStrategy GetStrategy(IDynamicCommandStrategy baseStrategy, IList<DecoratorCommandStrategy> delegatingStrategies)
		{
			var strategy = baseStrategy;

			// We use 'Reverse' so that the first items in the builder wrap the ones added later on.
			foreach (var delegatingStrategy in delegatingStrategies.Reverse())
			{
				// Stitch up all the delegating strategies together.
				delegatingStrategy.InnerStrategy = strategy;
				strategy = delegatingStrategy;
			}

			return strategy;
		}
	}
}
