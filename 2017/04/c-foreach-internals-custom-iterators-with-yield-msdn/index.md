

This month I’m going to explore the internals of a core construct of C# that we all program with frequently—the foreach statement. Given an understanding of the foreach internal behavior, you can then explore implementing the foreach collection interfaces using the yield statement, as I’ll explain.

Although the foreach statement is easy to code, I’m surprised at how few developers understand how it works internally. For exam­ple, are you aware that foreach works differently for arrays than on IEnumberable<T> collections? How familiar are you with the relationship between IEnumerable<T> and IEnumerator<T>? And, if you do understand the enumerable interfaces, are you comfortable implementing them using yield?

### What Makes a Class a Collection

By definition, a collection within the Microsoft .NET Framework is a class that, at a minimum, implements IEnumerable<T> (or the nongeneric type IEnumerable). This interface is critical because implementing the methods of IEnumerable<T> is the minimum needed to support iterating over a collection.

The foreach statement syntax is simple and avoids the complication of having to know how many elements there are. The runtime doesn’t directly support the foreach statement, however. Instead, the C# compiler transforms the code as described in the next sections.

foreach with Arrays: The following demonstrates a simple foreach loop iterating over an array of integers and then printing out each integer to the console:

```
int\[\] array = new int\[\]{1, 2, 3, 4, 5, 6};
foreach (int item in array)
{
  Console.WriteLine(item);
}
```

From this code, the C# compiler creates a CIL equivalent of the for loop:

```
int\[\] tempArray;
int\[\] array = new int\[\]{1, 2, 3, 4, 5, 6};
tempArray = array;
for (int counter = 0; (counter < tempArray.Length); counter++)
{
  int item = tempArray\[counter\];
  Console.WriteLine(item);
}
```

In this example, note that foreach relies on the support for the Length property and the index operator (\[\]). With the Length property, the C# compiler can use the for statement to iterate through each element in the array.

foreach with IEnumerable<T>: Although the preceding code works well on arrays where the length is fixed and the index operator is always supported, not all types of collections have a known number of elements. Furthermore, many of the collection classes, including Stack<T>, Queue<T> and Dictionary<TKey and TValue>, don’t support retrieving elements by index. Therefore, a more general approach of iterating over collections of elements is needed. The iterator pattern provides this capability. Assuming you can determine the first, next, and last elements, knowing the count and supporting retrieval of elements by index is unnecessary.

The System.Collections.Generic.IEnumerator<T> and nongeneric System.Collections.IEnumerator interfaces are designed to enable the iterator pattern for iterating over collections of elements, rather than the length-index pattern shown previously. A class diagram of their relationships appears in **Figure 1**.

 "C# foreach Internals and Custom Iterators with yield"

**Figure 1 A Class Diagram of the IEnumerator<T> and IEnumerator Interfaces**

IEnumerator, which IEnumerator<T> derives from, includes three members. The first is bool MoveNext. Using this method, you can move from one element within the collection to the next, while at the same time detecting when you’ve enumerated through every item. The second member, a read-only property called Current, returns the element currently in process. Current is overloaded in IEnumerator<T>, providing a type-specific implementation of it. With these two members on the collection class, it’s possible to iterate over the collection simply using a while loop:

```
System.Collections.Generic.Stack<int> stack =
  new System.Collections.Generic.Stack<int>();
int number;
// ...
// This code is conceptual, not the actual code.
while (stack.MoveNext())
{
  number = stack.Current;
  Console.WriteLine(number);
}
```

In this code, the MoveNext method returns false when it moves past the end of the collection. This replaces the need to count elements while looping.

(The Reset method usually throws a NotImplementedException, so it should never be called. If you need to restart an enumeration, just create a fresh enumerator.)

The preceding example showed the gist of the C# compiler output, but it doesn’t actually compile that way because it omits two important details concerning the implementation: interleaving and error handling.

State Is Shared: The problem with an implementation such as the one in the previous example is that if two such loops interleave each other—one foreach inside another, both using the same collection—the collection must maintain a state indicator of the current element so that when MoveNext is called, the next element can be determined. In such a case, one interleaving loop can affect the other. (The same is true of loops executed by multiple threads.)

To overcome this problem, the collection classes don’t support IEnumerator<T> and IEnumerator interfaces directly. Rather, there’s a second interface, called IEnumerable<T>, whose only method is GetEnumerator. The purpose of this method is to return an object that supports IEnumerator<T>. Instead of the collection class maintaining the state, a different class—usually a nested class so that it has access to the internals of the collection—will support the IEnumerator<T> interface and will keep the state of the iteration loop. The enumerator is like a “cursor” or a “bookmark” in the sequence. You can have multiple bookmarks, and moving any one of them enumerates over the collection independently of the others. Using this pattern, the C# equivalent of a foreach loop will look like the code shown in **Figure 2**.

**Figure 2 A Separate Enumerator Maintaining State During an Iteration**

