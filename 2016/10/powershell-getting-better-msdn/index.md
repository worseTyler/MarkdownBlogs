---
title: "Essential .NET: PowerShell Just Keeps Getting Better (MSDN)"
date: "2016-10-01"
categories: 
  - "net-core"
  - "blog"
  - "msdn-essential-net"
  - "powershell"
---

In a departure from my recent focus on .NET Core, in this month’s Essential .NET Column I’m going to focus on a host of new features that significantly increase the power of Windows PowerShell. To me, the most significant improvements are in the area of cross-platform support—yes, really, PowerShell now runs on Linux. Not only that, but it has also moved to open source on GitHub ([github.com/PowerShell/PowerShell](https://github.com/PowerShell/PowerShell)) so that the community at large can begin to bolster its features. Cool!!

But the most recent announcements don’t tell the whole story. Back in February, PowerShell 5.0 was released and it includes new or improved support for class and enum declarations, module and script discovery, package management and installation, OData endpoint access, enhanced transcription and logging, and more. In this article I’m going to review each of these features and provide examples.

### PowerShell Goes Cross-Platform

To begin, take a look at the following command script, which installs PowerShell on Ubuntu 14.04 from Windows PowerShell Host, along with a screenshot of the execution session from Windows Bash in **Figure 1** (For those of you not familiar with Bash running Ubuntu on Windows 10 Anniversary Update see “Installing Bash on Windows 10.”):

wget -O powershell.deb https://github.com/PowerShell/PowerShell/releases/download/v6.0.0-alpha.9/powershell\_6.0.0-alpha.9-1ubuntu1.14.04.1\_amd64.deb
sudo apt-get install libunwind8 libicu52
sudo dpkg -i powershell.deb
powershell

![ Figure 1 Installing and Running Windows PowerShell on Ubuntu 14.04 from Bash on Ubuntu on Windows](images/Figure-1-2.png)

Figure 1 Installing and Running Windows PowerShell on Ubuntu 14.04 from Bash on Ubuntu on Windows

Note that the command script specifically targets Ubuntu 14.04. For other platforms, the deb package URL and the prerequisite versions will vary. See [bit.ly/2bjAJ3H](https://bit.ly/2bjAJ3H) for instructions on your specific platform.

Many years ago now Jeffrey Snover tweeted that PowerShell could reasonably be expected to appear on Linux, but it has taken so long and there have been so few progress reports that even today as I use it I’m amazed. I mean, really? I’m running Bash on top of Ubuntu running on Windows (without leveraging any virtualization technology) and (assuming I don’t want to install PowerShell directly into the same Bash instance) using SSH to connect to a remote Bash session where I can install PowerShell and pipe .NET objects between commands within the Bash shell.

If I had suggested this would be possible a couple of years ago I doubt many would have believed me.

### Installing Bash on Windows 10

Starting with Windows 10 Anniversary Edition, you can install Bash natively onto Windows with the following Windows PowerShell command:

Get-WindowsOptionalFeature -Online -FeatureName \*linux\* | Enable-WindowsOptionalFeature -NoRestart -all –online

Note, however, that this is still in beta and, therefore, only enabled in developer mode (use “Get-Help WindowsDeveloper” to see how to explore developer mode).

Unfortunately, this feature does require a restart, but I include the -NoRestart option so that enabling the feature doesn’t directly trigger the restart.

### PowerShell Repositories and the PowerShell Gallery

While it’s great that you can write your own scripts and libraries, it’s likely that someone else in the community has already done something similar that you can leverage and improve upon. Until the advent of the PowerShell Gallery ([PowerShellGallery.com](https://PowerShellGallery.com)), however, you had to comb the Internet to find scripts and modules that might be useful—whether they were community contributions or official PowerShell product releases like Pscx or the Posh-Git module. One of the more recent PowerShell improvements (part of PowerShell 5.0) I’ve become completely dependent on is the new repository support, specifically the PowerShell Gallery. Imagine, for example, that you’ve been writing PowerShell for some time and, in so doing, you’ve become aware that there are many pitfalls to be avoided, if only there was a way to analyze your code and find them. With this in mind, you could browse to the PowerShell Gallery and search for an analyze module to install. Or, even better (because you presumably already have a PowerShell window open), you can leverage the PowerShellGet module’s Find-Module command (included with PowerShell 5.0):

Find-Module \*Analyze\* | Select-Object Name,Description

The output of which is shown in **Figure 2**.

![ Figure 2 Output of Find-Module Command](images/Figure-2-2.png)

Figure 2 Output of Find-Module Command

Note that if you don’t have a sufficiently modern version of NuGet installed, leveraging the PowerShellGet module will trigger a NuGet package update.

Assuming you find the module you want, you can view its contents via the Save-Module command. To install the module, use the Install-Module (in this case, Install-Module PSScriptAnalyzer) command. This will download the module and install it for you, making all the functions included in the module available. After installing the PSScriptAnalyzer module, you can invoke Invoke-ScriptAnalyzer $profile to scan your profile and identify concerns that the analyzer considers suboptimal. (Note that it’s no longer necessary to import a module in order to access it. Module functions are automatically indexed such that when you invoke a module function, the module will automatically import and be accessible on demand.)

Note that the PowerShell Gallery is configured as a repository by default:

\>Get-PSRepository
Name         InstallationPolicy   SourceLocation
---- ------------------ --------------
PSGallery    Untrusted            https://www.powershellgallery.com/api/v2/

As a result, Find-Module works without issue. However, Install-Module will prompt you with an untrusted repository warning. To avoid this, assuming you do indeed trust the repository, you can set it to trusted with the command:

Set-PSRepository -Name PSGallery -InstallationPolicy Trusted

Apt-Get for Windows with PowerShell Package Management Those of you who have spent any time as an IT professional in the Linux world have no doubt taken apt-get for granted—likely with install scripts that bootstrap their environment the moment they start up a new Linux instance. For those of you who haven’t, apt-get is a command-line way to download and install programs/packages and any dependencies quickly and easily from the Internet right from the command line. **Figure 1** shows a trivial example of such an installation when it leverages apt-get to install libunwind8 libicu52, on which PowerShell (on Ubuntu 14.04) depends. With PowerShell 5.0, the same functionality comes to Windows (I’m not sure whether to shout, “Yahoo!” or exasperatedly sigh, “Finally!”—perhaps both).

Just as there are repositories, like PowerShell Gallery, for PowerShell modules, PowerShell 5.0 also includes support for managing programs—called packages—in Windows. One such package manager is Chocolatey (chocolatey.org) and you can add it as a package repository using the following command:

Get-PackageProvider -Name chocolatey

This allows you to use PowerShell to find packages that have been deployed into Chocolatey. For example, if you want to install Visual Studio Code all you have to do is enter the commands:

Find-Package V\*S\*Code | Install-Package

As shown, wild cards are supported. Other Package commands to be familiar with are available using the following command, with the results shown in **Figure 3**:

Get-Help "-package" | Select-Object Name,Synopsis

![ Figure 3 Available Windows PowerShell Package Commands](images/Figure-3.png)

Figure 3 Available Windows PowerShell Package Commands

As you can see, you can both get and uninstall a package. Get-Package lists all the programs (and more) available from the Control Panel Programs and Features. Therefore, if you wanted to uninstall Notepad2, for example, you could use the command:

Get-Package Notepad2\* | Uninstall-Package

The ease this brings to auto­mating Windows computer setup is tremendous. I’ve been a Chocolatey fan for a number of years now and this integrates Chocolatey support directly into Windows. It ultimately brings package management to Windows in much the same way that Apt-Get does on Linux.

One thing to consider is that not only can the Chocolatey repos­itory be accessed via the \*-package\* PowerShell commands, but Chocolatey can also be installed directly. While not required, installing Chocolatey directly will occasionally provide a more robust feature set of package management functionality. Fortunately (and perhaps ironically), installing Chocolatey is simply a matter of invoking Install-Package Chocolatey, but (and this is an example of the discrepancies between Chocolatey and \*-Package behavior) the default install location will depend on which installation engine is used. Check out chocolatey.org/install for more information on the Chocolatey toolset, including installation instructions for your environment.

### OData with Export-ODataEndpointProxy

Another PowerShell 5.0 feature that’s worth mentioning is the ability to generate a set of methods that access an OData data source such as Visual Studio Team Services (VSTS). **Figure 4** demonstrates running the Export-ODataEndpointProxy on an OData service, a public sample Northwind OData service in this case.

![ Figure 4 Generating and Invoking an OData Proxy](images/Figure-4.png)

Figure 4 Generating and Invoking an OData Proxy

If you browse the generated module commands, you’ll notice that separate commands are generated for each entity (Advertisement, Category, Person and so forth), along with corresponding actions for each (Get, New, Remove, Set). One thing to note on the command line in **Figure 4** is the use of the -AllowUnsecureConnection parameter. This is necessary because the OData service used in this example doesn’t require authen­tication or encryption.

### Converting from Text to Objects with ConvertFrom-String

Another new command to appear in PowerShell 5.0 is ConvertFrom-String. It’s designed to take structured text as input and interpolate the structure so as to output an object based on the parsed text. You could use this, for example, to parse a flat file or (and this is where I find it extremely useful) to convert the text output from an executable into an object.

Consider, for example, SysInternal’s handle.exe program, (which you can install using the Install-Package Handle command—­leveraging package management as discussed in the previous section). As you’d expect from a command-line utility, it writes out text to stdout—in this case a list of open handles associated with a name. In PowerShell, however, you’ve grown accustomed to working with objects. And, to convert the text output into an object, you use the ConvertFrom-String function, as shown in **Figure 5**.

![ Figure 5 Utilizing ConvertFrom-String to Parse stdout into an Object](images/Figure-5.png)

Figure 5 Utilizing ConvertFrom-String to Parse stdout into an Object

**Figure 5** starts by showing the raw output of the handle.exe utility. Next, it demonstrates ConvertFrom-String without any parameters. As a result, the ConvertFrom-String utility simply splits the text on each line based on white space.

In the third example, I demonstrate the option of specifying a regular expression split pattern in order to fine-tune the parsing. However, note that familiarity with regular expressions isn’t required. You can instead specify a template—perhaps more accurately a sample—of either a file or a string, in which you parse the first few items manually. ConvertFrom-String then leverages the sample-parsed content and interprets how to parse the remainder of the input.

In the final example, I added the -PropertyNames parameter so as to assign meaningful names to the output.

In the end, ConvertFrom-String bridges the impedance mismatch of the text-based world of the traditional process stdout with a Power­Shell world built on objects. In this case, I can pipe the output into Stop-Process -Id mapping the pid value into the -Id parameter value.

### Classes and Enums

Finally, here’s a rundown on the new class and enumeration support. In PowerShell 5.0, two new keywords were added corresponding to the two structures so that you can now declare a class or an enumeration directly in PowerShell (rather than using Add-Type and passing C# code or perhaps instantiating a PSCustom­Object). The syntax is what you’d expect—see **Figure 6**.

**Figure 6 Declaring Classes and Enums in Windows PowerShell**

enum CustomProcessType {
  File
  Section
}
class CustomProcess {
  \[string\]$ProcessName;
  hidden \[string\]$PIDLabel;
  \[int\]$PID;
  hidden \[string\]$TypeLabel;
  \[CustomProcessType\]$Type;
  \[int\]$Handle;
  \[string\]$Path;
  CustomProcess(
    \[string\]$processName,\[string\]$pidLabel,\[int\]$pid,
    \[string\]$typeLabel,\[string\]$type,\[int\]$handle,\[string\]$path) {
    $this.ProcessName = $processName;
    $this.PIDLabel=$pidLabel;
    $this.PID=$pid;
    $this.TypeLabel=$typeLabel;
    $this.Type=$type;
    $this.Handle=$handle;
    $this.Path=$path;
  }
  CustomProcess() {}
  GetProcess() {
    Get-Process -Id $this.PID
  }
  static StopProcess(\[CustomProcess\]$process) {
    Stop-Process -Id $process.PID
  }
}

Notice in particular, that both properties and methods are supported. Furthermore, there are declaration modifiers like static and hidden, which designate the associated construct accordingly. Furthermore, inheritance is supported with a syntax very similar to C#:

class Employee : Person {}

Last, and also demonstrated in **Figure 6**, constructors can be declared. In this example, I declare a default constructor (with no parameters) and a second constructor that takes all the parameters. The constructors are invoked via the New-Object command by specifying either the -ArgumentList parameter (where an array of constructor arguments is listed) or else a HashTable argument is passed via the -Property parameter.

### Wrapping Up

By no means is this a complete list of new features in PowerShell 5.0. Other notable items include:

- Integration of archive (.zip file support) through the Compress-Archive and Expand-Archive commands.
- Get-Clipboard and Set-Clipboard commands that also work with the pipe operator.
- Out-File, Add-Content and Set-Content include a –NoNewline parameter, allowing for file content that omits the new-line character.
- The New-TemporaryFile command works similar to \[System.IO.Path\]::GetTempFileName (though not identically). Like its .NET equivalent, New-TemporaryFile doesn’t delete the temporary file, so be sure to save the output so you can remove the file once you’re done with it.
- SymbolicLinks can now be managed directly from the PowerShell cmdlets New-Item and Remove-Item.
- PowerShell Integrated Scripting Environment (ISE) now supports logging via the Start/Stop/Search-Transcript functions, which previously errored when called from PowerShell ISE.

Furthermore, while not supported immediately in the open source release of PowerShell, Microsoft fully intends to support Open SSH such that it will be a remoting transport option in PowerShell, as well as in Windows Remote Management, not long after this article is published.

All this to say, PowerShell just keeps getting better and better. If you haven’t learned it yet, get going.

_Thanks to the following IntelliTect technical experts for reviewing this article: Kevin Bost._

_This article was originally posted_ [_here_](https://docs.microsoft.com/en-us/archive/msdn-magazine/2016/october/essential-net-powershell-just-keeps-getting-better/) _in the October 2016 issue of MSDN Magazine._
