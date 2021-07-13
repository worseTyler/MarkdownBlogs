

While writing another MSDN magazine article on C# 6.0 (the third because the language continues to improve) I was looking for a good example of when to use the new “using static” feature.  Towards this effort, I used PowerShell to crawl through all the MSCORELIB types and output the ones meeting the following criteria:

- The type is public
- The type is static (at the CIL level this is represented by sealed and abstract)\[contextly\_sidebar id="8VJ5zi2gFvSUQ8nHtUZgSxLGP7XWsaBS"\]
- There exists members on the type that are not extension members

The resulting list of “common” types is relatively small (given the total number of types is over 3,000).

- System.Tuple
- System.BitConverter
- System.Buffer
- System.Console
- System.Convert
- System.Environment
- System.GC
- System.GC
- System.Math
- System.Threading.Interlocked

\[contextly\_sidebar id="aKXlk6ctDCvcJnq2VnmX1HSYxVbCV7XA"\]

- System.Threading.Tasks.Parallel
- System.Nullable
- System.Diagnostics.Contracts.Contract
- Microsoft.Win32.Registry
- System.Runtime.InteropServices.Marshal
- System.IO.Directory
- System.IO.File
- System.IO.Path
- System.Runtime.CompilerServices.RuntimeHelpers
- System.Security.SecurityManager

That’s 20 types out of 3,000 and some might argue that many of these are not so common or wouldn’t be used with the feature any way.

In addition, there are some ancillary (what I consider rarely used) types:

> `System.Deployment.Internal.InternalApplicationIdentityHelper, System.Deployment.Internal.InternalActivationContextHelper, System.Threading.Monitor, System.Threading.ThreadPool, System.Threading.Volatile, System.Threading.LazyInitializer, System.Collections.StructuralComparisons, System.Collections.Concurrent.Partitioner, System.Diagnostics.Contracts.Internal.ContractHelper, System.Runtime.CompilerServices.ContractHelper, System.Runtime.Serialization.FormatterServices, System.Globalization.CharUnicodeInfo, System.Security.Policy.ApplicationSecurityManager, System.Runtime.InteropServices.ComEventsHelper, System.Runtime.InteropServices.WindowsRuntime.WindowsRuntimeMarshal, System.Runtime.InteropServices.WindowsRuntime.WindowsRuntimeMetadata, System.Runtime.GCSettings, System.Runtime.ProfileOptimization, System.Runtime.Remoting.RemotingConfiguration, System.Runtime.Remoting.RemotingServices, System.Runtime.Versioning.VersioningHelper, System.Runtime.Versioning.CompatibilitySwitch`

Overall, this seems like a relatively small number rendering the new language feature (at least for MSCORELIB base classes), less useful that I originally expected.

Parenthetically, for those curious about the PowerShell script, the listing below shows what I used:

Function Test-IsStaticType {
    \[CmdletBinding()\]param(
        \[Parameter(Mandatory, ValueFromPipeline)\]\[Type\]$Type,
        \[bool\]$IsPublic = $true  #ToDo: Add pipe and mandetory
    )
    return ($Type.IsSealed -and $Type.IsAbstract) -and ($IsPublic -and $Type.IsPublic) # -and (!$Type.IsValueType) -and $Type.IsPublic 
}

 #ToDo: Convert to support Get-Member output of type Microsoft.PowerShell.Commands.MemberDefinition (possibly in addition to MethodInfo support)
Function Test-IsExtensionMethod {
   \[CmdletBinding()\]param(
        \[Parameter(Mandatory, ValueFromPipeline)\]\[System.Reflection.MethodInfo\]$Method
    )
    return $Method.IsStatic -and ($Method.CustomAttributes.Count -gt 0) -and
        ($Method.CustomAttributes.AttributeType -contains \[System.Runtime.CompilerServices.ExtensionAttribute\] )
}

Function Get-ReflectionExtensionMemebers {
   \[CmdletBinding()\]param(
        \[Parameter(Mandatory, ValueFromPipeline)\]\[Type\]$Type
    )
    
    $Type.GetMethods() | ?{ Test-IsExtensionMethod $\_  }
}

\[reflection.assembly\]::GetAssembly(\[system.console\]).GetTypes() | ?{ Test-IsStaticType $\_ } |  %{ 
    $methods = $\_.GetMethods() | ?{ -not (Test-IsExtensionMethod $\_) } | ?{ $\_.IsStatic }
    if($\_.GetProperties().Length -gt 0) {
        $properties = $\_ | Get-Member -Static -MemberType Property
    }
    if( ($methods -ne $null) -and ($properties -ne $null) ) {
        Write-OUtput $\_.FullName # -ForegroundColor Green
        $methods | select -ExpandProperty name | select -unique | ?{
            ($\_ -notlike "get\_\*") } | ?{ ($\_ -notlike "set\_\*") 
        } | %{
            #Write-Host "\`t$($\_)" -ForegroundColor White
        }
    }
}
