using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Chinook.DynamicMvvm;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// Extensions on <see cref="IViewModel"/> to validate properties.
	/// </summary>
	public static partial class IViewModelExtensions
	{
		/// <summary>
		/// Gets the <see cref="IValidator"/> for the given <see cref="IViewModel"/>.
		/// </summary>
		/// <typeparam name="TViewModel">Type of ViewModel</typeparam>
		/// <param name="viewModel">ViewModel</param>
		/// <returns><see cref="IValidator"/></returns>
		public static IValidator<TViewModel> GetValidator<TViewModel>(this TViewModel viewModel)
			where TViewModel : IViewModel
		{
			var validator = viewModel.ServiceProvider.GetService<IValidator<TViewModel>>();

			if (validator is null)
			{
				throw new InvalidOperationException($"You must register an implementation of type {typeof(IValidator<TViewModel>)}.");
			}

			return validator;
		}

		/// <summary>
		/// Adds validation to the <see cref="IViewModel"/> using <see cref="IValidator"/>.
		/// </summary>
		/// <typeparam name="TViewModel">Type of ViewModel</typeparam>
		/// <param name="viewModel">ViewModel</param>
		/// <param name="property">Property to validate</param>
		public static void AddValidation<TViewModel>(this TViewModel viewModel, IDynamicProperty property)
			where TViewModel : IViewModel
		{
			if (property is null)
			{
				throw new ArgumentNullException(nameof(property));
			}

			var key = $"{property.Name}_Validation";

			if (!viewModel.TryGetDisposable(key, out var _))
			{
				var validator = viewModel.GetValidator();

				viewModel.AddDisposable(key, new DynamicPropertyValidationDisposable(property, (ct, p) => viewModel.ValidateProperty(ct, p)));
			}
		}

		/// <summary>
		/// Validates a given <paramref name="property"/>.
		/// </summary>
		/// <typeparam name="TViewModel">Type of ViewModel</typeparam>
		/// <param name="viewModel"><see cref="IViewModel"/></param>
		/// <param name="ct"><see cref="CancellationToken"/></param>
		/// <param name="property"><see cref="IDynamicProperty"/></param>
		/// <returns>True if valid; false otherwise.</returns>
		public static async Task<ValidationResult> ValidateProperty<TViewModel>(
			this TViewModel viewModel,
			CancellationToken ct,
			IDynamicProperty property
		) where TViewModel : IViewModel
		{
			if (property is null)
			{
				throw new ArgumentNullException(nameof(property));
			}

			var validator = viewModel.GetValidator();

			// Validate a single property instead of the whole view model.
			var context = new ValidationContext<TViewModel>(
				viewModel,
				new PropertyChain(),
				new MemberNameValidatorSelector(new [] { property.Name })
			);

			var validationResult = await validator.ValidateAsync(context, ct);

			if (validationResult.IsValid)
			{
				viewModel.ClearErrors(property.Name);
			}
			else
			{
				var validationErrors = validationResult
					.Errors
					.Select(x => x as object);

				viewModel.SetErrors(property.Name, validationErrors);
			}

			return validationResult;
		}

		/// <summary>
		/// Validates a given <see cref="IViewModel"/>.
		/// </summary>
		/// <typeparam name="TViewModel">Type of ViewModel</typeparam>
		/// <param name="viewModel"><see cref="IViewModel"/></param>
		/// <param name="ct"><see cref="CancellationToken"/></param>
		/// <returns>Validation result</returns>
		public static async Task<ValidationResult> Validate<TViewModel>(this TViewModel viewModel, CancellationToken ct)
			where TViewModel : IViewModel
		{
			var validator = viewModel.GetValidator();

			var validationResult = await validator.ValidateAsync(viewModel, ct);

			if (validationResult.IsValid)
			{
				viewModel.ClearErrors();
			}
			else
			{
				var validationErrors = validationResult
					.Errors
					.GroupBy(x => x.PropertyName)
					.ToDictionary(
						x => x.Key,
						x => x.Select(y => y as object)
					);

				viewModel.SetErrors(validationErrors);		
			}

			return validationResult;
		}

		private class DynamicPropertyValidationDisposable : IDisposable
		{
			private CancellationTokenSource _cancellationTokenSource;
			private IDynamicProperty _property;
			private readonly Func<CancellationToken, IDynamicProperty, Task> _validationFunc;

			public DynamicPropertyValidationDisposable(IDynamicProperty property, Func<CancellationToken, IDynamicProperty, Task> validationFunc)
			{
				_property = property ?? throw new ArgumentNullException(nameof(property));
				_validationFunc = validationFunc ?? throw new ArgumentNullException(nameof(validationFunc));

				_property.ValueChanged += OnValueChanged;
			}

			private void OnValueChanged(IDynamicProperty property)
			{
				TryCancelExecution();

				_cancellationTokenSource = new CancellationTokenSource();

				_ = Task.Run(async () =>
				{
					try
					{
						await _validationFunc(_cancellationTokenSource.Token, property);
					}
					catch (Exception e)
					{
						this.Log().LogError(e, $"Validation failed for property '{property.Name}'.");
					}
				});
			}

			private void TryCancelExecution()
			{
				if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
				{
					_cancellationTokenSource.Cancel();
					_cancellationTokenSource.Dispose();
				}
			}

			public void Dispose()
			{
				TryCancelExecution();

				_property.ValueChanged -= OnValueChanged;
				_property = null;
			}
		}
	}
}
