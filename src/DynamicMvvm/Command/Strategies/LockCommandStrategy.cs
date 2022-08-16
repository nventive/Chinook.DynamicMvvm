using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chinook.DynamicMvvm
{
	public static partial class DynamicCommandStrategyExtensions
	{
		/// <summary>
		/// Will lock the command execution.
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <returns><see cref="IDynamicCommandBuilder"/></returns>
		public static IDynamicCommandBuilder Locked(this IDynamicCommandBuilder builder)
			=> builder.WithStrategy(new LockCommandStrategy());
	}

	/// <summary>
	/// This <see cref="DelegatingCommandStrategy"/> will lock the command execution.
	/// </summary>
	public class LockCommandStrategy : DelegatingCommandStrategy
	{
		private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

		/// <summary>
		/// Initializes a new instance of the <see cref="LockCommandStrategy"/> class.
		/// </summary>
		public LockCommandStrategy()
		{
		}

		/// <inheritdoc />
		public override async Task Execute(CancellationToken ct, object parameter, IDynamicCommand command)
		{
			try
			{
				await _semaphore.WaitAsync(ct);

				await base.Execute(ct, parameter, command);
			}
			finally
			{
				_semaphore.Release();
			}
		}
	}
}
