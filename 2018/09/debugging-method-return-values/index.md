
## ![Debugging like a Pro in Visual Studio](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2018/09/debugging-method-return-values/images/HeaderDebuggingLocalsWindow-1024x768.png)

## Viewing method return values in the Locals/Autos windows in Visual Studio

Visual Studio is packed with tons of great debugging tools. There are so many tools that it’s often hard to keep track of them all. Simply knowing about a tool can make a significant improvement to your productivity. In this post, we’ll look at how you can examine the return values from methods in the Locals and Autos windows.

Let’s take a look at a very simple program as an example. It prints out the even numbers from the fibonacci sequence until it [overflows its integer](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/checked).

static void Main(string\[\] args)
{
   IEnumerable<string> fibonacci = Fibonacci()
       .Where(x => x % 2 == 0)
       .Select((value, index) => $"{index + 1}: {value:N0}");

   foreach (string value in fibonacci)
   {
       Console.WriteLine(value);
   }
}

private static IEnumerable<int> Fibonacci()
{
   int previous = 1;
   int previousTwo = 1;

   yield return previous;
   yield return previousTwo;

   while (true)
   {
       int result;
       try
       {
           result = checked(previous + previousTwo);
       }
       catch (OverflowException)
       {
           yield break;
       }
       yield return result;

       previousTwo = previous;
       previous = result;
   }
}

In the Locals (CTRL+ALT+V, L or Debug >> Windows >> Locals) and Auto (CTRL+ALT+V, A or Debug >> Windows >> Autos) windows, you will see the returned values from each of the function calls when you [step over statements](https://docs.microsoft.com/en-us/visualstudio/debugger/navigating-through-code-with-the-debugger?view=vs-2015#BKMK_Step_over_Step_out) while debugging. You can then inspect these values individually just like any other variable.

![Debugging method return values in locals window](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2018/09/debugging-method-return-values/images/DebuggingLocals2.gif)

It is important to note, that you will only see these when stepping through code. If you [run to a particular location](https://docs.microsoft.com/en-us/visualstudio/debugger/navigating-through-code-with-the-debugger?view=vs-2015#BKMK_Break_into_code_by_using_breakpoints_or_Break_All),you will not see the individual method return values.
