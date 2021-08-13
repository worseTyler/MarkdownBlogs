## Class and New Constraints With Genric Methods
#
Consider a ``` CommandLineHandler ``` class that parses a command line string and populates an arbitrary object with the command line data.  Each property on the arbitrary object would correspond to a switch on the command line and, using reflection, the idea would be to initialize the data with the data on the command line.  For example, "``` Program.exe /help ```" would set a ``` Boolean ``` ``` Help ``` property to true and "``` Compress /Files:file1.bin file2.bin ```" would populate a ``` string[] Files ``` property with the two specified files.  (Attributes could be used for alias switches like "/?.")

On the ``` CommandLineHandler ``` we have a ``` TryParse() ``` method that takes the command line arguments and either the type of the object or an instance of the object.

```csharp
public class CommandLineHandler {
  public static bool TryParse(string[] args, object commandLine, out string errorMessage) {
    // Assign properties of commandLine 
    // with data from args  
  }
  public static object Parse(string[] args, Type commandLineType, out string errorMessage) {
    // Instantiate type, 
    // Assign properties from args 
    // return
  }
}
```

Both these signatures, however, are not restrictive enough.  The first signature takes the ``` commandLine ``` object but allows it to be a value type argument, for which ``` TryParse() ``` will be unable to set the properties (even though it gets boxed).  The second method would work for value types, but there is no restriction ensuring that the ``` commandLineType ``` has a default constructor.

These two methods serve as great examples of the need for generic methods with constraints.  On the first method we need a class constraint and on the second method a new constraint.

```csharp
public class CommandLineHandler {
  public static bool TryParse(string[] args, TCommandLine commandLine, out string errorMessage) where TCommandLine: class {
    // Assign properties of commandLine 
    // with data from args  
  }
  public static object TCommandLine Parse(string[] args, out string errorMessage) where TCommandLine: new() {
    // Instantiate type, 
    // Assign properties from args 
    // return  
  }
}
```

The class constraint ensures at compile time that the ``` TCommandLine ``` type is a reference type while the new constraint makes sure that the ``` TCommandLine ``` implements a default constructor.

(On ``` TryParse() ``` we could pass by reference (``` ref ```) but since we are not actually changing the reference for reference types this would be misleading.)
