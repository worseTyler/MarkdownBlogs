## Inaccuracies of Floats 

I have been playing around with the inaccuracies of floats and decided to share some of the simplest comparisons that might surprise folks that use the equality comparisons of floats indiscriminately.

The following code listing pretty much captures the issues:
```csharp
using System.Diagnostics;
... 
decimal decimalNumber = 4.2M; 
double doubleNumber1 = 0.1F \* 42F; 
double doubleNumber2 = 0.1D \* 42D; 
float floatNumber = 0.1F \* 42F;
Trace.Assert(decimalNumber != (decimal)doubleNumber1); 
// Displays: 4.2 != 4.20000006258488 
System.Console.WriteLine("{0} != {1}", decimalNumber, (decimal)doubleNumber1);
Trace.Assert((double)decimalNumber != doubleNumber1); 
// Displays: 4.2 != 4.20000006258488 
System.Console.WriteLine("{0} != {1}", (double)decimalNumber, doubleNumber1);
Trace.Assert((float)decimalNumber != floatNumber); 
// Displays: 4.2 != 4.2 
System.Console.WriteLine("{0} != {1}", (float)decimalNumber, floatNumber);
Trace.Assert(doubleNumber1 != (double)floatNumber); 
// Displays: 4.20000006258488 != 4.20000028610229 
System.Console.WriteLine("{0} != {1}", doubleNumber1, (double)floatNumber);
Trace.Assert(doubleNumber1 != doubleNumber2); 
// Displays: 4.20000006258488 != 4.2 
System.Console.WriteLine("{0} != {1}", doubleNumber1, doubleNumber2);
Trace.Assert(floatNumber != doubleNumber2); 
// Displays: 4.2 != 4.2 
System.Console.WriteLine("{0} != {1}", floatNumber, doubleNumber2);
Trace.Assert((double)4.2F != 4.2D); 
// Display: 4.19999980926514 != 4.2 
System.Console.WriteLine("{0} != {1}", (double)4.2F, 4.2D);
Trace.Assert(4.2F != 4.2D); 
// Display: 4.2 != 4.2 
System.Console.WriteLine("{0} != {1}", 4.2F, 4.2D);
```

I find the results notable in several regards:

1. You can use a double to expose the inaccuracy of a float.
    
2. Comparing decimalNumber and floatNumber reveals they are not equal even though printing the values out to 20 decimal places indicates they are equivalent.
    
3. doubleNumber1 and floatNumber are not equivalent even though they are both assigned the exact same calculated value in the code.  (In fact, the IL reveals the values are different.)
    
4. This is not just an issue of calculation as the last two assertions reveal.
    

The obvious question at this point is why?

1. float is only accurate to 7 digits so if you cast it to a data type that can hold more than that you will inevitable expose the "insignificant" portion such that it becomes significant.  (This is why (double)4.2F does not equal 4.2D.)
    
2. decimal, float and double get initialized with different calculated values because they require different levels of accuracy.  The decompiled IL code is as follows:
    
```csharp
decimal decimalNumber = 4.2; 
double doubleNumber1 = 4.200000062584877; 
double doubleNumber2 = 4.2000000000000002; 
float floatNumber = 4.2000003;
```

* * *

In response to and appreciation of Julian's post here I took the time to correct my post.  Thanks Julian!

I should perhaps delete the entire post but I think my carelessness requires a correction.  The primary modifications are as follows:

1. I updated the IL code.  Converting the hex values displayed by ILDasm.
2. Deleted the  "Trace.Assert((decimal)4.2F != 4.2M);."  "Trace.Assert(!4.2M.Equals(4.2F));" was what I should have posted.
3. I updated the variable names to be slightly better.
4. Deleted: "Even though floatNumber and doubleNumber2 are assigned the same values in IL they still don't evaluate as equal."  This was incorrect.  They are not assigned the same value in IL, only in C#.
5. Delete: "Any time you compare one <of thesetypes against another the Equals(object value) method is called and it returns false if the data type is not the same. "  It didn't really fit as I didn't use the Equals() method in any of my code and generally the Equals() method is overloaded with a parameter that takes the class type.
6. Deleted: "If you remove the calculations and simply assign 4.2F ...."  This was just incorrect (see colophon).

**Colophon:** The root cause of all the errors was the fact that I was using csc.exe for compiling and not VS.NET.  As a result, I forgot the /D:TRACE switch so assertions were ignored.  I am amazed that only one of the assertions in the end was invalid but regardless I should have been more careful.
