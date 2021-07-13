

By the time you read this, the C# 7 design team will have been discussing, planning, experimenting and programming for about a year. In this installment, I’ll sample some of the ideas they’ve been exploring.

In reviewing, be mindful that at this time these are still ideas for what to include in C# 7. Some of the ideas the team has simply talked about, while others have made it as far as experimental implementations. Regardless, none of the concepts are set; many may never see the light of day; and even those that are further down the road could be cut in the final stages of language finalization.

### Declaring Nullable and Non-Nullable Reference Types

Perhaps some of the most predominant ideas in the C# 7 discussion are those relating to further improvement in working with null—along the same lines as the null-conditional operator of C# 6.0. One of the simplest improvements might be complier or analyzer verification that accessing a nullable type instance would be prefaced with a check that the type is not, in fact, null.

On occasions when null isn’t desirable on a reference type, what if you could avoid null entirely? The idea would be to declare the intent that a reference type would allow nulls (string?) or avoid null (string!). In theory, it might even be possible to assume that all reference type declarations in new code are, by default, non-nullable. However, as pointed out by my “Essential C# 6.0” book co-author, Eric Lippert, ensuring a reference type would never be null at compile time is prohibitively difficult ([bit.ly/1Rd5ekS](https://bit.ly/1Rd5ekS)). Even so, what would be possible is to identify scenarios where a type could potentially be null and yet dereferenced without checking that it isn’t. Or, scenarios where a type might be assigned null in spite of the declared intent that it not be null.

For even more benefit, the team is discussing the possibility of leveraging the non-nullable type declaration on a parameter such that it would automatically generate a null check (although this would likely have to be an opt-in decision to avoid any undesirable performance degradation unless it can be made at compile time).

