using System;
using System.Collections.Generic;
using System.Text;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// This interface is used to decorate <see cref="IDynamicCommandStrategy"/>.
	/// </summary>
	public interface IDynamicCommandStrategyDecorator
	{
		/// <summary>
		/// Returns a decorated <paramref name="dynamicCommand"/>.
		/// </summary>
		/// <param name="dynamicCommand">DynamicCommand</param>
		/// <returns>Decorated DynamicCommand</returns>
		IDynamicCommandStrategy Decorate(IDynamicCommandStrategy dynamicCommand);
	}
}
