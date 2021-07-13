

I love [unit tests](https://intellitect.com/improving-unit-tests-with-automocker/). Specifically, I love unit tests that are easy to maintain. When I spend more time setting up a test then doing actual testing, it is a very strong code smell. Sloppy unmaintainable code has no place in your projects, especially not in your unit test project.  
   
In this post I would like to present two classes that I have found very helpful in keeping my unit tests simple and easy to maintain.  
   
   
**Testing INotifyPropertyChanged**  
   
In any MVVM project, it is important to unit test your view model classes. Quite often, you will want to verify that some interaction raises the [PropertyChanged event](https://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged(v=vs.110).aspx). Often times this results in code that looks like this:

```
public async Task LoadDataAsync()
{
    IsLoading = true;
    var data = await \_dataService.LoadData();
    //TODO: use data
    IsLoading = false;
}

\[TestMethod\]
public async Task IsLoadingIsSetWhileLoadingData()
{
    //Arrange
    var dataService = new Mock();
    var viewModel = new ViewModel( dataService.Object );
    var isLoadingValues = new List();
    viewModel.PropertyChanged += ( s, e ) =>
    {
        if ( e.PropertyName == nameof( ViewModel.IsLoading ) )
        	isLoadingValues.Add( viewModel.IsLoading );
    };
 
    //Act
    await viewModel.LoadDataAsync();
 
    //Assert
    CollectionAssert.AreEqual( new\[\] { true, false }, isLoadingValues );
}
```

This works but can quickly get repetitive as we test more properties we want to test. What we need is something to encapsulate all of the boilerplate so we can focus on writing good unit tests.  
   
This is how the unit test could be written:

```
\[TestMethod\]
public async Task IsLoadingIsSetWhileLoadingData()
{
    //Arrange
    var dataService = new Mock();
    var viewModel = new ViewModel( dataService.Object );
    var isLoadingChanges = viewModel.WatchPropertyChanges( nameof( ViewModel.IsLoading ) );
       
    //Act
    await viewModel.LoadDataAsync();
       
    //Assert
    CollectionAssert.AreEqual( new\[\] { true, false }, isLoadingChanges.ToList() );
}
```

Now all we need to do is implement the the WatchPropertyChanges extension method.

```
public static IPropertyChanges WatchPropertyChanges(
this INotifyPropertyChanged propertyChanged, string propertyName )
{
    if ( propertyChanged == null ) throw new ArgumentNullException( nameof( propertyChanged ) );
    if ( propertyName == null ) throw new ArgumentNullException( nameof( propertyName ) );
 
    return new PropertyChangedEnumerable( propertyChanged, propertyName );
}
```

Here is the implementation of PropertyChangedEnumerable.

```
private readonly List \_values = new List();
private readonly Func \_getPropertyValue;
private readonly string \_propertyName;
        	
 
public PropertyChangedEnumerable( INotifyPropertyChanged propertyChanged, string propertyName )
{
    \_propertyName = propertyName;
 
    const BindingFlags flags = BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public;
    var propertyInfo = propertyChanged.GetType().GetProperty( propertyName, flags );
    if ( propertyInfo == null ) throw new ArgumentException( $"Could not find public property getter for {propertyName} on {propertyChanged.GetType().FullName}" );
 
    var instance = Expression.Constant( propertyChanged );
    var propertyExpression = Expression.Property( instance, propertyInfo );
    \_getPropertyValue = Expression.Lambda<Func>( propertyExpression ).Compile();
 
    propertyChanged.PropertyChanged += OnPropertyChanged;
}
 
private void OnPropertyChanged( object sender, PropertyChangedEventArgs e )
{
    if ( string.Equals( \_propertyName, e.PropertyName, StringComparison.Ordinal ) )
    {
        var value = \_getPropertyValue();
        \_values.Add( value );
    }
}
 
public IEnumerator GetEnumerator()
{
    return \_values.GetEnumerator();
}
```

The bulk of the work is performed in the constructor. It builds up a [Func](https://msdn.microsoft.com/en-us/library/bb534960(v=vs.110).aspx), using reflection and [expression trees](https://msdn.microsoft.com/en-us/library/mt654263.aspx), that access the property’s getter, and when the property changed event is raised for the property it stores the value. This allows us to focus on the important parts of the unit test and simply retrieve an enumerable.  
The complete implementation can be found [here](https://github.com/Keboo/UnitTestHelpers/blob/master/UnitTestHelpers.Tests/PropertyChangedHelper.cs). It has additional functionality for waiting until particular changes occur. You can see usages in the [tests](https://github.com/Keboo/UnitTestHelpers/blob/master/UnitTestHelpers.Tests/PropertyChangedTests.cs).  
   
   
**Testing Events**  
   
We can expand the above property changed event listener to be used for any event not just INPC events. We can rewrite the unit test above to be:

```
\[TestMethod\]
public async Task CanWatchPropertyChangedEvents()
{
    //Arrange
    var dataService = new Mock();
    var viewModel = new ViewModel( dataService.Object );
    EventHelper.Event propertyChangedEvent = viewModel.WatchEvent( nameof( ViewModel.PropertyChanged ) );
 
    //Act
    await viewModel.LoadDataAsync();
 
    //Assert
    Assert.IsTrue( propertyChangedEvent.Raised );
    Assert.AreEqual( 2, propertyChangedEvent.Invocations.Count );
}
```

Once again we wrap the core functionality in a simple extension method that takes in the name of the event that we care about.

```
public static Event WatchEvent(this T @object, string eventName) where T : class
{
    if (@object == null) throw new ArgumentNullException(nameof(@object));
    if (eventName == null) throw new ArgumentNullException(nameof(eventName));
        	
    return WatchEvent((object)@object, eventName);
}
```

The Event object that it returns simply collects the parameters (sender and event args in most cases) that are passed when the event is raised. The WatchEvent overload is where the bulk of the work is done.

```
private static Event WatchEvent( object source, string eventName )
{
    var eventInfo = typeof( T ).GetEvent( eventName ) ?? source?.GetType().GetEvent( eventName );
    if ( eventInfo == null ) throw new ArgumentException( $"Could not find event {eventName} on {typeof( T ).FullName}" );
 
    var invokeMethod = eventInfo.EventHandlerType.GetMethod( nameof( EventHandler.Invoke ) );
    if ( invokeMethod == null ) throw new MissingMethodException( eventInfo.EventHandlerType.FullName, nameof( EventHandler.Invoke ) );
 
    var eventInvokedMethod = typeof( Event ).GetMethod( nameof( Event.EventInvoked ) );
 
    var rv = new Event();
    var instance = Expression.Constant( rv );
    var parameters = invokeMethod.GetParameters().Select( x => Expression.Parameter( x.ParameterType ) ).ToList();
    var convertedParams = parameters.Select( x => Expression.Convert( x, typeof( object ) ) );
    var array = Expression.NewArrayInit( typeof( object ), convertedParams );
    var methodInvoke = Expression.Call( instance, eventInvokedMethod, array );
    var eventHandler = Expression.Lambda( eventInfo.EventHandlerType, methodInvoke, parameters );
    //This expression is roughly equivalent to:
    //event += (p1, p2, ..., pn) => rv.EventInvoked(new\[\] {(object)p1, (object)p2, ..., (object)pn});
    eventInfo.AddEventHandler( source, eventHandler.Compile() );
 
    return rv;
}
```

Once again, it builds up a delegate using reflection and expression trees. In this case the delegate simply forwards the arguments passed to the Event object.  
The complete implementation can be found [here](https://github.com/Keboo/UnitTestHelpers/blob/master/UnitTestHelpers.Tests/EventHelper.cs).  
   
   
In both of these cases the boilerplate code for watching events or property changes is reduced to a single line of code. Making it easy to perform validation, while keeping the unit test easy to maintain.
