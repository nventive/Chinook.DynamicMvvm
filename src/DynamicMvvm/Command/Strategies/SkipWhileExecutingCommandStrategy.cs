using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Chinook.DynamicMvvm;

namespace Chinook.DynamicMvvm
{
	public static partial class DynamicCommandStrategyExtensions
	{
		/// <summary>
		/// Will skip executions if the command is already executing.
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <returns><see cref="IDynamicCommandBuilder"/></returns>
		public static IDynamicCommandBuilder SkipWhileExecuting(this IDynamicCommandBuilder builder)
			=> builder.WithStrategy(new SkipWhileExecutingCommandStrategy());
	}

	/// <summary>
	/// This <see cref="DelegatingCommandStrategy"/> will skip executions if the command is already executing.
	/// </summary>
	public class SkipWhileExecutingCommandStrategy : DelegatingCommandStrategy
	{
		public int _isExecuting;

		/// <summary>
		/// Initializes a new instance of the <see cref="SkipWhileExecutingCommandStrategy"/> class.
		/// </summary>
		public SkipWhileExecutingCommandStrategy()
		{
		}

		/// <inheritdoc />
		public override async Task Execute(CancellationToken ct, object parameter, IDynamicCommand command)
		{
			if (Interlocked.CompareExchange(ref _isExecuting, 1, 0) == 0)
			{
				try
				{
					await base.Execute(ct, parameter, command);
				}
				finally
				{
					_isExecuting = 0;
				}
			}
		}
	}
}
