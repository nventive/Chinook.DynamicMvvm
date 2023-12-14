# Breaking Changes

## 1.3.0
- The NuGet reference to `Microsoft.Extensions.Logging.Abstractions` now requires version 6.0.0 and up.

## 0.11.0
- `DecoratorCommandStrategy` not longer exists. Use `DelegatingCommandStrategy` instead.

## 0.10.0
- Replaced `IViewModelView` with `IDispatcher`.
- Replaced `IViewModel.View` with `IViewModel.Dispatcher`
