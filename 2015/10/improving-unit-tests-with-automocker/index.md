## Further Discussion on Improving Unit Tests With Automocker
#
In my [previous post](/unit-testing-with-mocks/), I presented an example of using mock objects to improve unit testing. In this post, I would like to expand upon my previous example to make the tests more robust in order to handle signature changes to the ViewModel’s constructor.

First, we will create another dependency to represent a service that returns some data:

```csharp
public interface IDataService
{
   IEnumerable<string> RetrieveData();
}
```

Now we will add a new method to the ViewModel class:

```csharp
public class ViewModel
{
   private readonly IDialogService _DialogService;
   private readonly IMessenger _Messenger;
   private readonly IDataService _DataService;
   
   public ViewModel( IDialogService dialogService, IMessenger messenger, IDataService dataService )
   {
       _DialogService = dialogService;
       _Messenger = messenger;
       _DataService = dataService;
   }
   
   public void Execute()
   {
       if ( _DialogService.ShowMessage( "Agree to continue?" ) )
       {
           _Messenger.Send( "Success message" );
       }
   }
   
   public IEnumerable<string> RetrieveData()
   {
       return _DataService.RetrieveData();
   }
}
```

This dependency change (adding IDataService to the constructor) causes our previous two unit tests to no longer compile, since each test was manually creating each dependency. Since we have not modified the Execute method, we should not need to modify these unit tests. We could consolidate the creation of our ViewModel using a [TestInitializeAttribute](https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.testtools.unittesting.testinitializeattribute.aspx), but this still requires us to manually create each of the dependencies. A better solution is to take advantage of a nuget package that can automatically populate these dependencies. There are several nuget packages that can do this, but for this example I will be using [Moq.AutoMocker](https://www.nuget.org/packages/Moq.AutoMock/0.4.0) v0.4.0.

Using AutoMocker, the arrange portion of our [previous unit tests](/unit-testing-with-mocks/) can be changed from:

```csharp
//Arrange
var dialogServiceMock = new Mock<IDialogService>();
var messengerMock = new Mock<IMessenger>();
var viewModel = new ViewModel( dialogServiceMock.Object, messengerMock.Object );

```

to:

```csharp
//Arrange
var mocker = new AutoMocker();
var viewModel = mocker.CreateInstance<ViewModel>();
var dialogServiceMock = mocker.GetMock<IDialogService>();
var messengerMock = mocker.GetMock<IMessenger>();
```

Now our previous two unit tests are now only coupled to dependencies that are used within the Execute method.

Now we can add a unit test for the RetrieveData method:

```csharp
[TestMethod]
public void WhenRetrievingDataItOnlyCallsDataService()
{
   //Arrange
   var mocker = new AutoMocker();
   var viewModel = mocker.CreateInstance<ViewModel>();
   var dataService = mocker.GetMock<IDataService>();
   
   dataService.Setup( x => x.RetrieveData() ).Returns( new[] { "Data1", "Data2" } ).Verifiable();
   
   //Act
   IEnumerable<string> retrievedData = viewModel.RetrieveData();
   
   //Assert
   CollectionAssert.AreEqual( new[] { "Data1", "Data2" }, retrievedData.ToArray() );
   mocker.VerifyAll();
}

```

This test passes, because it fails to properly assert that no other calls were made on the other dependencies. To address this, we first need to understand how Moq treats calls on Mock<> objects. First, Moq checks to see if the method call matches a Setup() that was done on the Mock<>. If it fails to find a matching Setup() call, it will either return the default value for the method’s return type or it will throw an exception. The former is referred to as a “loose” mock, and the latter a “strict” mock. This behavior can be controlled by passing the appropriate value to Mock<> constructor. When using AutoMocker, you can control how all mocks are created by passing the appropriate enum value to the AutoMocker constructor.

This will update our unit test to be:

```csharp
[TestMethod]
public void WhenRetrievingDataItOnlyCallsDataService()
{
   //Arrange
   var mocker = new AutoMocker(MockBehavior.Strict);
   ...
}
```

If you would prefer to specify your own mock (or actual object) for AutoMocker to use, you can do so by passing the appropriate object to AutoMocker’s Use method.

```csharp
var mocker = new AutoMocker();

var dataServiceMock = new Mock<IDataService>( MockBehavior.Strict );
mocker.Use(dataServiceMock);

IMessenger realMessenger = new Messenger();
mocker.Use(realMessenger);

var viewModel = mocker.CreateInstance<ViewModel>();

```

When AutoMocker creates the ViewModel it will use the specified dependencies and create loose mocks for all other dependencies. In this manner, you can take full control over the dependencies used while still decoupling your test from constructor changes that do not affect your unit test.

Using strict mocks is a great way to assert that only the expected calls were made on your dependencies and nothing more. In addition, AutoMocker can help you remove the coupling between your tests and an object’s dependencies. It can also save time, by not requiring you to fix compiler errors each time your object’s constructor changes.
