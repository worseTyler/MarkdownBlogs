
Nullable reference types—what? Aren’t all reference types nullable?

I love C# and I find the careful language design fantastic. Nonetheless, as it currently stands, and even after 7 versions of C#, we still don’t have a perfect language. By that I mean that while it’s reasonable to expect there will likely always be new features to add to C#, there are also, unfortunately, some problems. And, by problems, I don’t mean bugs but, rather, fundamental issues. Perhaps one of the biggest problem areas—and one that’s been around since C# 1.0—surrounds the fact that reference types can be null and, in fact, reference types are null by default. Here are some of the reasons why nullable reference types are less than ideal:

- Invoking a member on a null value will issue a System.NullReferenceException exception, and every invocation that results in a System.NullReferenceException in production code is a bug. Unfortunately, however, with nullable reference types we “fall in” to doing the wrong thing rather than the right thing. The “fall in” action is to invoke a reference type without checking for null.
- There’s an inconsistency between reference types and value types (following the introduction of Nullable<T>) in that value types are nullable when decorated with “?” (for example, int? number); otherwise, they default to non-nullable. In contrast, reference types are nullable by default. This is “normal” to those of us who have been programming in C# for a long time, but if we could do it all over, we’d want the default for reference types to be non-nullable and the addition of a “?” to be an explicit way to allow nulls.
- It’s not possible to run static flow analysis to check all paths regarding whether a value will be null before dereferencing it, or not. Consider, for example, if there were unmanaged code invocations, multi-threading, or null assignment/­replacement based on runtime conditions. (Not to mention whether analysis would include checking of all library APIs that are invoked.)
- There’s no reasonable syntax to indicate that a reference type value of null is invalid for a particular declaration.
- There’s no way to decorate parameters to not allow null.

As already said, in spite of all this, I love C# to the point that I just accept the behavior of null as an idiosyncrasy of C#. With C# 8.0, however, the C# language team is setting out with the intention of improving it. Specifically, they hope to do the following:

- Provide syntax to expect null: Enable the developer to explicitly identify when a reference type is expected to contain nulls—and, therefore, not flag occasions when it’s explicitly assigned null.
- Make default reference types expect non-nullable: Change the default expectation of all reference types to be non-nullable, but do so with an opt-in compiler switch rather than suddenly overwhelm the developer with warnings for existing code.
- Decrease the occurrence of NullReferenceExceptions: Reduce the likelihood of NullReferenceException exceptions by improving the static flow analysis that flags potential occasions where a value hasn’t been explicitly checked for null before invoking one of the value’s members.
- Enable suppression of static flow analysis warning: Support some form of “trust me, I’m a programmer” declaration that allows the developer to override the static flow analysis of the complier and, therefore, suppress any warnings of a possible NullReferenceException.

For the remainder of the article, let’s consider each of these goals and how C# 8.0 implements fundamental support for them within the C# language.

### Provide the Syntax to Expect Null

To begin, there needs to be a syntax for distinguishing when a reference type should expect null and when it shouldn’t. The obvious syntax for allowing null is using the ? as a nullable declaration—both for a value type and a reference type. By including support on reference types, the developer is given a way to opt-in for null with, for example:

string? text = null;

The addition of this syntax explains why the critical nullable improvement is summarized with the seemingly confusing name, “nullable reference types.” It isn’t because there’s some new nullable reference data type, but rather that now there’s explicit—opt-in—support for said data type.

Given the nullable reference types syntax, what’s the non-nullable reference type syntax? While this:

string! text = "Inigo Montoya"

may seem a fine choice, it introduces the question of what is meant by simply:

string text = GetText();

Are we left with three declarations, namely: nullable reference types, non-nullable reference types and I-don’t-know reference types? Ughh, No!!

Instead, what we really want is:

- Nullable reference types: string? text = null;
- Non-nullable reference types: string text = "Inigo Montoya"

This implies, of course, a breaking language change such that reference types with no modifier are non-nullable by default.

### Make Default Reference Types Expect Non-Nullable

