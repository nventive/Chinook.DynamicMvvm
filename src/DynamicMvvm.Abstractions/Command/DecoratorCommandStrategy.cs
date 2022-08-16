using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// This delegates the functionalities of <see cref="IDynamicCommandStrategy"/> to an inner strategy.
	/// You may override any member add customization.
	/// This class is an homologue to <see cref="System.Net.Http.DelegatingHandler"/>.
	/// </summary>
	public abstract class DelegatingCommandStrategy : IDynamicCommandStrategy
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DelegatingCommandStrategy"/> class.
		/// </summary>
		public DelegatingCommandStrategy()
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
