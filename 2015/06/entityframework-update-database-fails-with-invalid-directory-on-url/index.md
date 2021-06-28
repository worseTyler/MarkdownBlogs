
While running **Visual Studio 2015** RC (VS2015) you may encounter an error when referencing the **EntityFramework** Nuget package (at least on v6.1.3) stating the following:

Exception calling "CreateInstanceFrom" with "8" argument(s): 
    "Invalid directory on URL."
At ~\\packages\\EntityFramework.6.1.3\\tools\\EntityFramework.psm1:815 char:5
+ $domain.CreateInstanceFrom(
+ ~~~~~~~~~~~~~~~~~~~~~~~~~~~
 + CategoryInfo : NotSpecified: (:) \[\], MethodInvocationException
 + FullyQualifiedErrorId : ArgumentException

The error specifically occurs when running Update-Database  from within the Package Manager or when using Package Manager Console.

The issue arises from within the EntityFramework module file (EntityFramework.psm1).  The specific problem is due to a Get-PackageInstallPath()  invocation from within $packageInstallerServices.GetInstalledPackages().

function Get-PackageInstallPath($package)
{
    $componentModel = Get-VsComponentModel

    $packageInstallerServices = $componentModel.GetService(
        \[NuGet.VisualStudio.IVsPackageInstallerServices\])

    $vsPackage = $packageInstallerServices.GetInstalledPackages() |
        ?{ $\_.Id -eq $package.Id -and $\_.Version -eq $package.Version }

    return $vsPackage.InstallPath
}

The problem is that this line happens to return two installed packages of the same version number and PowerShell's loosly typed nature accepts the duplicates and simply concatenates them into a single string.  To address the issue, change the line to assume the first one by using Select-Object  to select only the first package:

$vsPackage = @($packageInstallerServices.GetInstalledPackages() | 
    ?{ $\_.Id -eq $package.Id -and $\_.Version -eq $package.Version }) | 
    Select-Object -First 1

Given this modification the Update-Database  command will run successfully.  However, you need to remember to check in the packages directory so that others on your team will be using the same modified package rather than simply downloading the latest, without said modification.

No doubt this is a fairly temporary problem but if you do encounter the error this solution could potentially save you considerable spelunking.
