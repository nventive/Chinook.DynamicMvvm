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
		/// <param name="innerStrategy"><see cref="IDynamicCommandStrategy"/></param>
		/// <returns><see cref="IDynamicCommandStrategy"/></returns>
		public static IDynamicCommandStrategy SkipWhileExecuting(this IDynamicCommandStrategy innerStrategy)
			=> new SkipWhileExecutingCommandStrategy(innerStrategy);
	}

	/// <summary>
	/// This <see cref="DecoratorCommandStrategy"/> will skip executions if the command is already executing.
	/// </summary>
	public class SkipWhileExecutingCommandStrategy : DecoratorCommandStrategy
	{
		public int _isExecuting;

		/// <summary>
		/// Initializes a new instance of the <see cref="SkipWhileExecutingCommandStrategy"/> class.
		/// </summary>
		/// <param name="innerStrategy"><see cref="IDynamicCommandStrategy"/></param>
		public SkipWhileExecutingCommandStrategy(IDynamicCommandStrategy innerStrategy)
			: base(innerStrategy)
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
