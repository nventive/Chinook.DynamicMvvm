using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Chinook.DynamicMvvm
{
	internal static partial class LoggerMessages
	{
		[LoggerMessage(1, LogLevel.Information, "ViewModel '{ViewModelName}' created.")]
		public static partial void LogViewModelCreated(this ILogger logger, string viewModelName);

		[LoggerMessage(2, LogLevel.Information, "ViewModel '{ViewModelName}' disposed.")]
		public static partial void LogViewModelDisposed(this ILogger logger, string viewModelName);

		[LoggerMessage(3, LogLevel.Debug, "Disposing ViewModel '{ViewModelName}'.")]
		public static partial void LogViewModelDisposing(this ILogger logger, string viewModelName);

		[LoggerMessage(4, LogLevel.Error, "Failed to dispose disposable '{DisposableKey}' of ViewModel '{ViewModelName}'.")]
		public static partial void LogViewModelFailedToDisposeDisposable(this ILogger logger, string disposableKey, string viewModelName, Exception exception);

		[LoggerMessage(5, LogLevel.Information, "ViewModel '{ViewModelName}' destroyed.")]
		public static partial void LogViewModelDestroyed(this ILogger logger, string viewModelName);

		[LoggerMessage(6, LogLevel.Debug, "Skipped '{MethodName}' on ViewModel '{ViewModelName}' because it's disposing.")]
		public static partial void LogViewModelSkippedMethodBecauseDisposing(this ILogger logger, string methodName, string viewModelName);

		[LoggerMessage(7, LogLevel.Debug, "Skipped '{MethodName}' for key '{DisposableKey}' on ViewModel '{ViewModelName}' because it's disposing.")]
		public static partial void LogViewModelSkippedMethodBecauseDisposing_DisposableKey(this ILogger logger, string methodName, string disposableKey, string viewModelName);

		[LoggerMessage(8, LogLevel.Debug, "Skipped '{MethodName}' for '{TypeName}.{PropertyName}' on ViewModel '{ViewModelName}' because it's disposing.")]
		public static partial void LogViewModelSkippedMethodBecauseDisposing_PropertyName(this ILogger logger, string methodName, string typeName, string propertyName, string viewModelName);

		[LoggerMessage(9, LogLevel.Debug, "Skipped '{MethodName}' for '{TypeName}.{PropertyName}' on ViewModel '{ViewModelName}' because it's disposed.")]
		public static partial void LogViewModelSkippedMethodBecauseDisposed_PropertyName(this ILogger logger, string methodName, string typeName, string propertyName, string viewModelName);

		[LoggerMessage(10, LogLevel.Debug, "Raised property changed for '{PropertyName}' from ViewModel '{ViewModelName}'.")]
		public static partial void LogViewModelRaisedPropertyChanged(this ILogger logger, string propertyName, string viewModelName);
				
		[LoggerMessage(11, LogLevel.Error, "Failed to raise PropertyChanged. Your IVewModel.Dispatcher is null. Make sure you set it. Make sure you link child viewmodels to their parent by using one of the following IViewModel extension method: AttachChild, GetChild, AttachOrReplaceChild.")]
		public static partial void LogViewModelFailedToRaisePropertyChangedWhenDispatcherIsNull(this ILogger logger, Exception exception);

		[LoggerMessage(20, LogLevel.Debug, "Executing command '{CommandName}'.")]
		public static partial void LogCommandExecuting(this ILogger logger, string commandName);

		[LoggerMessage(21, LogLevel.Information, "Executed command '{CommandName}'.")]
		public static partial void LogCommandExecuted(this ILogger logger, string commandName);

		[LoggerMessage(22, LogLevel.Error, "Failed to execute command '{CommandName}'.")]
		public static partial void LogCommandFailed(this ILogger logger, string commandName, Exception exception);

		[LoggerMessage(23, LogLevel.Error, "Failed to execute command '{CommandName}'. Consider using ErrorHandlerCommandStrategy.")]
		public static partial void LogCommandFailedConsiderUsingErrorHandlerCommandStrategy(this ILogger logger, string commandName, Exception exception);

		[LoggerMessage(24, LogLevel.Error, "Failed to execute command '{CommandName}' because it's disposed.")]
		public static partial void LogCommandFailedBecauseDisposed(this ILogger logger, string commandName);

		[LoggerMessage(30, LogLevel.Error, "Source observable subscription failed for property '{PropertyName}'. The property will no longer be updated by the observable.")]
		public static partial void LogDynamicPropertySourceObservableSubscriptionFailed(this ILogger logger, string propertyName, Exception exception);

		[LoggerMessage(31, LogLevel.Error, "Source task failed for property '{PropertyName}'.")]
		public static partial void LogDynamicPropertySourceTaskFailed(this ILogger logger, string propertyName, Exception exception);

		[LoggerMessage(32, LogLevel.Debug, "Skipped value setter on the property '{PropertyName}' because it's disposed.")]
		public static partial void LogDynamicPropertySkippedValueSetterBecauseDisposed(this ILogger logger, string propertyName);

		[LoggerMessage(40, LogLevel.Debug, "Deactivated observable source of property '{PropertyName}'.")]
		public static partial void LogDeactivatedObservableSource(this ILogger logger, string propertyName);

		[LoggerMessage(41, LogLevel.Debug, "Reactivated observable source of property '{PropertyName}'.")]
		public static partial void LogReactivatedObservableSource(this ILogger logger, string propertyName);

		[LoggerMessage(42, LogLevel.Debug, "Deactivated ViewModel '{ViewModelName}'.")]
		public static partial void LogDeactivatedViewModel(this ILogger logger, string viewModelName);

		[LoggerMessage(43, LogLevel.Debug, "Reactivated ViewModel '{ViewModelName}' and raised {PropertyChangedCount} property changes.")]
		public static partial void LogReactivatedViewModel(this ILogger logger, string viewModelName, int propertyChangedCount);

	}
}
