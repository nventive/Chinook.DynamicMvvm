# Breaking Changes

## 3.0.0
- Added support for .NET 8.
- Removed support for .NET 7.

## 2.0.0
- Added support for .NET 7.
- Updated Uno.WinUI to 5.0.19.
- Updated Windows SDK version from 18362 to 19041.
- Removed support for Xamarin.
- Removed support for .NET 6.
- Removed support for NetStandard2.0 in DynamicMvvm.Uno.WinUI.

## 1.4.1
- Dynamic properties no longer throw an `ObjectDisposedException` when we set their `Value` while they're disposed.
  - We've discovered that this safeguard is not needed and was causing unjustified issues when using dynamic properties in a multi-threaded environment. This is especially true with the _DynamicPropertyFromObservable_ variant, which can easily be disposed from a different thread than the one the observable source uses.
  - This change renders obsolete the `throwOnDisposed` parameter used in several constructors of `IDynamicProperty` and `IDynamicPropertyFactory` implementations.
    Those overloads are still present in the library, but they are marked as obsolete and will be removed in a future version.
  - You can still observe the events where a dynamic property is set while it's disposed by using logs. The event id is 32, the log level is `Debug`, and the message template is `"Skipped value setter on the property '{PropertyName}' because it's disposed."`

This breaking changes doesn't change the API definition.

## 1.3.0
- The NuGet reference to `Microsoft.Extensions.Logging.Abstractions` now requires version 6.0.0 and up.

This breaking change doesn't change the API definition.

## 0.11.0
- `DecoratorCommandStrategy` not longer exists. Use `DelegatingCommandStrategy` instead.

## 0.10.0
- Replaced `IViewModelView` with `IDispatcher`.
- Replaced `IViewModel.View` with `IViewModel.Dispatcher`
