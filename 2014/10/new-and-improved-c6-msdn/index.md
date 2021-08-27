

## New and Improved C# 6.0
#
Although C# 6.0 isn’t yet complete, it’s at a point now where the features are close to being finalized. There have been a number of changes and improvements made to C# 6.0 in the CTP3 release of the next version of Visual Studio, code-named “14,” since the May 2014 article, “A C# 6.0 Language Preview” ([msdn.microsoft.com/magazine/dn683793.aspx](https://msdn.microsoft.com/magazine/dn683793.aspx)).

In this article, I’ll introduce the new features and provide an update on the features discussed back in May. I’ll also maintain a comprehensive up-to-date blog describing updates to each C# 6.0 feature. Check it out at [intellitect.com/EssentialCSharp/](/EssentialCSharp/). Many of these examples are from the next edition of my book, “Essential C# 6.0” (Addison-Wesley Professional).

### Null-Conditional Operator

Even the newest .NET developers are likely familiar with the NullReferenceException. This is an exception that almost always indicates a bug because the developer didn’t perform sufficient null checking before invoking a member on a (null) object. Consider this example:

```csharp
public static string Truncate(string value, int length)
{
  string result = value;
  if (value != null) // Skip empty string check for elucidation
  {
    result = value.Substring(0, Math.Min(value.Length, length));
  }
  return result;
}
```

If it wasn’t for the check for null, the method would throw a NullReferenceException. Although it’s simple, having to check the string parameter for null is somewhat verbose. Often, that verbose approach is likely unnecessary given the frequency of the comparison. C# 6.0 includes a new null-conditional operator that helps you write these checks more succinctly:

```csharp
public static string Truncate(string value, int length)
{          
  return value?.Substring(0, Math.Min(value.Length, length));
}
[TestMethod]
public void Truncate_WithNull_ReturnsNull()
{
  Assert.AreEqual<string>(null, Truncate(null, 42));
}
```

As the Truncate_WithNull_ReturnsNull method demonstrates, if in fact the value of the object is null, the null-conditional operator will return null. This begs the question of what happens when the null-conditional operator appears within a call chain, as in the next example:

```csharp
public static string AdjustWidth(string value, int length)
{
  return value?.Substring(0, Math.Min(value.Length, length)).PadRight(length);
}
[TestMethod]
public void AdjustWidth_GivenInigoMontoya42_ReturnsInigoMontoyaExtended()
{
  Assert.AreEqual<int>(42, AdjustWidth("Inigo Montoya", 42).Length);
}
```

Even though Substring is called via the null-conditional operator, and a null value?.Substring could seemingly return null, the language behavior does what you would want. It short-circuits the call to PadRight, and immediately returns null, avoiding the programming error that would otherwise result in a NullReferenceException. This is a concept known as null-propagation.

The null-conditional operator conditionally checks for null before invoking the target method and any additional method within the call chain. Potentially, this could yield a surprising result such as in the statement text?.Length.GetType.

If the null-conditional returns null when the invocation target is null, what’s the resulting data type for an invocation of a member that returns a value type—given a value type can’t be null? For example, the data type returned from value?. Length can’t simply be int. The answer, of course, is a nullable (int?). In fact, an attempt to assign the result simply to an int will produce a compile error:

```csharp
int length = text?.Length; // Compile Error: Cannot implicitly convert type 'int?' to 'int'
```

The null-conditional has two syntax forms. First, is the question mark prior to the dot operator (?.). The second is to use the question mark in combination with the index operator. For example, given a collection, instead of checking for null explicitly before indexing into the collection, you can do so using the null conditional operator:

```csharp
public static IEnumerable<T> GetValueTypeItems<T>(
  IList<T> collection, params int[] indexes)
  where T : struct
{
  foreach (int index in indexes)
  {
    T? item = collection?[index];
    if (item != null) yield return (T)item;
  }
}
```

