## Creating Efficiency for Unit Testing
#
While writing my Essential C# (Addison-Wesley) I am creating lots of console applications.  Unit testing these has been rather cumbersome.  I decided to create an attribute that can redirect the console input and output so that I can supply various inputs and then test the output.

I went with MbUnit because of its ability to support [custom decorators](https://www.testdriven.net/wiki/default.aspx/MyWiki.ExtendingMbUnitWithYourTestDecorator).  Although MbUnit does support [ConsoleLikeFixtures](https://www.testdriven.net/wiki/default.aspx/MyWiki.ConsoleLikeFixture) and [ConsoleTesting](https://www.testdriven.net/wiki/default.aspx/MyWiki.TestingConsoleApplication) out of the box, I was looking to use decorators as follows:

**Test expected output**
```csharp
[Test] 
[ConsoleOutputExpected( "Test" )]  
public void Basic() 
{ 
    Console.Write( "Test" ); 
}
```

 **Provide automated input** 
```csharp
[Test] 
[ConsoleInput( "Test" )]  
public void Basic() 
{  
    string input = Console.ReadLine(); 
    Assert.AreEqual( "Test" , input); 
}
```

In addition, I wanted  ConsoleOutputExpected  to support wild-cards, regular expressions, and straight string comparison.

 **Use regular expressions in expected console output** 
```csharp
[Test] 
[ConsoleOutputExpected( "^Test" , SearchTypeOptions.UseRegularExpressions)]  
public void StringBeginsWith() 
{ 
    Console.Write( "Test5" ); 
}
```

 **Use wild-cards in expected console output** 
```csharp
[Test] 
[ConsoleOutputExpected( @"Test\* Test\*" , SearchTypeOptions.UseWildcards)]  
public void MultiLineEndsWith() 
{ 
    Console.WriteLine( "Test1" ); 
    Console.WriteLine( "Test2" ); 
}
```
Anyway, following [the instructions](https://www.testdriven.net/wiki/default.aspx/MyWiki.ExtendingMbUnitWithYourTestDecorator) I derived from  MbUnit.Core.Framework.DecoratorPatternAttribute  and implemented the Execute() method as follows:
```csharp
 public override Object Execute(Object o, IList args) 
 {  
     using  (MemoryStream stream = new MemoryStream())  
     using  (TextWriter writer = new StreamWriter(stream))  
     using  (TextReader reader = new StreamReader(stream)) 
     { 
         ((StreamWriter)writer).AutoFlush = true ;
        TextReader originalReader = Console.In;
        try  
        {  
            object result;
            Console.SetIn(reader);
            writer.Write(Attribute.Input);
            stream.Seek(0, SeekOrigin.Begin); 
            result = this.Invoker.Execute(o, args);  
            return null; 
        }  
        finally  
        { 
            Console.SetIn(originalReader); 
        } 
    } 
}
```
 In summary, the process was relatively simple and I was pleased with the results with two exceptions: 

1. The C# string literal syntax is really funky when text spans lines.  The problem is that code is indented but the text itself can't be because it is taken literally.  This makes for rather peculiar code.  I wish there was an elegant way to keep indentation within the source code but not the actual literal. 
    
2. The new Console methods in .NET 2.0 like  System.Console.CursorTop/Left  and  System.Console.SetCursorPosition()  cannot be redirected.  It always throws an invalid handle exception.  This makes unit testing these methods beyond reasonable.  I posted a bug about this problem in the hopes that Microsoft would fix the issue before release.
    

You can download the source code [here](https://intellitect.com/wp-content/uploads/binary/ConsoleInputOutputRedirectAttribute.zip). 