Switching standard reference declarations (no nullable modifier) to be non-nullable is perhaps the most difficult of all the requirements to reduce nullable idiosyncrasy. The fact of the matter is that today, string text; results in a reference type called text that allows text to be null, expects text to be null, and, in fact, defaults text to be null in many cases, such as with a field or array. However, just as with value types, reference types that allow null should be the exception—not the default. It would be preferable if when we assigned null to text or failed to initialize text to something other than null, the compiler would flag any dereference of the text variable (the compiler already flags dereferencing a local variable before it’s initialized).

Unfortunately, this means changing the language and issuing a warning when you assign null (string text = null, for example) or assign a nullable reference type (such as string? text = null; string moreText = text;). The first of these (string text = null) is a breaking change. (Issuing a warning for something that previously incurred no warning is a breaking change.) To avoid overwhelming developers with warnings as soon as they start using the C# 8.0 compiler, instead Nullability support will be turned off by default—thus no breaking change. To take advantage of it, therefore, you’ll need to opt-in by enabling the feature. (Note, however, that in the preview available at the time of this writing, Nullability is on by default.)

Of course, once the feature is enabled, the warnings will appear, presenting you with the choice. Choose explicitly whether the reference type is intended to allow nulls, or not. If it’s not, then remove the null assignment, thus removing the warning. However, this potentially introduces a warning later on because the variable isn’t assigned and you’ll need to assign it a non-null value. Alternatively, if null is explicitly intended (representing “unknown” for example), then change the declaration type to be nullable, as in:

string? text = null;

### Decrease the Occurrence of NullReferenceExceptions

