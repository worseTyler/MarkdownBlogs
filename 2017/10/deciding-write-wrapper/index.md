---
title: "Deciding to Write a Wrapper"
date: "2017-10-10"
categories: 
  - "blog"
  - "devops"
  - "test"
tags: 
  - "qa"
  - "selenium"
  - "testing"
  - "uitestcontrol"
  - "wrapper"
---

Two things happened around the time I finished up writing a wrapper for Microsoft's UITestControl class: I wondered if I should do the same thing for a web testing technology like Selenium, and I discovered that at least some discussions exist on if wrappers could be considered an [antipattern](https://stackoverflow.com/questions/2550961/wrappers-law-of-demeter-seems-to-be-an-anti-pattern). As I began to investigate writing a wrapper for Selenium, and looking at some existing examples from the team I was on, I quickly realized why there was some debate. I took a good long look back at the reasons why I originally wrote the UITestControl wrapper and decided that it was, in fact, a good decision. But I also learned to be more particular in deciding to write or use wrappers.

The main decision point I found was: does this provide additional, needed functionality to the source library without adding unnecessary fragility, obfuscation, or busywork whenever the source library is called or modified? So for example, the wrapper I wrote for UITestControl abstracts away a lot of the required calls for MSTest to find a UI element, but those calls pretty much never change for the average application. So the original call looked like this:

public WinButton CalculatorButton()
{
   parent = FindWinWindowUnderTest();
   var found = new UiTestControl(parent);
   found.SearchProperties.Add(UITestControl.PropertyNames.Name, "someCalcButton");
   found.WindowTitles.Add("Application Under Test Title");
   return found;
}

Calling the wrapper that automatically sets the parent window (unless you pass in an override) and the return type so you can use this for any WinControl or WpfControl type:

public WinButton CalculatorButton()
{
   return FindControlByName( "someCalcButton", c => new WinButton( c ) );
}

On top of that, the wrapper adds additional error checking to make the calls more robust. The important part here is that in testing applications with both WPF and WinForms controls, I have never needed to modify my calls beyond what the wrapper I wrote provides. A simple override handles a program that creates new windows. Beyond that, it’s busywork to specify that the window title really is the same for every call. Compare that to a wrapper class I found for Selenium that condensed:

public static IWebElement FindLoginButton
{
   get { return Browser.Driver.FindElement(By.Id(“someLoginButton”)); }
}

To:

public static IWebElement FindLoginButton
{
   get { return FindElementById(“someLoginButton”);
}

While saving characters on “Browser.Driver” (or some variation, depending on your implementation of Selenium) was nice for mild readability, the wrapper did nothing else at all. Providing no functional benefit for a minor difference in readability won’t matter to someone with the skillset to write Selenium tests. We wanted to build some additional error checking and polling statements around the Selenium calls so they were centralized and not spread out through individual tests. To do this we either had to update two to three places (the wrapper class itself, another class that the wrapper was referencing, and possibly the test calling of the wrapper, depending on the situation) or go around the wrapper entirely and modify the Selenium implementation directly.

In this case, we didn’t need a wrapper for selenium to eliminate lines of code, we needed some additional methods behind the scenes to help make our test cases more readable. Compare that to the case of UiTestControl where the wrapper helps eliminate a significant number of lines of code to make the method your test calls more readable and understandable.

Ultimately, the lesson I learned was one of convenience: does the wrapper consistently reduce my workload without making necessary future modifications more difficult? If so, it’s probably worth it. If not, then run in the opposite direction. And please, for the love of all QA, stop over-complicating your solution.

If you are running a Windows desktop automated test solution, consider looking into IntelliTect.TestTools.WindowsTestWrapper.
