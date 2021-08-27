

## Unit Testing Xamarin Forms
#
In a recent application that IntelliTect developed for a client, we were tasked with building a Xamarin forms application that supported Windows, Android, and iOS.  Due to the cross-platform support inherent with using Xamarin, a large majority of our code was common amongst all three implementations.  As with most cross-platform projects, however, a small amount of code needed to be customized for each individual platform.  While our common code was generally straightforward to unit test, it became clear that we needed a solution for unit testing the platform specific pieces of code that we were writing (in the [PCL](https://developer.xamarin.com/guides/cross-platform/application_fundamentals/pcl/introduction_to_portable_class_libraries/) code). This article will demonstrate the approach we took to tackle this problem.

While the majority of our code was shared, there were certain pieces of code that required special casing for different platforms, or potentially even for different device form factors (phone / tablet).  To do this special casing, Xamarin provides the [Xamarin.Forms.Device](https://developer.xamarin.com/guides/xamarin-forms/platform-features/device/) class that allows you to determine which platform and device idiom you are running on.  The Xamarin.Forms.Device class has several useful methods and properties that we used occasionally throughout our application.  Specifically, in our app we used the following properties and methods:

**Device.Idiom** – Specifies whether the device is a phone or a tablet

**Device.OS** – Specifies whether the device is an iOS, Android, or Windows device

**Device.BeginInvokeOnMainThread** – Used to run code on the UI thread, from a background thread

**Device.OpenURI** – Used to open a URI on the underlying platform. 

Since we used these features in our code, we needed a way to unit test our code, taking into account the different platforms and device types we were running on.  It is worth noting as well, that exercising Device properties and methods from within a unit test without using the method below causes Xamarin exceptions to be thrown because the tests are not running as a Xamarin forms application.

For this particular Xamarin Forms application we were building, we used [moq](https://github.com/moq) for our unit testing needs.  Therefore, we decided to build an interface that we would use in our code, rather than directly using the Xamarin.Forms.Device class.  This way, we could mock the interface, and write unit tests against it.

### **Interface**

The interface we developed looked something like this:

```csharp
public interface IXamarinFormsDevice
{
    void BeginInvokeOnMainThread(Action action);
    TargetPlatform OS { get; }
    TargetIdiom Idiom { get; }
    void OpenUri(Uri uri);
}
﻿
```

### **Wrapper**

We then implemented a wrapper class that implemented this interface.  The wrapper simply forwards calls to Xamarin.Forms.Device.

```csharp
public class FormsDeviceWrapper : IXamarinFormsDevice
{
   public TargetPlatform OS
   {
      get
      {
         return Device.OS;
      }
   }

   public void BeginInvokeOnMainThread(Action action)
   {
      Device.BeginInvokeOnMainThread(action);
   }

   public void OpenUri(Uri uri)
   {
      Device.OpenUri(uri);
   }

   public TargetIdiom Idiom
   {
      get
      {
         return Device.Idiom;
      }
   }
}

```

At this point, you may be wondering why we went through the trouble of creating this interface, when all our interface implementation does is call the Xamarin.Forms.Device implementation.  The answer to this is simple, it gave us a way to unit test any code that uses the Xamarin.Forms.Device class.  In our code, rather than directly using Xamarin.Forms.Device, we used the interface (via dependency injection using [SimpleIOC](https://msdn.microsoft.com/en-us/magazine/jj991965.aspx)), like this:

```csharp
   IXamarinFormsDevice formsDevice = SimpleIoc.Default.GetInstance();
```

We could then use the interface in our code, like this:

```csharp
   if(formsDevice.OS == TargetPlatform.iOS)
   {
      // do something iOS specific
   }
```

### **Example**

Let’s take a look at some sample code to show how we can unit test our code effectively, now that our code is using our new interface, rather than Xamarin.Forms.Device directly.  For this contrived example, let’s assume that we have a label on our Xamarin forms page, in which we want to show a different message for each device type:

```csharp
public static string GetLabel(IXamarinFormsDevice formsDevice)
{
   switch (formsDevice.OS)
   {
     case TargetPlatform.iOS:
       return "Hello iOS!";

     case TargetPlatform.Android:
       return "Hello Android!";

     case TargetPlatform.Windows:
       return "Hello Windows!";

     case TargetPlatform.WinPhone:
       return "Hello Windows Phone!";

     default:
       return "Unknown platform";
   }
}

 public void PrintLabel()
 {
     IXamarinFormsDevice formsDevice = SimpleIoc.Default.GetInstance();
     string labelText = GetLabel(formsDevice);
     Console.WriteLine(labelText);
 }
```

### **Tests**

Using moq, it is now quite easy to write a test for this method:

```csharp
[TestMethod]
public void GetLabel_iOS()
{
   Mock mockFormsDevice = new Mock();
   mockFormsDevice.SetupGet(x => x.OS).Returns(() => TargetPlatform.iOS);
   string label = LabelTest.GetLabel(mockFormsDevice.Object);
   Assert.AreEqual('Hello iOS!', label);
}

[TestMethod]
public void GetLabel_Android()
{
   Mock mockFormsDevice = new Mock();
   mockFormsDevice.SetupGet(x => x.OS).Returns(() => TargetPlatform.Android);
   string label = LabelTest.GetLabel(mockFormsDevice.Object);
   Assert.AreEqual('Hello Android!', label);
}

[TestMethod]
public void GetLabel_Windows()
{
   Mock<IXamarinFormsDevice> mockFormsDevice = new Mock<IXamarinFormsDevice>();
   mockFormsDevice.SetupGet(x => x.OS).Returns(() => TargetPlatform.Windows);
   string label = LabelTest.GetLabel(mockFormsDevice.Object);
   Assert.AreEqual("Hello Windows!", label);
}

[TestMethod]
public void GetLabel_WindowsPhone()
{
   Mock<IXamarinFormsDevice> mockFormsDevice = new Mock<IXamarinFormsDevice>();
   mockFormsDevice.SetupGet(x => x.OS).Returns(() => TargetPlatform.WinPhone);
   string label = LabelTest.GetLabel(mockFormsDevice.Object);
   Assert.AreEqual("Hello Windows Phone!", label);
}

```

That’s all there is to it, we have successfully created unit tests for our platform specific implementation.

_Written by Jason Peterson_
