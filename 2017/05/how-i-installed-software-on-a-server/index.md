
##### I was tasked with installing custom and generic software on a dozen new servers. As an SDET, I have never done this type of task before, so I googled some methods for automating this process since manually installing each server was time-consuming. PowerShell seemed to be the logical tool for running through my list of tasks. I was able to install all the necessary programs in under three hours per server. Before this process, it took almost a whole day per server. The only requirement for the code below is PowerShell version 3.0 or later.

Connecting to Server Remotely

##### I decided to run this script remotely. This would eliminate all manual steps on my part when the machine was required to restart. I would not have to log onto the server multiple times when a restart occurred.

$ServerName = "MyServer01"
$password = ConvertTo-SecureString "IlovePiZZa" -AsPlainText -Force
$cred = New-Object System.Management.Automation.PSCredential ("UsernameToServer", $password )
Enter-PSSession -ComputerName $ServerName -Credential $cred

Installing Windows Updates

##### I noticed the servers I was given were out of date. I found a function for downloading and installing all available Windows updates. The module I found was from [technet.microsoft.com](https://gallery.technet.microsoft.com/scriptcenter/2d191bcd-3308-4edd-9de2-88dff796b0bc/). Be sure to run "Import-Module PSWindowsUpdate" first.

Get-WUInstall -WindowsUpdate -IgnoreUserInput -WhatIf -Verbose

Custom Oracle Client

##### The biggest hiccup I had was installing the custom Oracle client directly from the PowerShell script. The documentation I found on Oracle's page was and still is, incorrect. It is possible to install Oracle from PowerShell and with an optional parameter to pass a .rsp file. This .rsp file contains the settings needed for your custom Oracle install. The section which tripped me up was the custom components. The example directly from Oracle shows double quotes around each option separated by commas. _This was the incorrect format,_ double quotes will throw an error when the command is run. The correct format is each custom component separated only by a comma. I figured this out by blind luck, but now I know for the future. The Oracle documentation also gives the format on how to run the setup installer with your .rsp file in the command line. In PowerShell, the command line can be reached by typing in cmd.exe and to exit, simply type exit. The -silent parameter at the end means the Oracle installer window will not display and the installer will run with no other input needed.

cmd.exe
C:\\Users\\UsernameToServer\\oracle\\setup.exe -responseFile "C:\\Users\\UsernameToServer\\InstallCustomOracle12c.rsp" -silent
exit

Message Queuing

##### The servers required the Message Queuing feature, Powershell has a nice way of achieving this.

Import-Module Servermanager
Add-WindowsFeature MSMQ

Custom .msi Installation

##### As part of this process, a custom installer was required for an internal program. We have a location storing old versions of this installer alongside the current version. The naming convention is not always consistent, so I couldn't find the latest version by the name. Instead, I used the file with the date most recently modified. I was also worried other file types might contaminate the installers folder, so I added a filter on the file type.

$dir ="C:\\Desktop\\Installers\\"
$filter = "\*.msi"
$latest = Get-ChildItem -Path $dir -Filter $filter | Sort-Object LastWriteTime -Descending | Select-Object -First 1
msiexec.exe ($dir + $latest.name)

Emailing Log Files

##### I want to verify the error logs to ensure none of the installers logged any issues. Instead of logging into the server myself and opening the log, I emailed the log I wanted to myself.

$TimeStamp = get-Date -f yyyyMMddhhmm
$Path = "C:\\Desktop\\Error Logs\\Error\_Log\_$ServerName\_$TimeStamp.csv"
Get-WinEvent -LogName "Application" -MaxEvents 100 -EA SilentlyContinue | Where-Object {$\_.id -in $EventID -and $\_.Timecreated -gt (Get-date).AddHours(-24)} | Sort TimeCreated -Descending | Export-Csv $Path -NoTypeInformation
write-host "Issuing email informing script has completed" -foreground "green"
$Outlook = New-Object -ComObject Outlook.Application
$Mail = $Outlook.CreateItem(0)
$Mail.To = "anna@sharpe.com"
$Mail.Subject = "Server Setup Complete"
$Mail.Body = "Server has been set up for Designer.  It is currently restarting."
$mail.Attachments.Add("C:\\Desktop\\Error Logs\\Error\_Log\_$ServerName\_$TimeStamp.csv")
$Mail.Send()

Restart Server

##### Once the email was sent, I restarted the machine and logged off. Job done!

Restart-Computer -wait
Exit-PSSession
