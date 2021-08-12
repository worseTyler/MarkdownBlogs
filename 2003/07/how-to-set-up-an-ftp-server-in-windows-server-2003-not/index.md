

In order to begin posting my weblog to a Windows 2003 box I needed to install and configure an FTP server.  Unfortunately, getting this to actually work has been way more difficult than the instructions here indicate.  Here is what I did:

- Ran the through the How to Set Up an FTP Server in Windows Server 2003 instructions
- Added another FTP site
    - On the Restrict FTP users to their own FTP home directory dialog I selected Isolate users (Users must be assigned an FTP home directory within the root of this FTP site.)
    - Home directory path was set to c:\\Inetpub\\ftproot
    - Permissions for both read and write were granted.
- Added a directory for the user I was logging in under

Tried to log in as user and got the following error:

> "530 User Test cannot log in, home directory inaccessible. Login failed."

Tried a number of things including logging in as anonymous but still I am unable to log in.

Arrrgghhhhh!! 
