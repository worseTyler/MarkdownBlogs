

## Converting Command Line String to ARGS 
#
In testing a command line I recently wanted to verify that the string passed on the command line was converted to the ```args[]``` array that was passed to ```Main(string[] args)```.  For example, given the command line

```Compress.exe /v:ReallyMakeItSmall myfile.txt "Read ME.txt"``` 

What does the call to  Main(string args[])  resolve  args  to?  There are two ways to test this.  Firstly, you can start a the process with the corresponding command line and then test the args array.  But this is cumbersome because data needs to be passed between the new process and the test process.

The alternate solution is to use the ```CommandLineToArgW()``` API. This API converts a string into is ```args```. The problem, however, is that it is a rather cumbersome API to call.  The declaration is as follows:

```csharp
[DllImport( "shell32.dll" , SetLastError = true )]
static extern IntPtr CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);
```
It is great that it claims to return the arguments but how the heck does one get at them?   pNumArgs  is presumably no help and  IntPtr  isn't exactly an array of strings either.  It turns out that you need to marshal the data back manually, as demonstrated in the following class:
```csharp
using  System;
using  System.Runtime.InteropServices;

namespace Dnp 
{  
    public abstract class CommandLineHandler 
    { 
        [DllImport("shell32.dll", SetLastError = true )] 
        static extern IntPtr CommandLineToArgvW( [MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);
        public static string [] CommandLineToArgs(string commandLine)
        {  
            string executableName;  
            return  CommandLineToArgs(commandLine, out executableName); 
        }
        public static string [] CommandLineToArgs(string commandLine, out string executableName) 
        { 
            int argCount; 
            IntPtr result;  
            string  arg; 
            IntPtr pStr; 
            result  =  CommandLineToArgvW(commandLine,  out  argCount);
            if  (result == IntPtr.Zero) 
            {  
                throw new System.ComponentModel.Win32Exception(); 
            }  
            else
            {  
                try  
                {  
                // Jump to location 0\*IntPtr.Size (in other words 0).  
                pStr = Marshal.ReadIntPtr(result, 0 * IntPtr.Size); 
                executableName = Marshal.PtrToStringUni(pStr);
                // Ignore the first parameter because it is the application   
                // name which is not usually part of args in Managed code.   
                string [] args = new string [argCount-1];  
                for(int i = 0; i < args.Length; i++) 
                    { 
                        pStr = Marshal.ReadIntPtr(result, (i+1) * IntPtr.Size); 
                        arg = Marshal.PtrToStringUni(pStr); 
                        args[i] = arg; 
                    }
                return  args; 
                }  
                finally  
                { 
                    Marshal.FreeHGlobal(result); 
                } 
            } 
        }
    // ... 
    } 
} 
```

Not exactly obvious.

UPDATE 6/6/2005

Added try-finally block and a call to ```Marshal.FreeHGlobal()``` thanks to comment by [Atif Aziz](https://www.raboof.com/). For reference on P/Invoke, SetLastError, and Win32Exception  I appreciate [Shawn Van Ness](https://www.windojitsu.com) comments here.
