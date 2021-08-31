

## C# Language Overview 
#
By the time you read this, Build—the Microsoft developer conference—will be over and developers will be thinking about how to respond to all that was presented: embrace immediately, watch with slight trepidation or ignore for the moment. For .NET/C# developers, the most significant announcement was undoubtedly the release of the next version of the C# compiler (“Roslyn”) as open source. Related to that are the language improvements themselves. Even if you don’t have plans to immediately adopt C# vNext—which I’ll refer to henceforth and unofficially as C# 6.0—at a minimum, you should be aware of its features and take note of those that might make it worth jumping right in.

In this article, I’ll delve into the details of what’s available in C# 6.0 at the time of this writing (March 2014) or in the now open source bits downloadable from [roslyn.codeplex.com](https://roslyn.codeplex.com). I’ll refer to this as a single release that I’ll term the March Preview. The C#-specific features of this March Preview are implemented entirely in the compiler, without any dependency on an updated Microsoft .NET Framework or runtime. This means you can adopt C# 6.0 in your development without having to upgrade the .NET Framework for either development or deployment. In fact, installing the C# 6.0 compiler from this release involves little more than installing a Visual Studio 2013 extension, which in turn updates the MSBuild target files.

As I introduce each C# 6.0 feature, you might want to consider the following:

- Was there a reasonable means of coding the same functionality in the past, such that the feature is mostly syntactic sugar—a short cut or streamlined approach? Exception filtering, for example, doesn’t have a C# 5.0 equivalent, while primary constructors do.
- Is the feature available in the March Preview? Most features I’ll describe are available, but some (such as a new binary literal) are not.
- Do you have any feedback for the team regarding the new language feature? The team is still relatively early in its release lifecycle and very interested in hearing your thoughts about the release (see msdn.com/Roslyn for feedback instructions).

Thinking through such questions can help you gauge the significance of the new features in relation to your own development efforts.

### Indexed Members and Element Initializers

To begin, consider the unit test in **Figure 1**.

