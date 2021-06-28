
One feature new to Visual Studio 11 that you will wonder how you managed without (perhaps instead you wondered why it was missing from Visual Studio for so long) is the concept of “Search Everywhere.”  In Visual Studio 11 search is now supported for the following:

- Solution Explorer\*
- Add Reference\*
- Integrated Quick Find\*
- New Test Explorer
- Error List
- Parallel Watch
- Toolbox
- TFS Work Items
- Visual Studio Commands

(\* indicates where similar search functionality exists within the Productivity Power Tools – targeting Visual Studio 2010)

Below we take a look at each search location in a more detail.

# Solution Explorer

Perhaps the most noticeable manifestation of Search Everywhere appears within the Solution Explorer.  As shown both in Figure 1, there is a **Search Solution Explorer** text box with keyboard shortcut "**Ctrl+;**" (you will want to memorize that almost immediately) that searches the solution and produces a search results window of matching Solution Explorer available nodes.

\[caption id="attachment\_2746" align="aligncenter" width="624" caption="Figure 1: Visual Studio 11 Solution Explorer"\][![Visual Studio 11 Solution Explorer](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2012/02/visual-studio-11-search-everywhere/images/022912_0753_VisualStudi1.png "Visual Studio 11 Solution Explorer")](/wp-content/uploads/2012/02/022912_0753_VisualStudi1.png)\[/caption\]

The results window displays such assets as projects, files, and identifiers – classes, and class member names. The result is similar in behavior to Visual Studio 2010’s **Navigate To** (Ctrl+,) functionality, but rather than a modal dialog, the **Search Solution Explorer** produces a search results window that is integrated into the IDE.  Like **Navigate To**, the **Search Solution Explorer** supports “fuzzy search” – entering the first (uppercase) letters of each word within an identifier will still locate the corresponding node.  For example, entering “MOCT” will display all instances of MockObjectContextTests but “moct” will not.  Note that the keyboard shortcut (Ctrl+;) switches focus to the search box even when the Solution Explorer is not visible. This is a handy shortcut when the Solution is mistakenly closed.

# Add Reference

\[caption id="attachment\_2747" align="aligncenter" width="844" caption="Figure 2: Add Reference Dialog"\][![Add Reference Dialog](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2012/02/visual-studio-11-search-everywhere/images/022912_0753_VisualStudi2.png "Add Reference Dialog")](/wp-content/uploads/2012/02/022912_0753_VisualStudi2.png)\[/caption\]

As shown in Figure 2, the Add Reference Dialog, still places the target reference into categories – Assemblies, Solution, and COM – but the experience of looking for a particular reference is significantly improved.  Possibly more noticeable than the Add Reference search is how quickly the **Add Reference** dialog opens.  Rather than seemingly enumerating the Global Assembly Cache and every COM object installed on the computer, the new dialog uses an index (of the installed components) and displays almost instantly. Double-clicking on any item selects it, and all referenced items are displayed with a checkmark next to the assembly name.

# Integrated Quick Find

While on the topic of search, another significant productivity improvement is a streamlined Find UI design – Quick Find.  Leveraging again the UI of the Productivity Power Tools, Visual Studio 11’s Find (Ctrl+F) and Find-Replace (Ctrl+H) functionality is integrated into the text editor window rather than a new pop-up window (see Figure 3).  The result is that Find and Find-Replace functionality remains in context.

[![Integrated Quick Find](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2012/02/visual-studio-11-search-everywhere/images/022912_0753_VisualStudi3.png "Integrated Quick Find")](/wp-content/uploads/2012/02/022912_0753_VisualStudi3.png)

Figure 3: Integrated Quick Find

Furthermore, the text entered in the Find box is immediately highlighted throughout all text editor windows – the currently active document, other open document windows, and any newly opened document windows. To change the search scope you can select from the dropdown available below the Replace text box.  Figure 5 shows the advanced search options – available after clicking the “Expand” button directly to the left of the Find text box.  For developers looking for the **Use Regular Expression**, **Match Whole Words**, and **Match Case** functionality in addition to the **Find in Files** capabilities (something no longer available in the toolbar as will be discussed shortly), there is a dropdown on the text box that not only displays recent search terms but also the additional search options.

