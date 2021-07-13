

## How and Why to Implement Decoupling Classes in Your Code

Estimated reading time: 9 minutes

Decoupling is a strategy for writing testable code when dealing with hard-to-test classes, and it can be an incredibly powerful tool to help you write clean, effective, and bug-free code.

### Contents

- [How and Why to Implement Decoupling Classes in Your Code](#h-how-and-why-to-implement-decoupling-classes-in-your-code)
    - [What Is Decoupling?](#h-what-is-decoupling)
    - [Why Should I Decouple?](#h-why-should-i-decouple)
    - [Decoupling in Action](#h-decoupling-in-action)
    - [Implementing a Decoupling Interface](#h-implementing-a-decoupling-interface)
    - [Creating a Testable Version of a Decoupling Interface](#h-creating-a-testable-version-of-a-decoupling-interface)
    - [Unit Testing Decoupled Code](#h-unit-testing-decoupled-code)
    - [Try it Yourself](#h-try-it-yourself)
    - [Have Questions?](#h-have-questions)

### What Is Decoupling?

Decoupling is a coding strategy that involves taking the key parts of your classes’ functionality (specifically the hard-to-test parts) and replacing them with calls to an interface reference of your own design. With decoupling, you can instantiate two classes that implement that interface. One class is for production that makes calls to the parts of your code that were initially hard to test. The other class is for testing purposes, which behaves however you determine. This can make unit testing your code much easier.

An example of this would be in a class that makes calls to `System.Console.Write()` and `System.Console.Read()`.  Instead of using the class `System.Console`, we will construct an interface IConsole and implement two classes that inherit from it. One class for production that extends our `IConsole.Read()` and `IConsole.Write()` calls to `System.Console` and another class for testing where we can specify the desired inputs and outputs for the testing scenario.

### Why Should I Decouple?

Certain classes in C# are notoriously hard to test, such as `System.Console`. `System.Console` which depends on user input which is antithetical to unit testing. Additionally, console output requires capturing and redirecting if you want to automate test results.  Without decoupling, unit-testing a class that utilizes `System.Console.Read()` or `System.Console.Write()` calls is a living nightmare (believe me, I’ve done it!). By implementing decoupling via an `IConsole` interface described above, we can make our code easily unit-testable without removing the `System.Console` functionality from the program.

This concept can be expanded to many kinds of classes and situations to make code more unit testable.

### Decoupling in Action

Using the `IConsole` example I mentioned earlier, let’s say I have a given class that makes lots of calls to `System.Console.ReadLine()` and `System.Console.WriteLine()`, and I want to test it. Let’s assume I want to write unit tests for this class below.

```
public class SportsTeam
{
    public string Sport { get; set; }
    public string TeamName { get; set; }

    public SportsTeam(string sport, string teamName)
    {
        Sport = sport;
        TeamName = teamName;
    }

    public void PrintTeamInfo()
    {
        System.Console.WriteLine(Sport + " - " + TeamName);
    }

    public void UpdateTeam()
    {
        System.Console.Write("Enter new team name --> ");
        var newName = Console.ReadLine();
        TeamName = newName;
    }
}
```

This class is very basic; it’s just here to help explain how to implement decoupling. However, even a simple, barebones class like this would take significant effort to unit test.

If you wanted to write the unit test method `UpdateTeam_TeamIsUpdated()`, you would have to capture `System.Out` using redirection and StringReaders/Writers to create “expectedValue” strings that include exactly what your output for the entire program will look like down to the spaces and carriage returns. You would then have to capture and redirect `System.in` (similar to how we handled `System.Out`) and then verify your captured variables. All of this work is required to test the three lines of code contained in `UpdateTeam()`.

### Implementing a Decoupling Interface

To circumvent the workload described above, we need to decouple `System.Console` from our current project. This is done by implementing a decoupling interface. Note that `System.Console` is a static class. Static classes can be very difficult to test because there is no effective way to inject them at runtime. Part of decoupling is replacing static (singleton) classes with instances that we can scope.

The purpose of this interface will be to replace all instances of `System.Console` in our `SportsTeam` class with references to an instance of the decoupling interface we will name `IConsole`. Then, we can utilize our `IConsole` reference just how we would use `System.Console`. This way, we can specify what implementation of `IConsole` we want to use when we instantiate our class: the one that is for production or the one that is for testing.

Let’s start by creating an `IConsole` interface that contains methods meant to mimic the functionality of `System.Console`.

```
public interface IConsole
{
    void Write(string value);
    void WriteLine(string value);
    string ReadLine();
}
```

Note: We only require the functionality that we need for our current project from `System.Console` in this interface. Here is what a production use implementation of `IConsole` will look like.

```
public class ProductionConsole : IConsole
{
    public void Write(string value)
    {
        System.Console.Write(value);
    }

    public void WriteLine(string value)
    {
        System.Console.WriteLine(value);
    }

    public string ReadLine()
    {
        return System.Console.ReadLine();
    }
}
```

The above code shows the production implementation of this `IConsole` interface wrapping our calls to `System.Console`. Because of this, the functionality of our `SportsTeam` class will not change in any way when we implement `IConsole` in place of our current `System.Console` calls.

We make this change in our `SportsTeam` class by implementing a private, read-only auto property of type `IConsole` named “Console,” which will be set at the classes instantiation via a parameter passed into the constructor. Since our `IConsole` interface contains methods with the same names and functionality as the ones in `System.Console` that we are using, this implementation barely changes our actual code. The only change is that instead of calling `System.Console.Write()`, we save ourselves some typing by calling a method on our private property using `Console.Write()`. If we had originally imported `System` into class, there would have been no changes to the core methods.

```
public class SportsTeam
{
    public string Sport { get; set; }
    public string TeamName { get; set; }

    private IConsole Console { get; }

    public SportsTeam(string sport, string teamName, IConsole console)
    {
        if(console == null)
            throw new ArgumentNullException(nameof(console));
        
        Sport = sport;
        TeamName = teamName;
        Console = console;
    }

    public void PrintTeamInfo()
    {
        Console.WriteLine(Sport + " - " + TeamName);
    }

    public void UpdateTeam()
    {
        Console.Write("Enter new team name --> ");
        var newName = Console.ReadLine();
        TeamName = newName;
    }
}
```

So far, we’ve completely decoupled `SportsTeam` from `System.Console`, which means that `SportsTeam` no longer makes any calls directly to `System.Console`, but instead makes calls to an object whose functionality we define. The usefulness of this decoupling will be evident in our unit tests.

### Creating a Testable Version of a Decoupling Interface

Now let’s see at what a testable version of our `IConsole` interface will look like.

```
public class TestableConsole : IConsole
{
    public TestableConsole()
    {
        LastWrittenLine = new List<string>();
    }

    public List<string> WrittenLines { get; set; }
    public void Write(string value)
    {
        LastWrittenLine.Add(value);
    }

    public void WriteLine(string value)
    {
        LastWrittenLine.Add(value);
    }

    public string LineToRead { get; set; }

    public string ReadLine()
    {
        return LineToRead;
    }
}
```

This version of `IConsole` contains all the necessary methods to implement the interface, but none of them do what we think they would. In fact, these methods don’t really perform any action at all. Rather, the purpose of this class is to enable us to define how and when these methods are called so that we can ensure that our code utilizing `IConsole` is functioning the way it should.

When we test a given bit of code, such as a method, we are only concerned with ensuring the functionality of that code. Rather than focusing on every function that the method makes calls to. System.Console has its own set of tests that ensure the functionality of System.Console.Write(), so it doesn’t matter that nothing gets printed to the screen when Console.Write() is called from a test (for that matter, we should also unit test our testing harness like TestableConsole ). All that matters is proving the appropriate methods are called under a given set of circumstances. As a result, our testable version of IConsole serves to record which of the `IConsole` methods have and have not been called.

Note: `WrittenLines` is a list of strings rather than a singular string. With this code, we can test methods that print multiple lines are functioning correctly.

### Unit Testing Decoupled Code

To unit test our `SportsTeam` class code, we need to instantiate our class, but when we do, we need to pass in our testable version of the `IConsole` interface.

```
\[TestClass\]
public class SportsTeamTests
{
    \[TestMethod\]
    public void PrintTeamInfo\_PrintsCorrectInformation()
    {
        var testConsole = new TestableConsole();
        var myTeam = new SportsTeam("Hockey", "Bruins", testConsole);

        myTeam.PrintTeamInfo();

        Assert.AreEqual<string>("Hockey - Bruins", testConsole.WrittenLines\[0\]);
    }

    \[TestMethod\]
    public void UpdateTeam\_TeamIsUpdated()
    {
        var testConsole = new TestableConsole();
        var myTeam = new SportsTeam("Hockey", "Bruins", testConsole);
        
        testConsole.LineToRead = "Rangers";

        myTeam.UpdateTeam();

        Assert.AreEqual<string>("Rangers", myTeam.TeamName);
        Assert.AreEqual<string>("Enter new team name --> ", 
            testConsole.WrittenLines\[0\]);
    }
}
```

The above code shows how we instantiate `TestableConsole` and pass it into the constructor of `SportsTeam`. We then call the given method desired and assert that the method is actually receiving (and would be printing) the expected information. We have now proven that, for these given situations, our method is behaving the way we want it to.

In the case of `UpdateTeam_TeamIsUpdated`, we specify what line the method would read by manually assigning `LineToRead`. This assignment simulates the user inputting something specific on the keyboard and checking the result. This idea is similar to manually testing your code back in your high school programming classes to see if it performed as intended. However, here we are automating the process to save time and effort in the long run.

NOTE: this is how `SportsTeam` would look if it were called from a production setting using our `ProductionConsole` that just extends `System.Console`.

```
class Program
{
    static void Main(string\[\] args)
    {
        var productionConsole = new ProductionConsole();
        SportsTeam myTeam = new SportsTeam("Hockey", "Bruins", productionConsole);
        
        myTeam.PrintTeamInfo();
        myTeam.UpdateTeam(); //requires input from user not displayed here
        myTeam.PrintTeamInfo();
    }
}
```

### Try it Yourself

Utilizing decoupling to enhance the flexibility and testability of a given project is just one of many. Many places use the principle of decoupling, such as separating classes for unit testing use and refactoring code so that it is more readable and functional.

Please note the given set of unit tests in this blog do not utilize the full power of unit testing. I favored simplicity to achieve elucidation.

Try it for Yourself: Take [this code](https://github.com/IntelliTect/blog-resources/tree/DecouplingClasses) and change it to make It functional in edge-case situations. What happens if you input `null` as a variable into the `SportsTeam` constructor? What happens if you update the team’s name with a negative integer? Change your code to handle these edge cases and then utilize the `TestableConsole` to write unit tests that prove the code behaves as expected in those situations.  Tweak the above code to handle these cases, and you’ll find yourself learning a lot about the usefulness of decoupling.

Helpful tools to use in your unit Testing are `Assert.AreEqual`, `Assert.IsNull`, and the testing method tag `[ExpectedException(typeOfExpection)]` - the last of which requires no asserts. For more information on these and more unit testing tools, consult the [Microsoft Unit-Testing Documentation](https://docs.microsoft.com/en-us/visualstudio/test/unit-test-basics?view=vs-2017).

Now head into the world with the power of decoupled classes and change your code for the better!

_Written by Raymond Shiner._

### Have Questions?

Ask a question about decoupling or comment below.

![](https://intellitect.comhttps://intellitect.com/wp-content/uploads/2021/04/blog-job-ad-2-1024x129.webp)
