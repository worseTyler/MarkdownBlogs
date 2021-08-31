
## What's New in Xamarin.Forms 4
#

Shell, Hot Reload, Hot Restart and Visual are just four of the new features in Xamarin.Forms 4 that will simplify and drastically reduce development time.

Check out my **full talk** on Xamarin at IntelliTect's 2019 event.

<iframe src="https://www.youtube.com/embed/P4SIlhoaIGs" allowfullscreen width="560" height="315"></iframe>

[Xamarin](https://dotnet.microsoft.com/apps/xamarin) is Microsoft's arm for mobile development. It does more than that as well, but that's the main point.

[Xamarin.Forms](https://docs.microsoft.com/en-us/xamarin/get-started/what-is-xamarin-forms) is all about sharing the code across a common UI. Anyone can write a class library. Xamarin.Forms shares the UI code and logic but the UI is native to each platform.

![An infographic on how Xamarin.Forms works taken from https://docs.microsoft.com/en-us/xamarin/get-started/what-is-xamarin-forms.](https://intellitect.com/wp-content/uploads/2019/10/xamarin-forms-architecture.png "Xamarin.Forms 4 and Its Time-saving New Features (Video)")

This [Microsoft](https://docs.microsoft.com/en-us/xamarin/get-started/what-is-xamarin-forms) infographic breaks down how Xamarin.Forms works.

It simplifies your process so you don't have to write the code twice, but it still looks like the right type of app and runs with native performance.

<iframe src="https://www.youtube.com/embed/Ue_DveRz8Tc" allowfullscreen width="560" height="315"></iframe>

How do you get from an XML style layout (XAML) to native code?

Xamarin.Forms uses a XAML framework with built-in renderers that go between native code and your shared UI logic.

There are different levels of functionality that each type of app needs. With Xamarin.Forms you maintain whatever level of control and responsibility that you desire.

With Xamarin.Forms, you can write upwards of 95 percent of your code in a way that it's shareable across-platform.

### Microsoft Learn - A Resource for Learning

Before we begin, perhaps you need a more detailed [overview of Xamarin.Forms 4](https://docs.microsoft.com/en-us/learn/browse/?products=xamarin)? Consider Microsoft Learn's free step-by-step learning paths and modules.

Xamarin University used to be the learning path for developers that wanted to become Xamarin certified developers. Now there's Microsoft Learn.

There are modules for Xamarin.Forms that were taken from Xamarin University as well as other classes that cover a multitude of topics.

### Xamarin.Essentials Library Simplifies Your Cross-platform Experience

What's in the box? What are the features? What's it capable of doing? Find out your answers in the [Xamarin.Essentials](https://docs.microsoft.com/en-us/xamarin/essentials/) library.

The Xamarin.Essentials library smooths over the differences between iOS and Android API and makes it easy to work with.

Now that the basics are covered, let's get to the exciting part!

### What's New in Xamarin.Forms 4?

All the new smaller Xamarin.Forms 4 improvements are about speeding up the development cycle:

- Make code changes
- Compile
- Deploy to device
- Test it
- Fix mistakes
- Do it all again

This cycle takes time, especially on mobile.

Some of the performance problems aren't within Microsoft's control, and that's OK. Xamarin found a way to cut down the development loop.

#### Hot Reload for XAML (Preview)

<iframe src="https://www.youtube.com/embed/52gR_tl2_-o" allowfullscreen width="560" height="315"></iframe>

[Hot Reload](https://devblogs.microsoft.com/xamarin/public-preview-xaml-hot-reload-xamarin-forms/) dramatically cuts the development cycle timeframe.

Now you can make edits and see them occur in your app without stopping the debugging process.

**Design Time Data**

In Xamarin.Forms you can put design time data on anything you want - this is different than most other XAML frameworks.

It doesn't have to be tied to:

- Item source
- Binding context
- Pre-populated items

For those that have worked in Universal Windows Platform (UWP) and Windows Presentation Foundation (WPF), I'm sure you're thinking, "Finally, this is what we've had for a while now!"

And as a result of Hot Reload, these changes are immediate and not just on simulators or emulators.

\*TIP - after you make incremental changes that are hot reloaded, you should relaunch your app because those steps that got you to a result may not be the same as the code that's already present.

Now, you may say, "Wouldn't it be better to edit all my code, not just XAML? What about C#? What about click handlers?"

Do you wish all your changes could deploy automatically?

Soon, they will.

#### Hot Restart (private preview)

<iframe src="https://www.youtube.com/embed/WeL3YlTRCOU" allowfullscreen width="560" height="315"></iframe>

The new [Hot Restart](https://devblogs.microsoft.com/xamarin/xamarin-hot-restart/) feature is similar to the "edit and continue" feature in C#. For anyone that's had to complete a long development process in mobile apps, you know it is going to save you tons of time.

#### Shell

<iframe src="https://www.youtube.com/embed/EDwAG88YgCw" allowfullscreen width="560" height="315"></iframe>

[Shell](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/shell/introduction) solves navigation problems between pages with:

- New navigation bar at the top
- Improved search functionality
- Ability to turn features on and off as needed

#### Visual

<iframe src="https://www.youtube.com/embed/obWl5n0yzzM" allowfullscreen width="560" height="315"></iframe>

[Visual](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/visual/) allows for design that is "Brand" focused not platform-specific.

Material Design is built into the box as Xamarin.Forms material Visual for a consistent look across platforms. There will be some minimal differences that are device-specific, but the Brand will remain the same.

### WPF updates

With .NET Core 3, WPF now has .NET Core support!

<iframe src="https://www.youtube.com/embed/f3D2LUzRbxE" allowfullscreen width="560" height="315"></iframe>

It still isn't cross-platform. But, you now have all the performance improvements of .NET Core.

#### Curious If You Should Use .NET Core in Your WPF App?

- If it's a new app - yes because .NET Core is the future.
- If it's a revision of an old app, maybe not. You'll need to evaluate the cost. It may not be worth it for apps that are only being maintained.

### Want More?

Here's the [link to all the code](https://github.com/IntelliTect-Samples/2019-10-23.VisualStudio2019Launch) for IntelliTect's Visual Studio 2019 event.

Also, consider checking out [MaterialDesignInXAML](https://github.com/ButchersBoy/MaterialDesignInXamlToolkit), a fully open-source toolkit that I maintain to assist the user in crafting desktop applications that allow Windows applications to leverage Google’s Material Design. I cover feature updates and bug fixes as well as mentorship of other devs in a chat-room setting.

Want more XAML? Consider my blogs:

- [5 Steps to Getting Started with Material Design In XAML](https://intellitect.com/getting-started-material-design-in-xaml/)
- [Material Design in XAML – How to make sense of the Dialog Host](https://intellitect.com/material-design-in-xaml-dialog-host/)

Have a question about Xamarin.Forms 4 that I didn't cover? Ask in the comments.