This example uses the null-conditional index form of the operator ?[…], causing indexing into collection only to occur if collection isn’t null. With this form of the null-conditional operator, the T? item = collection?[index] statement is behaviorally equivalent to:

```csharp
T? item = (collection != null) ? collection[index] : null.
```

Note that the null-conditional operator can only retrieve items. It won’t work to assign an item. What would that mean, given a null collection, anyway?

Note the implicit ambiguity when using ?[…] on a reference type. Because reference types can be null, a null result from the ?[…] operator is ambiguous about whether the collection was null or the element itself was, in fact, null.

One particularly useful application of the null-conditional operator resolves an idiosyncrasy of C# that has existed since C# 1.0—checking for null before invoking a delegate. Consider the C# 2.0 code in **Figure 1**.

**Figure 1 Checking for Null Before Invoking a Delegate**

```csharp
class Theremostat
{
  event EventHandler<float> OnTemperatureChanged;
  private int _Temperature;
  public int Temperature
  {
    get
    {
      return _Temperature;
    }
    set
    {
      // If there are any subscribers, then
      // notify them of changes in temperature
      EventHandler<float> localOnChanged =
        OnTemperatureChanged;
      if (localOnChanged != null)
      {
        _Temperature = value;
        // Call subscribers
        localOnChanged(this, value);
      }
    }
  }
}
```

Leveraging the null-conditional operator, the entire set implementation is reduced to simply:

```csharp
OnTemperatureChanged?.Invoke(this, value)
```

All you need now is a call to Invoke prefixed by a null-conditional operator. You no longer need to assign the delegate instance to a local variable in order to be thread safe or even to explicitly check the value for null before invoking the delegate.

C# developers have wondered if this would be improved for the last four releases. It’s finally going to happen. This feature alone will change the way you invoke delegates.

Another common pattern where the null-conditional operator could be prevalent is in combination with the coalesce operator. Instead of checking for null on linesOfCode before invoking Length, you can write an item count algorithm as follows:

```csharp
List<string> linesOfCode = ParseSourceCodeFile("Program.cs");
return linesOfCode?.Count ?? 0;
```

In this case, any empty collection (no items) and a null collection are both normalized to return the same count. In summary, the null-conditional operator will:

- Return null if the operand is null
- Short-circuit additional invocations in the call chain if the operand is null
- Return a nullable type (System.Nullable<T>) if the target member returns a value type
- Support delegate invocation in a thread safe manner
- Is available as both a member operator (?.) and an index operator (?[…])

### Auto-Property Initializers

Any .NET developer who has ever properly implemented a struct has undoubtedly been bothered by how much syntax it takes to make the type immutable (as .NET standards suggest it should be). At issue is the fact that a read-only property should have:

1. A read-only-defined backing field
2. Initialization of the backing field from within the constructor
3. Explicit implementation of the property (rather than using an auto-property)
4. An explicit getter implementation that returns the backing field

All of this is just to “properly” implement an immutable property. This behavior is then repeated for all properties on the type. So doing the right thing requires significantly more effort than the brittle approach. C# 6.0 comes to the rescue with a new feature called auto-property initializers (CTP3 also includes support for initialization expressions). The auto-property initializer allows assignment of properties directly within their declaration. For read-only properties, it takes care of all the ceremony required to ensure the property is immutable. Consider, for example, the FingerPrint class in this example:

```csharp
public class FingerPrint
{
  public DateTime TimeStamp { get; } = DateTime.UtcNow;
  public string User { get; } =
    System.Security.Principal.WindowsPrincipal.Current.Identity.Name;
  public string Process { get; } =
    System.Diagnostics.Process.GetCurrentProcess().ProcessName;
}
```

As the code shows, property initializers allow for assigning the property an initial value as part of the property declaration. The property can be read-only (only a getter) or read/write (both setter and getter). When it’s read-only, the underlying backing field is automatically declared with the read-only modifier. This ensures that it’s immutable following initialization.