```
System.Collections.Generic.Stack<int> stack =
  new System.Collections.Generic.Stack<int>();
int number;
System.Collections.Generic.Stack<int>.Enumerator
  enumerator;
// ...
// If IEnumerable<T> is implemented explicitly,
// then a cast is required.
// ((IEnumerable<int>)stack).GetEnumerator();
enumerator = stack.GetEnumerator();
while (enumerator.MoveNext())
{
  number = enumerator.Current;
  Console.WriteLine(number);
}
```

Cleaning up Following Iteration: Given that the classes that implement the IEnumerator<T> interface maintain the state, sometimes you need to clean up the state after it exits the loop (because either all iterations have completed or an exception is thrown). To achieve this, the IEnumerator<T> interface derives from IDisposable. Enumerators that implement IEnumerator don’t necessarily imple­ment IDisposable, but if they do, Dispose will be called, as well. This enables the calling of Dispose after the foreach loop exits. The C# equivalent of the final CIL code, therefore, looks like **Figure 3**.

**Figure 3 Compiled Result of foreach on Collections**

```
System.Collections.Generic.Stack<int> stack =
  new System.Collections.Generic.Stack<int>();
System.Collections.Generic.Stack<int>.Enumerator
  enumerator;
IDisposable disposable;
enumerator = stack.GetEnumerator();
try
{
  int number;
  while (enumerator.MoveNext())
  {
    number = enumerator.Current;
    Console.WriteLine(number);
  }
}
finally
{
  // Explicit cast used for IEnumerator<T>.
  disposable = (IDisposable) enumerator;
  disposable.Dispose();
  // IEnumerator will use the as operator unless IDisposable
  // support is known at compile time.
  // disposable = (enumerator as IDisposable);
  // if (disposable != null)
  // {
  //   disposable.Dispose();
  // }
}
```

Notice that because the IDisposable interface is supported by IEnumerator<T>, the using statement can simplify the code in **Figure 3** to what is shown in **Figure 4**.

**Figure 4 Error Handling and Resource Cleanup with using**

```
System.Collections.Generic.Stack<int> stack =
  new System.Collections.Generic.Stack<int>();
int number;
using(
  System.Collections.Generic.Stack<int>.Enumerator
    enumerator = stack.GetEnumerator())
{
  while (enumerator.MoveNext())
  {
    number = enumerator.Current;
    Console.WriteLine(number);
  }
}
```

However, recall that the CIL doesn’t directly support the using keyword. Thus, the code in Figure 3 is actually a more accurate C# representation of the foreach CIL code.

foreach without IEnumerable: C# doesn’t require that IEnumerable/IEnumerable<T> be implemented to iterate over a data type using foreach. Rather, the compiler uses a concept known as duck typing; it looks for a GetEnumerator method that returns a type with a Current property and a MoveNext method. Duck typing involves searching by name rather than relying on an interface or explicit method call to the method. (The name “duck typing” comes from the whimsical idea that to be treated as a duck, the object must merely implement a Quack method; it need not implement an IDuck interface.) If duck typing fails to find a suitable implementation of the enumerable pattern, the compiler checks whether the collection implements the interfaces.

### Introducing Iterators

Now that you understand the internals of the foreach implementation, it’s time to discuss how iterators are used to create custom implementations of the IEnumerator<T>, IEnumerable<T> and corresponding nongeneric interfaces for custom collections. Iterators provide clean syntax for specifying how to iterate over data in collection classes, especially using the foreach loop, allowing the end users of a collection to navigate its internal structure without knowledge of that structure.

The problem with the enumeration pattern is that it can be cumbersome to implement manually because it must maintain all the state necessary to describe the current position in the collection. This internal state might be simple for a list collection type class; the index of the current position suffices. In contrast, for data structures that require recursive traversal, such as binary trees, the state can be quite complicated. To mitigate the challenges associated with implementing this pattern, C# 2.0 added the yield contextual keyword to make it easier for a class to dictate how the foreach loop iterates over its contents.

Defining an Iterator:Iterators are a means to implement methods of a class, and they’re syntactic shortcuts for the more complex enumerator pattern. When the C# compiler encounters an iterator, it expands its contents into CIL code that implements the enumerator pattern. As such, there are no runtime dependencies for implementing iterators. Because the C# compiler handles implementation through CIL code generation, there’s no real runtime performance benefit to using iterators. However, there is a substantial programmer productivity gain in choosing iterators over manual implementation of the enumerator pattern. To understand this improvement, I’ll first consider how an iterator is defined in code.

Iterator Syntax: An iterator provides a shorthand implementation of iterator interfaces, the combination of the IEnumerable<T> and IEnumerator<T> interfaces. **Figure 5** declares an iterator for the generic BinaryTree<T> type by creating a GetEnumerator method (albeit, with no implementation yet).

**Figure 5 Iterator Interfaces Pattern**

