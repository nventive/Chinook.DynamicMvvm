# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
* Support for Android 12 (March, 2022)
* Support for Android 11 (March, 2021)

### Changed
* Replaced `IViewModelView` with `IDispatcher`.
  * Replaced `IViewModel.View` with `IViewModel.Dispatcher`
* Update UWP target to 19041 and net framework to 4.7.2
* Build with VS2022
* [#09] Update Android target to 10.0
* [#10] Update the source of the `ViewModelBase` private `ILogger` instance.
* [#39] Change target Uno.UI version to 4.0.7.

### Deprecated
* `IViewModelView` not longer exists. Use `IDispatcher` instead.
### Removed
* Support for Android 10 (March, 2022)

### Fixed

### Security
