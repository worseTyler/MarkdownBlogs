

## Need to do more complex routing with ASP.NET Core? Try Constraints:
#

### Constraints are a solution for serving Angular apps from a hybrid MVC application.

We recently had a unique challenge of serving up an angular app from an MVC application. This app needed to serve other MVC pages along with the angular site. Additionally, the routes that would serve the angular site would be configurable by the user in the format of /[site]/[page].

Our app had a dynamic list of sites, and each site could have a set of pages. However, this was entirely handled by the angular site's routing. When a URL was requested that started with a valid [site], the angular page needed to be returned. If the [page] didn’t exist, that was the angular app's responsibility.

The routing in ASP.NET Core was set-up at startup, so doing traditional routes didn't make sense because it would require a restart of the app every time a new site was added by the user. We turned to routing constraints to solve this issue.

### Startup.cs

A constraint allows you to provide code to validate a route before it is assigned to a controller. First, we set this up in our startup.cs configure method. This included the standard MVC routing so that other URLs will be served. Note that static files were served before this routing code is executed.

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    // ...
    app.UseStaticFiles();
    app.UseAuthentication();

    app.UseMvc(routes =>
    {
        routes.MapRoute(
            name: "site",
            template: "{site}/{page?}",
            constraints: new { site = new SiteRouteConstraint() },
            defaults: new {controller="Site", action="Index"}
        );

        routes.MapRoute(
            name: "default",
            template: "{controller=Home}/{action=Index}/{id?}"
        );
    });
}

```

The configuration of the 'site' route (above) accepted two URL parts: ‘site’ and ‘page,’ and could be expanded as necessary. Also, ‘page’ was only a placeholder. Next came the constraint. First, we specified the part of the URL we wanted to validate. In our case, it was the 'site' portion. This parameter was passed to the constraint as the routeKey. During execution, a SiteRouteConstraint was created, and the Match method was called.

### The Constraint Class

In our case, the constraint class looked like this.

```csharp
public class SiteRouteConstraint : IRouteConstraint
{
    public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
    {
        var site = values[routeKey]?.ToString();
        return SiteService.SiteExists(site);
    }
}
```

Here the routeKey was the string 'site' which was configured in the startup. The route values dictionary contained all the parsed parameters from the URL based on the parsing of the URL and default controller and action parameters. With a URL of/site1/page1, the values dictionary would contain site and page containing 'site1' and 'page1' respectively. It would also contain controller='Site' and action='Index'. We looked up the value of the 'site' parameter in the dictionary and passed that to our magical service. It returned 'true' if the site existed and 'false' if it didn't. When false was returned from the constraint, the router fell through to the next route.

### The Controller

As you may have noticed in the Configure method, the controller and action have been set irrespective of the site and page. This was because the angular routing handled this for us. We needed to make sure that the URL on the client browser didn't get changed by MVC as a redirect. We compiled our angular application to our wwwroot folder. Next, we needed to serve the file. Here is the SiteController class we used to return the index.html file from the root of our wwwroot folder.

```csharp
public class SiteController : Controller
{
    public IActionResult Index()
    {
        return File("index.html", "text/html");
    }
}
```

While not applicable in every case, constraints are a straightforward way to handle more complex routing scenarios, especially where one needs to be able to validate routes at runtime.
