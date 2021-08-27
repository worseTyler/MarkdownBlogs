

## .NET Core Dependency Injection
#
In my last two articles, Logging with .NET Core ([bit.ly/1Vv3Q39](https://bit.ly/1Vv3Q39)) and Configuration with .NET Core ([bit.ly/1OoqmkJ](https://bit.ly/1OoqmkJ)), I demonstrated how .NET Core functionality can be leveraged from both an ASP.NET Core project (project.json) as well as the more common .NET 4.6 C# project (\*.csproj).  In other words, taking advantage of the new framework is not limited to those who are writing ASP.NET Core projects.  In this column I’m going to continue to delve into .NET Core, this time with a focus on .NET Core dependency injection capabilities and how they enable an inversion of control (IoC) pattern.  As before, leveraging .NET Core functionality is possible from both “traditional” CSPROJ files and the emerging project.json type projects.  For the sample code, this time I’ll be using XUnit from a project.json project.

### **Why Dependency Injection?**

With .NET, instantiating an object is trivial with a call to the constructor via the new operator (that is, new MyService or whatever the object type is you wish to instantiate).  Unfortunately, an invocation like this forces a tightly coupled connection (a hard-coded reference) of the client (or application) code to the object instantiated, along with a reference to its assembly/NuGet package.  For common .NET types this isn’t a problem.  However, for types offering a “service,” such as logging, configuration, payment, notification, or even dependency injection, the dependency may be unwanted if you want to switch the implementation of the service you use.  For example, in one scenario a client might use NLog for logging, while in another they might choose Log4Net or Serilog.  And, the client using NLog might prefer not to dirty up their project with Serilog, so a reference to both logging services would be undesirable (see **Figure 1**).

[![Figure 1 Strongly Coupling the Client to the Logging Service Implementation](https://intellitect.com/wp-content/uploads/2016/05/di-host.png)](https://intellitect.com/wp-content/uploads/2016/05/di-host.png "Essential .NET: .NET Core Dependency Injection (MSDN)")

Figure 1 Strongly Coupling the Client to the Logging Service Implementation

To solve the problem of hard- coding a reference to the service implementation, dependency injection provides a level of indirection such that rather than instantiating the service directly with the new operator, the client (or application) will instead ask a service collection or “factory” for the instance, as shown in **Figure 2**. Furthermore, rather than asking the service collection for a specific type (thus creating a tightly coupled reference), you ask for an interface (such as ILoggerFactory) with the expectation that the service provider (in this case, NLog, Log4Net, or Serilog) will implement the interface.

[![Figure 2 Sequence Diagram Demonstrating Dependency Injection and IoC](https://intellitect.com/wp-content/uploads/2016/05/di-sequence-diagram-300x195.png)](https://intellitect.com/wp-content/uploads/2016/05/di-sequence-diagram.png "Essential .NET: .NET Core Dependency Injection (MSDN)")

Figure 2 Sequence Diagram Demonstrating Dependency Injection and IoC

The result is that while the client will directly reference the abstract assembly (Logging.Abstractions), defining the service interface, no references to the direct implementation will be needed, as shown in **Figure 3**.

[![Figure 3 Decoupling the Client/Library from the Specific Logging Implementaiton via Dependency Injection](https://intellitect.com/wp-content/uploads/2016/05/di-host-2.png)](https://intellitect.com/wp-content/uploads/2016/05/di-host-2.png "Essential .NET: .NET Core Dependency Injection (MSDN)")

Figure 3 Decoupling the Client/Library from the Specific Logging Implementation via Dependency Injection

We call the pattern of decoupling the actual instance returned to the client inversion of control.  This is because rather than the client determining what is instantiated, as it does when explicitly invoking the constructor with the new operator, dependency injection determines what will be returned.  Dependency injection registers an association between the type requested by the client (generally an interface) and the type that will be returned.  Furthermore, dependency injection generally determines the lifetime of the type returned, specifically, whether there will be a single instance shared between all requests for the type, a new instance for every request, or something in between.

One especially common need for dependency injection is in unit tests.  Consider a shopping cart service that, in turn, depends on a payment service.  Imagine writing the shopping cart service that leverages the payment service and trying to unit test the shopping cart service without actually invoking a real payment service.  What you want to invoke instead is a mock payment service.  To achieve this with dependency injection, your code would request an instance of the payment service interface from the dependency injection framework rather than calling, for example, new PaymentService.  Then, all that’s needed is for the unit test to “configure” the framework to return a mock payment service, as shown in **Figure 4**.

[![Figure 4 Unit Testing with Dependency Injection](https://intellitect.com/wp-content/uploads/2016/05/di-unit-testing.png)](https://intellitect.com/wp-content/uploads/2016/05/di-unit-testing.png "Essential .NET: .NET Core Dependency Injection (MSDN)")

Figure 4 Unit Testing with Dependency Injection

In contrast, the production host could configure the shopping cart to use one of the (possibly many) payment service options. And, perhaps most importantly, the references would be only to the payment abstraction, rather than to each specific implementation (see **Figure 5**).

[![Figure 5 Unit Testing with Dependency Injection so a Mock Type Instance Can Be Used](https://intellitect.com/wp-content/uploads/2016/05/di-unit-tests-host.png)](https://intellitect.com/wp-content/uploads/2016/05/di-unit-tests-host.png "Essential .NET: .NET Core Dependency Injection (MSDN)")

Figure 5 Unit Testing with Dependency Injection so a Mock Type Instance Can Be Used

Providing an instance of the “service” rather than the client directly instantiating it is the fundamental principle of dependency injection. And, in fact, some dependency injection frameworks allow a decoupling of the host from referencing the implementation by supporting a binding mechanism that’s based on configuration and reflection, rather than a compile-time binding. This decoupling of is known as the service locator pattern.

### **.NET Core Microsoft.Extensions.DependencyInjection**

To leverage the .NET Core framework, all you need is a reference to the Microsoft.Extensions.DependencyInjection.Abstractions NuGet package. This provides access to the IServiceCollection interface, which exposes a System.IServiceProvider from which you can call GetService<TService>. The type parameter, TService, identifies the type of the service to retrieve (generally an interface) and thus the application code obtains an instance:

```
ILoggingFactory loggingFactory = serviceProvider.GetService<ILoggingFactory>();
```

There are equivalent non-generic GetService methods that have Type as a parameter (rather than a generic parameter).  The generic methods allow for assignment directly to a variable of a particular type, whereas the non-generic versions require an explicit cast because the return type is Object.  Furthermore, there are generic constraints when adding the service type so that a cast can be avoided entirely when using the type parameter.

If no type is registered with the collection service when calling GetService, it will return null.  This is useful when coupled with the null propagation operator to add optional behaviors to the app.  The similar GetRequiredService method throws an exception when the service type is not registered.

As you can see, the code is trivially simple.  However, what’s missing is how to obtain an instance of the service provider on which to invoke GetService.  The solution is simply to first instantiate ServiceCollection’s default constructor, then register the type you want the service to provide.  An example is shown in **Figure 6**, in which you can assume each class (Host, Application, and PaymentService) are implemented in separate assemblies.  Furthermore, while the Host assembly knows which loggers to use, there is no reference to loggers in Application or PaymentService.  Similarly, the Host assembly has no reference to the PaymentServices assembly.  Interfaces are also implemented in separate “abstraction” assemblies.  For example, the ILogger interface is defined in Microsoft.Extensions.Logging.Abstractions assembly.

```csharp
public class Host
{
    public static void Main()
    {
        IServiceCollection serviceCollection = new ServiceCollection();

        ConfigureServices(serviceCollection);
        Application application = new Application(serviceCollection);

        // Run
        // ...
    }

    static private void ConfigureServices(IServiceCollection serviceCollection)
    {
        ILoggerFactory loggerFactory = new Logging.LoggerFactory();

        serviceCollection.AddInstance<ILoggerFactory>(loggerFactory);
    }
}

public class Application
{
    public IServiceProvider Services { get; set; }
    public ILogger Logger { get; set; }

        public Application(IServiceCollection serviceCollection)
    {
        ConfigureServices(serviceCollection);
        Services = serviceCollection.BuildServiceProvider();
        Logger = Services.GetRequiredService<ILoggerFactory>()
                .CreateLogger<Application>();
        Logger.LogInformation("Application created successfully.");

    }
        
    public void MakePayment(PaymentDetails paymentDetails)
    {
        Logger.LogInformation(
            $"Begin making a payment { paymentDetails }");
        IPaymentService paymentService = 
            Services.GetRequiredService<IPaymentService>();

        // ...
    }

    private void ConfigureServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IPaymentService, PaymentService>();
    }
}
public class PaymentService: IPaymentService
{
    public ILogger Logger { get; }

    public PaymentService(ILoggerFactory loggerFactory)
    {

        Logger = loggerFactory?.CreateLogger<PaymentService>();
        if(Logger == null)
        {
            throw new ArgumentNullException(nameof(loggerFactory));
        }
        
        Logger.LogInformation("PaymentService created");
    }
}
```

Figure 6 Registering and Requesting an Object from Dependency Injection

You can think of the ServiceCollection type conceptually as a name-value pair, where the name is the type of an object (generally an interface) you’ll later want to retrieve and the value is either the type that implements the interface or the algorithm (delegate) for retrieving that type.  The call to AddInstance, in the Host.ConfigureServices method in **Figure 6**, therefore, registers that any request for the ILoggerFactory type return the same LoggerFactory instance created in the ConfigureServices method.  As a result, both Application and PaymentService are able to retrieve the ILoggerFactory without any knowledge (or even an assembly/Nuget reference) to what loggers are implemented and configured.  Similarly, the application provides a MakePayment method without any knowledge as to which payment service is being used.

Note that ServiceCollection doesn’t provide GetService or GetRequiredService methods directly.  Rather, those methods are available from the IServiceProvider that is returned from the ServiceCollection.BuildServiceProvider method.  Furthermore, the only services available from the provider are those added before the call to BuildServiceProvider.

### **Service Lifetime**

In **Figure 6** I invoke the IServiceCollection AddInstance<TService>(TService implementationInstance) extension method.  Instance is one of four different TService lifetime options available with .NET Core.  It establishes that not only will the call to GetService return an object of type TService, but also that the specific implementationInstance registered with AddInstance is what will be returned.  In other words, registering with AddInstance saves the specific implementationInstance instance so it can be returned with every call to GetService (or GetRequiredService) with the AddInstance method’s TService type parameter.

In contrast, the IServiceCollection AddSingleton<TService> extension method has no parameter for an instance and instead relies on the TService having a means of instantiation via the constructor.  While a default constructor works, Microsoft.Extensions.DependencyInjection also supports non-default constructors whose parameters are also registered.  For example, you can call:

IPaymentService paymentService = Services.GetRequiredService<IPaymentService>()

and DI will take care of retrieving the ILoggingFactory concrete instance and leveraging it when instantiating the PaymentService class that requires an ILoggingFactory in its constructor.

If there’s no such means available in the TService type, you can instead leverage the overload of the AddSingleton extension method, which takes a delegate of type Func<IServiceProvider, TService> implementationFactory—a factory method for instantiating TService.   Whether you provide the factory method or not, the service collection implementation ensures that it will only ever create one instance of the TService type, thus ensuring that there’s a singleton instance.  Following the first call to GetService that triggers the TService instantiation, the same instance will always be returned for the lifetime of the service collection.

IServiceCollection also includes the AddTransient(Type serviceType, Type implementationType) and AddTransient(Type serviceType, Func<IServiceProvider, TService> implementationFactory) extension methods.  These are similar to AddSingleton except they return a new instance every time they’re invoked, ensuring you always have a new instance of the TService type.

Lastly, there are several AddScoped type extension methods.  These methods are designed to return the same instance within a given context and to create a new instance whenever the context—known as the scope—changes.  The behavior of ASP.NET Core conceptually maps to the scoped lifetime.  Essentially, a new instance is created for each HttpContext instance, and whenever GetService is called within the same HttpContext, the identical TService instance is returned.

In summary, there are four lifetime options for the objects returned from the service collection implementation: Instance, Singleton, Transient, and Scoped. The last three are defined in the ServiceLifetime enum (bit.ly/1SFtcaG).  Instance, however, is missing, since it’s a special case of Scoped in which the context doesn’t change.

Earlier I referred to the ServiceCollection as conceptually like a name-value pair with the TService type serving as the lookup.  The actual implementation of the ServiceCollection type is done in the ServiceDescription class (see bit.ly/1SFoDgu).  This class provides a container for the information required to instantiate the TService, namely the ServiceType (TService), the ImplementationType or ImplementationFactory delegate along with the ServiceLifetime.  In addition to the ServiceDescriptor constructors, there are a host of static factory methods on ServiceDescriptor that help with instantiating the ServiceDescriptor itself.

Regardless of which lifetime you register your TService with, the TService itself must be a reference type, not a value type.  Whenever you use a type parameter for TService (rather than passing Type as a parameter) the compiler will verify this with a generic class constraint.  One thing, however, that’s not verified is using a TService of type object.  You’ll want to be sure to avoid this, along with any other non-unique interfaces (such as IComparable perhaps).  The reason is that if you register something of type object, no matter what TService you specify in the GetService invocation, the object registered as a TService type will always be returned.

### **A Word On ActivatorUtilities**

Microsoft.Framework.DependencyInjection.Abstractions also includes a static helper class that provides a few useful methods in dealing with constructor parameters that aren’t registered with the IServiceProvider, a custom ObjectFactory delegate, or situations where you want to create a default instance in the event that a call to GetService returns null.  You can find examples where this utility class is used in both the MVC framework and the SignalR library.  In the first case, a method with a signature of CreateInstance<T>(IServiceProvider provider, params object[] parameters) exists which allows you to pass in constructor parameters to a type registered with the dependency injection framework for arguments that are not registered.  You may also have a performance requirement that lambda functions required to generate your types be compiled lambdas.  The CreateFactory(Type instanceType, Type[] argumentTypes) method that returns an ObjectFactory can be useful in this case.  The first argument is the type sought by a consumer, and the second argument is all the constructor types, in order, that match the constructor of the first type that you wish to use.  In it's implementation, these pieces are condensed down to a compiled lambda that will be very performant when called multiple times.  Finally, the GetServiceOrCreateInstance<T>(IServiceProvider provider) method provides an easy way to provide a default instance of a type that may have been optionally registered in a different place.  One use for this in the real world would be in a "freemium" type software where a default bare-bones implementation of an interface would be needed in the "free" mode, while a more feature-rich implementation would be registered after paying for a "premium" mode.

### **Dependency Injection for the Dependency Injection Implementation**

ASP.NET leverages dependency injection to such an extent that, in fact, you can use dependency injection within the dependency injection framework itself.  In other words, you’re not limited to using the ServiceCollection implementation of the dependency injection mechanism found in Microsoft.Extensions.DependencyInjection.  Rather, as long as you have classes that implement IServiceCollection (which is defined in Microsoft.Extensions.DependencyInjection.Abstractions; see bit.ly/1SKdm1z) or IServiceProvider (defined within the System namespace of .NET core lib framework) you can substitute your own dependency injection framework or leverage one of the other well established dependency injection frameworks including Ninject ([ninject.org](https://ninject.org), with a shout out to @IanfDavis for his work maintaining this over the years) and Autofac ([autofac.org](https://autofac.org)).

### **Wrapping Up**

As with .NET Core Logging and Configuration, the .NET Core dependency injection mechanism provides a relatively simple implementation of its functionality.  While you’re unlikely to find  the more advanced dependency injection functionality of some of the other frameworks, the .NET Core version is lightweight and a great way to get started.  Furthermore (and again like Logging and Configuration), the .NET Core implementation can be replaced with a more mature implementation.  Thus, you might consider leveraging the .NET Core dependency injection framework as a “wrapper” through which you can plug in other dependency injection frameworks as the need arises in the future.  In this way, you don’t have to define your own “custom” dependency injection wrapper but can leverage .NET Core’s as a standard one for which any client/application can plug in a custom implementation.

One thing to note about ASP.NET Core is that it leverages dependency injection throughout.  This is undoubtedly a great practice if you need it and it’s especially important when trying to substitute mock implementations of a library in your unit tests.  The drawback is that rather than a simple call to a constructor with the new operator, the complexity of dependency injection registration and GetService calls is needed.  I can’t help but wonder if perhaps the C# language could simplify this but, based on the current C# 7.0 design, that isn’t happening any time soon.

_Thanks to the following IntelliTect technical experts for reviewing this article: [Kelly Adams](/author/kelly/), [Kevin Bost](/author/kevin-bost/), [Ian Davis](https://twitter.com/ianfdavis) and [Phil Spokas](/author/phil/)._

_This article was originally posted [here](https://docs.microsoft.com/en-us/archive/msdn-magazine/2016/june/essential-net-dependency-injection-with-net-core) in the June 2016 issue of MSDN Magazine._
