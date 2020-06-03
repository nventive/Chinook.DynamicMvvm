using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// Extensions on <see cref="IDynamicCommand"/>.
	/// </summary>
	public static class IDynamicCommandExtensions
	{
		/// <summary>
		/// Executes the specified <see cref="IDynamicCommand"/> without a parameter.
		/// </summary>
		/// <param name="command"><see cref="IDynamicCommand"/></param>
		/// <returns><see cref="Task"/></returns>
		public static Task Execute(this IDynamicCommand command)
		{
			return command.Execute(null);
		}
	}
}
