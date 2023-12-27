# StoredValue
Simple key-value store for storing and retrieving values from memory or disk.
## Usage:
```csharp
var store = new StoredValue<string>("someString", "initial value", Location.LocalPersistent);
Console.WriteLine(store.Value);
store.Value = "Hello World";
Console.WriteLine(store.Value);
```
First run will return:
```
initial value
Hello World
```
Second and every subsequent run will return:
```
Hello World
Hello World
```
Please note that the value is stored in memory and will be lost when the application is closed. 
To persist the value to disk, use the `Location.LocalPersistent` location.

Also, if name is not unique, the value will be shared between all instances of the same name.

## Installation
Install via NuGet: https://www.nuget.org/packages/StoredValue/
## License
StoredValue is licensed under the MIT license.


Icon by: https://www.flaticon.com/authors/parzival-1997