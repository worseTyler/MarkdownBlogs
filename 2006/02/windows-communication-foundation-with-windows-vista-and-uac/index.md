

## Solution to Windows Vista "Access Denied"
#
> System.Net.HttpListenerException: Access is denied

The issue is User Access Control (UAC), a new feature of Windows Vista that causes processes to run as standard user even if you are logged in with a user that is the member of the Administrators group.  Opening the port for WCF requires administrative access and, unless the process is elevated, no such access is available so opening the port results in the access denied message.

To handle this error it is necessary to cause a Permit/Deny dialog to appear.  The same dialog appears when running administrative tools like Computer Management.

![Windows Vista Permit/Deny Dialog](https://intellitect.com/wp-content/uploads/binary/WindowsCommunicationFoundationWCFWithWindowsVistaAndUAC/WindowsVistaPermitDenyDialog.JPG "Windows Communication Foundation with Windows Vista and UAC")

Clicking the Permit button elevates the process, assuming the logged on user has the necessary permissions for the action.

One way to turn on the Permit/Deny dialog is to place a manifest into the same directory as the application executable.  The manifest file is named using the full application name (including the EXE extension) with an additional ".MANIFEST" suffix (WCFService.exe.MANIFEST for example).  The content of the file is XML specifying that the application requires administrator permissions so the dialog needs to be displayed to elevate the process.

```
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
    <assembly xmlns="urn:schemas-microsoft-com:asm.v1" manifestVersion="1.0">
	<trustInfo
		xmlns="urn:schemas-microsoft-com:asm.v3">
		<security>
			<requestedPrivileges>
				<requestedExecutionLevel level="requireAdministrator">
				</requestedPrivileges>
			</security>
		</trustInfo>
	</assembly> 
</xml>
```

Note that it is not possible to supply a manifest file to run elevated without the dialog.

Other ways of avoiding the Access Denied message are:

1. Turn off UAC by changing the security policy.   Open the Local Security Policy and browse to Security Settings->Local Policies->Security Options' and the User Account Protection options.  Specify all processes run elevated without prompting.
2. Increase your security vulnerability and logon as Built-In Administrator.
3. Launch the WCF Service process in elevated mode (right-click menu option).
4. Launch the WCF Service from a process, such as Visual Studio or the Command Prompt (MSH), that is running elevated.  Again, right click on the shortcut or executable and select Run Elevated....
