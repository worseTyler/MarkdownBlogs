## C# Compiler Optimization
#
It is commonly shown that a foreach loop compiles into the CIL (Common Intermediate Language) equivalent of the enmerator pattern.  In other words, the code snippet shown here:

```csharp
ArrayList array = new ArrayList();
...
foreach (int i in array)
{
    Console.WriteLine(i);
}
```

Will end compile into the CIL equivalent of this:

```csharp
ArrayList array;int   number;
IEnumerator enumerator;
IDisposable disposable;
array = new ArrayList();
...
enumerator = array.GetEnumerator();
try
{
    while (enumerator.MoveNext())
    {
        number = ((int)enumerator.Current);
        Console.WriteLine(number);
    }
}
finally
{
    disposable = (enumerator as IDisposable);
    if (disposable != null)
    {
        disposable.Dispose();
    }
}
```

What is not often shown, however, is that if a C# array is used in place of the collection class (ArrayList in the above code) like this snippet here:

```csharp
int[] array =
array = new int[]{1, 2, 3, 4, 5, 6};
foreach   (int i in array)
{
    Console.WriteLine(i);
}
```
Then the C# compiler will create the CIL equivalent of the following:

```csharp
int number;
int[] tempArray;
int[] array = new int[]{1, 2, 3, 4, 5, 6};
tempArray = array;
for   (int counter = 0; (counter < tempArray.Length); counter++)
{
    Console.WriteLine(tempArray[counter]);
}
```

In other words, the C# compiler optimizes the foreach loop based on the data that is being iterated over.  One can imagine that another optimization will be made once generics are available such that the enumerator pattern shown above will avoid the extra cast assuming that the collection supports IEnumerator<T>:

```csharp
IEnumerator<int enumerator = ...
...
while (enumerator.MoveNext())
{
    number = /* NO CAST */ enumerator.Current;
    Console.WriteLine(number);
}
...
```