Initializers can be any expression. For example, by leveraging the conditional operator, you can default the initialization value:

```csharp
public string Config { get; } = string.IsNullOrWhiteSpace(
  string connectionString =
    (string)Properties.Settings.Default.Context?["connectionString"])?
  connectionString : "<none>";
```

In this example, notice the use of declaration expression (see [intellitect.com/declaration-expressions-in-c-6-0/](/declaration-expressions-in-c-6-0/)) as discussed in the previous article. If you need more than an expression, you could refactor the initialization into a static method and invoke that.

### Nameof Expressions

Another addition introduced in the CTP3 release is support for nameof expressions. There are several occasions when you’ll need to use “magic strings” within your code. Such “magic strings” are normal C# strings that map to program elements within your code. For example, when throwing an ArgumentNullException, you’d use a string for the name of the corresponding parameter that was invalid. Unfortunately, these magic strings had no compile time validation and any program element changes (such as renaming the parameter) wouldn’t automatically update the magic string, resulting in an inconsistency that was never caught by the compiler.

On other occasions, such as when raising OnPropertyChanged events, you can avoid the magic string via tree expression gymnastics that extract the name. It’s perhaps a little more irritating given the operation’s simplicity, which is just identifying the program element name. In both cases, the solution was less than ideal.

To address this idiosyncrasy, C# 6.0 provides access to a “program element” name, whether it’s a class name, method name, parameter name or particular attribute name (perhaps when using reflection). For example, the code in **Figure 2** uses the nameof expression to extract the name of the parameter.

**Figure 2 Extracting the Parameter Name with a Nameof Expression**

```csharp
void ThrowArgumentNullExceptionUsingNameOf(string param1)
{
  throw new ArgumentNullException(nameof(param1));
}
[TestMethod]
public void NameOf_UsingNameofExpressionInArgumentNullException()
{
  try
  {
    ThrowArgumentNullExceptionUsingNameOf("data");
    Assert.Fail("This code should not be reached");
  }
  catch (ArgumentNullException exception)
  {
    Assert.AreEqual<string>("param1", exception.ParamName);
}
```

As the test method demonstrates, the ArgumentNullException’s ParamName property has the value param1—a value set using the nameof(param1) expression in the method. The nameof expression isn’t limited to parameters. You can use it to retrieve any programming element, as shown in **Figure 3**.

**Figure 3 Retrieving Other Program Elements**

```csharp
namespace CSharp6.Tests
{
  [TestClass]
  public class NameofTests
  {
    [TestMethod]
    public void Nameof_ExtractsName()
    {
      Assert.AreEqual<string>("NameofTests", nameof(NameofTests));
      Assert.AreEqual<string>("TestMethodAttribute",
        nameof(TestMethodAttribute));
      Assert.AreEqual<string>("TestMethodAttribute",
        nameof(
         Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute));
      Assert.AreEqual<string>("Nameof_ExtractsName",
        string.Format("{0}", nameof(Nameof_ExtractsName)));
      Assert.AreEqual<string>("Nameof_ExtractsName",
        string.Format("{0}", nameof(
        CSharp6.Tests.NameofTests.Nameof_ExtractsName)));
    }
  }
}
```

The nameof expression only retrieves the final identifier, even if you use more explicit dotted names. Also, in the case of attributes, the “Attribute” suffix isn’t implied. Instead, it’s required for compilation. It provides great opportunity to clean up messy code.

### Primary Constructors

Auto-property initializers are especially useful in combination with primary constructors. Primary constructors give you a way to reduce ceremony on common object patterns. This feature has been significantly improved since May. Updates include:

