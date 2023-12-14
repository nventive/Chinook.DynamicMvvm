using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Diagnostics;

namespace Chinook.DynamicMvvm
{
	/// <summary>
	/// This is a default implementation of <see cref="IViewModel"/>.
	/// </summary>
	public partial class ViewModelBase : IViewModel
	{
		private static readonly DiagnosticSource _diagnostics = new DiagnosticListener("Chinook.DynamicMvvm.IViewModel");

		private readonly ILogger _logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewModelBase"/> class.
		/// </summary>
		/// <param name="name">The name of the ViewModel.</param>
		/// <param name="serviceProvider">The service provider.</param>
		public ViewModelBase(string name = null, IServiceProvider serviceProvider = null)
		{
			Name = name ?? GetType().Name;
			ServiceProvider = serviceProvider ?? DefaultServiceProvider;
			CancellationToken = _cts.Token;

			_logger = typeof(ViewModelBase).Log();

			if (_diagnostics.IsEnabled("Created"))
			{
				_diagnostics.Write("Created", Name);
			}

			_logger.LogViewModelCreated(Name);
		}

		/// <inheritdoc />
		public string Name { get; }

		/// <inheritdoc />
		public IServiceProvider ServiceProvider { get; }

		/// <summary>
		/// Gets or sets the default <see cref="IServiceProvider"/>.
		/// </summary>
		public static IServiceProvider DefaultServiceProvider { get; set; }
	}
}
