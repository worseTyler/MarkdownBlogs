

## Deploying Windows With Psake and Web Deploy 
#
![Robot and human fist bump](https://intellitect.com/wp-content/uploads/2015/06/robot-300x203.jpg "Deploying Windows Services With Psake and Web Deploy")

At IntelliTect, a common pattern of our client solutions are windows services that process work on either a scheduled basis or watch a file location. We often use a combination of the [Topshelf framework](https://topshelf-project.com/) with the [TopShelf.Quartz](https://www.nuget.org/packages/Topshelf.Quartz/) job scheduling package to solve these problems. These packages expose a useful fluent interface to schedule multiple jobs in a service instance and take care of the service events-- including installation on the command line. While this is helpful from a code perspective in reducing boilerplate and increasing simplicity, this design lacks an easy way to deploy new releases.

Since working for IntelliTect (and specifically working with [Mark Michaelis](/mark-michaelis/) and [Kevin Bost](/kevin-bost/)), I've become a big fan of both PowerShell and the [psake build automation tool](https://github.com/psake/psake). Now that Nuget supports [solution-level packages](https://docs.nuget.org/release-notes/nuget-1.7#add-solution-level-packages.config-file), adding psake to a project couldn't be easier. Similar to the Ruby on Rails automation tool "rake", psake augments the PowerShell language with a simple task notation and immutable properties. Tasks are chained together to form dependency trees, and if a task fails, subsequent tasks are not run. Psake also provides helpful wrappers around msbuild and other result-code-returning command line executibles.

![Two workers lower a cement girder into position](https://intellitect.com/wp-content/uploads/2015/06/6946761849_710befb078_z-300x199.jpg "Deploying Windows Services With Psake and Web Deploy")

Generally, the Topshelf service solutions we design also include a web app project that acts as a job status monitor and configuration tool. This means that these Topshelf windows services are deployed to a server with IIS already installed. The "right-click Publish" functionality that [Web Deploy](https://www.iis.net/downloads/microsoft/web-deploy) (also referred to as msdeploy) affords us in the Visual Studio IDE is a great experience, so I sought to leverage that in deploying Topshelf windows services.  With a small amount of configuration, a vanilla Windows Server with IIS can be used to stop, deploy (leaving behind things like logs or config files), and restart a windows service.

## Installing and Configuring Web Deploy 3.5

1. I think installing msdeploy via the Web Platform Installer is easiest, so start there with [this link](https://go.microsoft.com/?linkid=9684518).
2. If it's not already installed, install the server Role "Web Management Service" (wmsvc) using the server manager (see Figure 1).

[![Web Management Service in Add Roles screenshot](https://intellitect.com/wp-content/uploads/2015/06/Web-Management-Service_1-150x150.jpg)](https://intellitect.com/wp-content/uploads/2015/06/Web-Management-Service_1.jpg "Deploying Windows Services With Psake and Web Deploy")

Figure 1

- Set up the IIS container for the web site that goes along with the service (this may be a bogus site if you don't require one).

- Grant AD group access to those who will be allowed deploy in "IIS Manager Permissions" (see Figure 2).

![IIS Manager Permissions screenshot](https://intellitect.com/wp-content/uploads/2015/06/IIS-Manager-Permissions-300x65.png "Deploying Windows Services With Psake and Web Deploy")

Figure 2

- Setup a "contentPath" and a "runCommand" for your service in the Management Service Delegation section in IIS Manager (see Figure 3).
    - Add a rule with a Provider of "contentPath"
    - Actions of "\*" (for all actions)
    - A Path Type of "Path Prefix"
    - A Path that points at the root directory of your windows service instance.
    - An Identity Type of "CurrentUser", specifying the AD group above you granted deployment rights to in Step 2.

[![Figure 3](https://intellitect.com/wp-content/uploads/2015/06/2015-05-22-09_40_02-h1414.corp_.com-Remote-Desktop-Connection-300x123.png)](https://intellitect.com/wp-content/uploads/2015/06/2015-05-22-09_40_02-h1414.corp_.com-Remote-Desktop-Connection.png "Deploying Windows Services With Psake and Web Deploy")

Figure 3

- Make sure WMSVC has sufficient rights to stop and start your service. Execute the following command from an elevated command prompt, and then restart WMSVC:  
    CMD> sc privs wmsvc SeChangeNotifyPrivilege/SeImpersonatePrivilege/SeAssignPrimaryTokenPrivilege/SeIncreaseQuotaPrivilege

## Build Your Psake Script

![Tesla Model S assembly line](https://intellitect.com/wp-content/uploads/2015/06/tesla-model-s-assembly-line-300x188.jpeg "Deploying Windows Services With Psake and Web Deploy")

In general, I like to pass in the environment (dev/test/staging/production) to my build script, so I will add that as a Property in my psake script, but pass it in via my psake bootstrapper. The bootstrap script's job is to load the psake module, and invoke it. I set up the solution to have build configurations that match the names of the environments. Building on the script that comes with the psake Nuget, my bootstrapper looks like this:

```powershell
\# Usage:
# PS> .\\Build.ps1 {Task Name} {Debug|Test|Staging|Production}
$task = ($args[0], "default" -ne $null)[0]
$environment = ($args[1], "Debug" -ne $null)[0]

Import-Module (Resolve-Path "$PSScriptRoot\\..\\packages\\psake.\*\\tools\\psake.psm1") -verbose
Invoke-psake "$PSScriptRoot\\Psake-Tasks.ps1" -task $task -properties @{"configuration"="$environment"} -Verbose;

if ($lastexitcode) { write-host "ERROR: $lastexitcode" -fore RED; exit $lastexitcode };
if ($psake.build_success -eq $false) { exit 1 } else { exit 0 }

```

### Properties

The passed-in configuration name is then used as an indexer into a hashtable that holds environment-specific details about the deployment. Also in the Properties, I like to store some other static values, like the locations of the msdeploy and mstest executibles. Here is an example Properties section:

```powershell
properties {
  $testExecutable = "$((dir env:VS1\*COMNTOOLS)[-1].Value)..\\IDE\\mstest.exe"
  $msDeployLocation = "C:\\Program Files (x86)\\IIS\\Microsoft Web Deploy V3\\"
  $configuration = "Debug"
  $deploy = @{
    "Debug" = @{
      "PublishProfileName" = "NONE";
      "BinaryLocation" = "bin\\Debug";
      "ServerName" = "localhost";
      "SiteName" = "MyServiceAdminDev";
      "ServiceFolder" = "C:\\Services\\MyService";
    };
    "Test" = @{
      "PublishProfileName" = "QASERVER";
      "BinaryLocation" = "bin\\Test";
      "ServerName" = "qaserver.domain.local";
      "SiteName" = "MyServiceAdminTest";
      "ServiceFolder" = "D:\\Deployments\\Services\\MyService";
    };
    "Staging" = @{
      "PublishProfileName" = "STAGING";
      "BinaryLocation" = "bin\\Staging";
      "ServerName" = "staging.domain.local";
      "SiteName" = "MyServiceAdminStaging";
      "ServiceFolder" = "D:Apps\\Services\\MyService";
    };
    "Production" = @{
      "PublishProfileName" = "PRODUCTION";
      "BinaryLocation" = "bin\\Production";
      "ServerName" = "myapp.domain.local";
      "SiteName" = "MyServiceAdmin";
      "ServiceFolder" = "D:Apps\\Services\\MyService";
    };
  }
}
```

### Tasks

Once you have your bases covered with the immutable properties you will need, it's time to add some Tasks to your psake script. Clean and Compile are two easy ones that use the msbuild helper exposed by psake:

```powershell
task Clean {
  exec { msbuild "$PSScriptRoot\\..\\Avista.Compass.DataSync.sln" /m /target:Clean }
}

task Compile {
  exec { msbuild "$PSScriptRoot\\..\\Avista.Compass.DataSync.sln" /m /p:Configuration=$configuration /p:VisualStudioVersion=12.0 }
}
```

Provided you name your tests projects ending with ".Tests", and use a [TestCategory] attribute of Unit to exclude integration tests, this task will run them all (failing if any tests don't pass):

```powershell
task Test -depends Compile {
  $unitTestProjects = Get-ChildItem -Filter "\*Tests" -Directory "$PSScriptRoot\\..\\"
  $unitTestProjects | %{
    $assemblyPath = "$PSScriptRoot\\..\\$_\\$($deploy[$configuration].BinaryLocation)\\$_.dll"
      exec { & $testExecutable /testcontainer:$assemblyPath /category:Unit }
  }

  Remove-Item $PSScriptRoot\\TestResults -Recurse
}
```

Using that standard deployment with a Publish Profile for a web application project means a web site deployment task looks like the following. Make sure to specify the AspNetCompilerPath so you can take advantage of pre-compiled Razor views.

```powershell
task DeployWebsite -depends Test {
  exec { msbuild "$PSScriptRoot\\..\\WebSite\\WebSite.csproj" /p:DeployOnBuild=true /p:PublishProfile="$($deploy[$configuration].PublishProfileName)" /p:Configuration=$configuration /p:AspnetCompilerPath="C:\\windows\\Microsoft.NET\\Framework\\v4.0.30319" /p:VisualStudioVersion=12.0 }
}
```

To deploy the windows Topshelf service, we need a task that calls msdeploy to execute the service stop, copy the files, and then start the service again. I found that because of PowerShell's "helpful" handling of quotes, and a broken argument parser inside msdeploy.exe, I had to use the Start-Process cmdlet to execute correctly.

```powershell
task DeployService -depends Test {
  $env:Path += ";$msDeployLocation"
  $binDir = Resolve-Path "$PSScriptRoot\\..\\ServiceProjectFolder\\$($deploy[$configuration].BinaryLocation)"
  Write-Host "Copying compiled files from: $binDir"
  $siteName = $($deploy[$configuration].SiteName)
  $servName = $($deploy[$configuration].ServerName)
  [string[]]$args = @( '-verb:sync', 
                        "-preSync:runCommand=\`'$serviceFolder\\MyService.exe stop\`',waitInterval=30000,dontUseCommandExe=true",
                        "-source:dirPath=$binDir", 
                        "-dest:computerName=\`'https://$servName\`:8172/msdeploy.axd?site=$siteName\`',authtype='NTLM',includeAcls='False',dirPath=$serviceFolder\\",
                        "-skip:Directory='Logs'",
                        "-skip:File='MyService.exe.config'",
                        "-verbose",
                        "-allowUntrusted",
                        "-postSync:runCommand=\`'$serviceFolder\\MyService.exe start\`',waitInterval=30000,dontUseCommandExe=true"
                        )
  Write-Host $args
  Start-Process "$msDeployLocation\\msdeploy.exe" -ArgumentList $args -NoNewWindow -Wait
}
```

Note the "-skip:" arguments that tell msdeploy to not overwrite the Logs folder or the config file. I generally use the built-in msbuild configuration file transformations, so I can have each environment's config checked into source code control, but setting that up is probably another blog post. This use of msdeploy assumes that you have already successfully deployed the service once manually. You could add preSync and postSync runCommands that also execute "MyService.exe uninstall" and "MyService.exe install" if you have specific things you need your service to do in those events. Also be advised that you will need to trust the self-signed SSL certificate that the WMSVC creates to secure it's communications.

Finally, we can create some meta-tasks that wire together our dependencies for convenience:

```powershell
task default -depends Compile,Test
task Deploy -depends DeployWebsite,DeployService
```

We now have a serviceable script that I can check in along with my source code and give to a QA person or operational person to build, unit test, and deploy both a web site and a windows service. Using psake's preCondition and postCondition blocks, you could even make assertions that your tasks were successful, or make a request of the site to "warm up" the app pool. If you have multiple web sites to deploy as part of a solution, simply make a hashtable of the project locations and create distinct deploy tasks for each one. Also bear in mind that if your ALM practices require you to deploy pre-built bits for your service, simply change the location that $binDir resolves in the DeployService task.
