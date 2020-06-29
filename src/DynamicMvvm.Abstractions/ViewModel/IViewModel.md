# IViewModel

`IViewModel` is a store of `IDisposable` that can be bound to a view.

When `IViewModel` is disposed, all the `IDisposable` attached to it will be disposed.

It is meant to store [DynamicProperties](../Property/IDynamicProperty.md), [DynamicCommands](../Command/IDynamicCommand.md), children ViewModels, etc.

## Getting started

In most cases, you will inherit from `ViewModelBase`.

```csharp
public class MyViewModel : ViewModelBase
{
}
```

You can attach `DynamicProperties` to your `IViewModel`.

```csharp
public class MyViewModel : ViewModelBase
{
  // This will create and attach a new IDynamicProperty named "MyOneWayProperty"
  // to your IViewModel. It's initial value will be default(int).
  // Since it only provides a getter, this property can only be used in one-way bindings.
  public int MyOneWayProperty => this.Get<int>();

  // This will create and attach a new IDynamicProperty named "MyTwoWayProperty"
  // to your IViewModel. It's initial value will be default(int).
  // Since it provides a getter and a setter, this property can be used in two-way bindings.
  public int MyTwoWayProperty
  {
    get => this.Get<int>();
    set => this.Set(value);
  }

  public MyViewModel()
  {
    // This will get the value of the IDynamicProperty named MyOneWayProperty
    var myValue = MyOneWayProperty;

    // This will set the value of the IDynamicProperty named MyTwoWayProperty
    MyTwoWayProperty = 20;
  }
}
```

You can attach `DynamicCommands` to your `IViewModel`.

```csharp
public class MyViewModel : ViewModelBase
{
  // This will create and attach a new IDynamicCommand named "MyCommand"
  // to your IViewModel. It will call the Execute method when executed.
  public IDynamicCommand MyCommand => this.GetCommand(Execute);

  private void Execute()
  {
    // This method will be called when MyCommand is executed.
  }
}
```

You can attach children ViewModels to your `IViewModel`.

```csharp
public class MyViewModel : ViewModelBase
{
  // This will create and attach a new child IViewModel named "MyChild"
  // to your IViewModel. The child will share its lifecycle with its parent ViewModel.
  public IViewModel MyChild => this.GetChild(() => new MyChildViewModel());
}

public class MyChildViewModel : ViewModelBase
{
}
```

You can resolve services from your `IViewModel`.

```csharp
public class MyViewModel : ViewModelBase
{
  // This will return the registered service of type IMyService.
  private IMyService _myService => this.ServiceProvider.GetRequiredService<IMyService>();
}
```

## Features

### Dynamic properties

There are multiple ways to create a `IDynamicProperty` from a `IViewModel`.

[Refer to this documentation for more information on DynamicProperties](../Property/IDynamicProperty.md).

```csharp
public class MyViewModel : ViewModelBase
{
  // This will create and attach a new IDynamicProperty named "MyProperty"
  // to your IViewModel. It's initial value will be default(int).
  public int MyProperty => this.Get<int>();

  // This will create and attach a new IDynamicProperty named "MyPropertyWithInitialValue"
  // to your IViewModel. It's initial value will be 10.
  public int MyPropertyWithInitialValue => this.Get(10));

  // This will create and attach a new IDynamicProperty named "MyPropertyFromTask"
  // to your IViewModel. It's initial value will be 10 and will be updated to 20
  // once MyTask completes.
  public int MyPropertyFromTask => this.GetFromTask<int>(MyTask, initialValue: 10);
  private Task<int> MyTask(CancellationToken ct) => Task.FromResult(20);

  // This will create and attach a new IDynamicProperty named "MyPropertyFromObservable"
  // to your IViewModel. It's initial value will be 10 and will be updated to 20
  // once MyObservable pushes a new value.
  public int MyPropertyFromObservable => this.GetFromObservable(MyObservable, initialValue: 10);
  private IObservable<int> MyObservable => Observable.Return(20);
}
```

You can change the creation of the `IDynamicProperty` by registering a `IDynamicPropertyFactory` in your `IServiceProvider`.

```csharp
private void ConfigureProperties(IServiceCollection services)
{
  services.AddSingleton<IDynamicPropertyFactory, DynamicPropertyFactory>();
}
```

### Reactive commands

