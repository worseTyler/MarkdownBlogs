---
title: "Quick Fix for the NETSDK1004 Compile Error"
date: "2020-01-10"
categories: 
  - "blog"
---

## What Does the NETSDK1004 Message Mean?

```
Error NETSDK1004: "Assets file 'C:project.assets.json' not found. Run a NuGet package restore to generate this file.
```

It occurred when I was refactoring the CSPROJ files within my solution. In the process, I created a **Directory.Build.props** file, copying out the `Project` root element from the CSPROJ file **including the SDK**.  (In my case, I happened to be moving out C# 8.0's Nullable element - `enable`.)  The result was the above compile error.

### The SDK XML Attribute was the Mistake

The SDK attribute causes the above error along with multiple occurrences of this associated warning:

```
warning MSB4011: "C:\Program Files\dotnet\sdk\3.1.100\Sdks\Microsoft.NET.Sdk\Sdk\Sdk.targets" cannot be imported again. It was already imported at "…\Directory.Build.props". This is most likely a build authoring error. This subsequent import will be ignored.
```

Pull out the `SDK` attribute and the error goes away. Here's the corrected **Directory.Build.props** file:

<Project>
     <PropertyGroup>
         <Nullable>enable</Nullable>
         <TargtFramework>NetCoreApp3.1</TargtFramework>
     </PropertyGroup>
</Project>

_Note: This example also pulls out the_ `_TargetFramework_` _element._

If you are working with a solution and all projects are C# 8.0, I recommend you refactor out the `Nullable` attribute into a Directory.Build.props file as shown in this example, rather than individually edit it in all the CSPROJ files independently.

### Want More?

Check out our blog _[How to Publish NuGets with Azure DevOps](https://intellitect.com/azure-devops-nugets/)_ for more information on NuGet packages!

![](images/blog-job-ad-2-1024x129.png)
