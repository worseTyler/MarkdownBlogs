---
title: "Implementing a Custom ILogger with Exception Handling for .NET Core"
date: "2016-03-04"
categories: 
  - "net"
  - "net-core"
  - "blog"
  - "c"
tags: 
  - "net-core"
---

Estimated reading time: 2 minutes

This article corresponds with the MSDN article: [_Essential .NET - Logging with .NET Core_](https://msdn.microsoft.com/magazine/mt694089). [This GitHub repo](https://github.com/IntelliTect-Samples/2016.04.01-EssentialNetLoggingWithNetCore) contains the code referenced in the article. Not, in particular, the unit test `LogCritical_Exception_Success` for an example of handling an exception using the custom logger.

  public void LogCritical\_Exception\_Success()
  {
        string message = "The amount of caffeine has reach critical levels.";
        CustomLogger.CustomLogger customLogger = null;
        CustomLoggerProvider logProvider =
        new CustomLoggerProvider((sender, eventArgs) => customLogger = eventArgs.CustomLogger);
        ApplicationLogging.LoggerFactory.AddProvider(logProvider);
        Logger.LogCritical(message, new Exception("Sample exception."));
        Assert.AreEqual($"{message}\\r\\nSystem.Exception: Sample exception.", customLogger.LogDataQueue.Dequeue());
  }

The Custom Logger implementation is straightforward:

 public class CustomLogger : ILogger 
  {
        public Queue LogDataQueue = new Queue();
        
        public IDisposable BeginScopeImpl(object state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            string message = string.Empty;

            if (formatter != null)
            {
                message = formatter(state, exception);
            }
            else
            {
                message = LogFormatter.Formatter(state, exception);
            }

            LogDataQueue.Enqueue(message);
        }
  }
  

Here is the the extension method for adding a custom logger provider:

  public static class CustomLoggerFactoryExtensions
  {
        public static ILoggerFactory AddCustomLogger(
            this ILoggerFactory factory, out CustomLoggerProvider logProvider)
        {
            logProvider = new CustomLoggerProvider();
            factory.AddProvider(logProvider);
            return factory;
        }
  }
﻿

The custom logger provider:

  public class CustomLoggerProvider : ILoggerProvider
  {
        public CustomLoggerProvider() { }
        public CustomLoggerProvider(EventHandler onCreateLogger)
        {
            OnCreateLogger = onCreateLogger;
        }
        public ConcurrentDictionary<string, customlogger=""> Loggers { get; set; } = new ConcurrentDictionary<string, customlogger="">();

        public ILogger CreateLogger(string categoryName)
        {
            CustomLogger customLogger = Loggers.GetOrAdd(categoryName, new CustomLogger());
            OnCreateLogger?.Invoke(this, new CustomLoggerProviderEventArgs(customLogger));
            return customLogger;
        }

        public void Dispose() { }

        public event EventHandler OnCreateLogger = delegate { };
  }
</string,></string,>

The provider event args:

  public class CustomLoggerProviderEventArgs
  {
        public CustomLogger CustomLogger { get; }
        public CustomLoggerProviderEventArgs(CustomLogger logger)
        {
            CustomLogger = logger;
        }
  }

One uses the ApplicationLogging static class to set up the custom logger with the following pattern:

  public static class ApplicationLogging
  {
        public static ILoggerFactory LoggerFactory { get; } = new LoggerFactory();
        public static ILogger CreateLogger() =>
            LoggerFactory.CreateLogger();
  }
/\*        The following pattern will set up the custom logger
            CustomLogger.CustomLogger customLogger = null;
            CustomLoggerProvider logProvider =
                new CustomLoggerProvider((sender, eventArgs) => customLogger = eventArgs.CustomLogger);
            ApplicationLogging.LoggerFactory.AddProvider(logProvider);
\*/
﻿

### Have a Question?

Check out my other [tutorials](/configuring-windows-smtp-server-on-windows-2008-for-relay/) and leave any questions in the comment section below!

![](images/blog-job-ad-2-1024x129.png)
