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
		private IDynamicCommandStrategy _innerStrategy;

		/// <summary>
		/// Initializes a new instance of the <see cref="DecoratorCommandStrategy"/> class.
		/// </summary>
		/// <param name="innerStrategy"><see cref="IDynamicCommandStrategy"/></param>
		public DecoratorCommandStrategy(IDynamicCommandStrategy innerStrategy)
		{
			InnerStrategy = innerStrategy;
		}

		/// <inheritdoc />
		public virtual IDynamicCommandStrategy InnerStrategy
		{
			get
			{
				if (_innerStrategy == null)
				{
					throw new InvalidOperationException($"Please set a {nameof(InnerStrategy)}");
				}

				return _innerStrategy;
			}

			set => _innerStrategy = value;
		}

		/// <inheritdoc />
		public virtual event EventHandler CanExecuteChanged
		{
			add => InnerStrategy.CanExecuteChanged += value;
			remove => InnerStrategy.CanExecuteChanged -= value;
		}

		/// <inheritdoc />
		public virtual bool CanExecute(object parameter, IDynamicCommand command)
			=> _innerStrategy.CanExecute(parameter, command);

		/// <inheritdoc />
		public virtual Task Execute(CancellationToken ct, object parameter, IDynamicCommand command)
			=> _innerStrategy.Execute(ct, parameter, command);

		/// <inheritdoc />
		public virtual void Dispose()
			=> _innerStrategy.Dispose();
	}
}