```
using System;
using System.Collections.Generic;
public class BinaryTree<T>:
  IEnumerable<T>
{
  public BinaryTree ( T value)
  {
    Value = value;
  }
  #region IEnumerable<T>
  public IEnumerator<T> GetEnumerator()
  {
    // ...
  }
  #endregion IEnumerable<T>
  public T Value { get; }  // C# 6.0 Getter-only Autoproperty
  public Pair<BinaryTree<T>> SubItems { get; set; }
}
public struct Pair<T>: IEnumerable<T>
{
  public Pair(T first, T second) : this()
  {
    First = first;
    Second = second;
  }
  public T First { get; }
  public T Second { get; }
  #region IEnumerable<T>
  public IEnumerator<T> GetEnumerator()
  {
    yield return First;
    yield return Second;
  }
  #endregion IEnumerable<T>
  #region IEnumerable Members
  System.Collections.IEnumerator
    System.Collections.IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }
  #endregion
  // ...
}
```

Yielding Values from an Iterator: The iterator interfaces are like functions, but instead of returning a single value, they yield a sequence of values, one at a time. In the case of BinaryTree<T>, the iterator yields a sequence of values of the type argument provided for T. If the nongeneric version of IEnumerator is used, the yielded values will instead be of type object.

To correctly implement the iterator pattern, you need to maintain some internal state to keep track of where you are while enumerating the collection. In the BinaryTree<T> case, you track which elements within the tree have already been enumerated and which are still to come. Iterators are transformed by the compiler into a “state machine” that keeps track of the current position and knows how to “move itself” to the next position.

The yield return statement yields a value each time an iterator encounters it; control immediately returns to the caller that requested the item. When the caller requests the next item, the code begins to execute immediately following the previously executed yield return statement. In **Figure 6**, the C# built-in data type keywords are returned sequentially.

**Figure 6 Yielding Some C# Keywords Sequentially**

```
using System;
using System.Collections.Generic;
public class CSharpBuiltInTypes: IEnumerable<string>
{
  public IEnumerator<string> GetEnumerator()
  {
    yield return "object";
    yield return "byte";
    yield return "uint";
    yield return "ulong";
    yield return "float";
    yield return "char";
    yield return "bool";
    yield return "ushort";
    yield return "decimal";
    yield return "int";
    yield return "sbyte";
    yield return "short";
    yield return "long";
    yield return "void";
    yield return "double";
    yield return "string";
  }
    // The IEnumerable.GetEnumerator method is also required
    // because IEnumerable<T> derives from IEnumerable.
  System.Collections.IEnumerator
    System.Collections.IEnumerable.GetEnumerator()
  {
    // Invoke IEnumerator<string> GetEnumerator() above.
    return GetEnumerator();
  }
}
public class Program
{
  static void Main()
  {
    var keywords = new CSharpBuiltInTypes();
    foreach (string keyword in keywords)
    {
      Console.WriteLine(keyword);
    }
  }
}
```

The results of **Figure 6** appear in **Figure 7**, which is a listing of the C# built-in types.

**Figure 7 A List of Some C# Keywords Output from the Code in Figure 6**

```
object
byte
uint
ulong
float
char
bool
ushort
decimal
int
sbyte
short
long
void
double
string
```

Clearly, more explanation is required but I’m out of space for this month so I’ll leave you in suspense for another column. Suffice it to say, with iterators you can magically create collections as properties, as shown in **Figure 8**—in this case, relying on C# 7.0 tuples just for the fun of it. For those of you wanting to look ahead, you can check out the source code or take a look at Chapter 16 of my “Essential C#” book.

**Figure 8 Using yield return to Implement an IEnumerable<T> Property**

```
IEnumerable<(string City, string Country)> CountryCapitals
{
  get
  {
    yield return ("Abu Dhabi","United Arab Emirates");
    yield return ("Abuja", "Nigeria");
    yield return ("Accra", "Ghana");
    yield return ("Adamstown", "Pitcairn");
    yield return ("Addis Ababa", "Ethiopia");
    yield return ("Algiers", "Algeria");
    yield return ("Amman", "Jordan");
    yield return ("Amsterdam", "Netherlands");
    // ...
  }
}
```

### Wrapping Up

In this column, I stepped back to functionality that’s been part of C# since version 1.0 and hasn’t changed much since the introduction of generics in C# 2.0. Despite the frequent use of this functionality, however, many don’t understand the details of what’s taking place internally. I then scratched the surface of the iterator pattern—leveraging the yield return construct—and provided an example.

Much of this column was pulled from my “[Essential C# 6](https://intellitect.com/intellitect-products/essential-c-sharp-6/)” book, which I’m currently in the midst of updating to “Essential C# 7.0.” For more information, check out Chapters 14 and 16.

_Thanks to the following IntelliTect technical experts for reviewing this article:_ _Kevin Bost__._

_This article was originally posted in the April 2017 issue of MSDN Magazine._

* * *