1. An optional implementation body for the primary constructor: This allows for things such as primary constructor parameter validation and initialization, which was previously not supported.
2. Elimination of field parameters: declaration of fields via the primary constructor parameters. (Not moving forward with this feature as it was defined was the right decision, as it no longer forces particular naming conventions in ways that C# previously was ambivalent.)
3. Support for expression bodied functions and properties (discussed later in this article).

With the prevalence of Web services, multiple-tier applications, data services, Web API, JSON and similar technologies, one common form of class is the data transfer object (DTO). The DTO generally doesn’t have much implementation behavior, but focuses on data storage simplicity. This focus on simplicity makes primary constructors compelling. Consider, for example, the immutable Pair data structure shown in this example:

```csharp
struct Pair<T>(T first, T second)
{
  public T First { get; } = first;
  public T Second { get; } = second;
  // Equality operator ...
}
```

The constructor definition—Pair(string first, string second)—is merged into the class declaration. This specifies the constructor parameters are first and second (each of type T). Those parameters are also referenced in the property initializers and assigned to their corresponding properties. When you observe the simplicity of this class definition, its support for immutability and the requisite constructor (initializer for all properties/fields), you see how it helps you code correctly. That leads to a significant improvement in a common pattern that previously required unnecessary verbosity.

Primary constructor bodies specify behavior on the primary constructor. This helps you implement an equivalent capability on primary constructors as you can on constructors in general. For example, the next step in improving the reliability of the Pair<T> data structure might be to provide property validation. Such validation could ensure a value of null for Pair.First would be invalid. CTP3 now includes a primary constructor body—a constructor body without the declaration, as shown in **Figure 4**.

**Figure 4 Implementing a Primary Constructor Body**

```csharp
struct Pair<T>(T first, T second)
{
  {
    if (first == null) throw new ArgumentNullException("first");
    First = first; // NOTE: Not working in CTP3
  }     
  public T First { get; }; // NOTE: Results in compile error for CTP3
  public T Second { get; } = second;
  public int CompareTo(T first, T second)
  {
    return first.CompareTo(First) + second.CompareTo(Second);
  }
// Equality operator ...
}
```

For clarity, I placed the primary constructor body at the first member of the class. However, this isn’t a C# requirement. The primary constructor body can appear in any order relative to the other class members.

Although not functional in CTP3, another feature of read-only properties is that you can assign them directly from within the constructor (for example, First = first). This isn’t limited to primary constructors, but is available for any constructor member.

An interesting consequence of support for auto-property initializers is that it eliminates many of the cases found in earlier versions where you needed explicit field declarations. The obvious case it doesn’t eliminate is a scenario where validation on the setter is required. On the other hand, the need to declare read-only fields becomes virtually deprecated. Now, whenever a read-only field is declared, you can declare a read-only auto-property possibly as private, if that level of encapsulation is required.

The CompareTo method has parameters first and second—seemingly overlapping the parameter names of the primary constructor. Because primary constructor names are in scope within the auto-property initializers, first and second may seem ambiguous. Fortunately, this isn’t the case. The scoping rules pivot on a different dimension you haven’t seen in C# before.

Prior to C# 6.0, scope was always identified by the variable declaration placement within code. Parameters are bound within the method they help declare, fields are bound within the class, and variables declared within an if statement are bound by the condition body of the if statement.

In contrast, primary constructor parameters are bound by time. The primary constructor parameters are only “alive” while the primary constructor is executing. This time frame is obvious in the case of the primary constructor body. It’s perhaps less obvious for the auto-property initializer case.

However, like field initializers translated to statements executing as part of a class initialization in C# 1.0+, auto-property initializers are implemented the same way. In other words, the scope of a primary constructor parameter is bound to the life of the class initializer and the primary constructor body. Any reference to the primary constructor parameters outside an auto-property initializer or primary constructor body will result in a compile error.

There are several additional concepts related to primary constructors that are important to remember. Only the primary constructor can invoke the base constructor. You do this using the base (contextual) keyword following the primary constructor declaration:

```csharp
class UsbConnectionException(
  string message, Exception innerException, HidDeviceInfo hidDeviceInfo):
    Exception  (message, innerException)
{
  public HidDeviceInfo HidDeviceInfo { get;  } = hidDeviceInfo;
}
```

If you specify additional constructors, the constructor call chain must invoke the primary constructor last. This means a primary constructor can’t have a this initializer. All other constructors must have them, assuming the primary constructor isn’t also the default constructor:

```csharp
public class Patent(string title, string yearOfPublication)
{
  public Patent(string title, string yearOfPublication,
    IEnumerable<string> inventors)
    ...this(title, yearOfPublication)
  {
    Inventors.AddRange(inventors);
  }
}
```

Hopefully, these examples help demonstrate that primary constructors bring simplicity to C#. They’re an additional opportunity to do simple things simply, instead of simple things in a complex fashion. It’s occasionally warranted for classes to have multiple constructors and call chains that make code hard to read. If you encounter a scenario where the primary constructor syntax makes your code look more complex rather than simplifying it, then don’t use primary constructors. For all the enhancements to C# 6.0, if you don’t like a feature or if it makes your code harder to read, just don’t use it.

### Expression Bodied Functions and Properties

Expression bodied functions are another syntax simplification in C# 6.0. These are functions with no statement body. Instead, you implement them with an expression following the function declaration.

For example, an override of ToString could be added to the Pair<T> class:

```csharp
public override string ToString() => string.Format("{0}, {1}", First, Second);
```

There’s nothing particularly radical about expression bodied functions. As with most of the features found in C# 6.0, they’re intended to provide a simplified syntax for cases where the implementation is simple. The return type of the expression must, of course, match the return type identified in the function declaration. In this case, ToString returns a string, as does the function implementation expression. Methods that return void or Task should be implemented with expressions that don’t return anything, either.

The expression bodied simplification isn’t limited to functions. You can also implement read-only (getter only) properties using expressions—expression bodied properties. For example, you can add a Text member to the FingerPrint class:

```csharp
public string Text =>
  string.Format("{0}: {1} - {2} ({3})", TimeStamp, Process, Config, User);
```

### Other Features

There are several features no longer planned for C# 6.0:

- The indexed property operator ($) is no longer available and isn’t expected for C# 6.0.
- The index member syntax isn’t working in CTP3, although it’s expected to return in a later release of C# 6.0:

```csharp
var cppHelloWorldProgram = new Dictionary<int, string>
{
[10] = "main() {",
[20] = "    printf(\"hello, world\")",
[30] = "}"
};
```

- Field arguments in primary constructors are no longer part of C# 6.0.
- Both the binary numeric literal and the numeric separator (‘_’) within a numeric literal aren’t currently certain to make it by release to manufacturing.

There are a number of features not discussed here because they were already covered in the May article, but static using statements (see [intellitect.com/static-using-statement-in-c-6-0/](/static-using-statement-in-c-6-0/)), declaration expressions (see [intellitect.com/declaration-expressions-in-c-6-0/](/declaration-expressions-in-c-6-0/)) and exception handling improvements (see [intellitect.com/csharp-exception-handling/](/csharp-exception-handling/)) are features that have remained stable.

### Wrapping Up

Clearly, developers are passionate about C# and want to ensure it maintains its excellence. The language team is taking your feedback seriously and modifying the language as it processes what users have to say. Don’t hesitate to visit [roslyn.codeplex.com](https://roslyn.codeplex.com) and let the team know your thoughts. Also, don’t forget to check out [intellitect.com/EssentialCSharp/](/EssentialCSharp/) for updates on C# 6.0 until it’s released.

_Thanks to the following Microsoft technical expert for reviewing this article: Mads Torgersen[](https://docs.microsoft.com/en-us/locale?target=https://docs.microsoft.com/en-us/archive/msdn-magazine/2014/october/csharp-the-new-and-improved-csharp-6-0)._

_This article was originally posted_ [_here_](https://docs.microsoft.com/en-us/archive/msdn-magazine/2014/october/csharp-the-new-and-improved-csharp-6-0) _in the October 2014 issue of MSDN Magazine._
