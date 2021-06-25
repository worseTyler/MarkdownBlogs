---
title: "Upgrading ASP.NET Core to RTM"
date: "2016-07-20"
categories: 
  - "net"
  - "net-core"
  - "asp-net"
  - "blog"
  - "entityframework"
---

Here are the steps I followed to get an ASP.NET Core project upgraded from an RC1 with some RC2 beta bits up to RTM. 

This project is a full framework project without cross-platform capability. It uses Bower, NPM, and Gulp. I started the process by creating a scaffolded new project and based the upgrade on that. I also found it helpful to unload all the projects and get the project.json file to restore the packages one at a time.

Dan Haley and I worked on this together and he injected a few of his comments in the document.

I initially had the intent of changing this from a list to a more paragraph style document. However since this is likely a one-time journey that will likely need to be taken in pieces, I opted to leave the outline structure. I apologize if the numbers are a bit messed up.

# Solution Level Items

1. Change the global.json file
    
    "version": "1.0.0-preview2-003121"
    

# Getting the ‘Data/Domain’ project to work.

1. Project.json changes
    1. Note: there still seem to be quite a few tooling issues. Some are around the squiggly lines indicating problems in the source. Closing and opening the file seems to help. Sometimes shutting down VS and restarting helps as well. It seems to get stuck and even if you delete a part of the file it won’t clear the warning until you restart VS.
        1. I’ve had good luck with doing the quick-fix.  The only option is “Sort Properties”, but after doing that it appears to realize there was a change and restores packages successfully.
    2. Fix references
        1. EntityFramework.Core to "Microsoft.EntityFrameworkCore": "1.0.0"
        2. EntityFramework.MicrosoftSqlServer to     "Microsoft.EntityFrameworkCore.SqlServer": "1.0.0"
        3. EntityFramework.Relational to "Microsoft.EntityFrameworkCore.Relational": "1.0.0"
        4. Microsoft.AspNet.Identity.EntityFramework to "Microsoft.AspNetCore.Identity.EntityFrameworkCore": "1.0.0"
        5. Change frameworks from net451 to net46.
        6. Remove the tags, projectUrl, and licenseUrl tags that are deprecated at the top level.
    3. At this point I could not get my packages to restore correctly. I finally deleted the project.lock.json file and VS rebuilt it automatically. Then everything was fine. Again tooling isn’t quite there yet.
2. Namespace changes in code
    1. If you don’t know how to find the right ones, commenting out the ones in error and the using the ctrl-. shortcut to add the right using statement works great. The you can search and replace that.
    2. I did a global search and replace for the following
        1. Microsoft.AspNet.Identity.EntityFramework to Microsoft.AspNetCore.Identity.EntityFrameworkCore
        2. Microsoft.Data.Entity.Metadata to Microsoft.EntityFrameworkCore.Metadata
        3. Microsoft.EntityFrameworkCore to Microsoft.EntityFrameworkCore
        4. Microsoft.AspNet.Identity to Microsoft.AspNetCore.Identity
3. Other changes to make things easier
    1. I named my EF context AppContext. There is another framework object called this so I chose to rename my AppDbContext. 
    2. Be careful with the rename because it can mess up your project.lock.json file.
4. It is good to check the build targets
    1. Unload the project and edit the csproj file
    2. The outputs should be something like
        
        <BaseIntermediateOutputPath Condition="'$(BaseIntermediateOutputPath)'=='' ">.\\obj</BaseIntermediateOutputPath>
        <OutputPath Condition="'$(OutputPath)'=='' ">.\\bin\\</OutputPath>
        
    3. Note this this will have a warning. It does in the template solution as well.
    4. Dan - This change happened automatically for me after restarting VS.  So possibly after changing the SDK version in global.json a VS restart makes sense.  It didn’t happen in the web project automatically.
5. Fix the new table naming scheme to be singular by default
    1. By Default EF Core 1.0 uses the name of the DbSet collection in the DbContext for the table name.
    2. In AppDbContext.OnModelCreating
    3. foreach (var entity in builder.Model.GetEntityTypes())
        {
            entity.Relational().TableName = entity.DisplayName();
        }
        
    4. Requires using Microsoft.EntityFrameworkCore.Metadata.Internal;
6. If this turns out to be a little heavy handed the following can be placed on each class to specify the table name
7. \[Table(“NameOfTable”)\]
    

# Data Test project

