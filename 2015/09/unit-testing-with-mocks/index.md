## Unit Testing With Mocks 
#
Unit testing is an integral part of the development process. However, when a class has several dependencies it can be tiring and cumbersome to create mock implementations for all of them. Leveraging a mocking library can save large amounts of time and make for better unit tests. Though there are many mocking libraries available for C#, I will be using my personal favorite [Moq](https://github.com/Moq/moq4) v4.2.x (pronounced “mock” or “mock-you”).

Consider the following code as an example:

```csharp
public interface IDialogService
{
   bool ShowMessage( string message );
}
```

```csharp
public interface IMessenger
{
   void Send(string message);
}

```

```csharp
public class ViewModel
{
   private readonly IDialogService _DialogService;
   private readonly IMessenger _Messenger;

   public ViewModel( IDialogService dialogService, IMessenger messenger )
   {
      _DialogService = dialogService;
      _Messenger = messenger;
   }

   public void Execute()
   {
      if ( _DialogService.ShowMessage( "Agree to continue?"" ) )
      {
         _Messenger.Send("Success message");
      }
   }
}
```

We have a simple ViewModel class that takes dependencies on two interfaces IDialogService and IMessenger. This is a pattern called [dependency injection](https://en.wikipedia.org/wiki/Dependency_injection). Code following this pattern is very easy to unit test by substituting [mock objects](https://en.wikipedia.org/wiki/Mock_object) for the dependencies.

In this case, we need to test our Execute method. There are two possible execution paths through the method, so we will have two unit tests (I favor using the [AAA pattern](https://www.c2.com/cgi/wiki?ArrangeActAssert) for unit tests).

```csharp
[TestMethod]
public void WhenUserAgreesToContinueItSendsMessage()
{
   //Arrange
   var dialogServiceMock = new Mock<IDialogService>();
   var messengerMock = new Mock<IMessenger>();
   var viewModel = new ViewModel( dialogServiceMock.Object, messengerMock.Object );

   dialogServiceMock
      .Setup( x => x.ShowMessage( "Agree to continue?" ) )
      .Returns( true )
      .Verifiable();
   messengerMock
      .Setup( x => x.Send( "Success message" ) )
      .Verifiable();

   //Act
   viewModel.Execute();

   //Assert
   dialogServiceMock.VerifyAll();
   messengerMock.VerifyAll();
}

```

There are two new mock objects that are created to satisfy the view model’s dependencies. Mock objects created in this way are referred to as “loose” mocks. When any method is invoked on the mock object it returns the default value for the method’s return type. This behavior is being overridden in the test using the Setup and Returns methods to provide our own return value when the parameters match those provided in the Setup call.

This test contains two expectations. First, the dialog service method ShowMessage should be invoked with the string “Agree to continue?”. When this occurs, the mock will return true. Finally, we mark this assertion as verifiable so we can assert that this method really did get invoked as we expected. Second, we expect the Send method on the messenger mock to be invoked with “Success message”.

For the actual running of the test the Execute method is invoked.

Finally the only assertions we need to make on the test, is to simply assert that our expectations on our mock objects were met using the VerifyAll method. This will throw an exception (and fail the unit test) if the expectations did not occur.

```csharp
[TestMethod]
public void WhenUserDoesNotAgreeToContinueItDoesNotSendMessage()
{
   //Arrange
   var dialogServiceMock = new Mock<IDialogService>();
   var messengerMock = new Mock<IMessenger>();
   var viewModel = new ViewModel( dialogServiceMock.Object, messengerMock.Object );

   dialogServiceMock
    .Setup( x => x.ShowMessage( It.IsAny<string>() ) )
    .Returns( false );

   //Act
   viewModel.Execute();

   //Assert
   messengerMock.Verify( x => x.Send( It.IsAny<string>() ), Times.Never() );
}
```

The second unit test setup is very similar to the first. The mocks are created with assertions for the expected behavior. However this time the behavior of the dialog service is changed to return false on all calls to ShowMessage.

For the verification step, we need to assert that there were no messages sent. So there is an explicit verification done to ensure that that the Send method was never called with any parameter.

Using mocks in this manner allows you to assert on expected behavior between a class and its dependencies. In a future post we will look at ways to generate the mock object to further improve upon these unit tests.
