

## **Microsoft Will Simplify Web API Authentication in a Soon-to-Be-Released Update!**
#
**UPDATE:** Michael Stokesbary spoke at the Spokane .NET Users Group about the now released update. [Click here](/video-asp-net-core-2-2/) for the video.

This week’s release of ASP.NET 2.2 gave us some performance enhancements, ability to do health checks on the application and code analyzers to help you improve your web API discoverability. Expect an out of band release that creates a dependency on IdentityServer in the near future.

Microsoft will be simplifying the process of authentication in the ASP.Net Core 2.2 release by adding a dependency on IdentityServer. They’ve been making improvements on security for many versions now. The ASP.NET Core team has simplified development by making everything a claim in the current version of MicrosoftIdentity (version 3) which is the version re-written for .NET Core. This change means that an IClaimsIdentity is returned for all authenticated users, no matter how authentication is performed. Developers no longer need to look for a WindowsIdentity or a ClaimsIdentity, now they can assume that everything is a ClaimsIdentity and proceed from there.

Individual user accounts (or forms-based authentication) have also received specific changes from Microsoft. In the .NET Core 2.1 update, a simple way of authentication was added to make it a Razor class library. This update allowed a developer to add authentication to their application without having to manually add all the UI elements that were necessary for user management, such as logging in and out, as well as user registration. Unfortunately, Microsoft hasn’t focused on how to secure Web API applications –until now.

### **How Change Authentication works in ASP.NET Core 2.1**

In the ASP.Net Core 2.1 release, users can click the “Change Authentication” button when creating a Web API project in Visual Studio and select “Individual User Accounts” as an option, but then they were presented with a screen to connect to an existing Azure AD B2C application (see screenshot).

![](https://intellitect.com/wp-content/uploads/2018/11/Stokes-Screenshot1.png)

These steps added the following line to the existing code and required extra configuration to get things working. In particular, it needed an Azure Service to be created/configured.

![](https://intellitect.com/wp-content/uploads/2018/11/Stokes-Code-2.png)

The code created also didn’t allow developers to use the same model for Web API authentication as they could use for current web applications (using the forms-based authentication). This paradigm also prevented user credentials from being stored in a local database. Previously, if the developer wanted to store credentials locally on their server, they had to implement some type of OpenID Connect Server to handle authentication so the Web API could be secured by bearer tokens. The process could have been completed through third-party libraries like [IdentityServer](https://identityserver.io/) or [OpenIDDict](https://github.com/openiddict/openiddict-core), but the developer would still have been required to set up and configure it correctly. There were not “turn-key ready” ways of doing authentication.

Microsoft will be using a tried and true product in their 2.2 update by adding a dependency on IdentityServer and providing a basic configuration around it, rather than roll out their own solution. The goal, I believe, is to use a product that developers are familiar with, that can be extended (or replaced) if the need arises, but should also make configuration a lot simpler so things can get up and running quickly.

Once updated, instead of creating a separate web application and making it a dedicated authentication server (which required the developer to create the forms-based authentication screens as well as add the dependency and configuration correctly for IdentityServer), the developer only needs to add some services and configuration to their newly created Web API application.

### **How to Implement a Secure Web API in ASP.NET Core 2.2**

Although ASP.NET Core 2.2 has been released, the future out of band update will have a new package called \`Microsoft.AspNetCore.ApiAuthorization.IdentityServer\`. After taking a dependency on this package, one can secure their API by adding the following simple code to the ConfigureServices method (Note: it should be added after the `AddDbContext` and `AddDefaultIdentity` calls):

![](https://intellitect.com/wp-content/uploads/2018/11/Stokes-Code3.png)

### **Beginner notes:**

- The `AddIdentityServer` method is a standard method that comes from the IdentityServer APIs.
- The `AddAuthentication` method is a standard method when just adding authentication to an application.
- Pay special attention to the `AddApiAuthorization` and `AddIdentityServerJwt` methods. They are available to the application when referencing `Microsoft.AspNetCore.ApiAuthorization.IdentityServer`.

`AddApiAuthorization` does the heavy lifting for configuring the above IdentityServer options. While not a part of IdentityServer, this new Microsoft method takes care of the configuration that, until now, were manual. This method, under the covers, is what is responsible for calling `AddIdentityResources`, `AddApiResources`, `AddClients` and `AddSigningCredentials`. For those familiar with IdentityServer, it should be pointed out that this just uses the [InMemory](https://identityserver.github.io/Documentation/docsv2/configuration/inMemory.html) version of each of those calls. In the past, a developer would have also called `AddConfigurationStores` to create database tables to store this information, but now all configuration data is stored in the `appsettings.json` files.

In a future blog post I’ll show you how to configure all of these options, but for now, at a minimum, the following two entries in the `appsettings.json` file should get things up and running once you begin using ASP.Net Core 2.2:

```csharp
“IdentityServer”: {
   “Key”: {
      “Type”: “Development” // Creates a Development signing certificate
   },
   “Clients”: {
      “ApiAuthSampleSPA”: { // The name of your client application
         “Profile”: “IdentityServerSPA” // Specifies to configure based on this application being an IdentityServerSPA
      }
   }
}
```

The `AddIdentityServerJwt` method will configure the necessary pieces so that the IdentityServer application knows how to host the secure Web API and authentication service in the same web application. This step will be essential because the authentication server is usually a separate entity from the APIs it’s securing. Microsoft didn’t create a “hack” (IdentityServer has supported this type of functionality for a while now), but it takes some extra setup on the developer’s end to get things working correctly. In this new release, Microsoft is taking care of the additional configuration that needs to happen to allow for this behavior.

After configuring the application, you will need to use the services you added, which means you will need to add the following line in your Configure method (below the `UseStaticFiles` method):

![](https://intellitect.com/wp-content/uploads/2018/11/Stokes-code4.png)

Once complete, the developer will be able to run their application to launch a Web API application and have it backed by a configured instance of IdentityServer. Bearer token authentication will be able to be used for making secure calls to their Web API methods.

### **Configuring IdentityServer will be simpler** with .**Net Core 2.2**

It won't require a separate web application to be created just for doing authentication, and Microsoft will be defaulting a lot of the necessary configuration so getting the endpoints up and running will take minimal effort.

One goal of this release is to make it much easier to build secure web API’s using IdentityServer. While the use of IdentityServer won’t be required (Using what you’ve used in the past will still work), this process will simplify a developer’s life by making it so one won’t have to understand the inner workings of an OpenID Connect authentication service to write secure code. For more about this, [click here](https://github.com/aspnet/Announcements/issues/307).

As of writing this article, .Net Core 2.2 has been released, but the above functionality isn't available. [Click here](https://github.com/aspnet/identity/tree/release/2.2) to see the work-in-progress code.
