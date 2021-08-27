

## Creating Fody Add-ins
#
[Fody](https://github.com/Fody/Fody) is a fantastic framework for creating IL weavers. For those who are unfamiliar with IL weaving, it is the art (and at times a seemingly magical incantation) of modifying an assembly _post-compile_. In this article we will walk through creating a library that can modify an existing .NET assembly.

Let’s start by looking at what actually happens when you compile C#. The compiler ([Roselyn](https://github.com/dotnet/roslyn)) performs a bunch of validation to ensure that your C# code is valid. It then generates an assembly of IL instructions. These IL instructions can are either [just-in-time (JIT) compiled](https://msdn.microsoft.com/en-us/library/ht8ecch6(v=vs.100).aspx) when the application is run or [compiled to native images](https://msdn.microsoft.com/en-us/library/ht8ecch6(v=vs.100).aspx) (using Ngen.exe).  It is these instructions that we will modify with our IL weaver.

Because the weaver is acting on the generated IL and not the original source code, it does not know (or care) about C# or its syntactic rules. IL weaving allows for creating assemblies [that violate C#](https://stackoverflow.com/a/8086788) rules. It also means that since you don’t have the compiler validating your code, it is very easy to create invalid assemblies that fail at runtime.

Even with the Roselyn becoming open source, Microsoft has still [not provided compiler hooks](https://stackoverflow.com/a/7839398/3755169) to allow for easy code injection/modification during the compilation process. This makes many [aspect-oriented programming (AOP)](https://en.wikipedia.org/wiki/Aspect-oriented_programming) style solutions, difficult if not completely impossible. This is where [Fody](https://github.com/Fody/Fody) attempts to fill the void. It is built on top of [Mono.Cecil](https://www.mono-project.com/docs/tools+libraries/libraries/Mono.Cecil/), a library for reading and writing managed IL assemblies. Fody also takes care of integrating with MSBuild and Visual Studio to make it easy to run your IL weavers as a post-compile build step. There are already several weavers that have been created for many common scenarios, you can find many of them in the [Fody repo list](https://github.com/Fody) or on [NuGet](https://www.nuget.org/packages?q=Fody).

Before attempting to write an IL weaver, it is important to have a solid grasp on [Common Intermediate Language](https://en.wikipedia.org/wiki/Common_Intermediate_Language) (CIL formerly Microsoft Intermediate Language or MSIL). One of the simplest ways to learn is to simply write some C# and look at the compiled IL to see what the C# compiler generated for you. Some tools that great for this are [Ildasm](https://msdn.microsoft.com/en-us/library/f7dy01k1(v=vs.110).aspx), [dotPeek](https://www.jetbrains.com/decompiler/), [ReSharper](https://www.jetbrains.com/help/resharper/2016.1/Viewing_Intermediate_Language.html), and [ILSpy](https://ilspy.net/). If you have access to ReSharper its IL viewer provides a nice link back to your original source code making it very easy to compare your original C# to the generated IL. If you are looking for a free solution, ILSpy is my favorite. It provides nice syntax highlighting, and hyperlinks the IL instructions to their documentation making it very helpful while learning.

This is where we will begin our adventure.

## Your first weaver

For our first weaver we will create one that does a [Debug.WriteLine](https://msdn.microsoft.com/en-us/library/system.diagnostics.debug.writeline(v=vs.110).aspx) at the beginning of all of our methods showing the parameters that were passed.

Start by creating a new class library and add the [Fody NuGet package](https://www.nuget.org/packages/Fody/). The name of this assembly **_must_** end in “.Fody”.  When Fody loads add-ins, it only loads assemblies that follow this naming scheme. The Fody NuGet package makes several changes that we need to undo. When your weaver is deployed as a NuGet, these changes are very helpful, but need to be removed so we can create a weaver.

Unload the project and edit the csproj file. Delete the Import that includes the Fody.targets file along with the Error check that ensures the file exists.

```powershell
<Import Project="..\\packages\\Fody.1.29.4\\build\\dotnet\\Fody.targets" Condition="Exists('..\\packages\\Fody.1.29.4\\build\\dotnet\\Fody.targets')" />

<Error Condition="!Exists('..\\packages\\Fody.1.29.4\\build\\dotnet\\Fody.targets')" Text="$([System.String]::Format('$(ErrorText)', '..\\packages\\Fody.1.29.4\\build\\dotnet\\Fody.targets'))" />

```

Fody loads its add-ins from the packages directory by looking for packages that have “.Fody” in the name. To facilitate testing, the assembly needs to be copied to the packages directory. Uncomment the AfterBuild target and add the following (adjust the package name to match your project):

```powershell
<Target Name="AfterBuild">

  <Copy SourceFiles="$(TargetPath)" DestinationFolder="$(SolutionDir)\\packages\\Example.Fody.1.0.0" />

</Target>

```

Reload the project, and delete the FodyWeavers.xml file as well. Finally, add references to the Mono.Cecil.\*.dlls that are located in the packages/Fody\* directory. Also add a reference to Microsoft.Build.Framework.

Create a new ModuleWeaver.cs file. This class will be instantiated and invoked by Fody. There are many properties and methods that will be populated if they are available. To make this process easier, simply copy in the example empty weaver from the Fody library. If you choose not to copy in the example, make sure that your ModuleWeaver class is in the [global namespace](https://msdn.microsoft.com/en-us/library/dfb3cx8s.aspx). This is required for Fody to find it. Everything in the weaver is entirely convention based. Many of the items in the example weaver are optional (see comments) and can be removed if you do not need them.

This forms the basic weaver project, it should look like this:

![image01](https://intellitect.com/wp-content/uploads/2016/09/image01-232x300.png "Creating a Fody Add-in")

We will start by determining exactly what we want our method to look like when we are done.

Consider the following method:

```csharp
private static int Add( int a, int b )
{
    return a + b;
}
```

Ideally we want the resulting method to look like:

```csharp
private static int Add( int a, int b )
{
    System.Diagnostics.Debug.WriteLine(string.Format("DEBUG: Add({0})", new object[] {a, b}));
    return a + b;
}
﻿
```

Examining IL for this method using ILSpy:

```csharp
.method private hidebysig static
int32 Add (
int32 a,
int32 b
) cil managed
{
// Method begins at RVA 0x2270
// Code size 49 (0x31)
.maxstack 5
.locals init (
[0] int32
)

IL_0000: nop
IL_0001: ldstr "DEBUG: Add({0})"
IL_0006: ldc.i4.2
IL_0007: newarr [mscorlib]System.Object
IL_000c: dup
IL_000d: ldc.i4.0
IL_000e: ldarg.0
IL_000f: box [mscorlib]System.Int32
IL_0014: stelem.ref
IL_0015: dup
IL_0016: ldc.i4.1
IL_0017: ldarg.1
IL_0018: box [mscorlib]System.Int32
IL_001d: stelem.ref
IL_001e: call string [mscorlib]System.String::Format(string, object[])
IL_0023: call void [System]System.Diagnostics.Debug::WriteLine(string)
IL_0028: nop
IL_0029: ldarg.0
IL_002a: ldarg.1
IL_002b: add
IL_002c: stloc.0
IL_002d: br.s IL_002f

IL_002f: ldloc.0
IL_0030: ret
}

```

The IL of the original method without the Debug.WriteLine call consisted of IL instructions IL_0028 through IL_0030. So the instructions that we will inject with our weaver are those from IL_0000 through IL_0023.

We will need to invoke three methods to make this work. To do this we will need to get MethodInfo objects for each of those methods. To avoid looking them up multiple times, we will create static references to each of them.

```csharp
private static readonly MethodInfo _stringJoinMethod;
private static readonly MethodInfo _stringFormatMethod;
private static readonly MethodInfo _debugWriteLineMethod;

static ModuleWeaver()
{
    //Find string.Join(string, object[]) method
    _stringJoinMethod = typeof( string )
        .GetMethods()
        .Where( x => x.Name == nameof( string.Join ) )
        .Single( x =>
            {
                var parameters = x.GetParameters();
                return parameters.Length == 2 &&
                       parameters[0].ParameterType == typeof( string ) &&
                       parameters[1].ParameterType == typeof( object[] );
            } );

    //Find string.Format(string, object) method
    _stringFormatMethod = typeof( string )
        .GetMethods()
        .Where( x => x.Name == nameof( string.Format ) )
        .Single( x =>
            {
                var parameters = x.GetParameters();
                return parameters.Length == 2 &&
                       parameters[0].ParameterType == typeof( string ) &&
                       parameters[1].ParameterType == typeof( object );
            } );

    //Find Debug.WriteLine(string) method
    _debugWriteLineMethod = typeof( System.Diagnostics.Debug )
        .GetMethods()
        .Where( x => x.Name == nameof( System.Diagnostics.Debug.WriteLine ) )
        .Single( x =>
            {
                var parameters = x.GetParameters();
                return parameters.Length == 1 &&
                       parameters[0].ParameterType == typeof( string );
            } );
}
```

We will now turn our attention to the ModuleWeaver.Execute method. When invoked, Fody will have set the ModuleDefinition property to be our assembly. Any changes that you make to the ModuleDefinition will be automatically saved at the end of the Execute method.

We want to inject code into all of the methods within our assembly, so we will start off our Execute method by simply iterating over all of the methods.

```csharp
public void Execute()
{
    foreach ( TypeDefinition type in ModuleDefinition.Types )
    {
        foreach ( MethodDefinition method in type.Methods )
        {
            ProcessMethod( method );
        }
    }
}

```

The MethodDefinition.Body property contains the collection of IL instructions. You can manipulate the instructions in this collection directly, or you can take advantage of the simple ILProcessor class provided by Mono.Cecil.

```csharp
private void ProcessMethod( MethodDefinition method )
{
    ILProcessor processor = method.Body.GetILProcessor();
    Instruction current = method.Body.Instructions.First();

    //Create Nop instruction to use as a starting point
    //for the rest of our instructions

    Instruction first = Instruction.Create( OpCodes.Nop );
    processor.InsertBefore( current, first );
    current = first;

    //Insert all instructions for debug output after Nop
    foreach ( Instruction instruction in GetInstructions( method ) )
    {
        processor.InsertAfter( current, instruction );
        current = instruction;
    }
}
```

For each method, we insert a [Nop](https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.nop(v=vs.110).aspx) instruction as an anchor point and then insert all of our instructions following the Nop instruction.

The GetInstructions method is where we build up the enumerable of IL instructions that we determined we needed from above.

```csharp
private IEnumerable<Instruction> GetInstructions( MethodDefinition method )
{
    yield return Instruction.Create( OpCodes.Ldstr, $"DEBUG: {method.Name}({{0}})" );
    yield return Instruction.Create( OpCodes.Ldstr, "," );

    yield return Instruction.Create( OpCodes.Ldc_I4, method.Parameters.Count );
    yield return Instruction.Create( OpCodes.Newarr, ModuleDefinition.ImportReference( typeof( object ) ) );

    for ( int i = 0; i < method.Parameters.Count; i++ )
    {
        yield return Instruction.Create( OpCodes.Dup );
        yield return Instruction.Create( OpCodes.Ldc_I4, i );
        yield return Instruction.Create( OpCodes.Ldarg, method.Parameters[i] );
        if ( method.Parameters[i].ParameterType.IsValueType )
            yield return Instruction.Create( OpCodes.Box, method.Parameters[i].ParameterType );
        yield return Instruction.Create( OpCodes.Stelem_Ref );
    }

    yield return Instruction.Create( OpCodes.Call, ModuleDefinition.ImportReference( _stringJoinMethod ) );
    yield return Instruction.Create( OpCodes.Call, ModuleDefinition.ImportReference( _stringFormatMethod ) );
    yield return Instruction.Create( OpCodes.Call, ModuleDefinition.ImportReference( _debugWriteLineMethod ) );
}
```

There are a few key details to point out.

First all of the calls to ModuleDefinition.ImportReference. Any reference that you use **_must_** be [imported](https://github.com/jbevain/cecil/wiki/Importing). This applies to types, as well as methods. If the reference has already been imported, no harm done.

The check for IsValueType is important, since we will have methods with parameters that are both value and reference types. Because we are putting the values into an object array, value types need to be [boxed](https://msdn.microsoft.com/en-us/library/yz2be5wk.aspx), while reference types do not.

That is everything, we now have a weaver that will now produce Debug output at the beginning of each of our methods.

## Using the weaver

To give our weaver a test drive, we need another assembly for it to process. To keep things simple, we will add a new console application, with only a single method call:

```csharp
using static System.Console;
namespace AssemblyToProcess
{
    public static class Program
    {
        static void Main( string[] args )
        {
            WriteLine( Add( 2, 4 ) );
            ReadLine();
        }

        private static int Add( int a, int b )
        {
            return a + b;
        }
    }
}
```

```
Console Output:  
6  
Debug Output:  
DEBUG: Main(System.String[])  
DEBUG: Add(2,4)
```

## Final thoughts

- Keep in mind the separation between C# and CIL. When writing weavers keep in mind that you are acting on the post-compiled CIL. This CIL will also be different between Debug and Release builds.
- Typically calls to Debug.Writeline are stripped out of Release builds. Our weaver is adding those calls in **_after_** the C# compiler would have stripped them out. We could fix our weaver by checking for the DEBUG constant in the ModuleWeaver. DefineConstants property.
- The initial nop that was inserted at the beginning of the method could also be optimized and removed in the case of Release builds. When compiling an assembly in Debug mode, a nop is inserted at the beginning of every method. These provide anchor points for breakpoints. This is why you can hit a breakpoint on the opening brace of a method in Debug mode, but not in Release mode.

You can find the complete solution [here](https://github.com/Keboo/ExampleFodyWeaver).

Additional references: [https://1drv.ms/f/s!At6Id87483cAmZdY9tev9w3g3gpKzg](https://1drv.ms/f/s!At6Id87483cAmZdY9tev9w3g3gpKzg)
