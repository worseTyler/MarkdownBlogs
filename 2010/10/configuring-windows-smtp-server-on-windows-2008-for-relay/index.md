

Estimated reading time: 6 minutes

My SMTP mail server requires that email be sent with TLS encryption and on port 587.  This makes it problematic for Team Foundation Server (TFS) to send emails directly.  To work around the problem, I installed the Windows 2008 SMTP Server service and configured it for relaying to my real SMTP mail server. 

### Contents

- [Install SMTP Server](#h-install-smtp-server)
- [Configuring the SMTP Service to Auto-Start](#h-configuring-the-smtp-service-to-auto-start)
- [Configuring the SMTP Service for Routing to Alternate SMTP Server](#h-configuring-the-smtp-service-for-routing-to-alternate-smtp-server)
- [Internet Information Services (IIS) Manager (IIS 7.0)](#h-internet-information-services-iis-manager-iis-7-0)
- [Internet Information Services (IIS) 6.0 Manager](#h-internet-information-services-iis-6-0-manager)
- [Command Line](#h-command-line)
- [Want More?](#h-want-more)

### Install SMTP Server

To begin, launch Server Manager and add the **SMTP Server** feature.

![image](https://intellitect.com/wp-content/uploads/2010/10/image.png "Configuring Windows SMTP Server on Windows 2008 for Relay")

This includes a dialog to add some additional items.

![image](https://intellitect.com/wp-content/uploads/2010/10/image1.png "Configuring Windows SMTP Server on Windows 2008 for Relay")

Click **Add Required Features** and, after the dialog closes, click **Next >** followed by **Install**.

The same can be done from the command line using:

> <table border="1"><tbody><tr><td><pre>ServerManagerCmd –Install SMTP-Server 
> </pre></td></tr></tbody></table>

### Configuring the SMTP Service to Auto-Start

Unfortunately, the service is not configured to auto-start by default so you need to go into the services to change this:

![image](https://intellitect.com/wp-content/uploads/2010/10/image2.png "Configuring Windows SMTP Server on Windows 2008 for Relay")

To perform the same action from the command line use the following PowerShell commands:

> <table border="1"><tbody><tr><td><pre>Get-Service SMTPSvc | Set-Service –StartupType Automatic</pre></td></tr></tbody></table>

From DOS you could use sc.exe.

### Configuring the SMTP Service for Routing to Alternate SMTP Server

Finally, you need to configure the SMTP Server to redirect to an alternate SMTP server (assuming it is not sending email directly).  This involves settings in both IIS 7.0 and IIS 6.  Open Internet Information Server (IIS 7.0) and select the server node

### Internet Information Services (IIS) Manager (IIS 7.0)

1. From inside **Internet Information Services (IIS) Manager**, browse to the server’s **SMTP E-mail** feature and open it.  
    ![image](https://intellitect.com/wp-content/uploads/2010/10/image3.png "Configuring Windows SMTP Server on Windows 2008 for Relay")
2. Inside the **SMTP E-mail** windows, enter in the “send from” email address, the remote SMTP server DNS name (or IP address) and the remote server port.  In addition, select **Specify credentials** and enter the credentials required to connect to the remote SMTP server.  
    ![image](https://intellitect.com/wp-content/uploads/2010/10/image5.png "Configuring Windows SMTP Server on Windows 2008 for Relay")

To perform the same action from the command line use the following commands (Powershell is optional):

> <table border="1"><tbody><tr><td><pre>appcmd.exe set config /commit:WEBROOT /section:smtp /from:Inigo.S.Montoya@IntelliTechture.com <br>/deliveryMethod:Network /network.port:587 /network.defaultCredentials:False <br>/network.host:smtp.intelliTechture.com /network.userName:Inigo.S.Montoya@intelliTechture.com <br>/network.password:***</pre></td></tr></tbody></table>
> 
> Where appcmd.exe is located in %windir%\\System32\\inetsrv\\appcmd.exe.

### Internet Information Services (IIS) 6.0 Manager

1. Launch **Internet Information Services (IIS) 6.0 Manager** locate the **[SMTP Virtual Server #]** node and open up the **Properties** dialog.  
    ![image](https://intellitect.com/wp-content/uploads/2010/10/image8.png "Configuring Windows SMTP Server on Windows 2008 for Relay")  
    As shows, I just added **127.0.0.1** so that I was only enabling the current box to send via this SMTP Server but you can also add a group of computers by subnet or an entire domain.
2. Next, navigate to the **Delivery** tab and modify the settings for all three buttons, **Outbound Security…**, **Outbound connections…**, and **Advanced…**.  
    ![image](https://intellitect.com/wp-content/uploads/2010/10/image9.png "Configuring Windows SMTP Server on Windows 2008 for Relay")
3. For **Outbound Security**, switch to **Basic Authentication** and enter the remote SMTP Server credentials in addition to checking **TLS encryption**.  
    ![image](https://intellitect.com/wp-content/uploads/2010/10/image10.png "Configuring Windows SMTP Server on Windows 2008 for Relay")
4. On the **Outbound Connections** tab, switch the TCP port to **587** (or whatever port you need).  
    ![image](https://intellitect.com/wp-content/uploads/2010/10/image11.png "Configuring Windows SMTP Server on Windows 2008 for Relay")
5. Finally, on the **Advanced Delivery** tab, identify the **Smart host** as the DNS (or IP address) of the remote SMTP server.  
    ![image](https://intellitect.com/wp-content/uploads/2010/10/image12.png "Configuring Windows SMTP Server on Windows 2008 for Relay")  
    For the **Fully-qualified domain name** refers to the server you are configuring, the one the SMTP-Service is being configure on.

Unfortunately, I didn’t come up with the command line for this particular action.  I did find two leads as to where there may be a command line solution, however:

1. The first is the smtpsetup.exe program (located in %windir%\\system32\\inetsrv\\).  This program takes an INF file with the configuration information. 
2. Secondly, the configuration information itself is stored in %windir%\\System32\\Inetsvr\\Metabase.xml. 

Even though figuring out the final command line seemed within reach, I had no information as to how the password was encrypted and stored into the file and without much to go on, I decided to move on.

### Command Line

Combining all the command lines together (except for the IIS 6.0 configuration) yields the following:

> <table border="1"><tbody><tr><td><pre>ServerManagerCmd –Install SMTP-Server
> <div></div>
> $SMTPService = Get-WmiObject win32_service -filter "name='SMTPSvc'"</pre><pre>$smtpservice.ChangeStartMode("Automatic")</pre><pre>Set-Alias appcmd "$env:windir\System32\inetsrv\appcmd.exe"</pre><pre>appcmd.exe set config /commit:WEBROOT /section:smtp /from:Inigo.S.Montoya@IntelliTechture.com <br>/deliveryMethod:Network /network.port:587 /network.defaultCredentials:False <br>/network.host:smtp.intelliTechture.com /network.userName:Inigo.S.Montoya@intelliTechture.com <br>/network.password:***</pre></td></tr></tbody></table>

Note: If you are forwarding to gmail’s SMTP server, here are the settings you need:

> SMTP Server: smtp.gmail.com
> 
> SMTP Port: **587**
> 
> TLS encryption: **Checked**

### Want More?

Curious about what else you can do with Windows? Check out _[AAD: How to Clone/Copy a Local Windows 10 Account to an Azure Active Directory Account](https://intellitect.com/clone-aad-windows-10/)_!

![](https://intellitect.com/wp-content/uploads/2021/04/blog-job-ad-2-1024x129.png)
