## Efficient Movement Through Branches With PowerShell Dynamic Parameters 
#
![Tree branches](https://intellitect.com/wp-content/uploads/2016/10/tree-439171_640-300x225.jpg "Moving Quickly Among Branches With PowerShell Dynamic Parameters")

Branches, anyone?

I am currently working on a large integration project that uses a PowerShell script within each sub-module to manage building, deploying and even launching Visual Studio. Due to a reliance on code namespaces matching with folder structures, these PowerShell scripts are sprinkled all over a large directory structure. Fortunately, they are well-named and exist in a predictable sub folder. Working from the PowerShell CLI, how does one efficiently move between folders to execute this script?

Enter PowerShell Dynamic Parameters and Void Tools' ["Everything"](https://www.voidtools.com/) search tools.

## Here is my recipe:

First, install Everything ([using chocolatey](https://chocolatey.org/packages/Everything)).

```powershell
PS> choco install everything
```

Second, configure it to run as a service (so you don't get a UAC popup every time you log on).

![Run As A Service](https://intellitect.com/wp-content/uploads/2016/10/2016-10-17-17_09_44-h1860-biztalk-dev-H1860-Remote-Desktop-Connection-300x292.png "Moving Quickly Among Branches With PowerShell Dynamic Parameters")

Run As A Service

Also configure it to expose its web interface on a local port.

![](https://intellitect.com/wp-content/uploads/2016/10/2016-10-17-17_09_20-h1860-biztalk-dev-H1860-Remote-Desktop-Connection-300x292.png)

Activate HTTP server

## Now for some PowerShell magic!

We will be querying Everything with the [Invoke-RestMethod Cmdlet](https://technet.microsoft.com/en-us/library/hh849971.aspx). We need to create a query that Everything can execute, and that will return the location of each script we care about. In my case, I wanted to find every instance of "Framework.ps1" that was a child of a folder called "Build" under a certain root path. On the Everything local web page, I experimented until I came up with criteria (documented [here](https://www.voidtools.com/support/everything/searching/)) that return the correct results: "path:D\\src\\Branch1 folder:Build child:Framework.ps1". Once completed, we need to encode it for use in an HTTP query string, and then invoke the RESTful service that Everything exposes. This API is documented [here](https://www.voidtools.com/support/everything/http/). Here is what I created (the $root argument is the root folder to start searching in):

```powershell
function Get-AllBuildFolders([string] $root) {

  $search = [uri]::EscapeDataString("path:$($root) folder:Build child:Framework.ps1")
  $uri = "https://localhost:8081/?s=$($search)&j=1&c=255&path_column=1"

  $result = Invoke-RestMethod -Uri $uri
  return $result.results | % { Join-Path -Path $_.path -ChildPath $_.name }
}
```

Since PowerShell handles deserializing JSON results with aplomb, I use a simple pipeline to return a list of strings, that are the fully-qualified paths to the folders where the scripts reside. Note: you may need to change the port on the URL to match where you are running the Everything web service.

Now that we have a simple way to get a list of the locations where we want to go, let's make a method to get there, but with some dynamic parameters to give us tab completion and a handy UI for choosing the value we want. First we need a static ValidateSet that will hold a list of the branches, which is the first argument, and will be used to filter the results from Everything:

```powershell
function Get-Workspace {
  [CmdletBinding()]
  param(
    [Parameter(Mandatory=$true, ValueFromPipeline=$true, Position=0)]
    [ValidateSet("D:\\Src\\Main", "D:\\Src\\Branch1", "D:\\Src\\Branch2")]
    [string]
    $branch
)
    
```

Next, we take advantage of the DynamicParam block to query the webservice and build up a new ValidateSet based on the results. We need to initialize a new [ParameterAttribute](https://msdn.microsoft.com/en-us/library/system.management.automation.parameterattribute(v=vs.85).aspx) manually with our ParameterSetName and other defaults:

```powershell
  DynamicParam {
    $ParameterName = 'workspace'

    $attribute = new-object System.Management.Automation.ParameterAttribute
    $attribute.ParameterSetName = "__AllParameterSets"
    $attribute.Mandatory = $true
    $attribute.Position = 1
```

Then we need to create a new collection of [Attributes](https://msdn.microsoft.com/en-us/library/system.attribute(v=vs.110).aspx) and add our attribute from above.

```powershell
    $attributeCollection = new-object -Type System.Collections.ObjectModel.Collection[System.Attribute]
    $attributeCollection.Add($attribute)
```

We then call our service above, put the results in a [ValidateSetAttribute](https://msdn.microsoft.com/en-us/library/system.management.automation.validatesetattribute(v=vs.85).aspx) and add it to the collection.

```powershell
    $values = Get-AllBuildFolders($branch)

    $ValidateSet = new-object System.Management.Automation.ValidateSetAttribute($values)
    $attributeCollection.Add($ValidateSet)
```

Finally, we construct a [RuntimeDefinedParameter](https://msdn.microsoft.com/en-us/library/system.management.automation.runtimedefinedparameter(v=vs.85).aspx) that contains our AttributeCollection and a [RuntimeDefinedParameterDictionary](https://msdn.microsoft.com/en-us/library/system.management.automation.runtimedefinedparameterdictionary(v=vs.85).aspx) to hold it. This is the object that DynamicParams block expects us to return.

```powershell
    $dynamicParameter = new-object -Type System.Management.Automation.RuntimeDefinedParameter($ParameterName, [string], $attributeCollection)
    $paramDictionary = new-object -Type System.Management.Automation.RuntimeDefinedParameterDictionary
    $paramDictionary.Add($ParameterName, $dynamicParameter)

    return $paramDictionary
```

In the Begin block of our function, we pull the value out of the $PSBoundParameters ambient variable, and in the Process block, change directory to that location. For brevity, I added an alias to a shorter command in my $Profile. Here is the [complete listing](https://gist.github.com/adamskt/21771391845cdd79397fc71ec6f54fd4).

## Next Steps

Bonus points would be awarded for interfacing with TFS (or your version control system of choice) to get the root folders of your workspace branches dynamically, instead of having a hard-coded ValidateSet. Once you have a basic understanding of the intricate gears in the PowerShell parameter system, you can't help but arrive at the conclusion that PowerShell is awesome.
