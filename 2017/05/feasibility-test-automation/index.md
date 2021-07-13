

With proper tools, technical approaches, and processes, automated testing improves software quality, time to market, and reduces overall costs. Starting early in the development process and maintaining a pragmatic approach will maximize chances of success. However, there is no “one size fits all” approach; proper approaches vary based on team resources and project needs. The correct approach will result in a tool that your team can use to produce quality code without the time and money associated with repeating manual tests, or not testing at all.

#### Integrate with the Software Development Lifecycle

Starting automation early and integrating within the development schedule maximize the chance of success and return on investment. If not treated as part of the development lifecycle, the development team loses the benefits of speedy and easily-repeatable automated tests, there is less collaboration between developers and QA, and bloats the release candidate testing effort as more tests will need to be run manually. In many cases, coming in at the tail end of the project is hard because project costs are expected to start falling off and any new changes should show some immediate benefit. Also, the difficulty is compounded for off-the-shelf customization, as will be touched on below. Unfortunately, several early steps can have a longer than expected lead time. Infrastructure and architecture choices, infrastructure setup, and the initial effort to write code to find the buttons, text fields, etc. in the application must happen before tests can be written. This lag can put a lot of visibility on the lack of results early on, and makes it difficult to see (or achieve) the long-term benefit. Think of the situation where your automation engineer is hired a month before release, and a week into the job a critical bug is found by someone else. Depending on the company culture, the natural follow-up question is "why didn't automation catch this already / can we add this to the automation suite?" without understanding that test agents are still being spun up, or a framework is still being worked on.

Starting early allows the QA engineers to experience some of the initial pains right along with feature development and allows automation to begin providing benefit within a cycle of a feature being implemented. It also facilitates the QA engineers working hand-in-hand with the developers to have a more intimate understanding of the feature while bridging the gap between the speed of a manual test with the long-term efficiency of a repeatable automated test.

#### Maximize Maintainability and Sustainability

Fragility can be a particular problem for automated UI tests. Identifying patterns in your automated tests to both handle feature volatility and ensure consistent, repeatable test execution is paramount. This fragility is expensive to maintain and often a result of a poorly architected solution, or a solution relying on a record and playback tool. This criticism touches on several problems that need to be directly addressed by the automation team via the following points...

#### Overcoming feature volatility

How volatile a feature is has a drastic impact on automation. If a feature has nebulous acceptance criteria or requirements, or multiple people fighting for ultimate say in what the feature does, automating could be an exercise of repeatedly writing and scrapping tests. Identifying these areas as soon as possible will help prioritize the automated tests that should be written first. Often automating the more concrete features first demonstrates the benefit, allowing the QA team to go back and address the fluctuating features later in the cycle.

#### Focus On Good Practices

Another way to minimize rework is to use good coding and testing practices. This can help handle unforeseen feature volatility and will help reduce the need to babysit existing tests. As an example, separating your control searching logic from the actual test allows the two components to be swapped out independently as needed.

```
TestCase1()
{
   LaunchApplication();
   Login("SomeUserName","SomePassword");
   Assert.True(WelcomeScreen.Displayed);
}

Login(string userName, string password)
{
   FindControlById("username-textbox").EnterText("userName");
   FindControlById("password-textbox").EnterText("password");
}
```

This way, if the username textbox changes, you only have to change the Login method, instead of every single test that tries to log in.

#### Deal with Fragility and Complexity Pragmatically

A pragmatic approach to automated testing can ensure longevity and continual improvement of the tests. There are circumstances where creating an automated test is a poor investment. Budgets, timelines, competing resources, uncommon implementations, and code written outside the team all have an affect on if or how much to automate. I see this most often with solutions that involve off-the-shelf implementations and vendors, where the development team might not have access to adding IDs to controls, and the control hierarchy is abstracted away behind some interface or go-between. In these cases, the amount of automation may need to be stripped down to bare-minimum, high-priority flows. Imagine a website login screen, where you want to automate clicking the "Login" button. It's the difference between the (pseudo) code:

```
//Find the Login button
FindById("btn-login");
```

And:

```
//Save off the buttons’ parent container, because it is also used for the cancel button
var loginParent = FindByType("LoginModule");
//Find the button containers
var okButtonParent = loginParent.FindByType("div");
//Find the Ok button
var okButton = okButtonParent .FindAllByType("btn").Where(Text == "Login").Displayed;
```

And this is a best case comparison. On a prior team, in an off-the-shelf application, I had to find unnamed windows inside of unnamed windows, then a sibling of a child to get to our actual customization. It was time-consuming and fragile. The benefit is still there, but it is small and the time factor can be too big of a burden to carry long-term for certain teams. In short, something is better than nothing. But, something small that runs reliably is better than “perfect / complete code coverage” with tests that constantly break, provide false results, and require constant changes.

On that same token, automation efforts should have the explicit goal of keeping the solution as simple as possible. If I can add IDs to controls, I should take the time to add the ID and test against that. It's simpler, easier to read, and easier to debug. As a follow on, creating complex, spaghetti code in your solution makes debugging failures exponentially harder. My general rule is that if I can’t identify why a test failed (whether it’s due to a problem with the test or a defect with the application under test) within ~20min, then the test logic is too complex or logging is inadequate. Likewise, if I can’t run the same test three or more times in a row and get the same results on the same build, then the test is not adequate to be included in continuous integration runs.

Automated testing can provide benefits to the team and business if done correctly. "Doing it right" means the team starts as early as possible, effectively prioritizes the order of the features to automate, and keeps things as simple as possible. With this strategy, automation can be one approach your team uses to ensure a quality end product without spending long cycles on repeating manual tests, playing catch up with automated tests, or not testing at all.
