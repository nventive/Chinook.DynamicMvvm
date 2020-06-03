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
		/// <param name="innerStrategy"><see cref="IDynamicCommandStrategy"/></param>
		/// <returns><see cref="IDynamicCommandStrategy"/></returns>
		public static IDynamicCommandStrategy Locked(this IDynamicCommandStrategy innerStrategy)
			=> new LockCommandStrategy(innerStrategy);
	}

	/// <summary>
	/// This <see cref="DecoratorCommandStrategy"/> will lock the command execution.
	/// </summary>
	public class LockCommandStrategy : DecoratorCommandStrategy
	{
		private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

		/// <summary>
		/// Initializes a new instance of the <see cref="LockCommandStrategy"/> class.
		/// </summary>
		/// <param name="innerStrategy"><see cref="IDynamicCommandStrategy"/></param>
		public LockCommandStrategy(IDynamicCommandStrategy innerStrategy)
			: base(innerStrategy)
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
