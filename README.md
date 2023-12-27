# StoredValue
Simple key-value store for storing and retrieving values from memory or disk.
Usage:
```csharp
var store = new StoredValue<string>();
store.Value = "Hello World";
Console.WriteLine(store.Value);
```
