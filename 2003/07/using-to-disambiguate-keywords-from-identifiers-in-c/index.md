

While reading the C# Language Specification today I noticed the use of the @ sign to disambiguate a keyword from an identifier.  When you do this the identifier is called a _verbatim identifier_.  So, for example, you could write code as follows:

```csharp
class @class {
  static void Main() {
    @static(false);
  }
  public static void @static(bool @bool) {
    if (@bool) System. ** Console ** .WriteLine("true");
    else System. ** Console ** .WriteLine("false");
  }
}
```

Why would you want to do this you ask?  Well, what happens if you use an assembly that has a C# keyword for a public name because that keyword did not happen to be a keyword in the original language of the assembly.   That would prevent you from calling this assembly perhaps unless there was a way to disambiguate.

Another place I would be very tempted is in the name of the variable that is returned from a function.  Often it is difficult to come up with a variable name.  "return" would be great but it is a keyword so you have to resort to "ret," which is an abbreviation or perhaps result, which just isn't quite the same.  Now I can just use @return.

```csharp
public string GetName() {
  string @return; ** Console ** .Write("Enter your name:");
  do {
    @return = ** Console ** .ReadLine();
  } while (@return.Length == 0);
  return @return;
}
```

Cool!  How 'bout making this a coding standard?

I am confident this idea will get lambasted but I still like it... so there!