(Ironically, C# 2.0 added nullable value types because there are numerous occasions—such as data retrieved from a database—where it’s necessary to let an integer contain null. And now, in C# 7, the team is looking to support the opposite for reference types.)

One interesting consideration with reference type support for things non-nullable (for example, string! text) is what the implementation would be in Common Intermediate Language (CIL). The two most popular proposals are whether to map it to a NonNullable<T> type syntax or to leverage attributes as in \[Nullable\] string text. The latter is currently the preferred approach.

### Tuples

Tuples is another feature under consideration for C# 7. This is a topic that has come up on multiple occasions for earlier versions of the language, but still hasn’t quite made it to production. The idea is that it would be possible to declare types in sets such that a declaration can contain more than one value and, similarly, methods can return more than one value. Consider the following sample code to understand the concept:

```
public class Person
{
  public readonly (string firstName, string lastName) Names; // a tuple
  public Person((string FirstName, string LastName)) names, int Age)
  {
    Names = names;
  }
}
```

As the listing shows, with tuples support, you can declare a type as a tuple—having two (or more) values. This can be leveraged anywhere a data type can be leveraged—including as a field, parameter, variable declaration or even a method return. For example, the following code snippet would return a tuple from a method:

```
public (string FirstName, string LastName) GetNames(string! fullName)
{
  string\[\] names = fullName.Split(" ", 2);
  return (names\[0\], names\[1\]);
}
public void Main()
{
  // ...
  (string first, string last) = GetNames("Inigo Montoya");
  // ...
}
```

In this listing there’s a method that returns a tuple and a variable declaration of first and last that the GetNames result is assigned to. Note that the assignment is based on the order within the tuple (not the names of the receiving variables). Considering some of the alternative approaches we have to use today—an array or collection, a custom type, out parameters—tuples are an attractive option.

There are numerous options that could go along with tuples. Here are a few under consideration:

- Tuples could have named or unnamed properties, as in:

```
var name = ("Inigo", "Montoya")
```

and:

```
var name = (first: "John", last: "Doe")
```

- The result could be an anonymous type or explicit variables, as in:

```
var name = (first: "John", last: "Doe")
```

or:

```
(string first, string last) = GetNames("Inigo Montoya")
```

- You can access the individual tuple items by name, as in:

```
Console.WriteLine($”My name is { names.first } { names.last }.”);
```

- Data types could be inferred where they're not identified explicitly (following the same approach used by anonymous types in general)

Although there are complications with tuples, for the most part they follow structures already well-established within the language, so they have pretty strong support for inclusion in C# 7.

### Pattern Matching

Pattern matching is also a frequent topic within the C# 7 design team’s discussion. Perhaps one of the more understandable renderings of this would be expanded switch (and if) statements that supported expression patterns in the case statements, rather than just constants. (To correspond with the expanded case statement, the switch expression type wouldn’t be limited to types that have corresponding constant values, either). With pattern matching, you could query the switch expression for a pattern, such as whether the switch expression was a specific type, a type with a particular member, or even a type that matched a specific “pattern” or expression. For example, consider how obj might be of type Point with an x value greater than 2:

```
object obj;
// ...
switch(obj) {
  case 42:
    // ...
  case Color.Red:
    // ...
  case string s:
    // ...
  case Point(int x, 42) where (Y > 42):
    // ...
  case Point(490, 42): // fine
    // ...
  default:
    // ...
}
```

Interestingly, given expressions as case statements, it would also be necessary to allow expressions as arguments on goto case statements.

To support the case of type Point, there would need to be some type of member on Point that handled the pattern matching. In this case, what’s needed is a member that takes two arguments of type int. A member, for example, such as:

```
public static bool operator is (Point self out int x, out int y) {...}
```

Note that without the where expression, case Point(490, 42) could never be reached, causing the compiler to issue an error or warning.

One of the limiting factors of the switch statement is that it doesn’t return a value, but rather executes a code block. An added feature of pattern matching might be support for a switchexpression that returns a value, as in:

```
string text = match (e) { pattern => expression; ... ; default => expression }
```

Similarly, the is operator could support pattern matching, allowing not only a type check but possible support for a more generic query as to whether particular members on a type exist.

### Records

In a continuation of the abbreviated “constructor” declaration syntax considered (but ultimately rejected) in C# 6.0, there is support for embedding the constructor declaration within the class definition, a concept known as “records.” For example, consider the following declaration:

```
class Person(string Name, int Age);
```

This simple statement would automatically generate the following:

- A constructor:

```
public Person(string Name, int Age)
{
  this.Name = Name;
  this.Age = Age;
}
```

- Read-only properties, thus creating an immutable type
- Equality implementations (such as GetHashCode, Equals, operator ==, operator != and so forth)
- A default implementation of ToString
- Pattern-matching support of the “is” operator

Although a significant amount of code is generated (considering only one short line of code created it all), the hope is that it might provide a correspondingly significant shortcut to manually coding what is essentially boilerplate implementations. Furthermore, all of the code can be thought of as “default” in that explicitly implementing any of it would take precedence and preclude generation of the same member.

One of the more problematic issues associated with records is how to handle serialization. Presumably leveraging records as data transfer objects (DTOs) is fairly typical and yet it isn’t clear what, if anything, can be done to support the serialization of such records.

In association with the records is support for with expressions. With expressions allow the instantiation of a new object based on an existing object. Given the person object declaration, for example, you could create a new instance via the following with expression:

```
Person inigo = new Person("Inigo Montoya", 42);
Person humperdink = inigo with { Name = "Prince Humperdink" };
```

The generated code corresponding to the with expression would be something like:

```
Person humperdink = new Person(Name: "Prince Humperdink", Age: inigo.42 );
```

An alternative suggestion, however, is that rather than depending on the signature of the constructor for the with expression, it might be preferable to translate it to the invocation of a With method, as in:

```
Person humperdink = inigo.With(Name: "Prince Humperdink", Age: inigo.42);
```

### Async Streams

To enhance support of async in C# 7, the notion of processing asynchronous sequences is intriguing. For example, given an IAsyncEnumerable, with a Current property and a Task<bool> MoveNextAsync method, you could iterate over the IAsyncEnumerable instance using foreach and have the compiler take care of invoking each member in the stream asynchronously—performing an await to find out if there’s another element in the sequence (possibly a channel) to process. There are a number of caveats to this that have to be evaluated, the least of which is the potential LINQ bloat that could occur with all the LINQ standard query operators that return IAsyncEnumerable. Additionally, it’s not certain how to expose CancellationToken support and even Task.ConfigureAwait.

### C# on the Command Line

As a lover of how Windows PowerShell makes the Microsoft .NET Framework available in a command-line interface (CLI), one area I’m particularly intrigued by (possibly my favorite feature under consideration) is support for using C# on the command line; it’s a concept more generically referred to as support for Read, Evaluate, Print, Loop (REPL). As one would hope, REPL support would be accompanied by C# scripting that doesn’t require all the usual formality (such as class declaration) in trivial scenarios that don’t need such ceremony. Without a compile step, REPL would require new directives for referencing assemblies and NuGet packages, along with importing additional files. The current proposal under discussion would support:

- #r to reference an additional assembly or NuGet package. A variation would be #r!, which would even allow access to internal members, albeit with some constraints. (This is intended for scenarios where you’re accessing assemblies for which you have the source code.)
- #l to include entire directories (similar to F#).
- #load to import an additional C# script file, in the same way you’d add it to your project except now order is important. (Note that importing a .cs file might not be supported because namespaces aren’t allowed in C# script.)
- #time to turn on performance diagnostics while executing.

You can expect the first version of C# REPL to be released with Visual Studio 2015 Update 1 (along with an updated Interactive Window that supports the same feature set). For more information check out Itl.tc/CSREPL, along with my column next month.

### Wrapping Up

With a year’s worth of material, there’s far too much to explore all that the design team has been doing, and even with the ideas I touched on, there are a lot more details (both caveats and advantages) that need to be considered. Hopefully, however, you now have an idea of what the team is exploring and how they’re looking to improve on the already brilliant C# language. If you’d like to review the C# 7 design notes directly, and possibly provide your own feedback, you can jump into the discussion at [bit.ly/CSharp7DesignNotes](https://bit.ly/CSharp7DesignNotes).

_Thanks to the following Microsoft technical expert for reviewing this article: Mads Torgerson._

_This article was originally posted [here](https://docs.microsoft.com/en-us/archive/msdn-magazine/2015/december/essential-net-designing-csharp-7) in the December 2015 issue of MSDN Magazine._
