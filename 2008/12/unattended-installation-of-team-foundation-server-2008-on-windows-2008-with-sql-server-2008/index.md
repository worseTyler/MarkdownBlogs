## Installation of 2008's Team Foundation Server Onto Windows With SQL Server
#
The following are my instructions for installing a new **Team Foundation Server 2008 with SP1** onto **Windows 2008** with **SQL Server 2008**. Throughout, I followed the [TFS install guide](https://www.microsoft.com/downloads/details.aspx?FamilyID=ff12844f-398c-4fe9-8b0d-9e84181d9923&displaylang=en) and tried to automate where it didn't distract me too much from the task at hand. I followed the Single-Server Team Foundation Server Installation.**Folder Layout**For the scripts to work successfully, you need the following placed into a local directory (probably without spaces in the name):

> .\\ [dotnetfx35.exe](https://www.microsoft.com/downloads/details.aspx?FamilyId=AB99342F-5D1A-413D-8319-81DA479AB0D7&displaylang=en).\\SQLServer2008\\ (SQL Server 2008 install).\\TFS2008\\ (TFS 2008 install) .\\ [TFS90sp1-KB949786-ENU.exe](https://www.microsoft.com/downloads/details.aspx?familyid=9E40A5B6-DA41-43A2-A06D-3CEE196BFE3D&displaylang=en).\\ [TFSSetup.exe](https://intellitect.com/wp-content/uploads/binary/InstallingTeamFoundationServer2008onWind_7484/TFSSetup.zip) (A custom TFS 2008 Setup utility I created for unattended installation) .\\ WSS3wSP1.x86.exe

**Prerequisites**Start with a mostly virgin Windows 2008 server. I say mostly because I joined the machine to the domain and installed all Windows Updates. In addition, I created a single TFSSERVICE domain account.**Installation**

1. Turn on remote administration. [Optional]
    
    REG ADD "hklm\\system\\currentcontrolset\\control\\terminal server" /f /v fDenyTSConnections /t REG_DWORD /d 0
    
2. Install IIS 7.0
    
    ServerManagerCmd -install Web-ServerServerManagerCmd -install Web-Http-RedirectServerManagerCmd -install Web-Asp-NetServerManagerCmd -install Web-Windows-Auth ServerManagerCmd -install Web-Mgmt-Compat
    
3. Installed the [.NET Framework 3.5 Service Pack 1 (Full Package)](https://download.microsoft.com/download/2/0/e/20e90413-712f-438c-988e-fdaa79a8ac3d/dotnetfx35.exe)
    
    // use /norestart to prevent autorestart .\\dotnetfx35.exe /qb
    
4. Installed SQL Server 2008.
    
    .\\SQLServer2008\\Setup.exe /ACTION=Install /FEATURES=SQLENGINE,FULLTEXT,AS,RS,SSMS, ADV_SSMS / INDICATEPROGRESS /QUIETSIMPLE /ERRORREPORTING=1 /INSTANCEDIR= "%ProgramData%\\Microsoft SQL Server" /SQMREPORTING=True /INSTANCENAME= "MSSQLSERVER" /AGTSVCACCOUNT= "NT AUTHORITY\\NETWORK SERVICE" /AGTSVCSTARTUPTYPE= "Automatic" /ISSVCSTARTUPTYPE= "Automatic" /ISSVCACCOUNT= "NT AUTHORITY\\NetworkService" /ASSVCACCOUNT= "NT AUTHORITY\\NETWORK SERVICE" /ASSVCSTARTUPTYPE= "Automatic" /ASDATADIR= "C:\\ProgramData\\Microsoft SQL Server\\MSAS10.MSSQLSERVER\\OLAP\\Data" /ASLOGDIR= "%ProgramData%\\Microsoft SQL Server\\MSAS10.MSSQLSERVER\\OLAP\\Log" /ASBACKUPDIR= "%ProgramData%\\Microsoft SQL Server\\MSAS10.MSSQLSERVER\\OLAP\\Backup" /ASTEMPDIR= "%ProgramData%\\Microsoft SQL Server\\MSAS10.MSSQLSERVER\\OLAP\\Temp" /ASCONFIGDIR= "%ProgramData%\\Microsoft SQL Server\\MSAS10.MSSQLSERVER\\OLAP\\Config" /ASPROVIDERMSOLAP= "1" /ASSYSADMINACCOUNTS= "BUILTIN\\Administrators" /SQLSVCSTARTUPTYPE= "Automatic" /SQLSVCACCOUNT= "NT AUTHORITY\\NETWORK SERVICE" /SQLSYSADMINACCOUNTS= "BUILTIN\\Administrators" /BROWSERSVCSTARTUPTYPE= "Automatic" /RSSVCACCOUNT= "NT AUTHORITY\\NETWORK SERVICE" /RSSVCSTARTUPTYPE= "Automatic" /RSINSTALLMODE= "FilesOnlyMode" /FTSVCACCOUNT= "NT AUTHORITY\\LOCAL SERVICE"
    
    The only customization I made was to move the data storage to C:\\ProgramData\\... rather than the default C:\\Program Files\\... . I prefer this because I like to keep all data separate from the program executables themselves. One other thing to note is that my log files are on the same disk as the databases - consider improving for your scenario. NOTE: The first phase of the SQL Server 2008 install is to install Windows Installer 4.5 if it isn't already installed. Unfortunately, this requires a reboot after which you need to re-run the SQL Server 2008 setup. Consider running an autologon.exe utility at the beginning of your installation (so that the password prompt occurs before you leave the computer to do the install) in order to set the computer to automatically logon after rebooting. You will also want to use the Runonce registry setting to re-run the SQL Server 2008 install.
    
5. Installed Windows SharePoint Services 3.0 with SP1
    
    IF NOT DEFINED TFSSERVICEPASSWORD ( SET /P TFSSERVICEPASSWORD=Enter the TFS Service password: )WSS3wSP1.x86.exe /Extract:"%TEMP%\\WSS3wSP1" /passive /quiet"%TEMP%\\WSS3wSP1\\setup.exe" /config "%~dp0Wss4TfsSingleServerConfig.xml"CHOICE /T 1 /D Y /M Waiting...RD /Q /S "%TEMP%\\WSS3wSP1SET PSCONFIG="%COMMONPROGRAMFILES%\\Microsoft Shared\\web server extensions\\12\\bin\\psconfig.exe"%PSCONFIG% -cmd configdb -create -server localhost -database SharePoint_Config -user %USERDOMAIN%\\TFSSERVICE -password %TFSSERVICEPASSWORD% -admincontentdatabase SharePoint_Admin_Content%PSCONFIG% -cmd adminvs -provision -port 2500 -windowsauthprovider onlyusentlmSET STSADM="%COMMONPROGRAMFILES%\\Microsoft Shared\\web server extensions\\12\\bin\\stsadm.exe"%STSADM% -o extendvs -exclusivelyusentlm -url https://%COMPUTERNAME%.%USERDNSDOMAIN% -ownerlogin %USERDOMAIN%\\tfsservice -owneremail "tfs@%USERDNSDOMAIN%" -sitetemplate sts -description "Team Foundation Server"%STSADM% -o siteowner -url "https://%COMPUTERNAME%.%USERDNSDOMAIN%" -secondarylogin %USERDOMAIN%\\%USERNAME% %PSCONFIG% -cmd adminvs -provision -port 2500 -windowsauthprovider onlyusentlm
    
    This script extracts WSSwSP1 to a temp directory and then executes the setup unattended using the Wss4TfsSingleServerConfig.xml file with ServerRole Web Frond End (WFE). The WSS setup doesn't perform any configuration. Instead, the psconfig.exe and stsadm.exe configure the server following the install. Note that I use port 2500 as my SharePoint central administration port (something I have standardized on). In addition, I use a fully qualified name for the WSS url. The Wss4TfsSingleServerConfig.xml contains the following:

```    
<Configuration>
	<Package Id= "sts"\>
		<Setting Id= "REBOOT" Value= "ReallySuppress"/>
		<Setting Id= "SETUPTYPE" Value= "CLEAN_INSTALL"/>
	</Package>
	<DATADIR Value= "%ProgramData%\\Microsoft SQL Server\\MSSQL$SHAREPOINT\\Data"/>
	<Logging Type= "verbose" Path= "%temp%" Template= "Microsoft Windows SharePoint Services 3.0 Setup(\*).log"/>
	<Setting Id= "SERVERROLE" Value= "WFE"/>
	<Setting Id= "UsingUIInstallMode" Value= "0"/>
	<Display Level= "none" CompletionNotice= "no" AcceptEULA= "Yes"/>
</Configuration>
```
    
6. Slipstreamed Team Foundation Server SP 1 into the .\\TFS2008wSP1 directory.
    
    > .\\TFS90sp1-KB949786-ENU.exe /extract:"%TEMP%\\TFS2008SP1"msiexec /a .\\TFS2008\\at\\vs_setup.msi /p "%TEMP%\\TFS2008SP1\\TFS90sp1-KB949786.msp" TARGETDIR="%CD%\\TFS2008wSP1\\AT" /L\*vx c:\\temp\\install.logCHOICE /T 1 /D Y /M Waiting... RD /Q /S "%TEMP%\\TFS2008SP1"
    
7. Since there are no unattended command line switches for the Team Foundation Server install, I created a custom executable called [TFSSetup.exe](https://intellitect.com/wp-content/uploads/binary/InstallingTeamFoundationServer2008onWind_7484/TFSSetup.zip) that I created to automate an unattended install of Team Foundation Server SP1. A few things to note about the custom executable:
    
    - It accepts the license terms and assumes the product key is defaulted.
        
    - The deployment directory is the default directory for the TFS setup.
        
    - It assumes a Single Server TFS deployment and uses the default database server name (%COMPUTERNAME%)
        
    - It assumes the TFS Service name is %USERDOMAIN%\\TFSSERVICE and that the password is stored in the environment variable TFSSERVICEPASSWORD (if it is not, the program will prompt for the password).
        
    - The SharePoint settings are https:\\\\%COMPUTERNAME%:2500 for the admin site and https:\\\\%COMPUTERNAME%.%USERDNSDOMAIN%/Sites for the Sites URL.
        
    - Alerts are not enabled.
        

**Possible Errors:**

**Error**: This installation package could not be opened. Contact the application vendor to verify that this is a valid Windows Installer package.

> Verify that the source directory (with the RTM version of TFS) is pointing to the AT directory.

**Error**: The installer has encountered an unexpected error installing this package. This may indicate a problem with this package. The error code is 2203.

> This is caused because some of the files are read-only. Run Attrib -R .\\\* /S /D on both the SP1 and TFSRTM directories.

**Error**: The upgrade patch cannot be installed by the Windows Installer service because the program to be upgraded may be missing, or the upgrade patch may update a different version of the program. Verify that the program to be upgraded exists on your computer and that you have the correct upgrade patch.**Error**: The server parameter specified with the configdb command is invalid. Failed to connect to the database server or the database name does not exist. Ensure the database server exists, is a Sql server, and that you have the appropriate permissions to access the database server. To diagnose the problem, review the extended error information located at C:\\Program Files\\Common Files\\Microsoft Shared\\Web Server Extensions\\12LOGS\\PSCDiagnostics_12_14_2008_18_24_58_443_440016503.log . Please consult the SharePoint Products and Technologies Configuration Wizard help for additional information regarding database server security configuration and network access.

> This error occurred because I included both the dbuser and dppassword when executing psconfig.exe configdb. Upon removing both those parameters, the error was avoided.

**Error**: The SQL Server Browser service is stopped.**Error**: The SQL Server Browser service is not set to start automatically.**Error**: TF220041: The specified Windows SharePoint Services site URL ( https://<DefaultSiteURL>/Sites) is not the default site collection site: (Sites).The default site collection site might have been renamed or removed. Make sure that the site exists and verify the correct URL,and then attempt installation. If the problem persists, you can choose to install and configure Windows SharePoint Services aspart of the Team Foundation Server installation process.

> This error occurs when you specify the Windows SharePoint Server URL without the "/Sites" suffix. Team Foundation Server 2008 does not work correctly against the root. You must specify includes the "/Sites" suffix to avoid this error.
