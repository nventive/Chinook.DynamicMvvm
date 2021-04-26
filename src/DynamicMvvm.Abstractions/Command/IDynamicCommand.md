# IDynamicCommand

A `IDynamicCommand` is a `ICommand` that will notify its subscribers when it is executing.

## Getting started

To create a `IDynamicCommand`, you use its constructor.

[Refer to the different implementations below for other ways to create a IDynamicCommand](#implementations).

```csharp
// This will create a DynamicCommand with a name of "MyCommand".
var myCommandStrategy = new ActionCommandStrategy(ExecuteCommand);
var myCommand = new DynamicCommand("MyCommand", myCommandStrategy);

// This method will be called when MyCommand is being executed.
void ExecuteCommand()
{
  // Command execution logic here.
}
```

To execute a `IDynamicCommand`, you use its `Execute` method.

```csharp
// This will execute the command and call the ExecuteCommand method.
await myCommand.Execute();
```

To be notified when the `IDynamicCommand` is executing, you subscribe to its `IsExecutingChanged` event.

```csharp
// This method will be called when the command is executing.
void OnIsExecutingChanged(object sender, EventArgs e)
{

}

myCommand.IsExecutingChanged += OnIsExecutingChanged;
```

## Features

### Implementations

There are multiple ways to create a `IDynamicCommand` using various implementations of `IDynamicCommandStrategy`.

You can create it from an action.

```csharp
// This will create a DynamicCommand with a name of "MyCommand".
// The ExecuteCommand method will be called when MyCommand is being executed.
var myCommandStrategy = new ActionCommandStrategy(ExecuteCommand);
var myCommand = new DynamicCommand("MyCommand", myCommandStrategy);
void ExecuteCommand() { }
```

You can create it from an action with a parameter.

```csharp
// This will create a DynamicCommand with a name of "MyCommandWithParameter".
// The ExecuteCommand method will be called when MyCommandWithParameter is being executed.
var myCommandStrategy = new ActionCommandStrategy<int>(ExecuteCommand);
var myCommand = new DynamicCommand("MyCommandWithParameter", myCommandStrategy);
void ExecuteCommand(int parameter) { }
```

You can create it from a task.

_Note: The `CancellationToken` will be cancelled if the `IDynamicCommand` is disposed while the command is executing._

```csharp
// This will create a DynamicCommand with a name of "MyCommandFromTask".
// The ExecuteCommand method will be called when MyCommandFromTask is being executed.
var myCommandStrategy = new TaskCommandStrategy(ExecuteCommand);
var myCommand = new DynamicCommand("MyCommandFromTask", myCommandStrategy);
Task ExecuteCommand(CancellationToken ct) => Task.CompletedTask;
```

You can create it from a task with a parameter.

_Note: The `CancellationToken` will be cancelled if the `IDynamicCommand` is disposed while the command is executing._

```csharp
// This will create a DynamicCommand with a name of "MyCommandFromTaskWithParameter".
// The ExecuteCommand method will be called when MyCommandFromTaskWithParameter is being executed.
var myCommandStrategy = new TaskCommandStrategy<int>(ExecuteCommand);
var myCommand = new DynamicCommand("MyCommandFromTaskWithParameter", myCommandStrategy);
Task ExecuteCommand(CancellationToken ct, int parameter) => Task.CompletedTask;
```

### Strategy Decorators

`IDynamicCommandStrategy` can be decorated in order to add new logic to it.

Here is a list of some decorators provided.

- [BackgroundCommandStrategy](Implementations/Strategies/BackgroundCommandStrategy.cs) : Executes the command on a background thread.
- [CanExecuteCommandStrategy](Implementations/Strategies/CanExecuteCommandStrategy.cs) : Attaches the `CanExecute` to the value of a `IDynamicProperty`.
- [ErrorHandlerCommandStrategy](Implementations/Strategies/ErrorHandlerCommandStrategy.cs) : Catches any exception during the execution and delegates it to an error handler.
- [LogCommandStrategy](Implementations/Strategies/DynamicCommandWithLogger.cs) : Adds logs to the command execution.
- [LockCommandStrategy](Implementations/Strategies/LockCommandStrategy.cs) : Locks the command execution.
- [CancelPreviousCommandStrategy](Implementations/Strategies/CancelPreviousCommandStrategy.cs) : Cancels the previous command execution when executing the command.
- [SkipWhileExecutingCommandStrategy](Implementations/Strategies/SkipWhileExecutingCommandStrategy.cs) : Skips executions if the command is already executing.
- [DisableWhileExecutingCommandStrategy](Implementations/Strategies/DisableWhileExecutingCommandStrategy.cs) : Disables the command when it's executing.

For example, you would add logs to a `IDynamicCommandStrategy` using the `WithLogs` extension.

```csharp
var myCommandStrategy = new ActionCommandStrategy(ExecuteCommand);

// This will decorate the command with logs
var myStrategyWithLogs = myCommandStrategy.WithLogs(logger);

var myCommand = new DynamicCommand("MyCommand", myStrategyWithLogs);
void ExecuteCommand() { }
```

You can create your own decorators by inheriting from `DecoratorCommandStrategy`.

```csharp
public class MyCustomCommandDecorator : DecoratorCommandStrategy
{
  public MyCustomCommandDecorator(IDynamicCommandStrategy innerStrategy) : base(innerStrategy)
  { }

  public override bool CanExecute(object parameter, IDynamicCommand command)
  {
    // Implement your custom decorator logic here.
    return base.CanExecute(parameter, command);
  }

  public override async Task Execute(CancellationToken ct, object parameter, IDynamicCommand command)
  {
    // Implement your custom decorator logic here.
    base.Execute(ct, parameter, command);
  }
}

public static class MyCustomCommandDecoratorExtensions
{
  public static IDynamicCommandStrategy WithMyCustomDecorator(this IDynamicCommandStrategy strategy)
    => new MyCustomCommandDecorator(strategy);
}

// This will decorate the strategy with your custom decorator
var myCommandWithMyCustomDecorator = myCommandStrategy.WithMyCustomDecorator();
```

### Command builder and factory

Instead of creating a `IDynamicCommand` using its constructor, you can use `IDynamicCommandBuilder` and `IDynamicCommandBuilderFactory`.

To create a `IDynamicCommand` using `IDynamicCommandBuilderFactory`, you use the factory methods followed by the `Build` method.

```csharp
// This will create a new IDynamicCommand using a IDynamicCommandBuilderFactory and IDynamicCommandBuilder.
var myFactory = new DynamicCommandBuilderFactory();
var myBuilder = myFactory.CreateFromAction("MyCommand", ExecuteCommand);
var myCommand = myBuilder.Build();
private void ExecuteCommand() { }
```

You can configure your commands locally using the various extension methods on `IDynamicCommandBuilder`.

```csharp
// This will create a new IDynamicCommand using a IDynamicCommandBuilderFactory and IDynamicCommandBuilder.
var myFactory = new DynamicCommandBuilderFactory();
var myBuilder = myFactory.CreateFromAction("MyCommand", ExecuteCommand)
  .WithLogs();
var myCommand = myBuilder.Build();
private void ExecuteCommand() { }
```

You can configure all commands created from the `IDynamicCommandBuilderFactory` by using a **default configuration**.

```csharp
// This will create a new IDynamicCommand using a IDynamicCommandBuilderFactory and IDynamicCommandBuilder.
var myFactory = new DynamicCommandBuilderFactory(c => c
  // This will run your commands on a background thread.
  .ExecuteOnBackgroundThread()

  // This will catch any errors unhandled by your commands.
  .CatchErrors(HandleCommandError)

  // This will add logs your commands.
  .WithLogs()
));

var myBuilder = myFactory.CreateFromAction("MyCommand", ExecuteCommand);
var myCommand = myBuilder.Build();

private void ExecuteCommand() { }
```

_Note: The **default** configuration wraps the **local** configuration which wraps the **implementation**. The hierarchy could look like the following._

```
- MyCommand
  - BackgroundCommandStrategy (default config)
    - ErrorHandlerCommandStrategy (default config)
      - LogCommandStrategy (default config)
        - CanExecuteStrategy (local config)
          - ... (other local strategies)
            - ActionCommandStrategy (implementation)
```

You can override the default configuration by manipulating the builder.

```csharp
// This will create a new IDynamicCommand using a IDynamicCommandBuilderFactory and IDynamicCommandBuilder.
var myFactory = new DynamicCommandBuilderFactory(c => c
  .ExecuteOnBackgroundThread()
  .CatchErrors(HandleCommandError)
  .WithLogs()
));

var myBuilder = myFactory.CreateFromAction("MyCommand", ExecuteCommand)
  // You can use WithoutStrategy to remove an existing strategy.
  .WithoutStrategy<BackgroundCommandStrategy>();

var myCommand = myBuilder.Build();

var myBuilder2 = myFactory.CreateFromAction("MyCommand2", ExecuteCommand)
  // You can use ClearStrategies to completely clear the default configuration to add your own.
  .ClearStrategies()
  .DisableWhileExecuting();

var myCommand2 = myBuilder2.Build();

private void ExecuteCommand() { }
```

### Code Snippets

You can install the Visual Studio Extension [Chinook Snippets](https://marketplace.visualstudio.com/items?itemName=nventivecorp.ChinookSnippets) and use code snippets to quickly generate dynamic commands.
All snippets related to `IDynamicCommand` start with `ckcmd`.
