

## A New Way to Add Visual Studio Tools to Your PowerShell Environment With Enter-VsDevShell
#
### PowerShell All the Things

 For many years I’ve enjoyed using a PowerShell console as my go-to CLI for interacting with .NET projects and solutions. So one of the first things I do when provisioning a new developer machine is grab a copy of my PowerShell profile from my [GitHub repository](https://github.com/adamskt/danja-zown). Even though I’ve been using Ubuntu as my main platform for development and have consequently gotten into ZSH and BASH scripting, I still use PowerShell daily, but now with a new cmdlet: Enter-VsDevShell.

### PowerShell + Visual Studio == Awesome

Until the 16.2.1 release[1](#fn1) of Visual Studio 2019, I used the inimitable [PowerShell Community Extensions](https://github.com/Pscx/Pscx) (PSCX) module to execute batch files to alter the current environment variables (PATH, etc.). I would use the `Invoke-BatchFile` cmdlet in my profile to execute the VsDevCmd.bat file in the Common7Tools folder of the current Visual Studio installation. This command required maintenance over time since the path was specific to the installed edition and major version of Visual Studio. Upon upgrading to the 16.2.1 release, my profile began coughing up an error:

```
"The input line is too long. The syntax of the command is incorrect."
```

Then things like MSBuild quit working. Bad, bad, not good.

### Watson, the Game is Afoot!

I initially thought something might have happened to the `Invoke-BatchFile` implementation in PSCX, prompting me to voice an issue to that team[2](#fn2). The `Invoke-BatchFile` command executes the specified batch file with, then calls `set` to dump the new environment table to file for parsing. The problem arose because the combined outputs from the batch file and resulting environment variable list exceeded the 8191 character limit[3](#fn3) established back in the hoary days of “Ye Olde Windows XP.” After this diversion down memory lane, I knew why it was breaking but didn’t know what I could do to fix it.

In trying to isolate the naughty command prompt behavior, I discovered the new Developer PowerShell for VS 2019 console. This console didn’t throw errors, and everything appeared to be working. However, after a bit of casting about for the location of the actual shortcut, I discovered that it loads a PowerShell module and calls a cmdlet (Enter-VsDevShell) within to achieve the same result as the batch file does for the regular `cmd.exe` console.

### Further Down the Rabbit Hole

The shortcut for the “Developer PowerShell for VS 2019” item lives in the folder C:ProgramDataMicrosoftWindowsStart MenuProgramsVisual Studio 2019Visual Studio Tools and executes the following command:

```powershell
C:WindowsSysWOW64WindowsPowerShellv1.0powershell.exe -noe -c "&{Import-Module Enter-VsDevShell ba5a33e2}"
```

The relative path to the module works because the “Start in” value of the shortcut is set to C:Program Files (x86)Microsoft Visual Studio2019Enterprise. These shortcut properties are all good and proper for most folks: copy the command section of the shortcut into your profile, make the path absolute to where you have installed Visual Studio, and continue merrily on your way. Of course, you will have to change the path and the InstanceId[4](#fn4) as you change editions or upgrade, but that’s not much of an onus for most people.

### Peace Falls Over the Hidden Valley… Or Does It?

At this point, I was getting MSBuild in my path, and things were working again. I had previously noticed that new PowerShell consoles ended up in my “Projects location” folder in Visual Studio options, as seen in the screenshot below:

![](https://intellitect.com/wp-content/uploads/2019/09/devenv_2019-08-28_08-36-27-1024x211.png)

Visual Studio 2019 Options dialog

This location isn’t an issue for most. It’s probably handy to get new consoles opening in the location where you do your work. Unfortunately, that wasn’t the case for me.

### Trouble Returneth

After a few days, I switched back to working on a client’s large application. Using PowerShell, the solution launched a fleet of IISExpress instances to stand up every site in their web app suite. Unfortunately, the launch scripts for this application were failing because they expected a new PowerShell console to remain in its working directory. As a result of this new behavior, that wasn’t the case: the IISExpress instances would start but crash silently on receiving the first HTTP request.

This problem was complicated to debug, especially since the behavior followed me between environments. I was eventually able to add the `-NoProfile` argument to the launch scripts and get back to being productive. However, I still wanted to understand how it actually worked. So, in my curiosity,  I cracked open Microsoft’s new PowerShell module with [JetBrains’ dotPeek](https://www.jetbrains.com/decompiler/) and went hunting.

### The Land of the Overworld is Saved Again

I discovered a parameter to the Enter-VsDevShell cmdlet called `-SkipAutomaticLocation`. This optional SwitchParameter suppresses the behavior that changes the current working directory to the repository location specified in the Visual Studio settings. At the time of this writing, I haven’t found any documentation on the cmdlets contained within Microsoft.VisualStudio.DevShell, I hope this is remedied soon.

There are two different ParameterSets for calling the Enter-VsDevShell cmdlet. One set requires the InstanceId referenced in the shortcut, and one provides the InstallationPath to Visual Studio itself. Both tell the cmdlet where to start looking for things to add to your environment. Using this insight, I discovered a [blog post by Jason Tucker](https://medium.com/@jtucker/visual-studio-devshell-e3080f0341af) that explains how to use `vswhere.exe` to derive the installation path. I needed a future-proof and non-harmful way to add Visual Studio environment variables to my PowerShell consoles. The key was in combining these two approaches:

```powershell
$installPath = &"C:Program Files (x86)Microsoft Visual StudioInstallervswhere.exe" -version 16.0 -property installationpath
Import-Module (Join-Path $installPath "Common7ToolsMicrosoft.VisualStudio.DevShell.dll")
Enter-VsDevShell -VsInstallPath $installPath -SkipAutomaticLocation
```

### Coda

There are some other interesting parameters you can pass to Enter-VsDevShell:

<table><tbody><tr><td><strong>Command</strong></td><td><strong>Description</strong></td></tr><tr><td>-Test</td><td>Appears to load all the batch files and report back the locations. This parameter might be useful if you have multiple versions of Visual Studio installed.</td></tr><tr><td>-DevCmdDebugLevel [None, Basic, Detailed, Trace]</td><td>Changes the verbosity and frequency of the messages it spews out during execution.</td></tr><tr><td>-SkipExistingEnvironmentVariables,<br>-StartInPath,<br>-DevCmdArguments,<br>-SetDefaultWindowTitle</td><td>These also exist, but I haven’t yet determined their utility.</td></tr></tbody></table>

In retrospect, I’m glad I was curious enough to dig into this new functionality shipping with Visual Studio. I wrote this post to highlight things that I found useful to me. Hopefully, better documentation is forthcoming!

Here is the current anemic help output from the cmdlet:

```powershell
NAME
    Enter-VsDevShell
SYNTAX
    Enter-VsDevShell [-VsInstallPath]  [-SkipExistingEnvironmentVariables] [-StartInPath ]
    [-DevCmdArguments ] [-DevCmdDebugLevel {None | Basic | Detailed | Trace}] [-SkipAutomaticLocation]
    [-SetDefaultWindowTitle]  []
    Enter-VsDevShell -VsInstanceId  [-SkipExistingEnvironmentVariables] [-StartInPath ]
    [-DevCmdArguments ] [-DevCmdDebugLevel {None | Basic | Detailed | Trace}] [-SkipAutomaticLocation]
    [-SetDefaultWindowTitle]  []
    Enter-VsDevShell [-Test] [-DevCmdDebugLevel {None | Basic | Detailed | Trace}]  []
PARAMETERS
    -DevCmdArguments 
    -DevCmdDebugLevel 
    -SetDefaultWindowTitle
    -SkipAutomaticLocation
    -SkipExistingEnvironmentVariables
    -StartInPath 
    -Test
    -VsInstallPath 
    -VsInstanceId 
        This cmdlet supports the common parameters: Verbose, Debug,
        ErrorAction, ErrorVariable, WarningAction, WarningVariable,
        OutBuffer, PipelineVariable, and OutVariable. For more information, see
        about_CommonParameters (https:/go.microsoft.com/fwlink/?LinkID=113216).
ALIASES
    None
REMARKS
    None
```

Please feel free to steal anything from my profiles in the repository above or offer up alternatives or improvements. I will also try to answer any questions left in the comments.

### Important Note About Enter-VsDevShell:

The `Enter-VsDevShell` cmdlet in the Microsoft module does not yet work with PowerShell Core. Please up-vote [this issue](https://developercommunity.visualstudio.com/idea/663594/microsoftvisualstudiodevshell-doesnt-work-with-pow.html) to raise its visibility if you are interested.

#### Footnotes

1. Release notes [here](https://docs.microsoft.com/en-us/visualstudio/releases/2019/release-notes#--visual-studio-2019-version-1621).
2. PSCX issue [#66](https://github.com/Pscx/Pscx/issues/66).
3. Discussed in [this KB article](https://support.microsoft.com/en-us/help/830473/command-prompt-cmd-exe-command-line-string-limitation).
4. I have no idea where this value comes from. Its source in the cmdlet is obscured by a call out to an unmanaged type.

### Want More?

Check out this [video](https://intellitect.com/powershell-dsc/) on Desired State Configuration (DSC) in PowerShell!

![](https://intellitect.com/wp-content/uploads/2021/04/blog-job-ad-2-768x97.png)
