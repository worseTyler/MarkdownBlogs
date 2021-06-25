---
title: "Using Nightly Builds for ASP.NET Core 1.0"
date: "2016-01-27"
categories: 
  - "blog"
---

> _Note: ASP.NET Core 1.0 is Microsoft's official branding of ASP.NET 5 (vNext). The renaming is not complete and thus the exact version numbers below in the examples reflect RC2 before the transition. However, the process should still be accurate. This page will be updated once the rename is complete._

# Overview

ASP.NET Core is an open source project. As a result, it is possible to use builds that are more current than those that are officially released: RC1, RC2, RTM, etc. These are typically called daily or nightly builds. While there may not be new builds exactly every night, the idea is that these builds represent the most current builds of the software. This is bleeding edge territory and can cause lots of headaches. However, if you are struggling with a problem and that issue has been fixed already but not released, these builds are the way to get that feature. This process is not for the faint of heart, but once conquered can provide some great benefits.

# Get an Unstable DNX

The first step to nightly builds is to get the most recent version of DNX. Fortunately that is easy with the dnvm tool. Run the following command at the command prompt to upgrade.  `dnvm upgrade -u` That -u means unstable. You've been warned.

You can use `dnvm list` to show the current versions of DNX you have installed. The one with the \*(asterisk) is the one that is active in your command window. The one with the alias of 'default' is what Visual Studio will use for running your project. However its Package Manager Console and Task Runner Explorer (Gulp) will use the one with the \* (asterisk). Search on dnvm to find more information about setting current and default configurations.

Run `dnvm list` and note the version, you will need it later in Visual Studio to configure your project.

# Configure Your Solution

Now that the latest version of dnvm is installed, you need to update your project to use that version of DNX. This is done via the solution level global.json file. If you don't have one in your project, add it as a JSON text file at the solution level. Here is sample content

{
  "sdk": {
    "version": "1.0.0-rc2-16357"
  },
  "Projects": \[ "src" \]
}

The 1.0.0-rc2-16357 should be replaced with whatever DNX version you noted in the previous step.

Note that if you open this project on another computer, Visual Studio will download the version of DNX specified in the global.json file for you.

Additionally, the Projects node of the global.json file specifies the places that should be searched for source code. If you want to debug external source for a DNX project, add the source location to this array. You can then add the project that you want to debug to your solution. You may need to remove the version number from the project.json file and replace it with an empty string "". This will cause Visual Studio to use the version in the solution regardless of the explicit version number on the project.

# Project References

Now comes the tedious process of updating your references. Make sure you do this with source control so you can easily get back to a known-good state.

The first step is to add the daily MyGet feed to Visual Studio so it knows where to find the latest bits. Go into the Visual Studio options and search for NuGet. Choose Package Sources and add the following source.

https://www.myget.org/F/aspnetvnext/api/v3/index.json
﻿

An option that will be independent of Visual Studio is to use a NuGet.config file. This is an XML file that is at the solution root. It is necessary for remote builds so the build process knows where to get the referenced packages. Here is an example NuGet.config file.

<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <packageSources>
        <add key="NuGet" value="https://api.nuget.org/v3/index.json" />
        <add key="DailyBuilds" value="https://www.myget.org/F/aspnetvnext/api/v3/index.json" />
    </packageSources>
</configuration>

Now that we can access the latest packages, we need to update our project.json files to use them. There are two ways to go about this.

1. Take the most current version and automatically get updates as they come out.
2. Take the most current version and stick with that version.

Note: While getting a non-current version is possible, it is very difficult to get all the dependencies right. If you like a version, I would recommend locking in at that version and checking your project.json files with all the right version numbers in to source control. Additionally, the NuGet packages for the nightly builds don't seem to stay up there forever, so locking in is a short term step before another release is done.

To get the latest versions, update your project.json file to look something like this.

 "dependencies": {
    "Microsoft.AspNet.IISPlatformHandler": "1.0.0-rc2-\*",
    "Microsoft.AspNet.Server.Kestrel": "1.0.0-rc2-\*",
    "Microsoft.AspNet.Mvc": "6.0.0-rc2-\*",
    "Microsoft.AspNet.Mvc.TagHelpers": "6.0.0-rc2-\*",
    "Microsoft.AspNet.Tooling.Razor": "1.0.0-rc2-\*",
    "Microsoft.AspNet.StaticFiles": "1.0.0-rc2-\*",
    "EntityFramework.MicrosoftSqlServer": "7.0.0-rc2-\*",
    "EntityFramework.Commands": "7.0.0-rc2-\*",
    "Microsoft.Extensions.Logging.Console": "1.0.0-rc2-\*",
    "Microsoft.Extensions.Logging.Debug": "1.0.0-rc2-\*",
    "Microsoft.AspNet.Diagnostics": "1.0.0-rc2-\*"
  },

Note that after the rc2- there is a \*(asterisk) that has the system get the most recent version. It is important to note that this only works after the final .(dot). So you can't just specify "\*" and have it work. This is only for the external dependencies in the project.json file. Any projects that you reference that are in your solution should have an empty string "" for the version number.

You will notice that when you are typing the version number, you will get a list of valid versions. This will help you to know which ones are currently available. The one with the largest number will obviously be the most current.

If you want to lock in version numbers, just select the most recent version from the IntelliSense list. Doing that might look something like this.

"dependencies": {
    "Microsoft.AspNet.IISPlatformHandler": "1.0.0-rc2-16069",
    "Microsoft.AspNet.Server.Kestrel": "1.0.0-rc2-16236",
    "Microsoft.AspNet.Mvc": "6.0.0-rc2-16770",
    "Microsoft.AspNet.Mvc.TagHelpers": "6.0.0-rc2-16770",
    "Microsoft.AspNet.Tooling.Razor": "1.0.0-rc2-16051",
    "Microsoft.AspNet.StaticFiles": "1.0.0-rc2-16107",
    "EntityFramework.MicrosoftSqlServer": "7.0.0-rc2-16649",
    "EntityFramework.Commands": "7.0.0-rc2-16649",
    "Microsoft.Extensions.Logging.Console": "1.0.0-rc2-15958",
    "Microsoft.Extensions.Logging.Debug": "1.0.0-rc2-15958",
    "Microsoft.AspNet.Diagnostics": "1.0.0-rc2-16405"
  },

Once you save the project.json file, Visual Studio will automatically restore the packages you specified. You can check the References node of your project for issues during package restore. This will also allow you to see package dependencies and any conflicts that might exist. While there can still be headaches here, at least it is pretty clear what is going on.

I would recommend starting at your most core project, the one without any dependencies on projects in your solution. Get this project to restore correctly, and then work to other projects. However, you may get errors about requesting one version and getting another. If so, you will need to update all the project.json files to make sure you are requesting consistent versions across your solution.

# Happy Coding

You now should have your project ready to use the nightly builds. Nightly builds are be exciting and challenging all at the same time. I commend you for giving them a try. Check out the GitHub page at https://github.com/aspnet
