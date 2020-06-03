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
		/// Will execute the command on a background thread.
		/// </summary>
		/// <param name="innerStrategy"><see cref="IDynamicCommandStrategy"/></param>
		/// <returns><see cref="IDynamicCommandStrategy"/></returns>
		public static IDynamicCommandStrategy OnBackgroundThread(this IDynamicCommandStrategy innerStrategy)
			=> new BackgroundCommandStrategy(innerStrategy);
	}

	/// <summary>
	/// This <see cref="DecoratorCommandStrategy"/> will execute the command on a background thread.
	/// </summary>
	public class BackgroundCommandStrategy : DecoratorCommandStrategy
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BackgroundCommandStrategy"/> class.
		/// </summary>
		/// <param name="innerStrategy"><see cref="IDynamicCommandStrategy"/></param>
		public BackgroundCommandStrategy(IDynamicCommandStrategy innerStrategy)
			: base(innerStrategy)
		{

		}

		/// <inheritdoc />
		public override Task Execute(CancellationToken ct, object parameter, IDynamicCommand command)
		{
			return Task.Run(() => base.Execute(ct, parameter, command));
		}
	}
}
