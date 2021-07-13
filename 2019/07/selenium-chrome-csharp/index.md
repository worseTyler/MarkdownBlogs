

## You're testing a desktop application with Selenium. How do you verify an item on the page after the browser launches?

I recently came up against this use case and couldn’t find a clear answer for C#. Every once in a great while, the need arises to have Selenium communicate to a browser that is already launched, sitting at some page. While the bulk of examples available involve launching the browser with Selenium and then attaching with a different driver, I couldn’t find a way to verify something on the page if Selenium _didn’t_ launch the browser.

Sure, I found a Python and Java example [here](https://www.teachmeselenium.com/how-to-connect-selenium-to-an-existing-browser-that-was-opened-manually/). There are also a couple of other Java tutorials floating around for how to do this a bit differently via the Canary (and newer) builds of Chrome, but I wanted to bring this goodness to the C# world for current capabilities. In the spirit of DevOps focusing on bringing teams together, both testers and developers could benefit from this. The working example that I’m providing is for testers wanting to test these sorts of scenarios as well as developers so that they have the information they need to launch with settings that will make this easier on their testers.

### Let's Get Started!

The repo I’m working out of is [here](https://github.com/PandaMagnus/AutomatedUiTestingExamples).

Cutting right to the chase, the two vital pieces of code are leveraging Chrome’s remote debugging capabilities. Launching Chrome out of a WPF application looks like this:

MainWindow:

```
private void LaunchBrowser\_Click(object sender, RoutedEventArgs e)
{
      Process proc = new Process();
      proc.StartInfo.FileName = @"C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe";
      proc.StartInfo.Arguments = "https://www.intellitect.com/blog/ --new-window --remote-debugging-port=9222 --user-data-dir=C:\\\\Temp";
      proc.Start();
}
```

The highlighted portion is critical. It tells Chrome which port to use for remote debugging and where to find the relevant user data.

PageUnderTest.cs

```
private IWebDriver Driver { get; set; }

// Some basic Selenium calls we’ll use in our tests
public void AttachToChrome()
{
    ChromeOptions options = new ChromeOptions();
    options.DebuggerAddress = "127.0.0.1:9222";

    // Using Polly library: https://github.com/App-vNext/Polly
    // Polly probably isn't needed in a single scenario like this, but can be useful in a broader automation project
    // Once we attach to Chrome with Selenium, use a WebDriverWait implementation
    var policy = Policy
      .Handle<InvalidOperationException>()
      .WaitAndRetry(10, t => TimeSpan.FromSeconds(1));

    policy.Execute(() => 
    {
        Driver = new ChromeDriver(options);
    });
}
```

We point Selenium at a debugger address (port included). Now, we can “attach” to the Chrome instance launched by our desktop app.

You can now drive the Chrome instance started by our small WPF app:

```
// AttachToChromeTest.cs
\[TestMethod\]
public void LaunchChromeAndAttach()
{
    // Open WPF application, make sure a button is present, then click it to launch Chrome
    Assert.IsTrue(Window.LaunchBrowserButton.Displayed, 
        "Expected button never appears.");
    Window.LaunchBrowserButton.Click();

    // Attach to new Chrome instance
    Page.AttachToChrome();

    // Verify Chrome launched to the correct page
    Assert.AreEqual("https://intellitect.com/blog/", Page.Driver.Url);
    Assert.IsTrue(Page.BlogList.Displayed);
    Assert.IsTrue(Page.BlogHeadings.Count > 0);
}
private Window Window => new Window();
private PageUnderTest Page { get; } = new PageUnderTest();
```

### Want More?

Check out my blog, _[The Feasibility of Test Automation](/feasibility-test-automation/),_ and discover the benefits of adding simple test automations to the early stages of your project.

### Got Some Other Tricky Use Cases?

Comment below. Let's solve them together!
