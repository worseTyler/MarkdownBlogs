
### **Overview**

ASP.NET 5 is in the Release Candidate phase of development. While this version does have a go-live license, the tooling for it in Visual Studio is not as streamlined as other parts of .net. As a result there are several areas where command line tools need to be used to configure the environment. While this may seem like steps backwards, the command line tools allow much easier application life-cycle management in areas like deployment and automated builds. Microsoft has heard the cries of the community that the current process is too painful and they are working to address the situation, probably in a Visual Studio Update 2 sometime in early 2016.

#### **What is DNX**

DNX.exe is the Dot Net eXecution environment. It is akin to the Node or Java runtime. DNX is a transitional executable that will be replaced with what appears to be DotNet.exe. DNX commands are typically structured like `DNX [Command] [SubCommand] -[option] [option value]`. DNU.exe is a utilities module that provides some additional features.

As per discussion with Damian Edwards on 12/15/2015 it appears that the commands for DNX will change in the RC2 release due out in February.

The paths given below are for a standard configuration Windows 10 system. Please modify them as needed for your configuration.

#### **Command Line Options**

There are several options for command line interfaces for DNX/DNU. You can use a regular command prompt. You can use the Package Manager Console in Visual Studio. PowerShell is also an option.

### **Installing**

This troubleshooting guide covers the typical install case. Assuming a 64-bit Windows OS. This is required to run DNX commands from the command line.

- Install or Upgrade Visual Studio 2015 Update 1
- Go to https://get.asp.net/ and click on the ASP.NET 5 area's button 'Install for Windows'. Install the downloaded file.
- Clear any old dnvm files.

c:
cd users\\\[user name\]\\.dnx\\bin
delete dnvm.cmd and dnvm.ps1.

- You will need to register the current version of DNX via the Dot Net Version Manager.

c:
cd Program Files\\Microsoft DNX\\Dnvm
dnvm setup

- Set the default version of DNX for the command line. Run the `dnvm list` command and find the largest version. Use the `dnvm use...` command. You can then run `dnvm list` command and it should have a \* by the version you are currently using as default. The runtime should be clr and the architecture should be x86.

dnvm list
dnvm use 1.0.0-rc1-final -persist
dnvm list

- Note that the alias column effects which DNX is used by Visual Studio. It should either be not set (blank for all versions) or the version you want to use in Visual Studio should be aliased as 'default'.
- Check the DNX version. The first is full information and the second is just the version.

dnvm
dnvm version

- Once complete, you should be able to run the command `dnx` from the root of c:.

### **NuGet Package Errors**

Project.json contains references to .net NuGet packages that are used in the solution. When packages don't load try these steps:

- Right click on your project's references folder and choose Restore Packages.
- Close Visual Studio and reopen your project. On project open, VS will restore your packages again.
- Via the command line, go into your project directory and run the command `dnu restore`
- Clear the .dnx cache at c:/users/\[username\]/.dnx/packages by deleting all the folders. Then run `dnu restore` for all your projects from the various project directories. (Yes, this is a hassle.)
- A restore can also be done by changing the project.json file and saving it.

Packages are downloaded to a package cache in c:/users/\[username\]/.dnx/packages. All packages found in your project.json files are downloaded here as a cache. In some rare cases, this cache seems to be corrupted and needs to be removed and reloaded from NuGet sources via a package restore.

If you are getting messages about not being able to restore packages, a likely cause is that there is not a NuGet feed in Visual Studio that contains the packages you want to load. This happens if you are using packages not found on nuget.org and haven't added the additional NuGet feed. Changing development machines and forgetting to update the NuGet package sources is a common culprit, especially if you are using the nightly builds. NuGet feeds can be added to your solution via the NuGet.config file.

Another common message is something like 'attempted to get version 5 but ended up with version 2.' This is typically caused by referencing two different versions of the same NuGet package in the same solution. The best way to track this down is to start at your lowest level project (no dependencies on other projects) and check the References node in the solution explorer and compare this with the packages and versions in the project.json file. An additional feature is that all the dependent packages and their versions are listed under the References node as well. A package may reference an older or newer version of a package than what is explicitly specified in your project.json.

### **Bower Component Package Errors**

Bower is the source for web-client libraries. The packages to be loaded are listed in the bower.json file in your project. For IntelliTect projects, these Bower packages get stored in the bower\_compontents folder in your solution. Bower typically contains source files as well as compiled versions. For example scss and css files. These source files are often compiled via a Gulp task. The compiled files are then copied into the wwwroot area of the solution for deployment again by a Gulp task.

When Bower packages don't load try these steps:

- Right click on your project's references folder and choose Restore Packages.
- Close Visual Studio and reopen your project. On project open, VS will restore your packages again.
- Make sure that the .bowerrc file contains `{ "directory": "bower_components" }`. Note that this is not the Visual Studio default for new web projects.
- Clear the Bower cache at ./bower\_components by deleting all the folders. Do a package restore per above.
- A restore can also be done by changing the bower.json file and saving it.

### **Node Packages and Gulp**

Node is a powerful JavaScript runtime engine that is used in many projects. Specifically Microsoft has adopted it as a means of running tools within the development environment via a system called Gulp. Gulp runs on Node and uses external components via the Node Package Manager (npm). Node gets installed with Visual Studio and it is not recommended to install it stand alone. Note that if you have Node installed stand alone, Visual Studio will not use that version by default, but you can run the new version via the command line.

Node packages are specified in the package.json file. These packages are used with Gulp. Note that by default this file may be hidden in your solution. Use show all files to see it.

If you are having issues with Node, npm or Gulp try these options.

- If Gulp tasks in the Task Runner Explorer are not showing correctly, the issue is likely that npm didn't restore packages correctly for Gulp. If Visual Studio crashes after adding a package try these steps. Note that you may need to use rimraf to delete long file names.
    - Close Visual Studio
    - Delete all folders under the node\_modules in your project folder.
    - Clear the node cache at C:\\Users\\\[user name\]\\AppData\\Roaming\\npm-cache by deleting all folders.
    - From the project folder run the following command. If you have Node in your path the full name isn't necessary.
    - `C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\Extensions\Microsoft\Web Tools\External\npm install`
- If you want to run node and npm from the command line you must add `C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\Extensions\Microsoft\Web Tools\External`  to your command line.
