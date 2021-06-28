
## Six Steps to Eliminate Password Storage

Managing the server administrator passwords for your Azure SQL Databases can be cumbersome and insecure. To that end, password authentication to a database is difficult to audit and represents a single point of failure in your company's security posture, especially when it comes to production databases.

The allow-list-based firewall that Azure provides for all databases by default provides some level of protection. However, if you have to allow your company's public IP address in order to grant access to DBAs and other users on your network, your attack surface is still wider than it should be if a database password were to be compromised.

Fortunately, Azure SQL Databases support fully-managed authentication using AAD user accounts and service principles. This allows users to connect to your database using their own username, password, and MFA method. Applications and services, including Azure App Services, can connect to the database using managed service principles in Azure.

This article assumes you have an Azure SQL Database, one-or-more .NET Azure App Services, or other Azure resources that support running as a service principal, and zero-or-more individual users who need to connect to your database. For other scenarios or technologies the same general procedure can be followed, but some technology-specific details will vary.

## Step 1: Configure Your Application

_The steps below assume you only have a .NET application that needs to authenticate with your database. If you have other resources using different technologies (aside from users using SQL Server Management Studio) that also need authentication, see [Microsoft's documentation on the subject](https://docs.microsoft.com/en-us/azure/azure-sql/database/authentication-aad-configure?tabs=azure-powershell#using-an-azure-ad-identity-to-connect-from-a-client-application)._

### Entity Framework or SqlClient Access

To begin, update your .NET application, adding or updating a package reference to `Microsoft.Data.SqlClient`, version 2.1 or higher. This is the first version of `Microsoft.Data.SqlClient` to natively support managed AAD authentication. If using Entity Framework Core, you must be using EF Core 5 or higher, as this is the first version of EF Core that supports `Microsoft.Data.SqlClient` 2.0+.  
  
For example, with EF Core, you should see the following references in your .csproj:

<PackageReference Include="Microsoft.Data.SqlClient" Version="2.1.1" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="5.0.1" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="5.0.1" />

It's important to note if you’re using some other means of database access (for example, `System.Data.SqlClient` with either direct SQL queries or Dapper), you must switch to use `Microsoft.Data.SqlClient` because `System.Data.SqlClient` is no longer receiving any new feature updates. For more on `Microsoft.Data.SqlClient`, see its [project site](https://aka.ms/sqlclientproject).

### Other Database Access

If you’re using any other libraries that connect to your database, see if those libraries are using `Microsoft.Data.SqlClient` version 2.0+ already, or if they otherwise have the ability to be configured to do just that. For example, to configure [Hangfire](https://www.hangfire.io/) to use `Microsoft.Data.SqlClient`, configure it as follows in your `Startup.cs`:

services.AddHangfire(config => config    
    .UseSqlServerStorage(() => new Microsoft.Data.SqlClient.SqlConnection(        
        Configuration.GetConnectionString("DefaultConnection")    
)));

## Step 2: Give Your Azure App Service an Identity

Open your App Service in the Azure Portal. Then, navigate to the “Identity” tab in the sidebar. Under the “System assigned” tab, flip the Status to “On” and save. Click Yes to any confirmation prompt that appears.

![Flipping System assigned tab to On in App Service in Azure Portal](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2021/02/passwordless-authentication-azure-sql/images/image.png)

Now, your App Service is running with access to the credentials of an AAD Service Principal. This Service Principal has the same name as the app service and can be assigned to any access group that you need.

## Step 3: Create an AAD Group to Control Database Server Admins

Next, navigate to your organization’s Azure Active Directory (AAD) resource in the Azure Portal. Open the “Groups” page under the “Manage” heading in the sidebar.

![Opening Groups in AAD in Azure Portal](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2021/02/passwordless-authentication-azure-sql/images/image-1.png)

Navigating to the Group administration area in the Azure Portal

On the Groups page, click “New group” to create a new group. Use this group to manage the users and services with admin-level access to your database. If you already have an appropriate group for this, skip ahead.

_Note that creating a new group in AAD requires specific permissions. If you do not have such permissions, ask an administrator to make the group for you and to assign you as an owner of that group. As an owner, you will then be able to manage membership._

![Naming group in AAD](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2021/02/passwordless-authentication-azure-sql/images/image-2.png)

Adding a new group to an AAD tenant

Next, create an appropriate name and description for the group. Assign group owners as desired. Group owners can be users or service principals and are able to manage the group including membership. Group owners aren't required to be members of the group ([Microsoft Docs](https://docs.microsoft.com/en-us/azure/active-directory/fundamentals/active-directory-accessmanagement-managing-group-owners)).

Following, under "Members" click the hyperlink. Then, search for and select the system-assigned identity created for your App Service in Step 2. This identity has the same name as your App Service resource – in this example, “`my-app-service-group-db-auth`”.

![Adding members to the group in AAD](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2021/02/passwordless-authentication-azure-sql/images/image-4.png)

Adding your app service's identity to the new AAD group.

#### Server-level Admins

Additionally, you can search for and add any other users you want to have server-level admin access to your database. These users will be able to log into the database using SQL Server Management Studio (SSMS) by selecting the “Azure Active Directory – Universal with MFA” authentication method. Ensure that all users with access to your databases – production or not – have multi-factor authentication enabled and enforced by your organization.

![SQL Server multi-factor authentication in a Database Engine](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2021/02/passwordless-authentication-azure-sql/images/image-5.png)

Connecting to the database using your own user account in SSMS

  
After selecting users, click “Create” on the new group page.

## Step 4: Assign the AAD Group as the Database Server Administrator

Navigate to your SQL Server resource in Azure (this is the server that contains your database, not the database resource itself). Open the “Active Directory admin” section. Then, set the admin to your AAD group we created earlier that already includes the App Service System-assigned identity.

![Assigning an AAD admin to an Azure SQL Server](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2021/02/passwordless-authentication-azure-sql/images/image-6.png)

Assigning an AAD admin to an Azure SQL Server

This will make all users and services that are members of this group into server-level administrators of the database server.

If you wish to add other users to your databases with less access than this, see [this Microsoft documentation page](https://docs.microsoft.com/en-us/azure/azure-sql/database/logins-create-manage) for more information on how Azure SQL Database security roles work, and [this page](https://docs.microsoft.com/en-us/azure/azure-sql/database/authentication-aad-configure?tabs=azure-powershell#create-contained-users-mapped-to-azure-ad-identities) for details on how to add additional AAD users or groups with specific database permissions.

Note that in order to allow _any_ AAD authentication to your database, there _must_ be an AAD admin configured for the database server. Removing the admin from the server will disable all other AAD authentication to that server.

## Step 5: Configure the App Service to Connect with the System-Assigned Identity

First, **make sure that you've deployed any code changes** that you may have made in Step 1. If you make this configuration change when your app isn't capable of handling it, it will crash!

For best results, perform this process in a non-production environment to validate that everything is configured properly before replicating this configuration in production.

Find the database connection string that your App Service uses to connect to your Azure SQL Database. Often, this is in the Configuration section of your App Service, but you may be managing it somewhere else (like an Azure Key Vault). Replace the “`User ID=*******;Password=******;`” section of your connection string with “`Authentication=Active Directory Managed Identity;”`. This will instruct `Microsoft.Data.SqlClient` to automatically obtain an OAuth2 token using from the App Service and send that token to the Azure SQL Database when connecting.

_If you're curious how this works under the hood, see [this pull request](https://github.com/dotnet/SqlClient/pull/730) in Microsoft.Data.SqlClient that added this feature, or [this specific file](https://github.com/dotnet/SqlClient/blob/9e3f0d625eb5ec15e069a1d8f118d3bac41b61f5/src/Microsoft.Data.SqlClient/src/Microsoft/Data/SqlClient/AzureManagedIdentityAuthenticationProvider.cs#L43) which performs token acquisition._

## Step 6: Success!

Your app and users should now be able to authenticate with your database as server admins without having to use a shared password.

Also, if you don't have any users or services that rely on the admin password, you can reset the admin password of your Azure SQL Server to a random value that you will not write down or store. If you ever need to get back into your database server using a password, an administrator of your database resource in Azure can always reset the password again to a known value.

#### Using managed authentication techniques helps keep your organization more secure:

1. The risk of a compromised master password can be eliminated
2. Individual users can be required to use MFA to access your databases
3. The set of users with databases access can be audited
4. Actual logins being performed against the database can be audited using [the auditing features of Azure SQL Databases](https://docs.microsoft.com/en-us/azure/azure-sql/database/auditing-overview#setup-auditing)

## Want more?

Check out [our other blogs](https://intellitect.com/blog/) and feel free to leave any questions or comments below!
