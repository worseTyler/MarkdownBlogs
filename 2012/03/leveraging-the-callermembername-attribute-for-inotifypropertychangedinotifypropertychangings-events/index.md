

```csharp
public class Person : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    public Person(string name)
    {
        Name = name;
    }

    private string _Name;
    public string Name
    {
        get
        {
            return _Name;
        }
        set
        {
            if (_Name != value)
            {
                _Name = value;
                OnPropertyChanged();
            }
        }
    }

    private void OnPropertyChanged([CallerMemberName]string property = null)
    {
        PropertyChangedEventHandler propertyChanged = PropertyChanged;
        if (propertyChanged != null)
        {
            propertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }

    // ...
}

[TestClass]
public class PersonTests
{

    [TestMethod]
    public void VerifyThisMethodHasTestMethodAttribute()
    {
        bool called = false;
        Person person = new Person("Inigo Montoya");
        person.PropertyChanged += (sender, eventArgs) =>
        {
            AreEqual("Name", eventArgs.PropertyName);
            called = true;
        };
        person.Name = "Princess Buttercup";
        IsTrue(called);
    }    
}
```