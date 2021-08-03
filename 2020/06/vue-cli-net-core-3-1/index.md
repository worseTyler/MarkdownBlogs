

Estimated reading time: 2 minutes

Vue is becoming a very popular front-end framework, but .NET Core 3.1 still doesn't officially support it. Thanks to VueCliMiddleware, it only takes a couple of steps to get set up.

We will start with an empty ASP.NET Core 3.1 web application template, use the Vue CLI to generate a Vue Client App and then use VueCliMiddleware to bring it all together.

### Prerequisites and Versions Used

- [Node](https://nodejs.org/en/download/) 12.18.1
- [Vue CLI](https://cli.vuejs.org/) 4.46
- [ASP.NET Core](https://dotnet.microsoft.com/download) 3.1
- [Visual Studio Code](https://code.visualstudio.com/download)

In a command window, start a new ASP.NET Core web app and move to that web app directory.

```csharp
dotnet new web -o VueApp
cd VueApp
```

Use the Vue CLI to create the Client App.

```csharp
vue create client-app
```

Add the required NuGet packages and open up the project in VS code.

```csharp
dotnet add package VueCliMiddleware
dotnet add package Microsoft.AspNetCore.SpaServices.Extensions
code .
```

Hot Modal Reloading (HMR) will not work in HTTPS with a self-signed certificate, so in launchSettings.json, swap HTTPS and HTTP so it will start as HTTP. This is only for use in a development setting. You can later configure your server to use HTTPS.

Modify startup.cs to use the VueCliMiddleware.

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VueCliMiddleware;

namespace ToDoApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSpaStaticFiles(opt => opt.RootPath = "client-app/dist");
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapToVueCliProxy(
                    "{\*path}",
                    new SpaOptions { SourcePath = "client-app" },
                    npmScript: (System.Diagnostics.Debugger.IsAttached) ? "serve" : null,
                    regex: "Compiled successfully",
                    forceKill: true
                    );
            });
        }
    }
}
```

Press F5 to run the app.

MapToVueCliProxy will start up the Vue Client App using Node and then use a proxy to send unmapped requests to the Vue Client App.

Now, you can enjoy the Vue.

### Using an Older Framework?

Check out [my blog](https://intellitect.com/vue-cli-net-core-3-1/) for the steps to connect .NET Core 2.1 SDK to Vue.

![](https://intellitect.com/wp-content/uploads/2021/04/Blog-job-ad-1024x127.png)
