

## Publishing a NuGet
#
Sharing your code as a NuGet is one of the best ways to ensure it gets used. Creating a NuGet package is quick and easy.

 

To start, we first need need a library to publish. I have already created My Awesome Library (MAL).

![](https://intellitect.com/wp-content/uploads/2017/02/PublishingANuget-3.png)

 

The first thing we need to do is [download the latest NuGet CLI](https://dist.nuget.org/index.html) (command line interface). For these examples I am going to place it alongside my project file and use the Package Manager Console inside of Visual Studio.

The first thing we need is a [.nuspec file](https://docs.nuget.org/Create/NuSpec-Reference). This is simply an XML manifest that describes the metadata of of our NuGet package. To create a default file run “nuget spec”

![](https://intellitect.com/wp-content/uploads/2017/02/PublishingANuget-1.png)

 

Inside the .nuspec file we can see all of the values, including tokens (the values that start and end with $), that can be modified. For now we will insert some reasonable default values.

![](https://intellitect.com/wp-content/uploads/2017/02/PublishingANuget-7.png)

The id, version, description, and authors are [all required](https://docs.microsoft.com/en-us/nuget/schema/nuspec#required-metadata-elements) and must have a value. The remaining values are [optional](https://docs.microsoft.com/en-us/nuget/schema/nuspec#optional-metadata-elements) and can be removed if they are not needed.

Many of the tokens will be automatically replaced with values from the AssemblyInfo.cs so we will just review those values.

![](https://intellitect.com/wp-content/uploads/2017/02/PublishingANuget-5.png)

The $id$ token will be replaced with the Assembly name from the project properties.

Now that we have the .nuspec file setup, all that is left is to create the NuGet package. Ensure that the project has been built, then run “nuget pack <project file>”.

![](https://intellitect.com/wp-content/uploads/2017/02/PublishingANuget-6.png)

This will create the NuGet package file (\*.nupkg).

At this point you have successfully create a NuGet package that you can deploy to any NuGet package source. For this example we will publish it to nuget.org.

To do this start by signing into your nuget.org account (or create a new one).

If you want to keep things simple you can simply upload your package files manually by going to the Upload Package screen.

![](https://intellitect.com/wp-content/uploads/2017/02/PublishingANuget-2.png)

However, this method does not work well for scripts and build systems. To publish the NuGet package from the command line you must generate an API key.

Open your account settings by clicking on your account name in the upper right corner, and scroll down to the API key section.

From here you will generate a new API key that the NuGet CLI can use to publish your packages.

![](https://intellitect.com/wp-content/uploads/2017/02/PublishingANuget-8.png)

Back on the console we can now publish our NuGet package by running "nuget push <Path to package file> <API key> -Source "

![](https://intellitect.com/wp-content/uploads/2017/02/PublishingANuget-4.png)

That is it, the NuGet package has been published.

It will take some time for nuget.org to index the new package. Until this happens your package may not show up in search results.