# New Test Explorer

Another manifestation of Search Everywhere is in the new Unit Test Explorer window.  Unit testing in Visual Studio 11 was overhauled and includes support for third party testing frameworks along with a consolidation of the testing windows into the new Unit Test Explorer window (see Figure 3).  The top of the new Unit Test Explorer window includes a search textbox, enabling search for a particular test.

\[caption id="attachment\_2749" align="aligncenter" width="314" caption="Figure 4: Visual Studio 11 Test Explorer"\][![Visual Studio 11 Test Explorer](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2012/02/visual-studio-11-search-everywhere/images/022912_0753_VisualStudi4.png "Visual Studio 11 Test Explorer")](/wp-content/uploads/2012/02/022912_0753_VisualStudi4.png)\[/caption\]

In addition to search, the new Unit Test Explorer includes the following functionality:

- Continuous Testing Immediately following a compile, the unit testing is automatically triggered to run (in a separate process so as not to impact the IDE responsiveness).  As a result, executing tests is no longer a separate action but rather something that is automatically part of the development process – in a similar way that syntax checking happens automatically as you write code.  If there are failing tests, only the failing ones will be executed as part of the continuous testing.  The idea behind this is to get the failing ones passing before executing any additional tests.
- Execution within a different process In Visual Studio 2010 the IDE was essentially blocked while unit tests executed.  In contrast, unit test execution within Visual Studio 11 runs concurrently with any IDE activity the developer may be working on.  This is due to the fact that in Visual Studio 11 the unit tests are asynchronously shelled out to a separate a process.
- Support for different unit testing frameworks As mentioned, Visual Studio 11’s unit testing framework support is extensible.  As a result, unit tests from other frameworks also appear within the new Unit Test Explore window.
- Sorting by relevance Unit tests within the Unit Test Explorer windows are sorted by relevance.  Failed tests bubble to the top.  Tests executed more recently appear before tests that were not run in the previous test execution.
- Execution time displayed The time it took to execute a unit tests appears next to each unit test.
- Mocks/stub support In Visual Studio 11 there is support for the generation of mocks/stubs of a class and this is integrated not only into the framework but all the way into the IDE tooling via “right-click generate mock/stub” contextual menus.

Those committed to unit testing will find the revamped functionality within Visual Studio 11 to be compelling to the point that this feature set alone will make it difficult to return to Visual Studio 2010.

# Error List

Yet another Search Everywhere location occurs in the Error List (see Figure 4).

\[caption id="attachment\_2750" align="aligncenter" width="616" caption="Figure 4: Visual Studio 11 Error List"\][![Visual Studio 11 Error List](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2012/02/visual-studio-11-search-everywhere/images/022912_0753_VisualStudi5.png "Visual Studio 11 Error List")](/wp-content/uploads/2012/02/022912_0753_VisualStudi5.png)\[/caption\]

By using the **Search Error List** textbox you can search across columns and all errors within the error list to focus on the errors you wish to address first.  Furthermore and again shown in Figure 4, Visual Studio 11 supports a filter so that developers are no longer overwhelmed by innocuous HTML warnings, for example, while they are investigating C# code.  Instead, using the filter button, developers can select the scope of warnings to be displayed.

# Parallel Watch

The Parallel Watch window is another area where search appears in Visual Studio 11.  Although not really a textual search, these watch windows include a **Filter by Boolean** expression search box that enables filtering across different threads and evaluating the expression for each thread to determine which threads evaluate the expression to true.

# TFS Work Items

An additional instance of Search Everywhere corresponds to TFS integration.  The Visual Studio 11 Team Explorer window is completely redesigned, and now includes a **Search Work Items** text box to search across work items.

# Visual Studio Commands

The last Search Everywhere feature discussed in this article appears in the Visual Studio 11 toolbar – enabling search over all the available commands and actions within Visual Studio 11 itself.  We explore this feature as part of Toolbar Improvements.
