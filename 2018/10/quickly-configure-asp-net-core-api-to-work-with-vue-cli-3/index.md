

 "Quickly Configure ASP.NET Core API to work with Vue CLI 3!"

## Use Vue CLI 3 with all the functionality of ASP.NET Core.

### \*\*\*UPDATE\*\*\*

**Using a newer framework? Check out [my blog](/vue-cli-net-core-3-1/) for steps using ASP.NET Core 3.1.**

In this tutorial, we will use the .NET WebAPI template to generate an API back-end and Vue CLI 3 to create the front-end and get them to work together.

Here is a link to my working version on GitHub [https://github.com/ykravtsov/vue\_app](https://github.com/ykravtsov/vue_app).

### Prerequisites

- [Node](https://nodejs.org/en/download/)
- [.Net Core 2.1 SDK](https://www.microsoft.com/net/download/archives)
- [Visual Studio Code](https://code.visualstudio.com/download)
- [C# for Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=ms-vscode.csharp)

### Let's Get Coding

- Install the [Vue CLI](https://cli.vuejs.org/) if you haven't done so already
- We're going to set up the .NET webAPI template for the API back-end and the Vue CLI template for the front-end
    - Open a console and run these commands:

```
dotnet new webapi -o vue\_app
vue create vue\_app
```

- Choose the merge option and follow the prompt for setting up Vue
- Install two node libraries needed for the ASP Hot Modal Reloading (HMR) then open the project in VS Code
    - Note: choose npm or Yarn to install the two node libraries needed for ASP HMR

```
cd vue\_app

npm install -D aspnet-webpack webpack-hot-middleware
# or
yarn add aspnet-webpack webpack-hot-middleware --dev

code .
```

Since we are going to be using the ASP.NET HMR, we’re going to redirect Vue to output to wwwroot and not run its version of HMR.

- Start by creating a file in the root directory named `vue.config.js` and add this content:

```
module.exports = {
    outputDir: 'wwwroot',
    baseUrl: "/",
    chainWebpack: config => {
        // aspnet uses the other hmr so remove this one
        config.plugins.delete('hmr');
    }
}
```

- We are going to configure the ASP HMR
    - replace the code in Startup.cs with this:

```
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace vue\_app
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version\_2\_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true,
                    ConfigFile = Path.Combine(env.ContentRootPath, @"node\_modules\\@vue\\cli-service\\webpack.config.js")
                });
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // here you can see we make sure it doesn't start with /api, if it does, it'll 404 within .NET if it can't be found
            app.MapWhen(x => !x.Request.Path.Value.StartsWith("/api"), builder =>
            {
                builder.UseMvc(routes =>
                {
                    routes.MapSpaFallbackRoute(
                        name: "spa-fallback",
                        defaults: new { controller = "Home", action = "Index" });
                });
            });
        }
    }
}
```

Now, let's go through what we just pasted into Setup.

The code below sets up ASP HMR and points to the webpack file that Vue CLI 3 generates.

```
app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
{
    HotModuleReplacement = true,
    ConfigFile = Path.Combine(env.ContentRootPath, @"node\_modules\\@vue\\cli-service\\webpack.config.js")
});
```

This enables serving static files:

```
app.UseStaticFiles();
```

This tells the router to use the MVC controller if it exists:

```
app.UseMvc(routes =>
{
    routes.MapRoute(
        name: "default",
        template: "{controller=Home}/{action=Index}/{id?}");
});
```

This last part will send all the other routes that weren't matched by MVC to our SPA unless it starts with /api, in which case we want to throw a 404 error.

```
// here you can see we make sure it doesn't start with /api, if it does, it'll 404 within .NET if it can't be found
app.MapWhen(x => !x.Request.Path.Value.StartsWith("/api"), builder =>
{
    builder.UseMvc(routes =>
    {
        routes.MapSpaFallbackRoute(
            name: "spa-fallback",
            defaults: new { controller = "Home", action = "Index" });
    });
});
```

- `Crate HomeController.cs` in the Controllers folder and add this content:

```
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace vue\_app.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return File("~/index.html", "text/html");
        }

        public IActionResult Error()
        {
            ViewData\["RequestId"\] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }
    }
}
```

We are ready to test it!

- Hit F5 to fire up the app.

Now we can build our Vue app with an ASP.NET Core API!