**Figure 1 Assigning a Collection via a Collection Initializer (Added in C# 3.0)**

```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
// ...
[TestMethod]
public void DictionaryIndexWithoutDotDollar()
{
  Dictionary<string, string> builtInDataTypes = 
    new Dictionary<string, string>()
  {
    {"Byte", "0 to 255"},
    // ...
    {"Boolean", "True or false."},
    {"Object", "An Object."},
    {"String", "A string of Unicode characters."},
    {"Decimal", "±1.0 × 10e-28 to ±7.9 × 10e28"}
  };
  Assert.AreEqual("True or false.", builtInDataTypes["Boolean"]);
}
```

Although it’s somewhat obscured by the syntax, **Figure 1** is nothing more than a name-value collection. As such, the syntax could be significantly cleaner: <index> = <value>. C# 6.0 makes this possible through the C# object initializers and a new index member syntax. The following shows int-based element initializers:

```csharp
var cppHelloWorldProgram = new Dictionary<int, string>
{
  [10] = "main() {",
  [20] = "    printf(\"hello, world\")",
  [30] = "}"
};
Assert.AreEqual(3, cppHelloWorldProgram.Count);
```

Note that although this code uses an integer for the index, Dictionary<TKey,TValue> can support any type as an index (as long as it supports IComparable<T>). The next example presents a string for the index data type and uses an indexed member initializer to specify element values:

```csharp
Dictionary<string, string> builtInDataTypes =
  new Dictionary<string, string> {
    ["Byte"] = "0 to 255",
    // ...
    // Error: mixing object initializers and
    // collection initializers is invalid
    // {" Boolean", "True or false."},
    ["Object"] = "An Object.",
    ["String"] = "A string of Unicode characters.",
    ["Decimal"] = "±1.0 × 10e?28 to ±7.9 × 10e28"
  };
```

Accompanying the new index member initialization is a new $ operator. This string indexed member syntax is specifically provided to address the prevalence of string-based indexing. With this new syntax, shown in **Figure 2**, it’s possible to assign element values in syntax much more like in dynamic member invocation (introduced in C# 4.0) than the string notation used in the preceding example.

**Figure 2 Initializing a Collection with an Indexed Member Assignment as Part of the Element Initializer**

```csharp
[TestMethod]
public void DictionaryIndexWithDotDollar()
{
  Dictionary<string, string> builtInDataTypes = 
    new Dictionary<string, string> {
    $Byte = "0 to 255",   // Using indexed members in element initializers
    // ...
    $Boolean = "True or false.",
    $Object = "An Object.",
    $String = "A string of Unicode characters.",
    $Decimal = "±1.0 × 10e?28 to ±7.9 × 10e28"
  };
  Assert.AreEqual("True or false.", builtInDataTypes.$Boolean);
}
```

To understand the $ operator, take a look at the AreEqual function call. Notice the Dictionary member invocation of “$Boolean” on the builtInDataTypes variable—even though there’s no “Boolean” member on Dictionary. Such an explicit member isn’t required because the $ operator invokes the indexed member on the dictionary, the equivalent of calling buildInDataTypes["Boolean"].

As with any string-based operator, there’s no compile-time verification that the string index element (for example, “Boolean”) exists in the dictionary. As a result, any valid C# (case-sensitive) member name can appear after the $ operator.

To fully appreciate the syntax of indexed members, consider the predominance of string indexers in loosely typed data formats such as XML, JSON, CSV and even database lookups (assuming no Entity Framework code-generation magic). **Figure 3**, for example, demonstrates the convenience of the string indexed member using the Newtonsoft.Json framework.

**Figure 3 Leveraging the Indexed Method with JSON Data**

```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
// ...
[TestMethod]
public void JsonWithDollarOperatorStringIndexers()
{
  // Additional data types eliminated for elucidation
  string jsonText = @"
    {
      'Byte':  {
        'Keyword':  'byte',
        'DotNetClassName':  'Byte',
        'Description':  'Unsigned integer',
        'Width':  '8',
        'Range':  '0 to 255'
                },
      'Boolean':  {
        'Keyword':  'bool',
        'DotNetClassName':  'Boolean',
        'Description':  'Logical Boolean type',
        'Width':  '8',
        'Range':  'True or false.'
                  },
    }";
  JObject jObject = JObject.Parse(jsonText);
  Assert.AreEqual("bool", jObject.$Boolean.$Keyword);
}
```

One final point to note, just in case it’s not already obvious, is that the $ operator syntax works only with indexes that are of type string (such as Dictionary<string, ...>).

### Auto-Properties with Initializers

Initializing a class today can be cumbersome at times. Consider, for example, the trivial case of a custom collection type (such as Queue<T>) that internally maintains a private System.Collections.Generic.List<T> property for a list of items. When instantiating the collection, you have to initialize the queue with the list of items it is to contain. However, the reasonable options for doing so with a property require a backing field along with an initializer or an else constructor, the combination of which virtually doubles the amount of required code.

With C# 6.0, there’s a syntax shortcut: auto-property initializers. You can now assign to auto-properties directly, as shown here:

```csharp
class Queue<T>
{
  private List<T> InternalCollection { get; } = 
    new List<T>; 
  // Queue Implementation
  // ...
}
```

Note that in this case, the property is read-only (no setter is defined). However, the property is still assignable at declaration time. A read/write property with a setter is also supported.

### Primary Constructors

Along the same lines as property initializers, C# 6.0 provides syntactic shortcuts for the definition of a constructor. Consider the prevalence of the C# constructor and property validation shown in **Figure 4**.

**Figure 4 A Common Constructor Pattern**

```csharp
[Serializable]
public class Patent
{
  public Patent(string title , string yearOfPublication)
  {
    Title = title;
    YearOfPublication = yearOfPublication;
  }
  public Patent(string title, string yearOfPublication,
    IEnumerable<string> inventors)
    : this(title, yearOfPublication)
  {
    Inventors = new List<string>();
    Inventors.AddRange(inventors);
  }
  [NonSerialized] // For example
  private string _Title;
  public string Title
  {
    get
    {
      return _Title;
    }
    set
    {
      if (value == null)
      {
        throw new ArgumentNullException("Title");
      }
      _Title = value;
    }
  }
  public string YearOfPublication { get; set; }
  public List<string> Inventors { get; private set; }
  public string GetFullName()
  {
    return string.Format("{0} ({1})", Title, YearOfPublication);
  }
}
```

There are several points to note from this common constructor pattern:

1. The fact that a property requires validation forces the underlying property field to be declared.
2. The constructor syntax is somewhat verbose with the all-too-common public class Patent{ public Patent(… repetitiveness.
3. “Title,” in various versions of case sensitivity, appears seven times for a fairly trivial scenario—not including the validation.
4. The initialization of a property requires explicit reference to the property from within the constructor.

To remove some of the ceremony around this pattern, without losing the flavor of the language, C# 6.0 introduces property initializers and primary constructors, as shown in **Figure 5**.

**Figure 5 Using a Primary Constructor**

```csharp
[Serializable]
public class Patent(string title, string yearOfPublication)
{
  public Patent(string title, string yearOfPublication,
    IEnumerable<string> inventors)
    :this(title, yearOfPublication)
  {
    Inventors.AddRange(inventors);
  }
  private string _Title = title;
  public string Title
  {
    get
    {
      return _Title;
    }
    set
    {
      if (value == null)
      {
        throw new ArgumentNullException("Title");
      }
      _Title = value;
    }
  }
  public string YearOfPublication { get; set; } = yearOfPublication;
  public List<string> Inventors { get; } = new List<string>();
  public string GetFullName()
  {
    return string.Format("{0} ({1})", Title, YearOfPublication);
  }
}
```

In combination with property initializers, primary constructor syntax simplifies C# constructor syntax:

- Auto-properties, whether read-only (see the Inventors property with only a getter) or read-write, (see the YearOfPublication property with both a setter and a getter), support property initialization such that the initial value of the property can be assigned as part of the property declaration. The syntax matches what’s used when assigning fields a default value at declaration time (declaration assigned _Title, for example).
- By default, primary constructor parameters aren’t accessible outside of an initializer. For example, there’s no yearOfPublication field declared on the class.
- When leveraging property initializers on read-only properties (getter only), there’s no way to provide validation. (This is due to the fact that in the underlying IL implementation, the primary constructor parameter is assigned to the backing field. Also noteworthy is the fact that the backing field will be defined as read-only in the IL if the auto-property has only a getter.)
- If specified, the primary constructor will (and must) always execute last in the constructor chain (therefore, it can’t have a this(…) initializer).

For another example, consider the declaration of a struct, which guidelines indicate should be immutable. The following shows a property-based implementation (versus the atypical public field approach):

```csharp
struct Pair(string first, string second, string name)
{
  public Pair(string first, string second) : 
    this(first, second, first+"-"+second)
  {
  }
  public string First { get; } = second;
  public string Second { get; } = first;
  public string Name { get; } = name;
  // Possible equality implementation
  // ...
}
```

Note that in the implementation of Pair, there’s a second constructor that invokes the primary constructor. In general, all struct constructors must—either directly or indirectly—invoke the primary constructor via a call to the this(…) initializer. In other words, it isn’t necessary that all constructors call the primary constructor directly, but that at the end of the constructor chain the primary constructor is called. This is necessary because it’s the primary constructor that calls the base constructor initializer and doing so provides a little protection against some common initialization mistakes. (Note that, as was true in C# 1.0, it’s still possible to instantiate a struct without invoking a constructor. This, for example, is what happens when an array of the struct is instantiated.)

Whether the primary constructor is on a custom struct or class data type, the call to the base constructor is either implicit (therefore invoking the base class’s default constructor) or explicit, by calling a specific base class constructor. In the latter case, for a custom exception to invoke a specific System.Exception constructor, the target constructor is specified after the primary constructor:

```csharp
class UsbConnectionException : 
  Exception(string message, Exception innerException,
  HidDeviceInfo hidDeviceInfo) :base(message, innerException)
{
  public HidDeviceInfo HidDeviceInfo { get;  } = hidDeviceInfo;
}
```

One detail to be aware of regarding primary constructors relates to avoiding duplicate, potentially incompatible, primary constructors on partial classes: Given multiple parts of a partial class, only one class declaration can define the primary constructor and, similarly, only this primary constructor can specify the base constructor invocation.

There’s one significant caveat to consider in regard to primary constructors as they’re implemented in this March Preview: There’s no way to provide validation to any of the primary constructor parameters. And, because property initializers are only valid for auto-properties, there’s no way to implement validation in the property implementation, either, which potentially exposes public property setters to the assignment of invalid data post instantiation, as well. The obvious workaround for the moment is to not use the primary constructor feature when validation is important.

Although somewhat tentative at the moment, there’s a related feature called the field parameter under consideration. The inclusion of an access modifier in the primary constructor parameter (such as private string title) will cause the parameter to be captured into class scope as a field with the name of title—matching the name and casing of the parameter). As such, title is available from within the Title property or any other instance class member. Furthermore, the access modifier allows the entire field syntax to be specified—including additional modifiers such as readonly, or even attributes like these:

```csharp
public class Person(
  [field: NonSerialized] private string firstName, string lastName)
```

Note that without the access modifier, other modifiers (including attributes) aren’t allowed. It’s the access modifier that indicates the field declaration is to occur in-line with the primary constructor.

(The bits available to me at the time of this writing didn’t include the field parameter implementation, but I am assured by the language team they will be included in the Microsoft Build version, so you should be able to try out field parameters by the time you read this. Given the relative “freshness” of this feature, however, don’t hesitate to provide feedback at msdn.com/Roslyn so it can be considered before the process is too far along for changes.)

### Static Using Statements

Another C# 6.0 “syntactic sugar” feature is the introduction of using static. With this feature, it’s possible to eliminate an explicit reference to the type when invoking a static method. Furthermore, using static lets you introduce only the extension methods on a specific class, rather than all extension methods within a namespace. **Figure 6** provides a “Hello World” example of using static on System.Console.

**Figure 6 Simplifying Code Clutter with Using Static**

```csharp
using System;
using System.Console;
public class Program
{
  private static void Main()
  {
    ConsoleColor textColor = ForegroundColor;
    try
    {
      ForegroundColor = ConsoleColor.Red;
      WriteLine("Hello, my name is Inigo Montoya... Who are you?: ");
      ForegroundColor = ConsoleColor.Green;
      string name = ReadLine(); // Respond: No one of consequence
      ForegroundColor = ConsoleColor.Red;
      WriteLine("I must know.");
      ForegroundColor = ConsoleColor.Green;
      WriteLine("Get used to disappointment");
    }
    finally
    {
      ForegroundColor = textColor;
    }
  }
}
```

In this example, the Console qualifier was dropped a total of nine times. Admittedly, the example is contrived, but even so, the point is clear. Frequently a type prefix on a static member (including properties) doesn’t add significant value and eliminating it results in code that’s easier to write and read.

Although not working in the March Preview, a second (planned) feature of using static is under discussion. This feature is support for importing only extension methods of a specific type. Consider, for example, a utility namespace that includes numerous static types with extension methods. Without using static, all (or no) extension methods in that namespace are imported. With using static, however, it’s possible to pinpoint the available extension methods to a specific type—not to the more general namespace. As a result, you could call a LINQ standard query operator by just specifying using System.Linq.Enumerable; instead of the entire System.Linq namespace.

Unfortunately, this advantage isn’t always available (at least in the March Preview) because only static types support using static, which is why, for example, there’s no using System.ConsoleColor statement in **Figure 6**. Given the current preview nature of C# 6.0, whether the restriction will remain is still under review. What do you think?

### Declaration Expressions

It’s not uncommon that in the midst of writing a statement, you find you need to declare a variable specifically for that statement. Consider two examples:

- When coding an int.TryParse statement, you realize you need to have a variable declared for the out argument into which the parse results will be stored.
- While writing a for statement, you discover the need to cache a collection (such as a LINQ query result) to avoid re-executing the query multiple times. In order to achieve this, you interrupt the thought process of writing the for statement to declare a variable.

To address these and similar annoyances, C# 6.0 introduces declaration expressions. This means you don’t have to limit variable declarations to statements only, but can use them within expressions, as well. **Figure 7** provides two examples.

**Figure 7 Declaration Expression Examples**

```csharp
public string FormatMessage(string attributeName)
{
  string result;
  if(! Enum.TryParse<FileAttributes>(attributeName, 
    out var attributeValue) )
  {
    result = string.Format(
      "'{0}' is not one of the possible {2} option combinations ({1})",
      attributeName, string.Join(",", string[] fileAtrributeNames =
      Enum.GetNames(typeof (FileAttributes))),
      fileAtrributeNames.Length);
  }
  else
  {
    result = string.Format("'{0}' has a corresponding value of {1}",
      attributeName, attributeValue);
  }
  return result;
}
```

In the first highlight in **Figure 7**, the attributeValue variable is declared in-line with the call to Enum.TryParse rather than in a separate declaration beforehand. Similarly, the declaration of file­AttributeNames appears on the fly in the call to string.Join. This enables access to the Length later in the same statement. (Note that the fileAttributeNames.Length is substitution parameter {2} in the string.Format call, even though it appears earlier in the format string—thus enabling fileAttributeNames to be declared prior to accessing it.)

The scope of a declaration expression is loosely defined as the scope of the statement in which the expression appears. In **Figure 7**, the scope of attributeValue is that of the if-else statement, making it accessible both in the true and false blocks of the conditional. Similarly, fileAttributeNames is available only in the first half of the if-statement, the portion matching the scope of the string.Format statement invocation.

Wherever possible the compiler will enable the use of implicitly typed variables (var) for the declaration, inferring the data type from the initializer (declaration assignment). However, in the case of out arguments, the signature of the call target can be used to support implicitly typed variables even if there’s no initializer. Still, inference isn’t always possible and, furthermore, it may not be the best choice from a readability perspective. In the TryParse case in **Figure 7**, for example, var works only because the type argument (FileAttributes) is specified. Without it, a var declaration wouldn’t compile and instead the explicit data type would be required:

```
Enum.TryParse(attributeName, out FileAttributes attributeValue)
```

In the second declaration expression example in **Figure 7**, an explicit declaration of string[] appears to identify the data type as an array (rather than a List<string>, for example). The guideline is standard to the general use of var: Consider avoiding implicitly typed variables when the resulting data type isn’t obvious.

The declaration expression examples in **Figure 7** could all be coded by simply declaring the variables in a statement prior to their assignment.

### Exception-Handling Improvements

There are two new exception-handling features in C# 6.0. The first is an improvement in the async and await syntax and the second is support for exception filtering.

When C# 5.0 introduced the async and await (contextual) keywords, developers gained a relatively easy way to code the Task-based Asynchronous Pattern (TAP) in which the compiler takes on the laborious and complex work of transforming C# code into an underlying series of task continuations. Unfortunately, the team wasn’t able to include support for using await from within catch and finally blocks in that release. As it turned out, the need for such an invocation was even more common than initially expected. Thus, C# 5.0 coders had to apply significant workarounds (such as leveraging the awaited pattern). C# 6.0 does away with this deficiency, and now allows await calls within both catch and finally blocks (they were already supported in try blocks), as shown in **Figure 8**.

**Figure 8 Await Calls from Within a Catch Block**

```csharp
try
{
  WebRequest webRequest =
    WebRequest.Create("https://IntelliTect.com");
  WebResponse response =
    await webRequest.GetResponseAsync();
  // ...
}
catch (WebException exception)
{
  await WriteErrorToLog(exception);
}
```

The other exception improvement in C# 6.0—support for exception filters—brings the language up-to-date with other .NET languages, namely Visual Basic .NET and F#. **Figure 9** shows the details of this feature.

**Figure 9 Leveraging Exception Filters to Pinpoint Which Exception to Catch**

```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using System.Runtime.InteropServices;
// ...
[TestMethod][ExpectedException(typeof(Win32Exception))]
public void ExceptionFilter_DontCatchAsNativeErrorCodeIsNot42()
{
  try
  {
    throw new Win32Exception(Marshal.GetLastWin32Error());
  }
  catch (Win32Exception exception) 
    if (exception.NativeErrorCode == 0x00042)
  {
    // Only provided for elucidation (not required).
    Assert.Fail("No catch expected.");
  }
}
```

Notice the additional if expression that follows the catch expression. The catch block now verifies that not only is the exception of type Win32Exception (or derives from it), but also verifies additional conditions—the particular value of the error code in this example. In the unit test in **Figure 9**, the expectation is the catch block will not catch the exception—even though the exception type matches—instead, the exception will escape and be handled by the ExpectedException attribute on the test method.

Note that unlike some of the other C# 6.0 features discussed earlier (such as the primary constructor), there was no equivalent alternate way of coding exception filters prior to C# 6.0. Until now, the only approach was to catch all exceptions of a particular type, explicitly check the exception context, and then re-throw the exception if the current state wasn’t a valid exception-catching scenario. In other words, exception filtering in C# 6.0 provides functionality that hitherto wasn’t equivalently possible in C#.

### Additional Numeric Literal Formats

Though it’s not yet implemented in the March Preview, C# 6.0 will introduce a digit separator, the underscore (_), as a means of separating the digits in a numerical literal (decimal, hex or binary). The digits can be broken into whatever grouping makes sense for your scenario. For example, the maximum value of an integer could be grouped into thousands:

```csharp
int number = 2_147_483_647;
```

The result makes it clearer to see the magnitude of a number, whether decimal, hex or binary.

The digit separator is likely to be especially helpful for the new C# 6.0 numeric binary literal. Although not needed in every program, the availability of a binary literal could improve maintainability when working with binary logic or flag-based enums. Consider, for example, the FileAttribute enum shown in **Figure 10**.

**Figure 10 Assigning Binary Literals for Enum Values**

```csharp
[Serializable][Flags]
[System.Runtime.InteropServices.ComVisible(true)]
public enum FileAttributes
{
  ReadOnly =          0b00_00_00_00_00_00_01, // 0x0001
  Hidden =            0b00_00_00_00_00_00_10, // 0x0002
  System =            0b00_00_00_00_00_01_00, // 0x0004
  Directory =         0b00_00_00_00_00_10_00, // 0x0008
  Archive =           0b00_00_00_00_01_00_00, // 0x0020
  Device =            0b00_00_00_00_10_00_00, // 0x0040
  Normal =            0b00_00_00_01_00_00_00, // 0x0080
  Temporary =         0b00_00_00_10_00_00_00, // 0x0100
  SparseFile =        0b00_00_01_00_00_00_00, // 0x0200
  ReparsePoint =      0b00_00_10_00_00_00_00, // 0x0400
  Compressed =        0b00_01_00_00_00_00_00, // 0x0800
  Offline =           0b00_10_00_00_00_00_00, // 0x1000
  NotContentIndexed = 0b01_00_00_00_00_00_00, // 0x2000
  Encrypted =         0b10_00_00_00_00_00_00  // 0x4000
}
```

Now, with binary numeric literals, you can show more clearly which flags are set and not set. This replaces the hex notation shown in comments or the compile time calculated shift approach:

```csharp
Encrypted = 1<<14.
```

(Developers eager to try this feature immediately can do so in Visual Basic .NET with the March Preview release.)

### Wrapping Up

In considering only these language changes, you’ll notice there’s nothing particularly revolutionary or earth-shattering in C# 6.0. If you compare it to other significant releases, like generics in C# 2.0, LINQ in C# 3.0 or TAP in C# 5.0, C# 6.0 is more of a “dot” release than a major one. (The big news being the compiler has been released as open source.) But just because it doesn’t revolutionize your C# coding doesn’t mean it hasn’t made real progress in eliminating some coding annoyances and inefficiencies that, once in your quiver of everyday use, you’ll quickly take for granted. The features that rank among my particular favorites are the $ operator (string index members), primary constructors (without field parameters), using static and declaration expressions. I expect each of these to quickly become the default in my coding, and likely even added into coding standards in some cases.

_Thanks to the following Microsoft technical expert for reviewing this article: Mads Torgersen._

_This article was originally posted_ [_here_](https://docs.microsoft.com/en-us/archive/msdn-magazine/2014/may/csharp-a-csharp-6-0-language-preview) _in the May 2014 issue of MSDN Magazine._
