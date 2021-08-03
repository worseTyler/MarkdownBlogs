

Welcome to the inaugural Essential .NET column. It’s here where you’ll be able to follow all that is happening in the Microsoft .NET Framework world, whether it’s advances in C# vNext (currently C# 7.0), improved .NET internals, or happenings on the Roslyn and .NET Core front (such as MSBuild moving to open source).

I’ve been writing and developing with .NET since it was announced in preview in 2000. Much of what I’ll write about won’t just be the new stuff, but about how to leverage the technology with an eye toward best practices.

I live in Spokane, Wash., where I’m the “Chief Nerd” for a high-end consulting company called IntelliTect (IntelliTect.com). IntelliTect specializes in developing the “hard stuff,” with excellence. I’ve been a Microsoft MVP (currently for C#) going on 20 years and a Microsoft regional director for eight of those years. Today, this column launches with a look at updated exception handling guidelines.

C# 6.0 included two new exception handling features. First, it included support for exception conditions—the ability to provide an expression that filters out an exception from entering catch block before the stack unwinds. Second, it included async support from within a catch block, something that wasn’t possible in C# 5.0 when async was added to the language. In addition, there have been many other changes that have occurred in the last five versions of C# and the corresponding .NET Framework, changes, which in some cases, are significant enough to require edits to C# coding guidelines. In this installment, I’ll review a number of these changes and provide updated coding guidelines as they relate to exception handling—catching exceptions.

### Catching Exceptions: Review

As is fairly well understood, throwing a particular exception type enables the catcher to use the exception’s type itself to identify the problem. It’s not necessary, in other words, to catch the exception and use a switch statement on the exception message to determine which action to take in light of the exception. Instead, C# allows for multiple catch blocks, each targeting a specific exception type as shown in **Figure 1**.

**Figure 1 Catching Different Exception Types**

```csharp
using System;
public sealed class Program
{
  public static void Main(string[] args)
    try
    {
       // ...
      throw new InvalidOperationException(
         "Arbitrary exception");
       // ...
   }
   catch(System.Web.HttpException exception)
     when(exception.GetHttpCode() == 400)
   {
     // Handle System.Web.HttpException where
     // exception.GetHttpCode() is 400.
   }
   catch (InvalidOperationException exception)
   {
     bool exceptionHandled=false;
     // Handle InvalidOperationException
     // ...
     if(!exceptionHandled)
       // In C# 6.0, replace this with an exception condition
     {
        throw;
     }
    }  
   finally
   {
     // Handle any cleanup code here as it runs
     // regardless of whether there is an exception
   }
 }
}
```

When an exception occurs, the execution will jump to the first catch block that can handle it. If there is more than one catch block associated with the try, the closeness of a match is determined by the inheritance chain (assuming no C# 6.0 exception condition) and the first one to match will process the exception. For example, even though the exception thrown is of type System.Exception, this “is a” relationship occurs through inheritance because System.Invalid­OperationException ultimately derives from System.Exception. Because InvalidOperationException most closely matches the exception thrown, catch(InvalidOperationException…) will catch the exception and not catch(Exception…) block if there was one.

Catch blocks must appear in order (again assuming no C# 6.0 exception condition), from most specific to most general, to avoid a compile-time error. For example, adding a catch(Exception…) block before any of the other exceptions will result in a compile error because all prior exceptions derive from System.Exception at some point in their inheritance chain. Also note that a named parameter for the catch block is not required. In fact, a final catch without even the parameter type is allowed, unfortunately, as discussed under general catch block.

On occasion, after catching an exception, you might determine that, in fact, it isn’t possible to adequately handle the exception. Under this scenario, you have two main options. The first option is to rethrow a different exception. There are three common scenarios for when this might make sense:

**Scenario No. 1** The captured exception does not sufficiently identify the issue that triggered it. For example, when calling System.Net.WebClient.DownloadString with a valid URL, the runtime might throw a System.Net.WebException when there’s no network connection—the same exception thrown with a non-existent URL.

**Scenario No. 2** The captured exception includes private data that should not be exposed higher up the call chain. For example, a very early version of CLR v1 (pre-alpha, even) had an exception that said something like, “Security exception: You do not have permission to determine the path of c:\\temp\\foo.txt.”

**Scenario No. 3** The exception type is too specific for the caller to handle. For example, a System.IO exception (such as Unauthorized­AccessException IOException FileNotFoundException DirectoryNotFoundException PathTooLongException, NotSupportedException or SecurityException ArgumentException) occurs on the server while invoking a Web service to look up a ZIP code.

When rethrowing a different exception, pay attention to the fact that it could lose the original exception (presumably intentionally in the case of Scenario 2). To prevent this, set the wrapping exception’s InnerException property, generally assignable via the constructor, with the caught exception unless doing so exposes private data that shouldn’t be exposed higher in the call chain. In so doing, the original stack trace is still available.

If you don't set the inner exception and yet still specify the exception instance after the throw statement (throw exception) the location stack trace will be set on the exception instance. Even if you rethrow the exception previously caught, whose stack trace is already set, it will be reset. A second option when catching an exception is to determine that, in fact, you cannot appropriately handle it. Under this scenario you will want to rethrow the exact same exception—sending it to the next handler up the call chain. The InvalidOperationException catch block of **Figure 1** demonstrates this. A throw statement appears without any identification of the exception to throw (throw is on its own), even though an exception instance (exception) appears in the catch block scope that could be rethrown. Throwing a specific exception would update all the stack information to match the new throw location. As a result, all the stack information indicating the call site where the exception originally occurred would be lost, making it significantly more difficult to diagnose the problem. Upon determining that a catch block cannot sufficiently handle an exception, the exception should be rethrown using an empty throw statement.

Regardless of whether you’re rethrowing the same exception or wrapping an exception, the general guideline is to avoid exception reporting or logging lower in the call stack. In other words, don’t log an exception every time you catch and rethrow it. Doing so causes unnecessary clutter in the log files without adding value because the same thing will be recorded each time. Furthermore, the exception includes the stack trace data of when it was thrown, so no need to record that each time. By all means log the exception whenever it’s handled or, in the case that it’s not going to be handled, log it to record the exception before shutting down a process.

### Throwing Existing Exceptions Without Replacing Stack Information

In C# 5.0, a mechanism was added that enables the throwing of a previously thrown exception without losing the stack trace information in the original exception. This lets you rethrow exceptions, for example, even from outside a catch block and, therefore, without using an empty throw. Although it’s fairly rare to need to do this, on some occasions exceptions are wrapped or saved until the program execution moves outside the catch block. For example, multithreaded code might wrap an exception with an AggregateException. The .NET Framework 4.5 provides a System.Runtime.ExceptionServices.ExceptionDispatchInfo class specifically to handle this scenario through the use of its static Capture and instance Throw methods. **Figure 2** demonstrates rethrowing the exception without resetting the stack trace information or using an empty throw statement.

**Figure 2 Using ExceptionDispatchInfo to Rethrow an Exception**

```csharp
using System
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
Task task = WriteWebRequestSizeAsync(url);
try
{
  while (!task.Wait(100))
{
    Console.Write(".");
  }
}
catch(AggregateException exception)
{
  exception = exception.Flatten();
  ExceptionDispatchInfo.Capture(
    exception.InnerException).Throw();
}
```

With the ExeptionDispatchInfo.Throw method, the compiler doesn’t treat it as a return statement in the same way that it might a normal throw statement. For example, if the method signature returned a value but no value was returned from the code path with ExceptionDispatchInfo.Throw, the compiler would issue an error indicating no value was returned. On occasion, developers might be forced to follow ExceptionDispatchInfo.Throw with a return statement even though such a statement would never execute at run time—the exception would be thrown instead.

### Catching Exceptions in C# 6.0

The general exception handling guideline is to avoid catching exceptions that you’re unable to address fully. However, because catch expressions prior to C# 6.0 could only filter by exception type, the ability to check the exception data and context prior to unwinding the stack at the catch block required the catch block to become the handler for the exception before examining it. Unfortunately, upon determining not to handle the exception, it’s cumbersome to write code that allows a different catch block within the same context to handle the exception. And, rethrowing the same exception results in having to invoke the two-pass exception process again, a process that involves first delivering the exception up the call chain until it finds one that handles it and, second, unwinding the call stack for each frame between the exception and the catch location.

Once an exception is thrown, rather than unwinding the call stack at the catch block only to have the exception rethrown because further examination of the exception revealed it couldn’t be sufficiently handled, it would obviously be preferable not to catch the exception in the first place. Starting with C# 6.0, an additional conditional expression is available for catch blocks. Rather than limiting whether a catch block matches based only on an exception type match, C# 6.0 includes support for a conditional clause. The when clause lets you supply a Boolean expression that further filters the catch block to only handle the exception if the condition is true. The System.Web.HttpException block in **Figure 1** demonstrated this with an equality comparison operator.

An interesting outcome of the exception condition is that, when an exception condition is provided, the compiler doesn’t force catch blocks to appear in the order of the inheritance chain. For example, a catch of type System.ArgumentException with an accompanying exception condition can now appear before the more specific System.ArgumentNullException type even though the latter derives from the former. This is important because it lets you write a specific exception condition that’s paired to a general exception type followed by a more specific exception type (with or without an exception condition). The runtime behavior remains consistent with earlier versions of C#; exceptions are caught by the first catch block that matches. The added complexity is simply that whether a catch block matches is determined by the combination of the type and the exception condition and the compiler only enforces order relative to catch blocks without exception conditions. For example, a catch(System.Exception) with an exception condition can appear before a catch(System.Argument­Exception) with or without an exception condition. However, once a catch for an exception type without an exception condition appears, no catch of a more specific exception block (say catch(System.ArgumentNullException)) may occur whether it has an exception condition. This leaves the programmer with the “flexibility” to code exception conditions that are potentially out of order—with earlier exception conditions catching exceptions intended for the later ones, potentially even rendering the later ones unintentionally unreachable. Ultimately, the order of your catch blocks is similar to the way you would order if-else statements. Once the condition is met, all other catch blocks are ignored. Unlike the conditions in an if-else statement, however, all catch blocks must include the exception type check.

### Updated Exception Handling Guidelines

The comparison operator example in **Figure 1** is trivial, but the exception condition isn’t limited to simplicity. You could, for example, make a method call to validate a condition. The only requirement is that the expression is a predicate—it returns a Boolean value. In other words, you can essentially execute any code you like from within the catch exception call chain. This opens up the possibility of never again catching and rethrowing the same exception again; essentially, you're able to narrow down the context sufficiently before catching the exception as to only catch it when doing so is valid. Thus, the guideline to avoid catching exceptions that you're unable to handle fully becomes a reality. In fact, any conditional check surrounding an empty throw statement can likely be flagged with a code smell and avoided. Consider adding an exception condition in favor of having to use an empty throw statement except to persist a volatile state before a process terminates.

That said, developers should limit conditional clauses to check the context only. This is important because if the conditional expression itself throws an exception, then that new exception is ignored and the condition is treated as false. For this reason, you should avoid throwing exceptions in the exception conditional expression.

### General Catch Block

C# requires that any object that code throws must derive from System.Exception. However, this requirement isn’t universal to all languages. C/C++, for example, lets any object type be thrown, including managed exceptions that don’t derive from System.Exception or even primitive types like int or string. Starting with C# 2.0, all exceptions, whether deriving from System.Exception or not, will propagate into C# assemblies as derived from System.Exception. The result is that System.Exception catch blocks will catch all “reasonably handled” exceptions not caught by earlier blocks. Prior to C# 1.0 however, if a non-System.Exception-derived exception was thrown from a method call (residing in an assembly not written in C#), the exception wouldn’t be caught by a catch(System.Exception) block. For this reason, C# also supports a general catch block (catch{ }) that now behaves identically to the catch(System.Exception exception) block except that there’s no type or variable name. The disadvantage of such a block is simply that there’s no exception instance to access, and therefore no way to know the appropriate course of action. It wouldn’t even be possible to log the exception or recognize the unlikely case where such an exception is innocuous.

In practice, the catch(System.Exception) block and general catch block—herein generically referred to as catch System.Exception block—are both to be avoided except under the pretense of “handling” the exception by logging it before shutting down the process. Following the general principle of only catch exceptions that you can handle, it would seem presumptuous to write code for which the programmer declares—this catch can handle any and all exceptions that may be thrown. First, the effort to catalog any and all exceptions (especially in the body of a Main where the amount of executing code is the greatest and context likely the least) seems monumental except for the simplest of programs. Second, there’s a host of possible exceptions that can be unexpectedly thrown.

Prior to C# 4.0 there was a third set of corrupted state exceptions for which a program was not even generally recoverable. This set is less of a concern starting in C# 4.0, however, because catching System.Exception (or a general catch block) will not in fact catch such exceptions. (Technically you can decorate a method with the System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions so that even these exceptions are caught, but the likelihood that you can sufficiently address such exceptions is extremely challenging. See [bit.ly/1FgeCU6](https://bit.ly/1FgeCU6) for more information.)

One technicality to note on corrupted state exceptions is that they will only pass over catch System.Exception blocks when thrown by the runtime. An explicit throw of a corrupted state exception such as a System.StackOverflowException or other System.SystemException will in fact be caught. However, throwing such would be extremely misleading and is really only supported for backward-compatibility reasons. Today’s guideline is not to throw any of the corrupted state exceptions (including System.StackOverflowException, System.SystemException, System.OutOfMemoryException, System.Runtime.Interop­Services.COMException, System.Runtime.InteropServices.SEH­Exception and System.ExecutionEngineException).

In summary, avoid using a catch System.Exception block unless it’s to handle the exception with some cleanup code and logging the exception before rethrowing or gracefully shutting down the application. For example, if the catch block could successfully save any volatile data (something that cannot necessarily be assumed as it, too, might be corrupt) before shutting down the application or rethrowing the exception. When encountering a scenario for which the application should terminate because it’s unsafe to continue execution, code should invoke the System.Environment.FailFast method. Avoid System.Exception and general catch blocks except to gracefully log the exception before shutting down the application.

### Wrapping Up

In this article I provided updated guidelines for exception handling—catching exceptions, updates caused by improvements in C# and the .NET Framework that have occurred over the last several versions. In spite of the fact that there were some new guidelines, many are still just as firm as before. Here’s a summary of the guidelines for catching exceptions:

- AVOID catching exceptions that you’re unable to handle fully.
- AVOID hiding (discarding) exceptions you don’t fully handle.
- DO use throw to rethrow an exception; rather than throw <exception object> inside a catch block.
- DO set the wrapping exception’s InnerException property with the caught exception unless doing so exposes private data.
- CONSIDER an exception condition in favor of having to rethrow an exception after capturing one you can’t handle.
- AVOID throwing exceptions from exception conditional expression.
- DO use caution when rethrowing different exceptions.
- RARELY use System.Exception and general catch blocks—except to log the exception before shutting down the application.
- AVOID exception reporting or logging lower in the call stack.

Go to [itl.tc/ExceptionGuidelinesForCSharp](/ExceptionGuidelinesForCSharp/) for a review of the details of each of these. In a future column I plan to focus more on the guidelines for throwing exceptions. Suffice it to say for now that a theme for throwing exceptions is: The intended recipient of an exception is a programmer rather than the end user of a program. Note that much of this material is taken from the next edition of my book, “Essential C# 6.0 (5th Edition)” (Addison-Wesley, 2015), which is available now at [itl.tc/EssentialCSharp](/essentialcsharp/).

_Thanks to the following technical experts for reviewing this article: Kevin Bost, Jason Peterson and Mads Torgerson._

_This article was originally posted [here](https://docs.microsoft.com/en-us/archive/msdn-magazine/2015/november/essential-net-csharp-exception-handling) in the November 2015 issue of MSDN Magazine._
