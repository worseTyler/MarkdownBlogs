

For the most part I have much of the install for Windows 2008 operating system and programs automated (unattended).  However, one thing that I found a little more difficult to find was a command line way to turn off IE's Enhanced Security (manually turned off from **Server Manager -> Configure IE ESC**.

I found an unattended method and created a batch file:

```
:: Backup registry keys 
REG EXPORT "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Active Setup\\Installed Components\\{A509B1A7-37EF-4b3f-8CFC-4F3A74704073}" 
"%TEMP%.HKEY_LOCAL_MACHINE.SOFTWARE.Microsoft.Active Setup.Installed Components.A509B1A7-37EF-4b3f-8CFC-4F3A74704073.reg" 
REG EXPORT "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Active Setup\\Installed Components\\{A509B1A7-37EF-4b3f-8CFC-4F3A74704073}" 
"%TEMP%.HKEY_LOCAL_MACHINE.SOFTWARE.Microsoft.Active Setup.Installed Components.A509B1A8-37EF-4b3f-8CFC-4F3A74704073.reg"

REG ADD "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Active Setup\\Installed Components\\{A509B1A7-37EF-4b3f-8CFC-4F3A74704073}" /v "IsInstalled" /t REG_DWORD /d 0 /f 
REG ADD "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Active Setup\\Installed Components\\{A509B1A8-37EF-4b3f-8CFC-4F3A74704073}" /v "IsInstalled" /t REG_DWORD /d 0 /f

Rundll32 iesetup.dll, IEHardenLMSettings 
Rundll32 iesetup.dll, IEHardenUser 
Rundll32 iesetup.dll, IEHardenAdmin

REG DELETE "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Active Setup\\Installed Components\\{A509B1A7-37EF-4b3f-8CFC-4F3A74704073}" /f /va REG DELETE "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Active Setup\\Installed Components\\{A509B1A8-37EF-4b3f-8CFC-4F3A74704073}" /f /va

:: Optional to remove warning on first IE Run and set home page to blank. 
REG DELETE "HKEY_CURRENT_USER\\Software\\Microsoft\\Internet Explorer\\Main" /v "First Home Page" /f 
REG ADD "HKEY_CURRENT_USER\\Software\\Microsoft\\Internet Explorer\\Main" /v "Default_Page_URL" /t REG_SZ /d "about:blank" /f 
REG ADD "HKEY_CURRENT_USER\\Software\\Microsoft\\Internet Explorer\\Main" /v "Start Page" /t REG_SZ /d "about:blank" /f
```

This seems to work well for Windows 2008.  I haven't tried it on other operating systems.
