

## Why to Not Use the System Diagnostics Assert Methods
#
Assertions should never appear in release code.  Assertions are a debug mechanism for revealing bugs within code during development.  Failed assertions in release code indicate the bug was missed and allow the option of debugging at the assertion location.  _However, end users should not be presented with dialogs for debugging an application.  Therefore,_ _``` System.Diagnostics.**Trace**.Assert() ``` should be treated as obsolete._

One advantage of ``` Debug.Assert() ``` is that the C# and VB compilers (not C++) eliminate the ``` Debug.Assert() ``` statement from the CIL code when the ``` DEBUG ``` precompile constant is not defined.  Therefore, even the conditional check in ``` Debug.Assert() ``` will never appear in a release builds.  In other words, the statement

``` System.Diagnostics.Debug.Assert( ReallyLongRunningMethodThatReturnsBool() ); ```

will not even call ``` ReallyLongRunningMethodThatReturnsBool() ``` when ``` DEBUG ``` is defined.  Therefore, use ``` Debug.Assert() ``` for expensive conditionals and replace the code in release builds with tests that explicitly check that the false conditional didn't occur.  ``` **Debug**.Assert() ``` is useful for when the condition specified is a long running/expensive method and the compiler is C#, Visual Basic.NET, or some compiler that pays attention to the ``` ConditionalAttribute ``` (the C++ CLR compiler does not).

Another advantage of the assert is that it stops execution at the exact location of the assert, rather than bubbling up to the exception handler or as an unhandled exception.  However, we still don't want the assertion situation (the bad situation) to be ignored in release code, which is what would happen with ``` Debug.Assert() ```.  Therefore, if the conditional is false, it should be reported as an exception not an assertion in release code.

**Conclusion:**

Don't use assertions unless:

1. The assertion conditional is expensive and you have unit tests in place that check the false assertion condition doesn't occur or
2. You plan on a search and replace for them before releasing.

Otherwise, give up on assertion methods and use exceptions in both release and debug builds.
