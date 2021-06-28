
Another C# 6.0 “syntactic sugar” feature is the introduction of _using_ _static_.  With this feature, it’s possible to eliminate an explicit reference to the type when invoking a static method.  Furthermore, using static lets you introduce only the extension methods on a specific class, rather than all extension methods within a namespace.  The code of **Listing 1** below provides a “Hello World” example of using static on System.Console.

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
            WriteLine("Hello, my name is Inigo Montoya...  Who are you?: ");
            ForegroundColor = ConsoleColor.Green;
            string name = ReadLine(); // Respoond: No one of consequence
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

In this example, the Console qualifier was dropped a total of 9 times.  Admittedly, the example is contrived, but even so, the point is clear.  Frequently a type prefix on a static member (including properties) doesn’t add significant value and eliminating it results in code that’s easier to write and read.

Although not working in the March Preview, a second (planned) feature of using static is under discussion.  This feature is support for importing only extension methods of a specific type.  Consider, for example, a “utility” namespace that includes numerous static types with extension methods.  Without using static, all (or no) extension methods in that namespace are imported.  With using static, however, it’s possible to pinpoint the available extension methods to a specific type—not to the more general namespace.  As a result, you could call a LINQ standard query operator by just specifying “using System.Linq.Enumerable;” instead of the entire System.Linq namespace.

Unfortunately, this advantage isn’t always available (at least in the March Preview) because only static types support using static, which is why, for example, there’s no “using System.ConsoleColor” statement in **Listing 1**.  Given the current preview nature of C# 6.0 , whether the restriction will remain is still under review.  What do you think?

See [A C# 6.0 Language Preview](https://msdn.microsoft.com/en-us/magazine/dn683793.aspx) for the full article text.
