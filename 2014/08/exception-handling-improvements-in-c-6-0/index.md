## Exception-Handling Features in C# 6.0
#
There are two new exception-handling features in C# 6.0.  The first is an improvement in the async and await syntax and the second is support for exception filtering.

When C# 5.0 introduced the async and await (contextual) keywords, developers gained a relatively easy way to code the Task-based Asynchronous Pattern (TAP) in which the compiler takes on the laborious and complex work of transforming C# code into an underlying series of task continuations.  Unfortunately, the team wasn’t able to include support for using await from within catch and finally blocks in that release.   As it turned out, the need for such an invocation was even more common than initially expected.  Thus, C# 5.0 coders had to apply significant workarounds (such as leveraging the awaiter pattern).  C# 6.0 does away with this deficiency, however, and now allows await calls within both catch and finally blocks (they were already supported in try blocks), as shown below.

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

The other exception improvement in C# 6.0—support for exception filters—brings the language up to date with other .NET languages, namely Visual Basic.NET and F#. Here's an example.

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
    catch (Win32Exception exception) if (exception.NativeErrorCode == 0x00042)
    {
        // Only provided for elucidation (not required).
        Assert.Fail("No catch expected."); 
    }
}
```

Notice the additional if expression that follows the catch expression.  The catch block now verifies that not only is the exception of type Win32Exception (or derives from it) but also verifies additional conditions—the particular value of the error code in this example.  In the unit test above, the expectation is the catch block will not catch the exception—even though the exception type matches—and instead, the exception will escape and be handled by the ExpectedException attribute on the test method.

Note that unlike some of the other C# 6.0 features discussed earlier (such as the primary constructor), there was no equivalent alternate way of coding exception filters prior to C# 6.0.  Until now, the only approach was to catch all exceptions of a particular type, explicitly check the exception context, and then re-throw the exception if the current state was not a valid exception-catching scenario.  In other words, C# 6.0’s exception filtering provides functionality that was hitherto not equivalently possible in C#.

See [A C# 6.0 Language Preview](https://msdn.microsoft.com/en-us/magazine/dn683793.aspx) for the full article text.
