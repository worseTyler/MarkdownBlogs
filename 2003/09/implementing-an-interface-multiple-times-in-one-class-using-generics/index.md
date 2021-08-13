## Declaring Generic Interfaces With C#
#
With the C# generics implementation that is currently under proposal it will be possible to declare generic interfaces, not just generic classes.  This is really cool as it avoids the ambiguity associated with returning an object such as in the System.Collections.IEnumerable interface.  In place of the non-generic interface we would use the generic System.Collections.Generics.IEnuerable version:

```csharp
public interface IEnumerable<T>
{
IEnumerator<TGetEnumerator();
}
```

Furthermore, it will be possible to implement an interface multiple times within one class.  For example, it should be possible to do the following:

```csharp
public interface IContainer<ItemType>
{
    IEnumerable<ItemTypeItems>();
}

public class Person: IContainer<Address>, IContainer<Phone>, IContainer<Email>
{
    ICollection<AddressIContainer<Address>.Items()
    {
        ...
    }

    ICollection<PhoneIContainer<Phone>.Items()
    {
        ...
    }

    ICollection<EmailIContainer<Email>.Items()
    {
        ...
    }
}
```
If email and phone were both strings this wouldn't be possible as the interface on both would be IContainer<string> but assuming each container holds a different type this works.

Interesting?  I like it....

(By the way, technically, the Person class does not implement the same interface multiple times because IContainer<Address> and IContainer<Phone> are different interfaces.  You cannot even cast between them.)
