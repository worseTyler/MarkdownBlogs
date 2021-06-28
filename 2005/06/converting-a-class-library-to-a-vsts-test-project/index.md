
I recently wanted to convert a NUnit test project to a VSTS Test Project.  The steps are relatively simple but I did come across one un-intuitive point #4 below:

1. Reference Microsoft.VisualStudio.QualityTools.UnitTesting.Framework.
2. Change the using NUnit.Framework declaratives to using Microsoft.VisualStudio.QualityTools.UnitTesting.Framework.
3. Search and replace the following: \[TestFixture\] => \[TestClass\] \[TestFixtureSetUp\] => \[ClassInitialize\] \[TestFixtureTearDown\] => \[ClassCleanup\] \[SetUp\] => \[TestInitialize\] \[TearDown\] => \[TestCleanup\] \[Test\] => \[TestMethod\] This can be done with a using alias declarative as well.
4. **Open the \*.\*proj file using Notepad equivalent and add the following to the element. <ProjectTypeGuids>{3AC096D0-A1C2-E12C-1390-A8335801FDAB};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}</ProjectTypeGuids>** (I found this information here.)

The last step can't be avoided, however, one alternative to changing all the attribute names is to place the following using declaration at the top of each file:

> #if NUnit using NUnit.Framework; #else using Microsoft.VisualStudio.QualityTools.UnitTesting.Framework; using TestFixture = Microsoft.VisualStudio.QualityTools.UnitTesting.Framework.TestClassAttribute; using TestFixtureSetUp = Microsoft.VisualStudio.QualityTools.UnitTesting.Framework.ClassInitializeAttribute; using TestFixtureTearDown = Microsoft.VisualStudio.QualityTools.UnitTesting.Framework.ClassCleanupAttribute; using TestSetUp = Microsoft.VisualStudio.QualityTools.UnitTesting.Framework.TestInitializeAttribute; using TestTeardown = Microsoft.VisualStudio.QualityTools.UnitTesting.Framework.TestCleanupAttribute; using TestAttribute = Microsoft.VisualStudio.QualityTools.UnitTesting.Framework.TestMethodAttribute; // Not required since attribute names are the same // using ExpectedException = Microsoft.VisualStudio.QualityTools.UnitTesting.Framework.ExpectedExceptionAttribute; // Attributes are the same but Microsoft's IgnoreAttribute does not take any parameters. // using Ignore = Microsoft.VisualStudio.QualityTools.UnitTesting.Framework.IgnoreAttribute; #endif

Perhaps a code snippet is in order for that monstrosity.

Using aliases is probably better for supporting both testing frameworks with a #define to switch from one to the next.  Note, however, that the IgnoreAttribute constructor in NUnit does not match Microsoft's IgnoreAttribute so this attribute will have to be edited.  (Hopefully the IgnoreAttribute occurs rarely if at all.)

IMO, this type of problem begs for project level using declaration support from the compiler or at least the IDE.  VS.NET for Visual Basic, for example, supports the project level using declaration saving developers from "Import System" at the top of every file.
