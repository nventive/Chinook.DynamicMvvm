using System;
using System.Collections.Generic;
using System.Text;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// This is a default implementation of <see cref="IDynamicCommandStrategyDecorator"/>.
	/// </summary>
	public class DynamicCommandStrategyDecorator : IDynamicCommandStrategyDecorator
	{
		private readonly Func<IDynamicCommandStrategy, IDynamicCommandStrategy> _delegatingDecorator;

		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicCommandStrategyDecorator"/> class.
		/// </summary>
		/// <param name="delegatingDecorator"></param>
		public DynamicCommandStrategyDecorator(Func<IDynamicCommandStrategy, IDynamicCommandStrategy> delegatingDecorator)
		{
			_delegatingDecorator = delegatingDecorator;
		}

		/// <inheritdoc />
		public IDynamicCommandStrategy Decorate(IDynamicCommandStrategy dynamicCommand)
		{
			if (_delegatingDecorator != null)
			{
				return _delegatingDecorator.Invoke(dynamicCommand);
			}

			return dynamicCommand;
		}
	}
}
