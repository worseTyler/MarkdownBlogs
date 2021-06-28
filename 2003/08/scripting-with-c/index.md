
As part of a writing project I am involved in at the moment I was looking for a C# scripting tool that would allow me to execute C# as a script rather than compiling it.  I looked at 3 different options:  1) [Alintex Script host](https://www.alintex.com/) 2) nsh and 3) NScript.  There are also two MSDN articles on this called [Script Happens](https://msdn.microsoft.com/library/default.asp?url=/library/en-us/dnclinic/html/scripting06112001.asp).

After trying each one out I went with NScript.  Essentially this allowed me most easily to write a C# file that I could then save with an NCS extension and run from the command line.  The NCS extension automatically caused the script to launch.

I did make one modification and that was to have NScript.exe be associated with .NCS files rather than NScriptW.exe.  I did this since the majority of my test are command line.  One annoying restriction was that System.Console.ReadLine() is not supported.  The code for this is relatively simple so it could be added.