There are multiple ways to create a `IDynamicCommand` from a `IViewModel`.

When disposed, the `IViewModel` will dispose all commands attached to it causing their cancellation.

[Refer to this documention for more information on IDynamicCommand](../Command/IDynamicCommand.md).

```csharp
public class MyViewModel : ViewModelBase
{
  // This will create and attach a new IDynamicCommand named "MyCommand"
  // to your IViewModel. It will call the Execute method when executed.
  public IDynamicCommand MyCommand => this.GetCommand(Execute);
  private void Execute() { }

  // This will create and attach a new IDynamicCommand named "MyCommandWithParameter"
  // to your IViewModel. It will call the ExecuteWithParameter method with its
  // command parameter when executed.
  public IDynamicCommand MyCommandWithParameter => this.GetCommand<int>(ExecuteWithParameter);
  private void ExecuteWithParameter(int parameter) { }

  // This will create and attach a new IDynamicCommand named "MyCommandFromTask"
  // to your IViewModel. It will call the ExecuteMyTask method when executed.
  // The CancellationToken will be cancelled if the IViewModel is disposed.
  public IDynamicCommand MyCommandFromTask => this.GetCommandFromTask(ExecuteMyTask);
  private async Task ExecuteMyTask(CancellationToken ct) => Task.CompletedTask;

  // This will create and attach a new IDynamicCommand named "MyCommandFromTaskWithParameter"
  // to your IViewModel. It will call the ExecuteMyTaskWithParameter method with its
  // command parameter when executed. The CancellationToken will be cancelled if the IViewModel is disposed.
  public IDynamicCommand MyCommandFromTaskWithParameter => this.GetCommandFromTask<int>(ExecuteMyTaskWithParameter);
  private async Task ExecuteMyTaskWithParameter(CancellationToken ct, int parameter) => Task.CompletedTask;
}
```

You can decorate a `IDynamicCommand` from its definition.

[Refer to this documention for more information on IDynamicCommand decorators](../Command/IDynamicCommand.md#decorators).

```csharp
public class MyViewModel : ViewModelBase
{
  // This will add logs to your command.
  public IDynamicCommand MyCommand => this.GetCommand(Execute, c => c.WithLogs());
  private void Execute() { }
}
```

You can change the creation of the `IDynamicCommand` by registering a `IDynamicCommandFactory` in your `IServiceProvider`.

```csharp
private void ConfigureCommands(IServiceCollection services)
{
  services.AddSingleton<IDynamicCommandFactory, DynamicCommandFactory>();
}
```

### Children view models

You can attach children ViewModels to your `IViewModel`.

This is useful if you want to share a set of functionalities between multiple ViewModels.

```csharp
public class MyViewModel : ViewModelBase
{
  // This will create and attach a new child IViewModel named "MyChild"
  // to your IViewModel. The child will share its lifecycle with its parent ViewModel.
  public IViewModel MyChild => this.GetChild(() => new MyChildViewModel());

  // This will do the same as above but is a shortcut if your
  // IViewModel doesn't require any parameter.
  public IViewModel MyChild2 => this.GetChild<MyChildViewModel>();
}

public class MyChildViewModel : ViewModelBase
{
}
```

### Dependency injection

You can resolve services from your `IViewModel` using the `IServiceProvider`.

```csharp
public class MyViewModel : ViewModelBase
{
  // This will return the registered service of type IMyService.
  private IMyService _myService => this.ServiceProvider.GetRequiredService<IMyService>();
}
```

You can also resolve services using the `InjectAttribute` from [Uno.Injectable](https://github.com/unoplatform/Uno.CodeGen/blob/master/doc/Injectable%20Generation.md).

_Note: Don't forget to make your class **partial**._

```csharp
public partial class MyViewModel : ViewModelBase
{
  // This will return the registered service of type IMyService.
  [Inject] private IMyService _myService;
}
```

### Disposable

You can register a `IDisposable` to your `IViewModel` if you want it to be disposed when the `IViewModel` is disposed.

```csharp
public partial class MyViewModel : ViewModelBase
{
  public MyViewModel(IDisposable myDisposable)
  {
	// The disposable will be disposed when this ViewModel is disposed.
    this.AddDisposable("MyDisposable", myDisposable);

    // You can get the disposable using its key.
	this.TryGetDisposable("MyDisposable", out var disposable);
  }
}
```
