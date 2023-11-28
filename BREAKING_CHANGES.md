# Breaking Changes

## 2.0.0
- Added support for .NET 7.
- Updated Uno.WinUI to 5.0.19.
- Updated Windows SDK version from 18362 to 19041.
- Removed support for Xamarin.
- Removed support for .NET 6.
- Removed support for NetStandard2.0 in DynamicMvvm.Uno.WinUI.

## 0.11.0
- `DecoratorCommandStrategy` not longer exists. Use `DelegatingCommandStrategy` instead.

## 0.10.0
- Replaced `IViewModelView` with `IDispatcher`.
- Replaced `IViewModel.View` with `IViewModel.Dispatcher`
