
While trying to set up a VPN Server using Windows 2003 I wanted to assign users access based on policy rather then individually selecting the dial-in or deny privilege on the user properties.  Unfortunately, the "Control access through Remote Access Policy" option was disabled.

After a short investigation I learned [here](https://support.microsoft.com/?kbid=313082) that some [Dial-In Options Unavailable with Active Directory in Mixed Mode](https://support.microsoft.com/default.aspx?scid=kb;EN-US;193897).  The article appears to be a Win2K issue but presumably the problem exists in Win 2003 also and that is why mine is disabled.
