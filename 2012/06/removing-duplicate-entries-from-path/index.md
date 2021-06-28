
As developers, we frequently end up with duplicate entries in our path. From the command line you can clean up your path using pathman.exe. Here's a PowerShell Script to find the duplicates and remove them using Pathman.exe: ``$extraPath=(($path.Split("`;") | group | ?{$_.Count -gt 1}).Values | %{$_[0]} pathman.exe /ru $extrapath``

Disclaimer: Works for us.
