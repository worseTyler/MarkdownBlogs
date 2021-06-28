
While trying to load an assembly into PowerShell recently, an exception occurred,

> "Exception calling "LoadFrom" with "1" argument(s): "Could not load file or assembly 'file:///...' or one of its dependencies. An attempt was made to load a program with an incorrect format."

The cause of the problem was a mismatch in the platform target of the assembly. The assembly targets "x86" for example, rather than "Any CPU" or "x64" - the latter is the default PowerShell platform on Windows 8+ and Windows 2012+.

To correct the problem you either need to recompile the assembly using a more appropriate platform target (see the Visual Studio project's property Build tab) or you need to run the 32-bit version of PowerShell:

`%windir%\syswow64\Windowspowershell\v1.0\powershell.exe`

or, for ISE:

`"%windir%\syswow64\Windowspowershell\v1.0\powershell_ise.exe"`

Windows 8+ runs the x64 version of PowerShell by default when launched from the Windows Menu and doesn't even show an x86 icon. To correct this, you need to show the Administrative Tools.