1. Project.json fixes
    1. Similar changes to project.json file
    2. Fix Xunit dependencies
        
        "xunit": "2.2.0-beta2-build3300"
        "dotnet-test-xunit": "2.2.0-preview2-build1029"
        
    3. System.Linq to 4.0.0
    4. System.Linq.Queryable to 4.0.0
    5. Add the version number when referencing a project in the solution. This was a change from RC1.
    6. Commands: test: dotnet-test-xunit

# Getting the Web project to work

The search and replace actions from above were global, so they are not covered again here.

1. Fix the project.json file
    1. Dependencies
        1. Remove EntityFramework.Commands
        2. "Microsoft.AspNet.Authentication.Cookies" to "Microsoft.AspNetCore.Authentication.Cookies": "1.0.0"
        3. Microsoft.AspNet.Diagnostics.Entity to "Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore": "1.0.0"
        4. Microsoft.AspNet.Identity.EntityFramework to "Microsoft.AspNetCore.Identity.EntityFrameworkCore": "1.0.0"
        5. Remove Microsoft.AspNet.IISPlatformHandler
        6. Microsoft.AspNet.Mvc to "Microsoft.AspNetCore.Mvc": "1.0.0"
        7. Microsoft.AspNet.Mvc.TagHelpers to "Microsoft.AspNetCore.Mvc.TagHelpers": "1.0.0"
        8. Microsoft.AspNet.Server.Kestrel to "Microsoft.AspNetCore.Server.Kestrel": "1.0.0"
        9. Microsoft.AspNet.StaticFiles to "Microsoft.AspNetCore.StaticFiles": "1.0.0"
        10. Microsoft.AspNet.Tooling.Razor to 
            
             "Microsoft.AspNetCore.Razor.Tools": {
                 "version": "1.0.0-preview2-final",
                 "type": "build"
               },
            
        11. Microsoft.Extensions.CodeGenerators.Mvc to
            
               "Microsoft.VisualStudio.Web.CodeGeneration.Tools": {
                 "version": "1.0.0-preview2-final",
                 "type": "build"
               },
               "Microsoft.VisualStudio.Web.CodeGenerators.Mvc":  {
                 "version": "1.0.0-preview2-final",
                 "type": "build"
               }
            
        12. Microsoft.Extensions.Configuration.Json: “1.0.0”
        13. Microsoft.Extensions.Configuration.UserSecrets: "1.0.0"
        14. Microsoft.Extensions.Logging: "1.0.0”
        15. Microsoft.Extensions.Logging.Console: "1.0.0",
        16. Microsoft.Extensions.Logging.Debug: "1.0.0"
        17. Microsoft.VisualStudio.Web.BrowserLink.Loader: “14.0.0”
        18. Microsoft.Extensions.Options.ConfigurationExtensions: “1.0.0”
        19. Remove Microsoft.Extensions.Configuration.FileProviderExtensions
        20. Add: "Microsoft.AspNetCore.Server.IISIntegration": "1.0.0"
        21. Add: "Microsoft.ApplicationInsights.AspNetCore": "1.0.0”
        22. Add: "Microsoft.AspNetCore.Diagnostics": "1.0.0"
    2. Add tools section
        1. Note: Not all of these are required, like the BundleMinifier.Core if you are using Gulp.
        2.  "tools": {
               "BundlerMinifier.Core": "2.0.238",
            Not needed using Gulp
               "Microsoft.AspNetCore.Razor.Tools": "1.0.0-preview2-final",
               "Microsoft.AspNetCore.Server.IISIntegration.Tools": "1.0.0-preview2-final",
               "Microsoft.EntityFrameworkCore.Tools": "1.0.0-preview2-final",
               "Microsoft.Extensions.SecretManager.Tools": "1.0.0-preview2-final",
               "Microsoft.VisualStudio.Web.CodeGeneration.Tools": {
                 "version": "1.0.0-preview2-final",
                 "imports": \[
                   "portable-net45+win8"
                 \]
               }
             },
            
    3. Remove commands
    4. Add Scripts
        1. Note: I don’t think the first one is necessary if you are using gulp.
        2.  "scripts": {
               "prepublish": \[ "bower install", "dotnet bundle" \],
            Not needed using Gulp
               "postpublish": \[ "dotnet publish-iis --publish-folder %publish:OutputPath% --framework %publish:FullTargetFramework%" \]
             }
            
    5. Add RuntimeOptions
        
         "runtimeOptions": {
           "configProperties": {
             "System.GC.Server": true
           }
         },
        
    6. Add BuildOptions
        
         "buildOptions": {
           "emitEntryPoint": true,
           "preserveCompilationContext": true
         },
        
    7. Remove CompilationOptions
    8. Add PublishOptions
        
         "publishOptions": {
           "include": \[
             "wwwroot",
             "Views",
             "Areas/\*\*/Views",
             "appsettings.json",
             "web.config"
           \]
         },
        
    9. Remove PublishExclude
    10. Note: The exclude node has been moved, but the Roslyn analysis code doesn’t seem to look in this new location. If you need excluded folders, use exclude at the root level.
