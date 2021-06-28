
# Full-stack testing of OData 4.0 and Web API 2.2 ASP.Net MVC controllers![image02](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2016/01/self-hosted-integration-testing-of-odata-and-webapi/images/image02-1024x576.jpg)

## Why Are Unit Tests of OData Web API Controllers Insufficient?

A [common pattern](https://www.asp.net/web-api/overview/testing-and-debugging/unit-testing-controllers-in-web-api) for testing ASP.Net Web API 2 controller methods is to call them directly after mocking or setting up the Request and Configuration properties. When OData v4 is then added to this mix, more setup or mocking is needed to fulfill all the dependencies placed on the controller pipeline. Because of the highly convention-based system for generating the metadata model and routing semantics this adds, I believe it is then easier to just test controller methods with the full stack, hosted inside the OWIN self-host. There are several scenarios where you can have a successful build with passing tests and still have a broken API:

- EDM Model errors
- ATOM/JSON serialization problems
- Routing and URI/argument parsing issues
- Deviations from the default ODataValidationSettings

### Code In This Article

All of the code in this article can be found in a public GitHub repository at [https://github.com/IntelliTect/webapi-odata-testing-example](https://github.com/IntelliTect/webapi-odata-testing-example). This example uses the latest tooling and frameworks to show a working example of testing OData v4 WebApi controllers in the OWIN self-host while still being able to leverage dependency injection and mocking.

The code is broken up into three projects: a Data project than houses the entities and data services to access them, a Web project that holds the OData controllers, and a Tests project that has the example tests.

## Generating An OData v4 Client

The OData query syntax is very verbose and highly structured. A developer can generate simple requests in a tool like Postman. These requests can then be modeled using an open-ended tool such as RestSharp, but this can cause false positives and can be difficult to maintain.. Since OData exposes the entire [data model](https://docs.oasis-open.org/odata/odata/v4.0/errata02/os/complete/part1-protocol/odata-v4.0-errata02-os-part1-protocol-complete.html#_Metadata_Document_Request) via the $metadata endpoint, there are many tools that will generate a client for consuming the API. An easy way to get a C# client is to use the [OData v4 Client Code Generator](https://visualstudiogallery.msdn.microsoft.com/9b786c0e-79d1-4a50-89a5-125e57475937) add-in to visual studio. Installing this VSIX package provides a network or file-based metadata interface for a T4 template to render a C# file that uses the Microsoft.OData.Client objects to provide easy access to your service.

### Using the Client Generator

After the VSIX is installed, in the Add New Item dialog, you will have the ability to add an OData Client:

![image05](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2016/01/self-hosted-integration-testing-of-odata-and-webapi/images/image05.png)

Once added, open the .tt file and edit the MetadataDocumentUri constant to point at a running instance of your web API. You can also specify a root namespace for your client as well as some other settings, including some methods to customize the naming. Run the T4 template and a complete proxy for your OData v4 web API controllers will be generated.

You will have to remember to regenerate this client any time your contract changes. This client will enable us to make intelligent and deep assertions about the kind of data we expect to get back from the API. You will not need this VSIX to actually build the code, which means not all developers on the project will have to install it-- just the "caretakers" of OData metadata and object schema. This also means you won’t have to do any special incantations on the build server either.

## Setting Up The OWIN Self-Host

The Microsoft.Owin.Hosting NuGet package provides a simple static WebApp.Start method that has a signature like:

public static IDisposable Start(string url, Action startup);

![image00](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2016/01/self-hosted-integration-testing-of-odata-and-webapi/images/image00-300x208.png)

We can take advantage of injecting a `Action`  that has a Ninject kernel of our own making so that controllers created in this instance of our web app will get whatever mock objects we want to pass in. This also allows us to configure the OData service route to our liking (without security, for example). In the Tests project, there is a simple static TestHelpers class with a method for wiring this up to a passed in Ninject kernel:

internal static void ConfigureWebApi( IAppBuilder app, IKernel kernel )    {
      var config = new HttpConfiguration
      {
        IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always
      };
      config.MapODataServiceRoute( "TestOdataRoute",
          null,
          ModelBuilder.GetEdmModel() );
      config.EnsureInitialized();
      app.UseNinjectMiddleware( () => kernel );
      app.UseNinjectWebApi( config );
}

Notice that we are referencing an EDM model generator back in the web project so that we can be assured that OData yields that same model metadata as when our app runs normally. For debugging purposes, the IncludeErrorDetailPolicy is set to always include exception details within the scope of our OWIN hosted app.

Inside a unit test, we can now stand up a testable instance of our entire web app (including the OData WebApi controllers) in this fashion:

var kernel = new StandardKernel();
kernel.Bind().ToConstant( someMock.Object );
using ( WebApp.Start( "http://localhost:1234/", app => TestHelpers.ConfigureWebApi( app, kernel ) ) )
{
 // Make calls here
 // Controllers will use the mock bound by Ninject to fulfill requests
}

In the example code in GitHub, the ConfigureWebApi method also has an overload that uses the same Ninject kernel configuration as the normally-hosted web app so that you can write full end-to-end integration tests. See the `FullIntegrationTest.ItShouldReturnDataFromSql` test for an example of this.

## Writing Useful Integration Tests

![image01](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2016/01/self-hosted-integration-testing-of-odata-and-webapi/images/image01-300x225.jpg)

Now that we have the ability to encapsulate the web API inside of a unit test and make full-stack calls all the way down to the persistence layer, the tendency will be to write tests for everything. Initially, this will seem like you are increasing quality, but quickly you will realize that you have a fragile rat’s nest of deterministic tests that randomly fail if not run in the right order and you are spending more time tracing broken tests than actually writing new code. This is not a good place to be in. Instead, assume that your data persistence layer is solid (or just include some broad read-only smoke tests to make the bosses happy), and test against mock data service objects that are isolated and always return predictable results.

In the example code, the Cars controller has 100% full-stack test coverage using mock objects, without too much ceremony. This provides a high degree of security that changes to attributes of the model, business logic in the controller or OData configuration will be caught by our tests. To prove this assertion, remove the `[Required]` attribute from the Name property of the Car model and re-run the Car controller tests. `WhenPuttingCars.IfMissingRequiredFieldsItReturnsBadRequest` now fails. If your project is exposing a public OData API, this type of testing can help you identify breaking changes early on.

## Final Thoughts

Avoiding a ["not invented here"](https://en.wikipedia.org/wiki/Not_invented_here) mindset in RESTful web API development is definitely a good thing. However, by using a complex and convention-based stack such as OData v4 on top of ASP.Net WebApi to accomplish this goal, we can test too little of the API surface and thus find bugs at a later and more expensive time. By using the technologies provided in the OWIN self-host, automatic OData client generation and mocking, we can test the full stack of our API in a fast and easy fashion. Doing so means that we will find bugs in our code earlier (ultimately with less expense) and not pass accidental breaking changes to consumers of our APIs.
