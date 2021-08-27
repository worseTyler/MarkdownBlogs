

## Don't Think Senior Developers Struggle? Think Again.
#
Whenever a bug arises, the key to success for developers at all levels is to remain humble during the debugging process.

Way back in college, when I was first learning to program, I recall one of my professors telling the class, “printf isn’t broken; the bug is in your code.”

In C programming, printf is a foundational method that all new developers learn as a simple way to display output. I didn’t realize it at the time, but learning to unpack my professor’s statement would take me years.

Bugs can be frustrating for developers, and in that frustration, it is easy to blame things outside of our control. Surely the bug is not in _**my code**_; the problem must undoubtedly be in this other library, in the pipelines, in the compiler, etc.

When facing a complex issue, it is a matter of humility to approach a problem starting from the position of “_**my code**_ is broken, why does this not work?”

After many years, I want to claim that I have mastered this skill, but all too often, I find myself looking for bugs in the wrong places. What should you do if you think the bug is outside of your code? Compilers and frameworks are just other code pieces; though typically well-tested, they are not immune to bugs. What follows is a tale of me debugging one such bug and the struggle to remember that “printf isn’t broken.”

### `TestRecorder` in XAML Test

I have an open-source project called [XAMLTest](https://github.com/Keboo/XAMLTest) that improves the testability of XAML styles and templates. In late March of 2021, I was implementing a new feature within the library.

As a result of previously updating the library to use C# 9, I also slipped in some changes to update the code to use the latest C# 9 features. I got the code working, and all the tests passed in Visual Studio, so I submitted a pull request (PR). Shockingly, I found that my CI system, GitHub Actions, rejected the PR because a couple of unit tests failed.

I designed XAMLTest as a UI testing library, so it contains a class called `TestRecorder` to capture screenshots to make historical debugging visual failures a bit easier. To determine the file path to store the screenshots, it uses the [`CallerFilePathAttribute`](https://docs.microsoft.com/dotnet/api/system.runtime.compilerservices.callerfilepathattribute?view=net-5.0) and the [`CallerMemeberNameAttribute`](https://docs.microsoft.com/dotnet/api/system.runtime.compilerservices.callermembernameattribute?view=net-5.0). The constructor for `TestRecorder` looks like this:

```csharp
public TestRecorder(IApp app,
    [CallerFilePath] string callerFilePath = "",
    [CallerMemberName] string unitTestMethod = "")
{
    …
}
```

These attributes have existed for a while in C#. They will cause the compiler to look up the relevant information at compile-time and inject the appropriate strings into that constructor’s caller. This function of the compiler is quite handy as this constructor is expected to be called from a unit test:

```csharp
[TestMethod]
public async Task SaveScreenshot_SavesImage()
{
    IApp app = new Simulators.App();
    TestRecorder testRecorder = new TestRecorder(app);
    …
}
```

After it compiles, the code will functionally look something like this:

```csharp
[TestMethod]
public async Task SaveScreenshot_SavesImage()
{
    IApp app = new Simulators.App();
    TestRecorder testRecorder = new TestRecorder(app, @"C:\\DirectorPath\\TestRecorderTests.cs", "SaveScreenshot_SavesImage");
    …
}
```

The below test also happens to be one of the tests that started failing.

```csharp
[TestMethod]
public async Task SaveScreenshot_SavesImage()
{
    IApp app = new Simulators.App();
    TestRecorder testRecorder = new(app);

    Assert.IsNotNull(await testRecorder.SaveScreenshot());

    string? file = GetScreenshots(testRecorder).Single();

    string? fileName = Path.GetFileName(file);
    Assert.AreEqual(nameof(TestRecorderTests), Path.GetFileName(Path.GetDirectoryName(file)));
    …
} 
```

### Reviewing the Failures

Looking over the failures, it was clear that all the tests that were making assertions on the screenshot’s file path were failing.

That didn’t seem right. All these tests were previously working, and I had not made any changes to the `TestRecorder` class. I had only made slight modifications to the unit tests themselves (Note: the [target-typed new expression](https://docs.microsoft.com/dotnet/csharp/language-reference/proposals/csharp-9.0/target-typed-new) when instantiating the `TestRecorder` above). A little voice in the back of my head repeated the time-honored phrase “printf is not broken.”

I started checking the docs for the two `Caller*Attributes`. Perhaps my usage of them subtly changed behavior? Maybe the use of nullable reference types did something different? Nope, no luck there. The docs even showed an example like my usage.

I added more logging statements and ran more pipeline builds. Sure enough, when running on GitHub Actions, both of those constructor parameters were getting passed as empty strings, not the expected compiler-provided values. The first question in my mind was why am I getting different behavior locally. I started looking for differences between my machine and the GitHub Actions agent.

I target both .NET Core 3.1 and .NET 5, so I knew I had at least two SDKs installed.

![SDKs](https://intellitect.com/wp-content/uploads/2021/04/Screenshot-31.png "Debugging Frustrations: A Senior Developer's Story")

I also noted that I had a 3.0.0 version of .NET Core and a preview version of 5.0.2 installed.

### Making a Breakthrough

I then decided to copy the commands that the pipeline was running and run them locally. This decision was the first breakthrough. The same tests that had failed in the CI system were also failing locally.

Success! I could now test locally.

I switched back to Visual Studio and reran all the tests there. They all passed.  That little void repeated, a little louder this time, “printf is not broken.”

I like to live dangerously, so I run preview versions of Visual Studio. In this case, I was running Version 16.10.0 Preview 1.0. I switched from my feature branch back to the master branch and reran all the tests. They passed on both the command line and when run from Visual Studio. I moved back to my feature branch, and the tests started failing again when run on the command line with the .NET CLI.

At this point, I knew that some change in my pull request was causing the failure, but the only difference related to the failing test was that subtle switch to the target-typed new expression for `TestRecorder`. I reverted the change and reran the tests; they all passed.

I was a bit shocked.

As the little voice began to shout in my ear, I told it to shut up and started searching the Roslyn repository for similar bug reports.

It only took a moment to come across [an account of the same problem](https://github.com/dotnet/roslyn/issues/50475) from three months earlier. More impressively, it appears that the C# had already [found and fixed the issue](https://github.com/dotnet/roslyn/issues/49547) two months before that report (they work fast!).

I downloaded the latest .NET 5 SDK (5.0.201) and reran the tests; they all passed, confirming the theory that the target-typed new expression had a bug in the previous version.

### Staying Humble in the Debugging Process

Remember, all developers struggle!

It is essential to start from a position of humility, assume it is **_your_** _**code**_ that is broken, and slog through the debugging process until you reach an understanding position.

In the end, what the bug was or how it got fixed does not matter; what is important is the debugging process.

Sometimes there may be a legitimate bug outside of your code, but it is crucial to start by looking in your code for the bug and working backward until you are confident about where the bug is and why it exists.

Start with the assumption that “printf is not broken.”

I hope that sharing my struggle helps you in your future debugging.

“Remember, I’m pulling for you. We’re all in this together.” – Red ([The Red Green Show](https://en.wikipedia.org/wiki/The_Red_Green_Show))

### Want More?

I’ve shared other anecdotes about maintaining humility while [doing code reviews](/code-reviews/)! If this helped you at all, check it out too!
