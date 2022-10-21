# Chinook.DynamicMvvm
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg?style=flat-square)](LICENSE) ![Version](https://img.shields.io/nuget/v/Chinook.DynamicMvvm.Abstractions?style=flat-square) ![Downloads](https://img.shields.io/nuget/dt/Chinook.DynamicMvvm.Abstractions?style=flat-square)

The `Chinook.DynamicMvvm` packages assists in .Net MVVM (Model - View - ViewModel) development.

## Cornerstones

- **Highly Extensible**
  - Everything is interface-based to easily allow more implementations.
  - A single framework can't cover everything. Our architecture is designed in a way that allows you to integrate your favorites tools easily. 
- **Declarative Syntax**
  - We aim to understand the behavior of a property by glancing at its declaration.

### More like this
The Chinook namespace has other recipes for .Net MVVM applications.
- [Chinook.DataLoader](https://github.com/nventive/Chinook.DataLoader): Customizable async data loading recipes.
- [Chinook.Navigation](https://github.com/nventive/Chinook.Navigation): Navigators for ViewModel-first navigation.
- [Chinook.BackButtonManager](https://github.com/nventive/Chinook.BackButtonManager): An abstraction to deal with hardware back buttons.

## Getting Started

1. Add the `Chinook.DynamicMvvm` nuget package to your project.
1. Create your first ViewModel. Here's one that covers the basics.
   ```csharp
   using Chinook.DynamicMvvm;
   // (...)
   public class MainPageViewModel : ViewModelBase
   {
     public string Content
     {
       get => this.Get(initialValue: string.Empty);
       set => this.Set(value);
     }

     public IDynamicCommand Submit => this.GetCommand(() =>
     {
       Result = Content;
     });

     public string Result
     {
       get => this.Get(initialValue: string.Empty);
       private set => this.Set(value);
     }
   }
   ```
   > 💡 Want to go **fast**? We recommend installing the [Chinook Snippets](https://marketplace.visualstudio.com/items?itemName=nventivecorp.ChinookSnippets) Visual Studio Extension to benefit from code snippets. All our snippets start with `"ck"` (for "Chinook") and they will help you write those properties and commands extra fast.
1. Set this `MainPageViewModel` as the `DataContext` of your `MainPage` in `MainPage.xaml.cs`.
   ```csharp
   public MainPage()
   {
     this.InitializeComponent();
     DataContext = new MainPageViewModel();
   }
   ```
   Here is some xaml for that `MainPage.xaml` that demonstrates the basics.
   ```xml
   <Page x:Class="ChinookSample.MainPage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
     <StackPanel>
        <TextBox Text="{Binding Content, Mode=TwoWay}" />

        <Button Content="Submit"
                Command="{Binding Submit}" />

        <TextBlock Text="{Binding Result}" />
     </StackPanel>
   </Page>
   ```
1. Configure a `System.IServiceProvider` containing the following:
   - `IDynamicCommandBuilderFactory`
   - `IDynamicPropertyFactory`

   Here is a simple code sample that does that using `Microsoft.Extensions.DependencyInjection` and `Microsoft.Extensions.Hosting`.
   ```csharp
   var serviceProvider = new HostBuilder()
     .ConfigureServices(serviceCollection => serviceCollection
       .AddSingleton<IDynamicCommandBuilderFactory, DynamicCommandBuilderFactory>()
       .AddSingleton<IDynamicPropertyFactory, DynamicPropertyFactory>()
     )
     .Build()
     .Services;
   ```
1. Set the `IServiceProvider` into `ViewModelBase.DefaultServiceProvider` in the startup of your application.
   ```csharp
   ViewModelBase.DefaultServiceProvider = serviceProvider;
   ```
   > 💡 It's also possible to avoid using this public static provider and pass it via the constructor of `ViewModelBase`.
1. You're all set. You can start your app!

## Features
The previous setup is pretty basic. Let's see what else we can do!

### Dispatcher
Set an `IDispatcher` to allow setting properties from any thread.
The `IDispatcher` ensures the `INotifyPropertyChanged.PropertyChanged` event is raised on the main thread.
This is optional, but you'll likely need it.

For WinUI or Uno.WinUI apps, install the `Chinook.DynamicMvvm.Uno.WinUI` nuget package.
You can then use `DispatcherQueueDispatcher` or `BatchingDispatcherQueueDispatcher`.
```csharp
public MainPage()
{
    this.InitializeComponent();
    DataContext = new MainPageViewModel()
    {
        Dispatcher = new DispatcherQueueDispatcher(this)
    };
}
```

For UWP or Uno.UI apps, install the `Chinook.DynamicMvvm.Uno` nuget package.
You can then use `CoreDispatcherDispatcher` or `BatchingCoreDispatcherDispatcher`.
```csharp
public MainPage()
{
    this.InitializeComponent();
    DataContext = new MainPageViewModel()
    {
        Dispatcher = new CoreDispatcherDispatcher(this)
    };
}
```

### Create simple properties
Using `IViewModel.Get`, you can declare ViewModel properties that will raise the `INotifyPropertyChanged.PropertyChanged` event of the ViewModel when set.
Under the hood, an `IDynamicProperty` is lazy-initialized.

> 🔬 `IDynamicProperty` simply represents a property of a ViewModel.
> It has a name, a value, and an event to notify that the property's value changed.
> Having this interface is great because it allows the creation of custom implementations with various behaviors.
> You'll see that with the next sections of this document.
```csharp
public string Content
{
  get => this.Get(initialValue: string.Empty);
  set => this.Set(value);
}
```
> 💡 If you use [Chinook Snippets](https://marketplace.visualstudio.com/items?itemName=nventivecorp.ChinookSnippets), you can quickly generate a property from value using the snippets `"ckpropv"` (**c**hinoo**k** **prop**erty from **v**alue) or `"ckpropvg"` (**c**hinoo**k** **prop**erty from **v**alue **g**et-only).

> 🎓 We like to call "dynamic properties" the properties of a ViewModel that are backed with a `IDynamicProperty`.
> You can still used _regular properties_ in your ViewModels, but they will not raise the `PropertyChanged` event automatically when they change.
> ```csharp
> public string Title => "Hello"; // Regular property
> public string Subtitle { get; } // Regular property
> 
> public long Counter => this.GetFromObservable(ObserveTimer()); // Dynamic Property
> public bool IsFavorite // Dynamic property
> {
>   get => this.Get(initialValue: false);
>   set => this.Set(value);
> }
> ```
> You should prefer _regular properties_ over _dynamic properties_ for data that **never changes**, simply because dynamic properties allocate more memory.


### Create properties from `IObservable<T>`
If you're familiar with [System.Reactive](https://github.com/dotnet/reactive), you'll probably like this.
Using `IViewModel.GetFromObservable`, you can declare a ViewModel property from an `IObservable<T>`.
The property automatically updates itself when the observable pushes a new value.
```csharp
using System.Reactive.Linq;
// (...)
public long Counter => this.GetFromObservable(Observable.Timer(
  dueTime: TimeSpan.Zero,
  period: TimeSpan.FromSeconds(1))
);		
```
> 💡 If you use [Chinook Snippets](https://marketplace.visualstudio.com/items?itemName=nventivecorp.ChinookSnippets), you can quickly generate a property from observable using the snippets `"ckpropo"` (**c**hinoo**k** **prop**erty from **o**bservable) or `"ckpropog"` (**c**hinoo**k** **prop**erty from **o**bservable **g**et-only).

### Create properties from `Task<T>`
Using `IViewModel.GetFromTask`, you can create a property that updates itself based on a `Task<T>` result.
```csharp
// This property is initialized with the value 10, but changes to 100 after 1 second.
public int Number => this.GetFromTask(async ct =>
{
  await Task.Delay(1000, ct);
  return 100;
}, initialValue: 10);
```
> 💡 If you use [Chinook Snippets](https://marketplace.visualstudio.com/items?itemName=nventivecorp.ChinookSnippets), you can quickly generate a property from task using the snippets `"ckpropt"` (**c**hinoo**k** **prop**erty from **t**ask) or `"ckproptg"` (**c**hinoo**k** **prop**erty from **t**ask **g**et-only).

### Decide whether you want a property setter
This could seem obvious, but any _`IDynamicProperty`-backed_ property can be _readonly_ by simply omitting the property setter.

```csharp
public long Counter1 => this.GetFromObservable(ObserveTimer());

public long Counter2
{
  get => this.GetFromObservable(ObserveTimer());
  set => this.Set(value);
}

public long Counter3
{
  get => this.GetFromObservable(ObserveTimer());
  private set => this.Set(value);
}

private IObservable<long> ObserveTimer() => Observable.Timer(
  dueTime: TimeSpan.Zero,
  period: TimeSpan.FromSeconds(1));

private void SomeMethod()
{
  Counter1 = 0; // Doesn't build
  Counter2 = 0; // Builds
  Counter3 = 0; // Builds
}
```
In this code, all properties are updated from an observable. However, 
- `Counter1` can't be set manually.
- `Counter2` can be set manually from anywhere (including `TwoWay` bindings).
- `Counter3` can be set manually only from the ViewModel (which excludes `TwoWay` bindings).
> 💡 If you use [Chinook Snippets](https://marketplace.visualstudio.com/items?itemName=nventivecorp.ChinookSnippets), you can quickly generate a get-only property using the `"ckprop"` snippets ending with `"g"` (for **g**et-only).

### Access the underlying `IDynamicProperty` instance
You can access the backing `IDynamicProperty<T>` instance of any property from any ViewModel by using `IViewModel.GetProperty()`.
```csharp
public MainPageViewModel()
{
  IDynamicProperty<string> contentProperty;
  contentProperty = this.GetProperty(vm => vm.Content);
  // or
  contentProperty = this.GetProperty<string>(nameof(Content));
}

public string Content
{
  get => this.Get(initialValue: string.Empty);
  set => this.Set(value);
}
```
You can then interact with the property object itself.
```csharp
contentProperty.Value = "Hello";
// This sets the property value. It also raises the PropertyChanged event on the ViewModel.

contentProperty.ValueChanged += prop => Console.WriteLine($"Property {prop.Name} changed to {prop.Value}.");
// This allows you to easily observe changes to a property value. Note that prop.Value is strongly type as a string here. 
```
> 💡 With the `IDynamicProperty` instance, even a _get-only_ property can be set manually.
> Use this knowledge responsively.

#### Observe a property value using `IObservable`
If you are used to [System.Reactive](https://github.com/dotnet/reactive), you can install the `Chinook.DynamicMvvm.Reactive` package to benefit from some more extensions methods.
```csharp
IObservable<string> observable;
observable = contentProperty.Observe(); // Gets an IObservable that yields when the value changes.
observable = contentProperty.GetAndObserve(); // Gets an IObservable that yields when the value changes and starts with the current value.
```

### Create commands from `Action` or `Action<T>`
Using `IViewModel.GetCommand`, you can create a command using an `Action`, or `Action<T>` when you want a command parameter.
```csharp
public IDynamicCommand SayHi => this.GetCommand(() =>
{
  Console.WriteLine("Hi");
});

public IDynamicCommand SaySomething => this.GetCommand<string>(parameter =>
{
  Console.WriteLine(parameter);
});
```
> 💡 If you use [Chinook Snippets](https://marketplace.visualstudio.com/items?itemName=nventivecorp.ChinookSnippets), you can quickly generate a command from `Action` using the snippets `"ckcmda"` (**c**hinoo**k** **c**o**m**man**d** from **a**ction) or `"ckcmdap"` (**c**hinoo**k** **c**o**m**man**d**  from **a**ction with **p**arameter).

### Create commands from an async method
Using `IViewModel.GetCommand`, you can create a `async` command using a `Func<Task>`, or `Func<T, Task>` when you want a command parameter.
```csharp
public IDynamicCommand WaitASecond => this.GetCommandFromTask(async ct =>
{
  await Task.Delay(1000, ct);
});

public IDynamicCommand Wait => this.GetCommandFromTask<int>(async (ct, parameter) =>
{
  await Task.Delay(TimeSpan.FromSeconds(parameter), ct);
});
```
> 💡 If you use [Chinook Snippets](https://marketplace.visualstudio.com/items?itemName=nventivecorp.ChinookSnippets), you can quickly generate a command from `Task` using the snippets `"ckcmdt"` (**c**hinoo**k** **c**o**m**man**d** from **t**ask) or `"ckcmdtp"` (**c**hinoo**k** **c**o**m**man**d**  from **t**ask with **p**arameter).


The provided `CancellationToken ct` is cancelled when the `IDynamicCommand` is disposed.
The command itself is disposed when the ViewModel is disposed.
You decide when the ViewModel gets disposed.
> 💡 Check out [Chinook.Navigation](https://github.com/nventive/Chinook.Navigation) if you want a ViewModel-based navigation system that automatically deals with disposing ViewModels.

### Customize command behavior
The `IDynamicCommand` declarations come with an optional builder.
You can use this builder to customize the behavior of any command.
> 🔬The command implementation is done using a strategy pattern (very similarly to the [HTTP Message Handlers](https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/http-message-handlers)).
> The builder simply accumulates strategies and chains them together when the command is built.

You can create a base configuration for all your commands at the factory level.
```csharp
.AddSingleton<IDynamicCommandBuilderFactory>(s =>
    new DynamicCommandBuilderFactory(builder => builder
        .WithLogs(s.GetRequiredService<ILogger<IDynamicCommand>>())
        .OnBackgroundThread()
))
```
You can add more configuration at the command declaration level.
The builders are additive, meaning that the configuration at the command declaration level is applied after the one at the factory level.
```csharp
public IDynamicCommand Submit => this.GetCommand(() =>
{
  Result = Content;
}, builder => builder
  .SkipWhileExecuting()
);
```
> 🔬 For the previous code the strategy chain looks like this:
> ```
> - LoggerCommandStrategy (factory level)
>  - BackgroundCommandStrategy (factory level)
>   - SkipWhileExecutingCommandStrategy (command declaration level)
>    - ActionCommandStrategy, which actually executes the method (command declaration level)
> ```
> The strategy execution starts from the top and goes down the chain and then comes back up the chain for any subsequent processing.
> This means that strategies allow adding behavior both before and after the actual command execution.

#### Supported strategies
- [BackgroundCommandStrategy](src/DynamicMvvm/Command/Strategies/BackgroundCommandStrategy.cs) : Executes the command on a background thread.
- [CanExecuteCommandStrategy](src/DynamicMvvm/Command/Strategies/CanExecuteCommandStrategy.cs) : Attaches the `CanExecute` to the value of a `IDynamicProperty`.
- [ErrorHandlerCommandStrategy](src/DynamicMvvm/Command/Strategies/ErrorHandlerCommandStrategy.cs) : Catches any exception during the execution and delegates it to an error handler.
- [LogCommandStrategy](src/DynamicMvvm/Command/Strategies/DynamicCommandWithLogger.cs) : Adds logs to the command execution.
- [LockCommandStrategy](src/DynamicMvvm/Command/Strategies/LockCommandStrategy.cs) : Locks the command execution.
- [CancelPreviousCommandStrategy](src/DynamicMvvm/Command/Strategies/CancelPreviousCommandStrategy.cs) : Cancels the previous command execution when executing the command.
- [SkipWhileExecutingCommandStrategy](src/DynamicMvvm/Command/Strategies/SkipWhileExecutingCommandStrategy.cs) : Skips executions if the command is already executing.
- [DisableWhileExecutingCommandStrategy](src/DynamicMvvm/Command/Strategies/DisableWhileExecutingCommandStrategy.cs) : Disables the command when it's executing.
- [RaiseCanExecuteOnDispatcherCommandStrategy](src/DynamicMvvm/Command/Strategies/RaiseCanExecuteOnDispatcherCommandStrategy.cs) : Raises the `CanExecuteChanged` on the ViewModel's `IDispatcher`.

### Observe whether a command is executing
`IDynamicCommand` adds an `IsExecuting` property and an `IsExecutingChanged` event to the classic `System.Windows.Input.ICommand`.
`IDynamicCommand` and also implements `INotifyPropertyChanged`, meaning that you can do a XAML binding on `IsExecuting`.
> 💡 This can be usefull if you want to add a loading indicator in your button's `ControlTemplate`.

### Add child ViewModels
Using `IViewModel.GetChild`, you can declare an _inner ViewModel_ in an existing ViewModel.
This is great to extract repeating code or simply to separate concerns.

```csharp
public SettingsViewModel Settings => this.GetChild<SettingsViewModel>();
```
> ⚠ When creating child ViewModels, it's important to use the `GetChild`, `AttachChild`, or `AttachOrReplaceChild` methods to ensure linking the `IDispatcher` of the child to its parent.
>
> Consider the following code.
> ```csharp
> public SettingsViewModel Settings { get; } = new SettingsViewModel();
> ```
> This might seem to work at first, but know that the `IDispatcher` of  `Settings` is not set.
> Therefore, `PropertyChanged` events might not be raised on the correct thread, which could result in errors.

#### ItemViewModels
Child ViewModels are also very useful when using what we like to call _ItemViewModels_, meaning an item from a list.
This recipe is quite powerful when you want to change a property on a list item without updating the whole list itself.

Here's an example where `ItemViewModel.IsFavorite` can be manipulated directly and any XAML binding to it will update as expected.
```csharp
public class ItemViewModel : ViewModelBase
{
  public string Title { get; init; }
	
  public bool IsFavorite
  {
    get => this.Get(initialValue: false);
    set => this.Set(value);
  }
}

// (...)

public MainPageViewModel()
{
  IEnumerable<string> someSource = Enumerable
    .Range(0, 10)
    .Select(i => i.ToString());
  Items = someSource
    .Select(title => this.AttachChild(new ItemViewModel { Title = title }, name: title))
    .ToArray();
}

public ItemViewModel[] Items { get; }
```
> 💡 ItemViewModels are great when individual list items change overtime.
> However, when your list items don't update themselves, you should probably avoid creating ItemViewModels.

### Resolve services from a ViewModel
Using `IViewModel.GetService`, you can easily get a service from the service provider that you set.
Note that the `IServiceProvider` is also directly exposed via `IViewModel.ServiceProvider`.
```csharp
var logger = this.GetService<ILogger<MainPageViewModel>>();
```

### Add disposables to a ViewModel
Using `IViewModel.AddDisposable`, you can add any `IDisposable` object to a `IViewModel`.
When the ViewModel is disposed, all added disposables are disposed as well.
You can also get or remove previously added disposables using `IViewModel.TryGetDisposable` and `IViewModel.RemoveDisposable`.

> 💡 Adding disposables can be useful when subscribing to observables or events.
> You can easily setup the unsubscription to happen when the ViewModel is disposed.
>
> Check out [Chinook.Navigation](https://github.com/nventive/Chinook.Navigation) if you want a ViewModel-based navigation system that automatically deals with disposing ViewModels.

> 🔬 `IViewModel` can be seen as a dictionary of `IDisposable` objects.
> `IDynamicProperty` and `IDynamicCommand` both implement `IDisposable`, so that's how they're actually stored.
> It's the same thing for child ViewModels.
> This architecture contributes to the great extensibility of this library.
> You can see [Chinook.DataLoader](https://github.com/nventive/Chinook.DataLoader) as a demonstration of extensibility.

### Add errors
`IViewModel` implements `INotifyDataErrorInfo`.
You can use `IViewModel.SetErrors` and `IViewModel.ClearErrors` to manipulate the error info.

### Add validation using [FluentValidations](https://fluentvalidation.net/)
You can install `Chinook.DynamicMvvm.FluentValidation` package to gain access to helpful extension methods.
The first step to add validation is to declare a validator on your ViewModel.
```csharp
public class MainPageValidator : AbstractValidator<MainPageViewModel>
{
  public MainPageValidator()
  {
    RuleFor(vm => vm.Content).NotEmpty();
  }
}
```
You can add all validators of your app to the service provider by using this line in your configuration.
```csharp
serviceCollection.AddValidatorsFromAssemblyContaining(typeof(App), ServiceLifetime.Singleton)
```
For any dynamic property, you can use `IViewModel.AddValidation` to automatically run the validation rules when a property's value changes.
```csharp
public MainPageViewModel()
{
  this.AddValidation(this.GetProperty(vm => vm.Content));
}
```
You can also run the validation manually.
```csharp
public IDynamicCommand Submit => this.GetCommandFromTask(async ct =>
{
  var result = await this.Validate(ct);
  if (result.IsValid)
  {
    Result = Content;
  }
});
```
<!-- TODO: Add doc about CollectionTracking -->

## Breaking Changes

Please consult [BREAKING_CHANGES.md](BREAKING_CHANGES.md) for more information about breaking changes and version history.

## License

This project is licensed under the Apache 2.0 license - see the
[LICENSE](LICENSE) file for details.

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on the process for
contributing to this project.

Be mindful of our [Code of Conduct](CODE_OF_CONDUCT.md).
