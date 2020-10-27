using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chinook.DynamicMvvm
{
	public static partial class DynamicCommandStrategyExtensions
	{
		/// <summary>
		/// Will attach the <see cref="ICommand.CanExecute(object)"/> to the specified <see cref="IDynamicProperty"/>.
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <param name="canExecute"><see cref="IDynamicProperty"/> that affects the CanExecute</param>
		/// <returns><see cref="IDynamicCommandBuilder"/></returns>
		public static IDynamicCommandBuilder WithCanExecute(this IDynamicCommandBuilder builder, IDynamicProperty<bool> canExecute)
			=> builder.WithStrategy(new CanExecuteCommandStrategy(canExecute));
	}

	/// <summary>
	/// This <see cref="DecoratorCommandStrategy"/> will attach
	/// its <see cref="ICommand.CanExecute(object)"/> to the value of a <see cref="IDynamicProperty"/>.
	/// </summary>
	public class CanExecuteCommandStrategy : DecoratorCommandStrategy
	{
		private readonly IDynamicProperty<bool> _canExecute;

		/// <summary>
		/// Initializes a new instance of the <see cref="CanExecuteCommandStrategy"/> class.
		/// </summary>
		/// <param name="canExecute">Can execute property</param>
		public CanExecuteCommandStrategy(IDynamicProperty<bool> canExecute)
		{
			_canExecute = canExecute;

			_canExecute.ValueChanged += OnCanExecuteChanged;
		}

		public override IDynamicCommandStrategy InnerStrategy
		{
			get => base.InnerStrategy;
			set
			{
				if (base.InnerStrategy != null)
				{
					base.InnerStrategy.CanExecuteChanged -= OnInnerCanExecuteChanged;
				}

				base.InnerStrategy = value;

				if (base.InnerStrategy != null)
				{
					base.InnerStrategy.CanExecuteChanged += OnInnerCanExecuteChanged;
				}
			}
		}

		/// <inheritdoc />
		public override event EventHandler CanExecuteChanged;

		/// <inheritdoc />
		public override bool CanExecute(object parameter, IDynamicCommand command)
		{
			return _canExecute.Value && InnerStrategy.CanExecute(parameter, command);
		}

		private void RaiseCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}

		private void OnCanExecuteChanged(IDynamicProperty property)
		{
			RaiseCanExecuteChanged();
		}

		private void OnInnerCanExecuteChanged(object sender, EventArgs e)
		{
			RaiseCanExecuteChanged();
		}

		/// <inheritdoc />
		public override void Dispose()
		{
			_canExecute.ValueChanged -= OnCanExecuteChanged;
			InnerStrategy.CanExecuteChanged -= OnInnerCanExecuteChanged;

			base.Dispose();
		}
	}
}
