# StoredValue
Simple key-value store for storing and retrieving values from memory or disk.
Usage:
```csharp
var store = new StoredValue<string>("someString", "initial value", Location.LocalPersistent);
store.Value = "Hello World";
Console.WriteLine(store.Value);

```

Icon by: https://www.flaticon.com/authors/parzival-1997