

It’s not uncommon that in the midst of writing a statement, you find you need to declare a variable specifically for that statement.  Consider two examples:

- In the midst of coding an int.TryParse() statement, you realize you need to have a variable declared for the out argument into which the parse results will be stored.
- While writing a for-statement, you discover the need to cache a collection (such as a LINQ query result) to avoid re-executing the query multiple times.  In order to achieve this, you interrupt the thought process of writing the for-statement to declare a variable.

To address these and similar annoyances, C# 6.0 introduces _declaration expressions_.  This means you don’t have to limit variable declarations to statements only, but can use them as well within expressions.  The code in **Listing 1** below provides two examples:

```csharp
public string FormatMessage(string attributeName)
{
    string result;
    if(! Enum.TryParse<FileAttributes>(attributeName, out var attributeValue) )
    {
        result = string.Format("'{0}' is not one of the possible {2} option combinations ({1})",
            attributeName, 
            string.Join(",", string[] fileAtrributeNames = Enum.GetNames(typeof (FileAttributes))),
                fileAtrributeNames.Length);
    }
    else
    {
        result = string.Format("'{0}' has a corresponding value of {1}",
            attributeName, attributeValue);
    }
    return result;
}
```

In the first highlight within the code, the attributeValue variable is declared in-line with the call to Enum.TryParse() rather than in a separate declaration beforehand.  Similarly, the declaration of fileAttributeNames appears on the fly in the call to string.Join().  This enables access to the Length later in the same statement.  (Note that the fileAttributeNames.Length is substitution parameter {2} in the string.Format() call, even though it appears earlier in the format string—thus enabling fileAttributeNames to be declared prior to accessing it).

The scope of a declaration expression is loosely defined as the scope of the statement in which the expression appears.  In **Listing 1**, the scope of attributeValue is that of the if-else statement, making it accessible both in the true and false blocks of the conditional.  Similarly, fileAttributeNames is available only in the first half of the if-statement, the portion matching the scope of the string.Format() statement invocation.

Wherever possible the compiler will enable the use of implicitly typed variables (var) for the declaration, inferring the data type from the initializer (declaration assignment).  However, in the case of out arguments, the signature of the call target can be used to support implicitly typed variables even if there is no initializer.  Still, inference isn’t always possible and, furthermore, it may not be the best choice from a readability perspective.  In the TryParse() case below, for example, var works only because the type argument (FileAttributes) is specified.  Without it, a var declaration would not compile and instead the explicit data type would be required:

Enum.TryParse(attributeName, out FileAttributes attributeValue)

In the second declaration expression example in **Listing 1**, an explicit declaration of string[] appears to identify the data type as an array (rather than a List<string>, for example).  The guideline is standard to the general use of var: Consider avoiding implicitly typed variables when the resulting data type is not obvious.

The declaration expression examples in **Listing 1** could all be coded by simply declaring the variables in a statement prior to their assignment.

See [A C# 6.0 Language Preview](https://msdn.microsoft.com/en-us/magazine/dn683793.aspx) for the full article text.
