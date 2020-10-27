using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// This is a base implementation for <see cref="IDynamicCommandStrategy"/> decorators.
	/// </summary>
	public abstract class DecoratorCommandStrategy : IDynamicCommandStrategy
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DecoratorCommandStrategy"/> class.
		/// </summary>
		public DecoratorCommandStrategy()
		{
		}

		/// <inheritdoc />
		public virtual IDynamicCommandStrategy InnerStrategy { get; set; }

		/// <inheritdoc />
		public virtual event EventHandler CanExecuteChanged
		{
			add => InnerStrategy.CanExecuteChanged += value;
			remove => InnerStrategy.CanExecuteChanged -= value;
		}

		/// <inheritdoc />
		public virtual bool CanExecute(object parameter, IDynamicCommand command)
			=> InnerStrategy.CanExecute(parameter, command);

		/// <inheritdoc />
		public virtual Task Execute(CancellationToken ct, object parameter, IDynamicCommand command)
			=> InnerStrategy.Execute(ct, parameter, command);

		/// <inheritdoc />
		public virtual void Dispose()
			=> InnerStrategy.Dispose();
	}
}
