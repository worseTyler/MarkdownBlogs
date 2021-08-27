

## Essential Net Logging With .NET Core  1.0 
#
In the February issue, I delved into the new configuration API included in the newly named .NET Core 1.0 platform (see [bit.ly/1OoqmkJ](https://bit.ly/1OoqmkJ)). (I assume most readers have heard about the recently renamed .NET Core 1.0, which was formerly referred to as .NET Core 5 and part of the ASP.NET 5 platform [see [bit.ly/1Ooq7WI](https://bit.ly/1Ooq7WI)].) In that article I used unit testing in order to explore the Microsoft.Extensions.Configuration API. In this article I take a similar approach, except with Microsoft.Extensions.Logging. The key difference in my approach is that I’m testing it from a .NET 4.6 CSPROJ file rather than an ASP.NET Core project. This emphasizes the fact that .NET Core is available for you to consider using immediately—even if you haven’t migrated to ASP.NET Core projects.

Logging? Why on earth do we need a new logging framework? We already have NLog, Log4Net, Loggr, Serilog and the built-in Microsoft.Diagnostics.Trace/Debug/TraceSource, just to name a few. As it turns out, the fact that there are so many logging frameworks is actually one of the driving factors that make Microsoft.Exten­sions.Logging relevant. As a developer faced with the myriad of choices, you’re likely to select one knowing you might have to switch to another one later. Therefore, you’re probably tempted to write your own logging API wrapper that invokes whichever particular logging framework you or your company chooses this week. Similarly, you might use one particular logging framework in your application, only to find that one of the libraries you’re leveraging is using another, causing you to have to write a listener that takes the messages from one to the other.

What Microsoft is providing with Microsoft.Extensions.Logging is that wrapper so everyone doesn’t have to write their own. This wrapper provides one set of APIs that are then forwarded to a provider of your choosing. And, while Microsoft includes providers for things like the Console (Microsoft.Extensions.Logging.Console), debugging (Microsoft.Extensions.Logging.Debug), the event log (Microsoft.Extensions.Logging.EventLog) and TraceSource (Microsoft.Estensions.Logging.TraceSource), it has also collaborated with the various logging framework teams (including third parties like NLog, Serilog, Loggr, Log4Net and more) so that there are Microsoft.Extensions.Logging compatible providers from them, too.

## Getting Started

The root of the logging activity begins with a log factory, as shown in **Figure 1**.

**Figure 1 How to Use Microsoft.Extensions.Logging**

```csharp
public static void Main(string[] args = null)
{
  ILoggerFactory loggerFactory = new LoggerFactory()
    .AddConsole()
    .AddDebug();
  ILogger logger = loggerFactory.CreateLogger<Program>();
  logger.LogInformation(
    "This is a test of the emergency broadcast system.");
}
```

As the code demonstrates, to begin you instantiate a Microsoft.Extensions.Logging.LoggerFactory, which implements ILoggerFactory in the same namespace. Next, you specify which providers you want to utilize by leveraging the extension method of ILoggerFactory. In **Figure 1**, I specifically use Microsoft.Extensions.Logging.ConsoleLoggerExtensions.AddConsole and Microsoft.Extensions.Log­ging.DebugLoggerFactoryExtensions.AddDebug. (Although the classes are both in the Microsoft.Extensions.Logging namespace, they’re actually found in the Microsoft.Extensions.Log­ging.Console and Microsoft.Extensions.Logging.Debug NuGet packages, respectively.)

The extension methods are simply convenient shortcuts for the more general way to add a provider—ILoggerFactory.AddProvider­(ILoggerProvider provider). The shortcut is that the AddProvider method requires an instance of the log provider—likely one whose constructor requires a log-level filter expression—while the extension methods provide defaults for such expressions. For example, the constructor signature for ConsoleLoggerProvider is:

```csharp
public ConsoleLoggerProvider(Func<string, LogLevel, bool> filter,
  bool includeScopes);
```

This first parameter is a predicate expression that allows you to define whether a message will appear in the output based on the value of the text logged and the log level.

For example, you could call AddProvider with a specific Console­LoggerProvider instance that was constructed from a filter of all messages higher (more significant) than LogLevel.Information:

```csharp
loggerFactory.AddProvider(
  new ConsoleLoggerProvider(
    (text, logLevel) => logLevel >= LogLevel.Verbose , true));
```

(Interestingly, unlike the extension methods that return an ILoggerFactory, AddProvider returns void—preventing the fluid type syntax shown in **Figure 1**.)

It’s important to be cognizant that, unfortunately, there’s some inconsistency between log providers as to whether a high log-level value is more or less significant. Does a log level of 6 indicate a critical error occurred or is it just a verbose diagnostic message? Microsoft.Extensions.Logging.LogLevel uses high values to indicate higher priority with the following LogLevel enum declaration:

```csharp
public enum LogLevel
{
  Debug = 1,
  Verbose = 2,
  Information = 3,
  Warning = 4,
  Error = 5,
  Critical = 6,
  None = int.MaxValue
}
```

Therefore, by instantiating a ConsoleLoggerProvider that writes messages only when the logLevel >= LogLevel.Verbose, you’re excluding only Debug-level messages from being written to the output.

Note that you can add multiple providers to the log factory, even multiple providers of the same type. Therefore, if I add an invocation of ILoggerFactory.AddProvider to **Figure 1**, a call to ILogger.LogInformation would display a message on the console twice. The first console provider (the one added by AddConsole) defaults to displaying anything LogLevel.Information or higher. However, an ILogger.LogVerbose call would appear only once as only the second provider (added via the AddProvider method) would successfully avoid being filtered out.

### Logging Patterns

As **Figure 1** demonstrates, the root of all logging begins with a log factory from which you can request an ILogger via the ILoggerFactory.CreateLogger<T> method. The generic type T in this method is to identify the class in which the code executes, so it’s possible to write out the class name in which the logger is writing messages. In other words, by calling loggerFactory.CreateLogger<Program>, you essentially initiate a logger specific to the Program class so that each time a message is written, it’s also possible to write the execution context as being within the Program class. Thus, the console output of **Figure 1** is:

```csharp
info: SampleWebConsoleApp.Program[0]
      This is a test of the emergency broadcast system.
```

This output is based on the following:

- “info” results from the fact that this is a LogInformation method call.
- “SampleWebConsoleApp.Program” is determined from T.
- “[0]” is the eventId—a value I didn’t specify so it defaults to 0.
- “This is a test of the emergency broadcast system.” is the messages argument passed to LogInformation.

Because the value Program indicates class-level context, you’ll likely want to instantiate a different logger instance for each class from which you want to log. For example, if Program creates and calls into a Controller class instance, you’ll want to have a new logger instance within the Controller class that was created via another method call where T is now Controller:

```csharp
loggerFactory.CreateLogger<Controller>()
```

As you may notice, this requires access to the same logger factory instance on which the providers were previously configured. And while it’s conceivable you could pass the logger factory instance into every class from which you want to perform logging, it would quickly become a hassle that would beg for refactoring.

The solution is to save a single static ILoggerFactory as a static property that’s available for all classes when instantiating their object’s specific ILoggger instance. For example, consider adding an ApplicationLogging static class that includes a static ILoggerFactory instance:

```csharp
public static class ApplicationLogging
{
  public static ILoggerFactory LoggerFactory {get;} = new LoggerFactory();
  public static ILogger CreateLogger<T>() =>
    LoggerFactory.CreateLogger<T>();
}
```

The obvious concern in such a class is whether the LoggerFactory is thread-safe. And, fortunately, as the AddProvider method shown in **Figure 2** demonstrates, it is.

**Figure 2 The Microsoft.Extensions.Logging.LoggerFactory AddProvider Implementation**

```csharp
public void AddProvider(ILoggerProvider provider)
{
  lock (_sync)
  {
    _providers = _providers.Concat(new[] { provider }).ToArray();
    foreach (var logger in _loggers)
    {
      logger.Value.AddProvider(provider);
    }
  }
}
```

Because the only data in the ILogger instance is determined from the generic type T, you might argue that each class could have a static ILogger that each class’s object could leverage. However, assuming the programming standard of ensuring thread safety for all static members, such an approach would require concurrency control within the ILogger implementation (which isn’t there by default), and likely result in a significant bottleneck as locks are taken and released. For this reason, the recommendation, in fact, is to have an individual ILogger instance for each instance of a class. The result, therefore, is an ILogger property on each class for which you wish to support logging (see **Figure 3**).

**Figure 3 Adding an ILogger Instance to Each Object That Needs Logging**

```csharp
public class Controller
{
  ILogger Logger { get; } =
    ApplicationLogging.CreateLogger<Controller>();
  // ...
  public void Initialize()
  {
    using (Logger.BeginScopeImpl(
      $"=>{ nameof(Initialize) }"))
    {
      Logger.LogInformation("Initialize the data");
      //...
      Logger.LogInformation("Initialize the UI");
      //...
    }
  }
}
```

### Understanding Scopes

Frequently, providers support the concept of “scope” such that you could (for example) log how your code traverses a call chain. Continuing the example, if Program invokes a method on a Controller class, that class in turn instantiates its own logger instance with its own context of type T. However, rather than simply displaying a message context of info: SampleWebConsoleApp.Program[0] followed by info: SampleWebConsoleApp.Controller[0], you might wish to log that Program-invoked Controller and possibly even include the method names themselves. To achieve this, you activate the concept of scope within the provider. **Figure 3** provides an example within the Initialize method via the invocation of Logger.BeginScopeImpl.

Using the logging pattern while leveraging the scope activation will result in a Program class that might look a little like **Figure 4**.

**Figure 4 An Updated Implementation of Program**

```csharp
public class Program
{
  static ILogger Logger { get; } =
    ApplicationLogging.CreateLogger<Program>();
  public static void Main(string[] args = null)
  {
    ApplicationLogging.LoggerFactory.AddConsole(true);
    Logger.LogInformation(
      "This is a test of the emergency broadcast system.");
    using (Logger.BeginScopeImpl(nameof(Main)))
    {
      Logger.LogInformation("Begin using controller");
      Controller controller = new Controller();
      controller.Initialize();
      Logger.LogInformation("End using controller");
    }
    Logger.Log(LogLevel.Information, 0, "Shutting Down...", null, null);
  }
}
```

The output of **Figure 3** combined with **Figure 4** is shown in **Figure 5**.

**Figure 5 Console Logging Output with Scopes Included**

```csharp
info: SampleWebConsoleApp.Program[0]
      This is a test of the emergency broadcast system.
info: SampleWebConsoleApp.Program[0]
      => Main
      Begin using controller
info: SampleWebConsoleApp.Controller[0]
      => Main => Initialize
      Initialize the data
info: SampleWebConsoleApp.Controller[0]
      => Main => Initialize
      Initialize the UI
info: SampleWebConsoleApp.Program[0]
      => Main
      End using controller
info: SampleWebConsoleApp.Program[0]
      Shutting down...
```

Notice how the scope automatically unwinds to no longer include Initialize or Main. This functionality is provided by the fact that BeginScopeImpl returns an IDisposable instance that automatically unwinds the scope when the using statement calls Dispose.

### Leveraging a Third-Party Provider

To make available some of the most prominent third-party logging frameworks, Microsoft collaborated with its developers and ensured there are providers for each. Without indicating a preference, consider how to connect up the NLog framework, as demonstrated in **Figure 6**.

**Figure 6 Configuring NLog as a Microsoft.Extensions.Logging Provider**

```csharp
[TestClass]
public class NLogLoggingTests
{
  ILogger Logger {get;}
    = ApplicationLogging.CreateLogger<NLogLoggingTests>();
  [TestMethod]
  public void LogInformation_UsingMemoryTarget_LogMessageAppears()
  {
    // Add NLog provider
    ApplicationLogging.LoggerFactory.AddNLog(
      new global::NLog.LogFactory(
        global::NLog.LogManager.Configuration));
    // Configure target
    MemoryTarget target = new MemoryTarget();
    target.Layout = "${message}";
    global::NLog.Config.SimpleConfigurator.ConfigureForTargetLogging(
      target, global::NLog.LogLevel.Info);
    Logger.LogInformation(Message);
     Assert.AreEqual<string>(
      Message, target.Logs.FirstOrDefault<string>());
  }
}
```

Most of this code is well-known to those familiar with NLog. First, I instantiate and configure an NLog target of type NLog.Targets.MemoryTarget. (There are numerous NLog targets and each can be identified and configured in the NLog configuration file, in addition to using configuration code as shown in **Figure 6**.) Notice that while similar in appearance, the Layout is assigned a literal value of ${message}, not a string interpolated value.

Once added to the LoggerFactory and configured, the code is identical to any other provider code.

### Exception Handling

Of course, one of the most common reasons to log is to record when an exception is thrown—more specifically, when the exception is being handled rather than re-thrown or when the exception is entirely unhandled (see [bit.ly/1LYGBVS](https://bit.ly/1LYGBVS)). As you’d expect, Microsoft.Extensions.Logging has specific methods for handling an exception. Most such methods are implemented in Microsoft.Extensions.Logging.LoggerExtensions as extension methods to ILogger. And, it’s from this class that each method specific to a particular log level (ILogger.LogInformation, ILogger.LogDebug, ILogger.LogCritical and so forth) is implemented. For example, if you want to log a LogLevel.Critical message regarding an exception (perhaps before gracefully shutting down the application), you’d call:

```csharp
Logger.LogCritical(message,
  new InvalidOperationException("Yikes..."));
```

Another important aspect of logging and exception handling is that logging, especially when handling exceptions, should not throw an exception. If an exception is thrown when you log, presumably the message or exception will never get written and could potentially go entirely unnoticed, no matter how critical. Unfortunately, the out-of-the-box ILogger implementation—Microsoft.Extensions.Logging.Logger—has no such exception handling, so if an exception does occur, the calling code would need to handle it—and do so every time Logger.LogX is called. A general approach to solving this is to possibly wrap Logger so as to catch the exception. However, you might want to implement your own versions of ILogger and ILoggerFactory (see [bit.ly/1LYHq0Q](/implementing-a-custom-ilogger-with-exception-handling-for-net-core/) for an example). Given that .NET Core is open source, you could even clone the class and purposely implement the exception handling in your very own LoggerFactory and ILogger implementations.

### Wrapping Up

I started out by asking, “Why on Earth would we want yet another logging framework in .NET?” I hope by now this is clear. The new framework creates an abstraction layer or wrapper that enables you to use whichever logging framework you want as a provider. This ensures you have the maximum flexibility in your work as a developer. Furthermore, even though it’s only available with .NET Core, referencing .NET Core NuGet packages like Microsoft.Extensions.Logging for a standard Visual Studio .NET 4.6 project is no problem.

_Thanks to the following IntelliTect technical experts for reviewing this article: Kevin Bost_.

_This article was originally posted_ [_here_](https://docs.microsoft.com/en-us/archive/msdn-magazine/2016/april/essential-net-logging-with-net-core) _in the April 2016 issue of MSDN Magazine._
