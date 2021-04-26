# IDynamicProperty

`IDynamicProperty` represents a property that will notify its subscribers when its value changes.

It **always has a value** that can be accessed synchronously.

## Getting started

To create a `IDynamicProperty`, you use its constructor.

[Refer to the different implementations below for other ways to create a IDynamicProperty](#implementations).

```csharp
// This will create a DynamicProperty with a name of "MyProperty" and a value of 10.
var myProperty = new DynamicProperty<int>("MyProperty", value: 10);
```

To synchronously get or set the value of a `IDynamicProperty`, you use its `Value` property.

```csharp
// This will return the last value of the DynamicProperty.
var myValue = myProperty.Value;

// This will set the value of the DynamicProperty to 30.
// The property will notify its subscribers of this change.
myProperty.Value = 30;
```

To be notified when the value of a `IDynamicProperty` changes, you need to subscribe to its `ValueChanged` event.

```csharp
// This method will be called everytime the value of the property changes.
void OnValueChanged(IDynamicProperty property)
{
  var newValue = property.Value;
  
  // ...
}

myProperty.ValueChanged += OnValueChanged;
```

## Features

### Implementations

There are multiple ways to create a `IDynamicProperty`.

You can create a property from a **value**.

```csharp
// This will create a DynamicProperty with a name of "MyProperty" and a value of 10.
var myProperty = new DynamicProperty<int>("MyProperty", value: 10);
```

You can create a property from an **observable**.

```csharp
// This will create a DynamicProperty with a name of "MyPropertyFromObservable" and an initial value of 10.
// When the observable pushes the value of 20, the property will be updated and will notify its subscribers of this change.
var myObservable = Observable.Return(20);
var myPropertyFromObservable = new DynamicPropertyFromObservable<int>("MyPropertyFromObservable", myObservable, initialValue: 10);
```

You can create a property from a **task**.

```csharp
// This will create a DynamicProperty with a name of "MyPropertyFromTask" and an initial value of 10.
// When the task completes, the property will be updated with its result and will notify its subscribers of this change.
Task<int> MyTask(CancellationToken ct) => Task.FromResult(20);
var myPropertyFromTask = new DynamicPropertyFromTask<int>("MyPropertyFromTask", MyTask, initialValue: 10);
```

### Extensions

You can observe the value of a `IDynamicProperty` using the `Observe` extension.

```csharp
myProperty.Observe().Subscribe(value =>
{
  // This will be called everytime the value of the property changes.
});
```

You can get and observe the value of a `IDynamicProperty` using the `GetAndObserve` extension.

```csharp
myProperty.GetAndObserve().Subscribe(value =>
{
  // This will be called a first time with the initial value of the property.
  // It will then be called everytime the value of the property changes.
});
```

### Code Snippets

You can install the Visual Studio Extension [Chinook Snippets](https://marketplace.visualstudio.com/items?itemName=nventivecorp.ChinookSnippets) and use code snippets to quickly generate dynamic properties.
All snippets related to `IDynamicProperty` start with `ckprop`.
