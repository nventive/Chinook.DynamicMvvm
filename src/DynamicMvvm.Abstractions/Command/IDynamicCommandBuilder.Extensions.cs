using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// Offers extension methods on <see cref="IDynamicCommandBuilder"/>.
	/// </summary>
	public static class DynamicCommandBuilderExtensions
	{
		/// <summary>
		/// Adds a <paramref name="strategy"/> to the builder.
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <param name="strategy">The strategy to add.</param>
		/// <param name="wrapExisting">When true, the <paramref name="strategy"/> is added at the start of the list, so that it wraps all existing strategies already present in the list.</param>
		/// <returns><see cref="IDynamicCommandBuilder"/></returns>
		public static IDynamicCommandBuilder WithStrategy(this IDynamicCommandBuilder builder, DecoratorCommandStrategy strategy, bool wrapExisting = false)
		{
			if (wrapExisting)
			{
				builder.Strategies.Insert(0, strategy);
			}
			else
			{
				builder.Strategies.Add(strategy);
			}
			return builder;
		}

		/// <summary>
		/// Removes strategies matching <typeparamref name="TStrategy"/> from the builder.
		/// </summary>
		/// <typeparam name="TStrategy">Any type implementing <see cref="IDynamicCommandStrategy"/>.</typeparam>
		/// <param name="builder">The builder.</param>
		/// <returns><see cref="IDynamicCommandBuilder"/></returns>
		public static IDynamicCommandBuilder WithoutStrategy<TStrategy>(this IDynamicCommandBuilder builder)
			where TStrategy : IDynamicCommandStrategy
		{
			var itemsToRevome = builder.Strategies.Where(s => s is TStrategy).ToList();
			foreach (var item in itemsToRevome)
			{
				builder.Strategies.Remove(item);
			}
			return builder;
		}

		/// <summary>
		/// Removes all strategies from the builder.
		/// </summary>
		/// <remarks>
		/// This can be usefull if you want to completely discard the default configuration.
		/// Note that the <see cref="IDynamicCommandBuilder.BaseStrategy"/> is not modified, only <see cref="IDynamicCommandBuilder.Strategies"/> is cleared.
		/// </remarks>
		/// <param name="builder">The builder.</param>
		/// <returns><see cref="IDynamicCommandBuilder"/></returns>
		public static IDynamicCommandBuilder ClearStrategies(this IDynamicCommandBuilder builder)
		{
			builder.Strategies.Clear();
			return builder;
		}
	}
}
