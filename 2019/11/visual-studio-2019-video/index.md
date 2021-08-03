

## Now's the time to switch to Visual Studio 2019 (VS 2019).

Check out my video that was recorded at our Visual Studio 2019 event and get a crash course on the new features of VS 2019.

<iframe src="https://www.youtube.com/embed/gboTs-Dv04w" allowfullscreen width="560" height="315"></iframe>

I recommend watching the full video (above) for an in-depth explanation of Visual Studio 2019 features. If you're looking for some highlights of what I consider to be the major features and changes, see below.

### What are the New Features?

#### Convert to Interpolated String

![Screenshot showing how to convert to interpolated string](https://intellitect.com/wp-content/uploads/2019/11/Convert-to-Interpolated-string.jpeg "Visual Studio 2019 Refactoring Galore (Video)")

You can right-click (Ctrl+.) on the format method and convert it to interpolated string because it's easier to read, assuming you're not doing localization.

#### Renames at the csprog Level

Finally! You can rename your application, and it automatically changes the Assembly Name and Default namespaces in the project properties.

<iframe src="https://www.youtube.com/embed/h-ZgRTwGAJU" allowfullscreen width="560" height="315"></iframe>

It doesn't change the source code; it just changes the project file renders inside the Project Properties Window in Visual Studio.

_\*Note - It does not update resource files either. All other namespaces stay the same._

#### Document Health Indicator

![Screenshot showing the locations of the document health indicator.](https://intellitect.com/wp-content/uploads/2019/11/Document-Health-Indicator.png "Visual Studio 2019 Refactoring Galore (Video)")

There's now an indicator of how good your source code is doing. It appears in a couple of places, and you can navigate between the warnings.

#### Code cleanup

![Screenshot showing code cleanup options](https://intellitect.com/wp-content/uploads/2019/11/Code-Cleanup.jpeg "Visual Studio 2019 Refactoring Galore (Video)")

There's a new dropdown for code cleanup that you can configure. You would think that it would be extensible. It will be, but it isn't available right now.

#### New Search Feature

<iframe src="https://www.youtube.com/embed/BydhuY18NRs" allowfullscreen width="560" height="315"></iframe>

It now searches all the variables accessed through the debugger and finds the value entered. It's navigating through the hierarchy of the data in either autos, locals or the watch window.

It seems like a minor feature, but it's a cool addition and an incredibly efficient way to find the required value.

#### Null Check

<iframe src="https://www.youtube.com/embed/WZ2cC6veFeY" allowfullscreen width="560" height="315"></iframe>

You can now add a null check automatically as your values come in. This check is critical because the nullability feature in C# 8 only expresses your intent. It does NOT actually make any changes to verify that you do not have Null, so this is a valuable check to run.

#### Improved Search

It's now expanding into multiple resources, including the installer, and it indexes all of them.

If you want to search within install, you can now search by individual components, and it will pull up all the tools related to your search.

#### Visual Studio Installer Includes Side by Side Installs

You can work on bleeding-edge or older versions. Appropriately, they will go into different directories that can be updated however you need.

This flexibility now includes customizing multiple dot releases so that you can work on projects for various customers within the version that they request.

_\*Note - Visual Studio installs with the Azure Powershell module, and that has been updated to the AZ module. Those are not compatible with each other, so be sensitive to what you're installing._

#### Theme Customization

You can control whether your environment color theme is dark or light. The easiest way to do this is to enter "theme" in the Search (Ctrl+q).

#### Performance Loading

<iframe src="https://www.youtube.com/embed/qzVrlsg1OvY" allowfullscreen width="560" height="315"></iframe>

The project/solution is significantly faster. When there is a delay, a tip appears indicating that loading will continue in parallel, but you can begin coding.

#### Solution Filters

If you have large projects, you can save a subset using .slnf files (Unfortunately, dotnet CLI doesn't read this file at the time of this writing).

#### Docker Support

It has been available for a while, but what's interesting now is that if you don't have Docker installed and would like to, it will walk you through the installation process. Of course, after installation, it will allow you to debug within a Docker container, allowing for cross-platform debugging.

#### Code Analysis

<iframe src="https://www.youtube.com/embed/RG3EGcy1ZMU" allowfullscreen width="560" height="315"></iframe>

You'll be prompted to install the "visx" package, however, do so with caution as it only works in the Visual Studio UI, not on the command line.

Instead, I recommend installing the code analysis packages. Be careful when you search for analyzers as there are several. Always read to confirm it's what you're looking for. At IntelliTect, we install IntelliTectAnalyzers, which in turn pulls in additional analyzers that we have dependencies on.

#### Pull Requests

Interaction with Git has been improved.

![Screenshot of new Pull request page.](https://intellitect.com/wp-content/uploads/2019/11/Pull-requests-github-integration.png "Visual Studio 2019 Refactoring Galore (Video)")

Emojis can now be embedded in your source code.

![Screenshot showing emoji functionality.](https://intellitect.com/wp-content/uploads/2019/11/Review-Pull-requests.png "Visual Studio 2019 Refactoring Galore (Video)")

Check out [this blog](https://devblogs.microsoft.com/visualstudio/code-reviews-using-the-visual-studio-pull-requests-extension/) for a deeper dive into pull requests.

#### Time Travel Debugging (TTD)

![Screenshots showing time travel debugging setup.](https://intellitect.com/wp-content/uploads/2019/11/Time-Travel-Debugging.png "Visual Studio 2019 Refactoring Galore (Video)")

If you run windbg, it will launch TTD that will allow you to debug unmanaged code on your machine and will save the entire dump of the file. It's a great feature if you have a hard to find bug.

Want more on TTD? [Click here](https://docs.microsoft.com/en-us/windows-hardware/drivers/debugger/debugging-using-windbg-preview).

#### Test Explorer - My Favorite Feature

<iframe src="https://www.youtube.com/embed/EVRqo53SjVo" allowfullscreen width="560" height="315"></iframe>

I'm a little biased, but I'm excited about what's now available in the test explorer. The entire UI has been rewritten. You can now:

- Run tests after build
- Run tests in parallel
- Analyze code coverage for all tests
- Select a box in Azure DevOps to reject a build if code coverage drops, and you can make sure all new code coming in has unit tests associated with it

_\*Bonus - There is a new keyboard shortcut. Control-RL will rerun your tests as you modify your code._

#### Nullable Reference Types

<iframe src="https://www.youtube.com/embed/UCeb01JUfbc" allowfullscreen width="560" height="315"></iframe>

In C# 8, a string type can be configured to not null by default unless it's decorated with a nullability operator. However, if Microsoft took your existing code and changed all of the reference types to be not null by default, then there's a good chance it wouldn't compile.

To avoid such broken code, you need to opt in to the nullability feature by setting a Nullability element to "enable" within your csproj.

![](https://intellitect.com/wp-content/uploads/2019/11/EnablingNullability-1024x405.png)

Rather than enabling your entire project for nullability, you can also pinpoint the functionality to a block of code.

Want more on C# 8 features before you move on? Check out [this video](https://youtu.be/c5nsvjQ1I_g) from my talk at the Spokane .NET Users group.

### Where is Visual Studio Headed?

I know of at least two teams that are trying to look at data of reported bugs and associate that data with their fixed bugs so that they can determine patterns to fix future bugs through machine learning.

Intellisense utilizes C# 7 or 8 features to alert you to potential bugs.

### Wrap Up

That was just a taste of the new features of Visual Studio 2019 (and there were more on the full video).

Here's the [link to all the code](https://github.com/IntelliTect-Samples/2019-10-23.VisualStudio2019Launch) for IntelliTect's Visual Studio 2019 event.

Have a question? Ask me in the comments.
