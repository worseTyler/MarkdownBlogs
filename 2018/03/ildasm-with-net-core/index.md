

## Need to Convert CIL Code into Text? Try ILDasm!

The C# compiler converts C# code to common intermediate language (CIL) code and not to machine code. The processor can directly understand machine code, but CIL code needs to be converted before the processor can execute it.

Consider using the Intermediate Language Disassembler (ILDasm) when you need to convert CIL code into text. ILDasm is a CIL disassembler provided in C# that is used to view the assembly content of all classes and methods available in .NET application. It is available in Visual Studio and .NET SDK.

The following six steps will help you master this tool by providing a sample of how to use ildasm with a .NET Core project.

1. From the command prompt, change directories into an empty folder called "HelloWorld"  
    mkdir HelloWorld; cd HelloWorld
2. Create a .NET Core console application  
    dotnet new console
3. Add a package reference to the project for dotnet-ildasm (the latest version)  
    dotnet add package dotnet-ildasm -v '\*'
4. Replace the PackageReference node with the DotNetCliToolReference.  
    (type .\\HelloWorld.csproj) -replace '<PackageReference Include="dotnet-ildasm"','<DotNetCliToolReference Include="dotnet-ildasm"' | Set-Content -Path .\\HelloWorld.csproj
5. Run dotnet restore in order to download the package (only required for .NET Core versions prior to 2.0)  
    dotnet restore
6. At this point, dotnet should have a new verb, ildasm, that you can use to extract the CIL from your assembly as text
    
    dotnet ildasm .\\bin\\Debug\\netcoreapp2.0\\HelloWorld.dll -t
    

## Want the full output?

Here's the output from running the above commands.

```
.assembly extern System.Runtime

{

.publickeytoken = ( B0 3F 5F 7F 11 D5 0A 3A )

.ver 4:2:0:0

}

.assembly extern System.Console

{

.publickeytoken = ( B0 3F 5F 7F 11 D5 0A 3A )

.ver 4:1:0:0

}

.assembly 'HelloWorld'

{

.custom instance void \[System.Runtime\]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 )

.custom instance void \[System.Runtime\]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78 63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )

.custom instance void \[System.Runtime\]System.Runtime.Versioning.TargetFrameworkAttribute::.ctor(string) = ( 01 00 18 2E 4E 45 54 43 6F 72 65 41 70 70 2C 56 65 72 73 69 6F 6E 3D 76 32 2E 30 01 00 54 0E 14 46 72 61 6D 65 77 6F 72 6B 44 69 73 70 6C 61 79 4E 61 6D 65 00 )

.custom instance void \[System.Runtime\]System.Reflection.AssemblyCompanyAttribute::.ctor(string) = ( 01 00 0A 48 65 6C 6C 6F 57 6F 72 6C 64 00 00 )

.custom instance void \[System.Runtime\]System.Reflection.AssemblyConfigurationAttribute::.ctor(string) = ( 01 00 05 44 65 62 75 67 00 00 )

.custom instance void \[System.Runtime\]System.Reflection.AssemblyDescriptionAttribute::.ctor(string) = ( 01 00 13 50 61 63 6B 61 67 65 20 44 65 73 63 72 69 70 74 69 6F 6E 00 00 )

.custom instance void \[System.Runtime\]System.Reflection.AssemblyFileVersionAttribute::.ctor(string) = ( 01 00 07 31 2E 30 2E 30 2E 30 00 00 )

.custom instance void \[System.Runtime\]System.Reflection.AssemblyInformationalVersionAttribute::.ctor(string) = ( 01 00 05 31 2E 30 2E 30 00 00 )

.custom instance void \[System.Runtime\]System.Reflection.AssemblyProductAttribute::.ctor(string) = ( 01 00 0A 48 65 6C 6C 6F 57 6F 72 6C 64 00 00 )

.custom instance void \[System.Runtime\]System.Reflection.AssemblyTitleAttribute::.ctor(string) = ( 01 00 0A 48 65 6C 6C 6F 57 6F 72 6C 64 00 00 )

.hash algorithm 0x00008004

.ver 1:0:0:0

}

.module 'HelloWorld.dll'

// MVID: {b715e71d-d629-4da3-bb80-91ca22b56227}

.imagebase 0x00400000

.file alignment 0x00000200

.stackreserve 0x00100000

.subsystem 0x0003  // WindowsCui

.corflags 0x00000001  // ILOnly

.class private auto ansi beforefieldinit HelloWorld extends \[System.Runtime\]System.Object

{

.method private hidebysig static void Main() cil managed

{

.entrypoint

// Code size 13

.maxstack 8

IL\_0000: nop

IL\_0001: ldstr "Hello. My name is Inigo Montoya."

IL\_0006: call void \[System.Console\]System.Console::WriteLine(string)

IL\_000b: nop

IL\_000c: ret

} // End of method System.Void HelloWorld::Main()

.method public hidebysig specialname rtspecialname instance void .ctor() cil managed

{

// Code size 8

.maxstack 8

IL\_0000: ldarg.0

IL\_0001: call instance void \[System.Runtime\]System.Object::.ctor()

IL\_0006: nop

IL\_0007: ret

} // End of method System.Void HelloWorld::.ctor()

} // End of class HelloWorld
```

Questions? Post them in the comments.
