---
title: "Docker: Adding PostgreSQL to .Net Core - Part 2"
date: "2019-03-06"
categories: 
  - "blog"
tags: 
  - "net-core"
  - "angular"
  - "c-sharp"
  - "c"
  - "docker-dockerize"
  - "dot-net"
  - "dot-net-core"
  - "postgres"
  - "postgresql"
---

## Add Postgres To .NET Core

Estimated reading time: 8 minutes

We’ll break down what you need to know about Postgres and build out our API code in this installment on Docker.

So far, we’ve written some basic startup scripts for our Gadget Depot project. You can get the final project template [here](https://github.com/the-fool/Dotnet-Postgres-Docker).

We had scaffolded all the Docker features of the app, but now we need to hack on the application source code to get things up and running. Out of the box, .NET Core is not expecting to work with PostgreSQL - this is the first issue we're going to fix.

Adding PostgreSQL to .NET is a relatively simple procedure:

- Include a reference to the .NET PostgreSQL package
- Add a connection string in our app’s configuration
- Register the PostgreSQL service in the ASP.NET startup.

### Contents

- [Add Postgres To .NET Core](#h-add-postgres-to-net-core)
    - [Add the Npgsql Dependency](#h-add-the-npgsql-dependency)
    - [Configure .NET Database Connection](#h-configure-net-database-connection)
    - [Add the Npgsql Entity Framework Service](#h-add-the-npgsql-entity-framework-service)
    - [Build the API](#h-build-the-api)
    - [Model a Gadget](#h-model-a-gadget)
    - [Include Gadgets in the Database Context](#h-include-gadgets-in-the-database-context)
    - [Initialize the Database](#h-initialize-the-database)
    - [Write an API Controller for Gadgets](#h-write-an-api-controller-for-gadgets)
    - [Build an Angular Web Client](#h-build-an-angular-web-client)
    - [Wrap-up](#h-wrap-up)
    - [Want More?](#h-want-more)

### Add the Npgsql Dependency

To teach .NET how to interface with PostgreSQL, we're going to add the [Npgsql](https://www.npgsql.org/efcore/index.html) library by adding the reference to Npgsql to your `.Backend/GadgetDepot/GadgetDepot.csproj` file:

<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>netcoreapp2.1</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <Folder Include="wwwroot\\" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.App" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="2.1.2" />
  </ItemGroup>

</Project>

That's all for added dependencies!

Next, we need to give our app a connection string for the dockerized PostgreSQL database. This connection string specifies the username, password, host address and database name for our connection.

### Configure .NET Database Connection

Update your `appsettings.json` to resemble the following:

{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  },
  "ConnectionStrings": {
    "DbContext": "Username=postgres;Password=postgres;Server=db;Database=gadget"
  }
}

Notice the key=value: `Server=db`. Where does the hostname `db` come from? This is just the name we gave our database service in the `docker-compose.yml`. Internally, Docker sets up a kind of DNS for addressing services from within the networked containers, where each service's name functions as its hostname. So, directing the .NET program to the hostname `db` will send it straight toward the PostgreSQL instance.

### Add the Npgsql Entity Framework Service

The last bit of code needed to set up our PostgreSQL connection in the .NET app is an Entity Framework adapter.

We'll add this adapter service to the `Startup` class in the `Backend/GadgetDepot/Startup.cs` file. Update the `ConfigureServices` method in your `Startup` class so that it includes the call to the `IServiceCollection.AddEntityFrameworkNpgsql` method, making use of the connection string we created above.

public class Startup
{
  public Startup(IConfiguration configuration)
  {
    Configuration = configuration;
  }

  public IConfiguration Configuration { get; }

  public void ConfigureServices(IServiceCollection services)
  {
    services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version\_2\_1);

    //
    // Add this following call to provide PostgreSQL support
    //
    services.AddEntityFrameworkNpgsql().AddDbContext<DbContext>(options =>
      options.UseNpgsql(Configuration.GetConnectionString("DbContext")));

  }

  public void Configure(IApplicationBuilder app, IHostingEnvironment env)
  {
    if (env.IsDevelopment())
    {
      app.UseDeveloperExceptionPage();
    }
    else
    {
      app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseMvc();
  }
}

That'll do it! Now you have an ASP.NET Core app communicating with PostgreSQL.

Congrats!

All that remains is to load up some gadgets and display them in the Angular app. Gadget Depot is going to have its deadline met.

### Build the API

Time to build out the REST API for our gadgets. This section isn't really about PostgreSQL or Docker in particular, so we will stay at a high level and move quickly without getting bogged down in details.  We are going to declare a model, alter our database to support this model and write a basic REST API controller. For a deeper look into how to develop REST APIs with ASP.NET Core, take a gander at the [official Microsoft docs](https://docs.microsoft.com/en-us/aspnet/core/data/ef-rp/intro?view=aspnetcore-2.2&tabs=visual-studio).

### Model a Gadget

For our requirements, a gadget is just a name and nothing else. To model it in our app, go to the `GadgetDepot/Models` directory and add a new class `Gadget` in `Gadget.cs`:

namespace GadgetDepot.Models
{
    public class Gadget
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

That'll do just fine. Next, we need to integrate this model declaration with our database. To do this, we need to implement the `DbContext` interface with our new `Gadget` class taking a leading role.

### Include Gadgets in the Database Context

Create the file `Backend/GadgetDepot/ApiDbContext.cs`, and write our custom Gadget Depot `DbContext` to include the `Gadget` model:

using Microsoft.EntityFrameworkCore;
using GadgetDepot.Models;

namespace GadgetDepot
{
  public class ApiDbContext : DbContext 
  {
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }

    public DbSet<Gadget> Gadgets { get; set; }
  }
}

This class declares that our app's persistence layer has a set of gadgets that we will implement in PostgreSQL as a single table.

To use this context, swap out the `DbContext` that we declared in the `Startup` class. Be sure to add a `using GadgetDepot.Models` at the head of the file, then make the change to the `ConfigureServices` method in the `GadgetDepot.Startup` class:

public void ConfigureServices(IServiceCollection services)
{
  services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version\_2\_1);

  services.AddEntityFrameworkNpgsql().AddDbContext<ApiDbContext>(options =>
      options.UseNpgsql(Configuration.GetConnectionString("DbContext")));
}

As a result of this change, Entity Framework Core now expects a table called "Gadgets" in the PostgreSQL database. At this stage, we're primed to make a migration file and update the schema of the database.

Navigate your shell to the `Backend/GadgetDepot` project and run the following commands to create and apply a migration.

docker run -v $(pwd):/app -w /app microsoft/dotnet dotnet ef migrations add Initial
docker run -v $(pwd):/app -w /app microsoft/dotnet dotnet ef database update

### Initialize the Database

As a final flourish, we can insert some test gadgets into the database.  Create the file `Backend/GadgetDepot/DbInitializer.cs` with the following class:

using GadgetDepot.Models;
using System.Linq;

namespace GadgetDepot
{
  public static class DbInitializer
  {
    public static void Initialize(ApiDbContext ctx)
    {
      ctx.Database.EnsureCreated();
      if (!ctx.Gadgets.Any())
      {
        ctx.Gadgets.Add(new Gadget { Name = "plumbus" });
        ctx.Gadgets.Add(new Gadget { Name = "flux capacitor" });
        ctx.Gadgets.Add(new Gadget { Name = "spline reticulator" });
        ctx.SaveChanges();
      }
    }
  }
}

The static `Initialize` method will programmatically make sure that the database `gadget` is existent, and if the "Gadgets" table is empty, it will add a few test rows.

We can make this method run at startup by inserting a call to `Initialize` in the `Main` method of the `Program` class. The method has a dependency on a database context, so we need to get that context from within `Main`.

Alter your `Backend/GadgetDepot/Program.cs` file to mimic the following:

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GadgetDepot 
{
  public class Program 
  {
    public static void Main(string\[\] args)
    {
      var host = CreateWebHostBuilder(args).Build();
      using(var scope = host.Services.CreateScope())
      {
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<ApiDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
          DbInitializer.Initialize(context);
        }
        catch (Exception ex)
        {
          logger.LogError(ex, "An error occurred creating the DB.");
        }
      }

      host.Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string\[\] args) =>
      WebHost.CreateDefaultBuilder(args)
        .UseStartup<Startup>();
  }
}

The trick here is to create a scope wherein we can access the services -- most importantly, an `ApiDbContext` instance. Passing this context into the `DbInitializer.Initialize` method allows it to make a connection with the database, and execute its routine.

Note that this `Initialize` call will run _every time_ the app boots. In the future, we might want a more sophisticated way to condition whether or not we want this code to run, but for the sake of immediate development, this is good enough for Gadget Depot.

### Write an API Controller for Gadgets

To round out our API, we need to add a controller class for exposing the gadget data. As with the steps it took to provision the database, you can find an in-depth guide to this facet of ASP.NET programming in [other tutorials](https://docs.microsoft.com/aspnet/core/tutorials/first-web-api).

We'll just copy in the code so we can get our Gadget Depot app delivered on schedule!

Create the file `Backend/GadgetDepot/Controllers/GadgetController.cs`:

using System;
using System.Collections.Generic;
using System.Linq;
using GadgetDepot.Models;
using Microsoft.AspNetCore.Mvc;

namespace GadgetDepot.Controllers 
{
  \[Route("api/\[controller\]")\]
  \[ApiController\]
  public class GadgetsController : ControllerBase
  {
    ApiDbContext \_ctx;

    public GadgetsController(ApiDbContext ctx)
    {
      \_ctx = ctx;
    }

    \[HttpGet\]
    public ActionResult<IEnumerable<Gadget>> Get()
    {
      return \_ctx.Gadgets.ToList();
    }
  }
}

Now `localhost:5000/api/gadgets` returns our list of gadget inventory in nice, JSONified form. All that's left to do is make our Angular app consume this API.

### Build an Angular Web Client

So long to the .NET code.

Move over to the `frontend` directory and get ready to write an Angular app.

Truth be told, it's not going to be much of an app at all, and Angular is certainly overkill for what we're setting out to accomplish. But, it's easy to set up using Docker and the `ng` tool, so we may as well lay a good foundation for future iteration on Gadget Depot's web app.

Change the `src/app/app.component.ts` file so that the main component will connect to the REST API backend:

import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

interface Gadget {
  id: number;
  name: string;
}

@Component({
  selector: 'app-root',
  template: \`
  <h1>Gadget Depot</h1>
  <ul>
    <li \*ngFor="let gadget of gadgets">
      {{ gadget }}
    </li>
  </ul>
  \`
})
export class AppComponent implements OnInit {
  gadgets: string\[\] = \[\];

  constructor(private http: HttpClient) { }

  ngOnInit() {
    this.http.get<Gadget\[\]>('http://localhost:5000/api/gadgets')
      .subscribe(gs => {
        this.gadgets = gs.map(g => g.name);
      });
  }
}

Be sure to import the `HTTPClientModule` in your `AppModule`, and your Gadget Depot client is officially minimally viable. Which is to say: it works!

Ship it to Gadget Depot - we're done here!

### Wrap-up

We used Docker to build a whole full-stack web app. ASP.NET Core, PostgreSQL and Angular all working together right out of the box.

Kudos!

To run your app, all that needs doing is a call to `docker-compose` in the root directory:

docker-compose up

Watch as all your services network together and spring to life in a cozy, containerized world unto themselves.

### Want More?

In our first [blog](https://intellitect.com/docker-scaffold/) in our series, we dug deep into all things Docker and learned how to scaffold our app. Make sure to check it out if you need a refresher!

![](images/Blog-job-ad-1024x127.png)
