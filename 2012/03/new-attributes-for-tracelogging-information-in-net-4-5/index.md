

## New Attributes for Trace Logging in NET 4.5
#
There is a new set of .NET attributes in .NET 4.5 to help with gathering trace information.  Before .NET 4.5, tracing the line number, member name, and source file name required using the stack trace in combination with the PDB files.  In .NET 4.5, however, there is a new mechanism for doing this using attributes on optional parameters.  Consider the code below:

```csharp
using System;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
 
namespace IntelliTrace.Runtime.CompilerServices
{
    public class Tracer
    {
        static public void Write(string message, [CallerLineNumber] int lineNumer = 0, 
            [CallerMemberName] string memberName = null, [CallerFilePath] string fileName = null)
        {
            Assert.AreEqual<int>(CallerInfoTests.LineNumber, lineNumer);
            Assert.AreEqual<string>("CallWrite_WithNoParameters_DefaultAreSetToCallerInfo", memberName);
            Assert.AreEqual<string>("CallerInfoTests.cs", Path.GetFileName(fileName));
        }
    }
    
    [TestClass]
    public class CallerInfoTests
    {
        public const int LineNumber = 27;
 
        [TestMethod]
        public void CallWrite_WithNoParameters_DefaultAreSetToCallerInfo()
        {
            Tracer.Write("Test message");
        }
 
        [TestMethod]
        public void CallWrite_WithNoParameters_DefaultsAreSetToCallerInfoExceptForSpecifiedLineNuber()
        {
            Tracer.Write("Test message", LineNumber, "CallWrite_WithNoParameters_DefaultAreSetToCallerInfo");
        }
    }
}
```

 

Note the ```static public void Write(string message, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = null, [CallerFilePath] string fileName = null)``` method signature and how it uses the parameter attributes ```CallerLineNumber```,```CallerMemberName```, and ```CallerFilePath``` from the ```System.Runtime.CompilerServices``` namespace.  By placing the "CallerInfo" attributes on the parameters - which must also be set as default parameters, the parameters are set with corresponding values automatically (ignoring the default attributes specified in the method signature). As a result, the ```Tracer.Write()``` method then has access to "stack" information at compile time, rather than having to iterate over the stack at runtime (possibly a significant performance hit depending on the frequency).

The data associated with the CallerInfo attributes is injected into the IL code at compile time. Decompiling the IL code reveals the caller invocation sets the optional parameters as follows:

```csharp
Tracer.Write("Test message", 0x1b, 
     "CallWrite_WithNoParameters_DefaultAreSetToCallerInfo", 
     @"c:\\Data\\SCC\\CallerInfoTracing\\CallerInfoTests.cs");
```
As a result, if no parameter is specified explicitly in the code by the programmer, the compiler injects a default value corresponding to the attribute specified - at the callee (embedding the full file name for the fileName parameter).

**#BrilliantlySimple**
