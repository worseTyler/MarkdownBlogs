---
title: "SQL Server 2008 SP2 Uninstall Fails"
date: "2011-09-06"
categories: 
  - "blog"
tags: 
  - "sql-server"
---

Recently, I tried opening an MDF file from my SQL Server Express instance only to discover be prompted with the following error:

> The database cannot be opened because it is version 661. This server supports version 662 and earlier. A downgrade path is not supported.

This error message is completely misleading, however, as it indicates that my installation of SQL Server is later than the database version that I am attempting to open.  In actual fact, the  issue is that SQL Server 2008 (SP2) supports 662 while while SQL Server 2008 R2 SP1 supports version 661.  A more accurate message might be:

> The database cannot be opened because it is version 661. This server supports versions 662, 655 and earlier than 655. A downgrade path is not supported (see [here](https://rusanu.com/2010/11/23/this-server-supports-version-662-and-earlier/))

Executing SELECT @@Version on my SQL Server Express instance confirmed that indeed, I was running SQL Server 2008 SP2 rather than 2008 R2 as I had expected:

> Microsoft SQL Server 2008 (SP2) - 10.0.4000.0 (X64) Sep 16 2010 19:43:16 Copyright (c) 1988-2008 Microsoft Corporation Express Edition (64-bit) on Windows NT 6.1 <X64> (Build 7601: Service Pack 1)

No problem, I would just upgrade by running the SQL Server 2008 R2 Express install.  Unfortunately, this failed with the following message:

> Could not open key: HKEY\_LOCAL\_MACHINE\\SOFTWARE\\Microsoft\\\\WwanSvc\\Profiles
> 
> (Yes, there were two slashes but regardless, the key was not to be found when I looked in the registry.)

Arghhh!!!  This brings back [nightmares from a while back when installing SQL Server 2008](/sql-server-2008-install-nightmare/). (That was a different machine and different circumstances but still, the scars remain.)

Searches on the registry key were depressing and misleading pointing to serious viruses and the like.  Yikes!

Next I tried just uninstalling the Database Engine and related components (similar but with screenshots), but this too failed with the same message.

**Uninstall Solution:** The solution in the end was to [uninstall via the command line](https://msdn.microsoft.com/en-us/library/ms144259.aspx) which worked:

> "C:\\Program Files\\Microsoft SQL Server\\100\\Setup Bootstrap\\SQLServer2008R2\\setup.exe" /ACTION=uninstall /FEATURES=SQL /INSTANCENAME=SQLExpress

Of course moving on to the installation step was not trivial either.  Running the SQL Server 2008 R2 install failed with messages like:

> Instance Name SQLEXPRESS for product SQL Server Database Services is already used.  Each SQL Server instance must have a unique instance name.

In addition, the **Service Accounts** tab was blank (along with the **Collation** tab) … there was simply no text and, therefore, not way to specify the account under which SQL Server would run. (Fiddlesticks!!)  Interestingly, installing SQL Express with Advanced Services didn’t have this problem – it showed services accounts for SQL Server Reporting Services and Business Intelligence Services.

The next step was to browse to the C:\\Program Files\\Microsoft SQL Server directory and delete all MSSQL10\*.SQLEXPRESS instances.  However, the installation continued to crash repeatedly before displaying the **Complete with failures** screen and the following so called “information:”

- Your SQL Server 2008 R2 installation completed with failures.
- Instance Name SQLEXPRESS for product SQL Server Database Services is already used. Each SQL Server instance must have a unique instance name.
- Previous instance properties are only valid in upgrade and repair of upgrade scenario.
- Registry properties are not valid under this context.
- Object reference not set to an instance of an object.

Installation using the command line failed as well.

**Final Installation Resolution:** After many more installation/uninstallation attempts the final resolution was to delete the HKEY\_LOCAL\_MACHINE\\Software\\Microsoft\\Microsoft SQL Server\\SQLEXPRESS registry key (thanks to Alpha Wang for finding this). Without this entry, the SQLEXPRESS installation went through to completion.

!$%@&#%&^\*$^&\*#$%@#^!!!
