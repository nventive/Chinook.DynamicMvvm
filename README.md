# Chinook.DynamicMvvm
<!-- ALL-CONTRIBUTORS-BADGE:START - Do not remove or modify this section -->
[![All Contributors](https://img.shields.io/badge/all_contributors-3-orange.svg?style=flat-square)](#contributors-)
<!-- ALL-CONTRIBUTORS-BADGE:END -->

The `Chinook.DynamicMvvm` library assists in MVVM (Model - View - ViewModel) development.

There are 3 main components to `Chinook.DynamicMvvm`:

* [IViewModel](src/DynamicMvvm.Abstractions/ViewModel/IViewModel.md): `INotifyPropertyChanged` object which contains properties and commands
* [IDynamicProperty](src/DynamicMvvm.Abstractions/Property/IDynamicProperty.md): a property that will notify its subscribers when its value changes.
* [IDynamicCommand](src/DynamicMvvm.Abstractions/Command/IDynamicCommand.md): a `ICommand` that will notify its subscribers when it is executing.

[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](LICENSE)

## Getting Started

Add the `Chinook.DynamicMvvm` nuget package and its dependencies to your project.

If you want to use [Reactive Extensions](https://github.com/dotnet/reactive), we recommend installing `Chinook.DynamicMvvm.Reactive` to gain access to relevant `IDynamicProperty` and `IDynamicCommand` extension methods

If you want to use [FluentValidations](https://fluentvalidation.net/), we recommend installing `DynamicMvvm.FluentValidation` to gain access to `IViewModel` extension methods.

We also recommend installing the [Chinook Snippets](https://marketplace.visualstudio.com/items?itemName=nventivecorp.ChinookSnippets) Visual Studio Extension to benefit from code snippets.

## Features

Please refer to the documentation for [IViewModel](src/DynamicMvvm.Abstractions/ViewModel/IViewModel.md#Features), [IDynamicProperty](src/DynamicMvvm.Abstractions/Property/IDynamicProperty.md#Features) and [IDynamicCommand](src/DynamicMvvm.Abstractions/Command/IDynamicCommand.md#Features) for a full list of the features! Here's a small code sample showing the interaction between `IViewModel`, `IDynamicProperty` and `IDynamicCommand`:

```csharp
public class MyViewModel : ViewModelBase
{
  // This will create and attach a new IDynamicProperty named "MyProperty"
  // to your IViewModel. It's initial value will be default(int).
  public int MyProperty => this.Get<int>();

  // This will create and attach a new IDynamicProperty named "MyStringProperty"
  // to your IViewModel. It's initial value will be "hello". It can be bound two-way.
  public string MyStringProperty
  {
    get => this.Get(initialValue: "hello");
    set => this.Set(value);
  }

  // This will create a DynamicProperty with a name of "MyAsyncProperty" and an initial value of 10.
  // When the task completes, the property will be updated with its result and will notify its subscribers of this change.
  public int MyAsyncProperty => this.GetFromTask(MyTask, initialValue: 10);
  
  private Task<int> MyTask(CancellationToken ct) => Task.FromResult(20);

  // This will create and attach a new IDynamicCommand named "MyCommand"
  // to your IViewModel. It will call the Execute method when executed.
  public IDynamicCommand MyCommand => this.GetCommand(() => 
  {
    // Your custom logic.
  }));

  // This will create a DynamicCommand with a name of "MyAsyncCommandWithParameter".
  // The ExecuteCommand method will be called when MyAsyncCommandWithParameter is being executed.
  public IDynamicCommand MyAsyncCommandWithParameter => this.GetCommandFromTask<int>(ExecuteCommand);
  
  private async Task ExecuteCommand(CancellationToken ct, int parameter)
  {
    // Your custom async logic.
  }
}
```

## Changelog

Please consult the [CHANGELOG](CHANGELOG.md) for more information about version
history.

## License

This project is licensed under the Apache 2.0 license - see the
[LICENSE](LICENSE) file for details.

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on the process for
contributing to this project.

Be mindful of our [Code of Conduct](CODE_OF_CONDUCT.md).

## Contributors

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->
<table>
  <tr>
    <td align="center"><a href="https://github.com/jeanplevesque"><img src="https://avatars3.githubusercontent.com/u/39710855?v=4" width="100px;" alt=""/><br /><sub><b>Jean-Philippe Lévesque</b></sub></a><br /><a href="https://github.com/nventive/Chinook.DynamicMvvm/commits?author=jeanplevesque" title="Code">💻</a> <a href="https://github.com/nventive/Chinook.DynamicMvvm/commits?author=jeanplevesque" title="Tests">⚠️</a></td>
    <td align="center"><a href="https://github.com/jeremiethibeault"><img src="https://avatars3.githubusercontent.com/u/5444226?v=4" width="100px;" alt=""/><br /><sub><b>Jérémie Thibeault</b></sub></a><br /><a href="https://github.com/nventive/Chinook.DynamicMvvm/commits?author=jeremiethibeault" title="Tests">⚠️</a> <a href="https://github.com/nventive/Chinook.DynamicMvvm/commits?author=jeremiethibeault" title="Code">💻</a></td>
    <td align="center"><a href="https://github.com/MatFillion"><img src="https://avatars0.githubusercontent.com/u/7029537?v=4" width="100px;" alt=""/><br /><sub><b>Mathieu Fillion</b></sub></a><br /><a href="https://github.com/nventive/Chinook.DynamicMvvm/commits?author=MatFillion" title="Code">💻</a></td>
    <td align="center"><a href="https://github.com/jcantin-nventive"><img src="https://avatars0.githubusercontent.com/u/43351943?v=4" width="100px;" alt=""/><br /><sub><b>Julie Cantin</b></sub></a><br /><a href="https://github.com/nventive/Chinook.DynamicMvvm/commits?author=jcantin-nventive" title="doc">📖</a></td>
  </tr>
</table>

<!-- markdownlint-enable -->
<!-- prettier-ignore-end -->
<!-- ALL-CONTRIBUTORS-LIST:END -->
<!-- ALL-CONTRIBUTORS-LIST:END -->