Given a way to declare types as either nullable or non-nullable, it’s now up to the compiler’s static flow analysis to determine when the declaration is potentially violated. While either declaring a reference type as nullable or avoiding a null assignment to a non-nullable type will work, new warnings or errors may appear later on in the code. As already mentioned, non-nullable reference types will cause an error later on in the code if the local variable is never assigned (this was true for local variables before C# 8.0). In contrast, the static flow analysis will flag any dereference invocation of a nullable type for which it can’t detect a prior check for null and/or any assignment of the nullable value to a value other than null. **Figure 1** shows some examples.

**Figure 1 Examples of Static Flow Analysis Results**

string text1 = null;
// Warning: Cannot convert null to non-nullable reference
string? text2 = null;
string text3 = text2;
// Warning: Possible null reference assignment
Console.WriteLine( text2.Length ); 
// Warning: Possible dereference of a null reference
if(text2 != null) { Console.WriteLine( text2.Length); }
// Allowed given check for null

Either way, the end result is a decrease in potential NullReference­Exceptions by using static flow analysis to verify a nullable intent.

As discussed earlier, the static flow analysis should flag when a non-nullable type will potentially be assigned null—either directly or when assigned a nullable type. Unfortunately, this isn’t foolproof. For example, if a method declares that it returns a non-nullable reference type (perhaps a library that hasn’t yet been updated with nullability modifiers) or one that mistakenly returns null (perhaps a warning was ignored), or a non-fatal exception occurs and an expected assignment doesn’t execute, it’s still possible that a non-nullable reference type could end up with a null value. That’s unfortunate, but support for nullable reference types should decrease the likelihood of throwing a NullReferenceException, though not eliminate it. (This is analogous to the fallibility of the compiler’s check when a variable is assigned.) Similarly, the static flow analysis won’t always recognize that the code, in fact, does check for null before dereferencing a value. In fact, the flow analysis only checks the nullability within a method body of locals and parameters, and leverages method and operator signatures to determine validity. It doesn’t, for example, delve into the body of a method called IsNullOrEmpty to run analysis on whether that method successfully checks for null such that no additional null check is required.

### Enable Suppression of Static Flow Analysis Warning

Given the possible fallibility of the static flow analysis, what if your check for null (perhaps with a call such as object.ReferenceEquals(s, null) or string.IsNullOrEmpty()) is not recognized by the compiler? When the programmer knows better that a value isn’t going to be null, they can dereference following the ! operator (for example, text!) as in:

string? text;...
if(object.ReferenceEquals(text, null))
{  var type = text!.GetType()
}

Without the exclamation point, the compiler will warn of a possible null invocation. Similarly, when assigning a nullable value to a non-nullable value you can decorate the assigned value with an exclamation point to inform the compiler that you, the programmer, know better:

string moreText = text!;

In this way, you can override the static flow analysis just like you can use an explicit cast. Of course, at runtime the appropriate verification will still occur.

### Wrapping Up

The introduction of the nullability modifier for reference types doesn’t introduce a new type. Reference types are still nullable and compiling string? results in IL that’s still just System.String. The difference at the IL level is the decoration of nullable modified types with an attribute of:

System.Runtime.CompilerServices.NullableAttribute

In so doing, downstream compiles can continue to leverage the declared intent. Furthermore, assuming the attribute is available, earlier versions of C# can still reference C# 8.0-compiled libraries—albeit without any nullability improvements. Most important, this means that existing APIs (such as the .NET API) can be updated with nullable metadata without breaking the API. In addition, it means there’s no support for overloading based on the nullability modifier.

There’s one unfortunate consequence to enhancing null handling in C# 8.0. The transition of traditionally nullable declarations to non-nullable will initially introduce a significant number of warnings. While this is unfortunate, I believe that a reasonable balance has been maintained between irritation and improving one’s code:

Warning you to remove a null assignment to a non-nullable type potentially eliminates a bug because a value is no longer null when it shouldn’t be.

Alternatively, adding a nullable modifier improves your code by being more explicit about your intent.

Over time the impedance mismatch between nullable updated code and older code will dissolve, decreasing the NullReferenceException bugs that used to occur.

The nullability feature is off by default on existing projects so you can delay dealing with it until a time of your choosing. In the end you have more robust code. For cases where you know better than the compiler, you can use the ! operator (declaring, “Trust me, I’m a programmer.”) like a cast.

#### Further Enhancements in C# 8.0

There are three main additional areas of enhancement under consideration for C# 8.0:

Async Streams: Support for asynchronous streams enables await syntax to iterate over a collection of tasks (Task<bool>). For example, you can invoke

foreach await (var data in asyncStream)

and the thread will not block for any statements following the await, but will instead “continue with” them once the iterating completes. And the iterator will yield the next item upon request (the request is an invocation of Task<bool> MoveNextAsync on the enumerable stream’s iterator) followed by a call to T Current { get; }.

Default Interface Implementations: With C# you can implement multiple interfaces such that the signatures of each interface are inherited. Furthermore, it’s possible to provide a member implementation in a base class so that all derived classes have a default implementation of the member. Unfortunately, what’s not possible is to implement multiple interfaces and also provide default implementations of the interface—that is, multiple inheritance. With the introduction of default interface implementations, we overcome the restriction. Assuming a reasonable default implementation is possible, with C# 8.0 you’ll be able to include a default member implementation (properties and methods only) and all classes implementing the interface will have a default implementation. While multiple inheritance might be a side benefit, the real improvement this provides is the ability to extend interfaces with additional members without introducing a breaking API change. You could, for example, add a Count method to IEnumerator<T> (though implementing it would require iterating over all the items in the collection) without breaking all classes that implemented the interface. Note that this feature requires a corresponding framework release (something that hasn’t been required since C# 2.0 and generics).

Extension everything: With LINQ came the introduction of extension methods. I recall having dinner with Anders Hejlsberg at the time and asking about other extension types, such as properties. Mr. Hejlsberg informed me that the team was only considering what was needed for implementing LINQ. Now, 10 years later, that assumption is being re-evaluated and they are considering the addition of extension methods for not only properties, but also for events, operators and even potentially constructors (the latter opens up some intriguing factory pattern implementations). The one important point to note—especially when it comes to properties, is that the extension methods are implemented in static classes and, therefore, there’s no additional instance state for the extended type introduced. If you required such state, you’d need to store it in a collection indexed by the extended type instance, in order to retrieve the associated state.

_Thanks to the following Microsoft technical experts for reviewing this article: Kevin Bost, Grant Ericson, Tom Faust, Mads Torgersen_.

_This article was originally posted [here](https://docs.microsoft.com/en-us/archive/msdn-magazine/2018/february/essential-net-csharp-8-0-and-nullable-reference-types) in the February 2018 issue of MSDN Magazine._
