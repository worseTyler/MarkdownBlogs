
In VS.NET 2005 Beta 1 it was possible to define a variable of type

> Nullable<Nullable<Nullable<int>>> number;

and, therefore,

> int??? number;

Nullable<T> only allows type parameters that are value types through the use of the struct constraint as follows:

> public struct Nullable<T> : IFormattable, IComparable, IComparable<Nullable<T>>, INullable **where T : struct** { // ... }

In Beta 1 this still allowed a T type parameter of Nullable<T>, however, because (as is shown above) Nullable<T> was a struct.

In Beta 2, the meaning of the struct constraint has improved to avoid this.  Now the struct constraint only allows value types except System.Nullable<T>.  For example, the following code is prohibited:

> class AClass<T> where T : struct { public void Method() { // ERROR: The type 'int?' must be a non-nullable // value type in order to use it as parameter // 'T' in the generic type or method AClass<int?> trial; } }

Thus avoiding the problem described earlier.