2. Namespace updates (global search and replace
    1. Microsoft.AspNet. With Microsoft.AspNetCore.
    2. Microsoft.AspNetCore.Identity.EntityFramework; to Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    3. Note that the ; at the end to prevent other items that have already been changed from changing again.
3. Create a Program.cs at the root of the project
    1. This is the new way to bootstrap your site.
        
           public class Program
           {
               public static void Main(string\[\] args)
               {
                   var host = new WebHostBuilder()
                       .UseKestrel()
                       .UseContentRoot(Directory.GetCurrentDirectory())
                       .UseIISIntegration()
                       .UseStartup<Startup>()
                       .Build();
                host.Run();        
                }
            }
        
4. Startup.cs changes
    1. Usings
        1. Microsoft.Data.Entity to Microsoft.EntityFrameworkCore
    2. From Startup
        1. Remove parameter IApplicationEnvironment appEnv
        2. Change .SetBasePath(appEnv.ApplicationBasePath) to .SetBasePath(env.ContentRootPath)
    3. Change IConfiguration from static to instance and change type to IConfigurationRoot
        1. This was a change from our original project and may not apply to other projects
    4. From ConfigureServices
        1. Change:
            1. services.AddEntityFramework().AddSqlServer().AddDbContext<AppContext>(options => options.UseSqlServer(Configuration\["Data:DefaultConnection:ConnectionString"\]));
            2. If you are deploying to Azure, this change will require a modification to your Azure settings to point to the right database.
            3. To: services.AddDbContext<AppDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
    5. From Configure
        1. Added back App Insights Telemetry items (see scaffolded project)
        2. Remove: app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());
    6. Remove Main, because it is now in Program.cs
5. Update Usings in classes
    1. HttpContext.User.GetUserId() has been removed. Here is an extension method.
        
               public static string GetUserId(this ClaimsPrincipal principal)
               {
                   if (principal == null) throw new ArgumentNullException(nameof(principal));
                   var claim = principal.FindFirst(ClaimTypes.NameIdentifier);
                   return claim != null ? claim.Value : null;
               }
        
6. AppSettings.json
    1. Change Data to
        
         "ConnectionStrings": {
           "DefaultConnection": "Server=(localdb)\\\\mssqllocaldb;Database=aspnet-CoreWebRtm-5bebb517-bfb0-4459-9e89-f6bd208cc3ce;Trusted\_Connection=True;MultipleActiveResultSets=true"
         },
        
    2. Note: ConnectionStrings is at the root, not under Data
    3. Change Logging: LogLevel: Default to Debug
7. In Properties
    1. Debug
        1. Change ASPNET\_ENV  to ASPNETCORE\_ENVIRONMENT
8. Web.config
    1. Add to aspNetCore
        1. forwardWindowsAuthToken="false"
    2. Change stdoutLogEnabled="false"
        1. stdoutLogFile=".\\logs\\stdout"
9. In project file
    1. Change <Import Project="$(VSToolsPath)\\DNX\\Microsoft.DNX.Props" to  <Import Project="$(VSToolsPath)\\DotNet\\Microsoft.DotNet.Props"
    2. Change <Import Project="$(VSToolsPath)\\DNX\\Microsoft.DNX.targets” to <Import Project="$(VSToolsPath)\\DotNet.Web\\Microsoft.DotNet.Web.targets"
10. When using GetRequiredService for dependency injection, change Resolver to HttpContext?.RequestServices?.

# Views

1. Add to \_ViewImports
    
    @using Microsoft.AspNetCore.Identity
    @inject Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration TelemetryConfiguration
    
2. Change User.IsSignedIn() to
    1. SignInManager.IsSignedIn(User)
    2. Add above: @inject SignInManager<AppUser> SignInManager 
    3. Note: AppUser is your user class.
3. Change User.GetUserName() to
    1. @UserManager.GetUserName(User)
    2. Add: @inject UserManager<AppUser> UserManager

# Other Issues

