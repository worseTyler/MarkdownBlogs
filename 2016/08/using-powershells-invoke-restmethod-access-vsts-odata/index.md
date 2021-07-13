

Given that Microsoft has abandoned their TFS CmdLets, one alternative is to access VSTS data is through the OData interface using the Invoke-RestMethod.

Before you begin you need to setup Alternate authentication credentials by navigating to https://<yourtenant>.visualstudio.com/\_details/security/altcreds.  Once these are established, you will likely want to save them to a local PowerShell variable so that they can be reused for each Invoke-RestMethod invocation.

```
$vstsCredentials = Get-Credential
```

**IMPORTANT**: When entering your credentials, be sure to prefix it with your tenant name.  For example, if your VSTS URL is <domain>.visualstudio.com and your alternate credential username is "InigoMontoya", you will want to enter a username of <domain>\\InigoMontoya.

Next, you want to invoke the appropriate TFS OData resource URL (see https://tfsodata.visualstudio.com/) corresponding to the type of data you want to access (Projects, WorkItems, Builds, etc).  For example, to access the projects associated with your tenant you would use:

```
$projectsData = Invoke-RestMethod -Uri 'https://tfsodata.visualstudio.com/DefaultCollection/Projects?$format=json' -Credential $vstsCredentials
```

To view the results you would dereference the result as follows:

```
$projectsData.d.results | Select-Object Name,Collection
```

The result will be a list of projects and their corresponding collection URL (which are likely all the same on VSTS).
