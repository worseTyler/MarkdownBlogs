

Initially this post began as a long question to Microsoft but as I began to describe the problem I realized the rather obvious solution.  Anyway, since it took me a little while to solve I decided to post it anyway:

If you previously had the 1.2 Framework installed and then you uninstalled it in order to install the most recent drops you may find that the "Lib" environment variable still contains the 1.2 path.  You need to update this environment variable to no longer include the 1.2 path and instead only have the path to the 1.1 Framework.  In my case this required updating both the system and user "Lib" environment variable.  (It appears you do not need to add the 2.0 path if you happen to have a more recent drop than the PDC bits.)

The exact error that occured for me appears below:

> **Compilation Error** **Description:** An error occurred during the compilation of a resource required to service this request. Please review the following specific error details and modify your source code appropriately. **Compiler Error Message:** CS1668: Warning as Error: Invalid search path 'C:\\Program Files\\Microsoft Visual Studio .NET Whidbey\\SDK\\v1.2\\Lib\\' specified in 'LIB environment variable' -- 'The system cannot find the path specified. ' **Source Error:**
> 
> \[No relevant source lines\]
> 
> Source File:    Line: 0

Hopefully this doesn't happen for anyone else but if it happens to hopefully this will save you some time.
