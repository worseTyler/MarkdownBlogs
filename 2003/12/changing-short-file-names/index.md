

## Attempt at Setting Short File Names 
#
I find "Documents and Settings" and "Program Files" extremely cumbersome names that the OS creates by default.  I am reluctant to change the names (by changing the HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\ProfileList\\ProfilesDirectory string for example) because some programs my hard code these paths.  Instead, what I would like is to change the short file name from something like "Docume~1" to "Data" or the like.  I located the setsfn.exe program that is able to set short file names.  The command line is as follows:

> Set Short File Name [Version 1.00]
> 
> Sets the 8.3 SFN (Short File Name) for the specified file or folder. Requires Windows XP or later.
> 
> Syntax: setsfn /F:filename [/SFN:shortname] [/Y]
> 
> /F:    Specifies an existing file or folder to modify. /SFN:  Specifies the new short name.  Must be a valid 8.3 format name. /Y     Suppresses the 'Are you sure' prompt. /? or -? displays this syntax and always returns 1. A successful completion returns 0.
> 
> Copyright 2003 Marty List, www.optimumx.com

Unfortunately, this doesn't work on the two directories I specify above because of the fact that they are always in use.  My next attempt is to see whether I can set them during an automatic install.
