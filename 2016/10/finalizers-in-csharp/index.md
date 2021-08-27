

## Finalizers in C#
#
The following is an excerpt from Chapter 9 of [Essential C# 6](https://IntelliTect.com/EssentialCSharp6/).

Finalizers allow developers to write code that will clean up a class’s resources. Unlike constructors that are called explicitly using the new operator, finalizers cannot be called explicitly from within the code. There is no new equivalent such as a delete operator. Rather, the garbage collector is responsible for calling a finalizer on an object instance. Therefore, **developers cannot determine at compile time exactly when the finalizer will execute**. All they know is that the finalizer will run sometime between when an object was last used and when the application shuts down normally. (Finalizers might not execute if the process is terminated abnormally. For instance, events such as the computer being turned off or a forced termination of the process will prevent the finalizer from running.)

The finalizer declaration is identical to the destructor syntax of C#’s predecessor—namely, C++. As shown below, the finalizer declaration is prefixed with a tilde before the name of the class.

```csharp
using System.IO;

class TemporaryFileStream
{
  public TemporaryFileStream(string fileName)
  {
      File = new FileInfo(fileName);

      Stream = new FileStream(
          File.FullName, FileMode.OpenOrCreate,
          FileAccess.ReadWrite);
  }

  public TemporaryFileStream()
      : this(Path.GetTempFileName()) { }

  // Finalizer
  ~TemporaryFileStream()
  {
      Close();
  }

  public FileStream Stream { get; }

  public FileInfo File { get; }

  public void Close()
  {
      Stream?.Dispose();
      File?.Delete();
  }
}
```

Finalizers do not allow any parameters to be passed, so they cannot be overloaded. Furthermore, **finalizers cannot be called explicitly**—that is, only the garbage collector can invoke a finalizer. Access modifiers on finalizers are therefore meaningless, and as such, they are not supported. Finalizers in base classes will be invoked automatically as part of an object finalization call.

Because the garbage collector handles all memory management, finalizers are not responsible for de-allocating memory. Rather, they are responsible for freeing up resources such as database connections and file handles—resources that require an explicit activity that the garbage collector doesn’t know about.

Finalizers execute on their own thread, making their execution even less deterministic. This indeterminate nature makes an unhandled exception within a finalizer (outside of the debugger) difficult to diagnose because the circumstances that led to the exception are not clear. From the user’s perspective, the unhandled exception will be thrown relatively randomly and with little regard for any action the user was performing. For this reason, you should take care to avoid exceptions within finalizers. Instead, you should use defensive programming techniques such as checking for nulls (refer to Listing 9.20).