1. In the data project if you are using identity there are a few things that need to have length restrictions put in place. This causes migrations to fail with an error about the column not supporting an index.
    
    2. Limit field length to 256 characters
    3. Use: , maxLength: 256
    4. Example: NormalizedEmail = table.Column<string>(nullable: true, maxLength: 256),
    5. AspNetRoles
        1. Name
        2. NormalizedName
    6. AspNetUsers
        1. Email
        2. NormalizedEmail
        3. NormalizedUserName
        4. UserName
2. There are still significant tooling hassles
    1. A reboot of the system seemed to clear up some permissions issues
    2. Restarting Visual Studio seems to help in lots of cases
    3. There are some times when VS will complain about references that have been fixed. However, everything restores fine and runs.
    4. The exclude item in the project.json file is deprecated, but Roslyn still seems to use it. Node\_modules needed to be excluded so that it wouldn’t try to traverse down the tree to folders more than 260 characters, the Windows limit.
    5. Deleting a .lock.json file often cleans up dependency issues. In some cases VS seems to get confused as to when to restore. Deleting this file makes it very clear and VS will automatically restore them. Some reference errors you get will be in this file even after you have fixed your project.json and a ‘restore’ has happen in VS.
    6. The bottom line is that not all problems are ‘rationally’ solved. The old reboot/restart magic is alive and well, to frustrating effect.
3. Getting EF migrations working
    1. Follow these instructions to get migrations working
        1. https://benjii.me/2016/06/entity-framework-core-migrations-for-class-library-projects/
    2. In the data project’s project.json
        1. add to dependencies section
            1. "Microsoft.EntityFrameworkCore.Design":  "1.0.0-preview2-final"
        2. To the tools section
            1. "Microsoft.EntityFrameworkCore.Tools": "1.0.0-preview2-final"
    3. In order for EF to work right now this needs to be an executable project. We need an entry point.
        1. Add to project.json
            
            buildOptions: {
               "emitEntryPoint": true,
               "preserveCompilationContext": true
            }
            
    4. Add a Program.cs
        
        public static void Main(string\[\] args) { }
        
    5. Add a Context Factory
        
        public class AppDbContextFactory : IDbContextFactory<AppDbContext>
        {
           public AppDbContext Create(DbContextFactoryOptions options)
           {
               var builder = new DbContextOptionsBuilder<AppDbContext>();
               builder.UseSqlServer(
        "Server=(localdb)\\\\mssqllocaldb;Database=Migrations;Trusted\_Connection=True;MultipleActiveResultSets=true");
               return new AppDbContext(builder.Options);
           }
        }
        
    6. Migration Commands
        1. From the folder of the data project
        2. Add: dotnet ef migrations add \[MigrationName\] -c AppDbContext
        3. Remove: dotnet ef migrations remove
        4. Update to current migration: dotnet ef database update
        5. Update Database to specific migration: dotnet ef database update \[MigrationName\]
    7. Existing migrations will need the following using statement replacements:
        1. Microsoft.Data - Remove
        2. Microsoft.Data.Entity - Microsoft.EntityFrameworkCore
        3. Microsoft.Data.Entity.Infrastructure - Microsoft.EntityFrameworkCore.Infrastructure
        4. Microsoft.Data.Entity.Metadata - Microsoft.EntityFrameworkCore.Metadata
        5. Microsoft.Data.Entity.Migrations - Microsoft.EntityFrameworkCore.Migrations
4. Change the JSON serializer back to the Pascal case default
    1. The serializer was updated to do camel case now by default. The following code should revert to the old way.
        
        services.AddMvc()
          .AddJsonOptions(opt =>
          {
            var resolver  = opt.SerializerSettings.ContractResolver;
            if (resolver != null)
              (resolver as DefaultContractResolver).NamingStrategy = null;
          });
        

 

# Getting Test Projects to Work

1. Unload the project and edit the .xproj file.
    1. Replace DNX\\Microsoft.DNX.Props with DotNet\\Microsoft.DotNet.Props
    2. Replace DNX\\Microsoft.DNX.targets with DotNet\\Microsoft.DotNet.targets
    3. Make sure the BaseIntermediateOutputPath and OutputPath are set as mentioned above.
2. Edit the project.json file.
    1. Remove the commands node.
    2. Add “testRunner”: “xunit” at the root level.
    3. Replace xUnit dependencies:
        
        "xunit": "2.2.0-beta2-build3300"
        "dotnet-test-xunit": "2.2.0-preview2-build1029"
