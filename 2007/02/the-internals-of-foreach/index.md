

## Advantages of a Foreach Loop
#
Many moons ago I discussed the foreach loop.  I expand on that post here as I continue my series for the MSDN C# Developer Center.

The advantage of a foreach loop over a for loop is the fact that it is unnecessary to know the number of items within the collection when an iteration starts. This avoids iterating off the end of the collection using an index that is not available. As Bill Wagner pointed out in his [Custom Iterators](https://msdn2.microsoft.com/en-us/vcsharp/bb264519.aspx) article last week, a foreach loop also allows code to iterate over a collection without first loading the collection in entirety into memory. In this article I am going to explore how the foreach statement works under the covers. This sets the stage for a follow on discussion of how the yield statement works.

### **``` foreach ``` with Arrays**

Consider the foreach code listing shown in Listing 1:

Listing 1: ``` foreach ``` with Arrays

```csharp 
int[] array = new int[]{1, 2, 3, 4, 5, 6}; 
foreach(int item in array) 
{ 
    Console.WriteLine(item); 
} 
```

From this code, the C# compiler creates CIL equivalent of a for loop like this (Listing 2):

Listing 2: Compiled Implementation of foreach with Arrays

```csharp
int[] tempArray;
int[] array = new int[] {
  1,
  2,
  3,
  4,
  5,
  6
};
tempArray = array;
for (int counter = 0;
  (counter<tempArray.Length); counter++) {
  readonly int item = tempArray[counter];
  Console.WriteLine(item);
}
```

Since the collection is an array and indexing the array is fast, the foreach loop implementation relies on support for the Length property and the index operator ([]). However, these two array features are not available on all collections. Many collections do not have a known number of elements and many collection classes do not support retrieving elements by index.

### **foreach with IEnumerable<T>**

To address this, the foreach loop uses the ``` System.Collections.Generic.IEnumerator<T> ```. (Given C# 2.0's generics support, there is very little reason to consider using the non-generic equivalent collections so I will ignore them from this discussion except to say their behavior is almost identical.) ``` IEnumerator<T> ``` is designed to enable the iterator pattern for iterating over collections of elements, rather than the length-index pattern shown in earlier. ``` IEnumerator<T> ``` includes three members. The first is bool ``` MoveNext() ```. Using this method, we can move from one element within the collection to the next while at the same time detecting when we have enumerated through every item using the Boolean return. The second member, a read-only property called Current, returns the element currently in process. With these two members on the collection class, it is possible to iterate over the collection simply using a while loop as demonstrated in the code listing below (Listing 3):

Listing 3: Iterating over a collection using ``` while ```

```csharp
System.Collections.Generic.Stack<int>stack = new System.Collections.Generic.Stack<int>();
int number;
// ...   
// This code is conceptual, not that the actual code.   
while (stack.MoveNext()) {
  number = stack.Current;
  Console.WriteLine(number);
}
```

The ``` MoveNext() ``` method in this listing returns false when it moves past the end of the collection. This replaces the need to count elements while looping. (The last member on ``` IEnumerator<T> ```, ``` Reset() ```, will reset the enumeration.) This while listing shows the gist of the C# compiler output, but it doesn't actually work this way because it omits two important details about the actual implementation: interleaving and error handling.

### **Interleaving**

The problem with is that if there are two (or more) interleaving loops over the same collection, one foreach inside another, then the collection must maintain a state indicator of the current element so that when ``` MoveNext() ``` is called, the next element can be determined. The problem is that one interleaving loop can affect the other. (The same is true of loops executed by multiple threads.)

![C:\Dev\Projects\Books\EssentialC#\12-08.IEnumeratorAndIEnumeratorInterfaces.png](https://intellitect.com/wp-content/uploads/binary/TheInternalsofforeach_D7D/clip_image0022.gif "The Internals of foreach")

Figure 1: ``` IEnumerable ``` class diagram

To overcome this problem, the ``` IEnumerator<T> ``` interfaces are not supported by the collection classes directly. As shown in Figure 1, there is a second interface called ``` IEnumerable<T> ``` whose only method is ``` GetEnumerator() ```. The purpose of this method is to return an object that supports ``` IEnumerator<T> ```. Rather than the collection class maintaining the state itself, a different class, usually a nested class so that it has access to the internals of the collection, will support the ``` IEnumerator<T> ``` interface and keep the state of the iteration loop. Using this pattern, the C# equivalent of a foreach loop will look like what's shown in Listing 4.

Listing 4: A separate enumerator maintains state during an iteration

```csharp
System.Collections.Generic.Stack<int>stack = new System.Collections.Generic.Stack<int>();
int number;
System.Collections.Generic.Stack<int>.IEnumerator<int>enumerator;
// ...   
// If IEnumerable<T> is implemented explicitly,   
// then a cast is required.   
// ((IEnumerable)stack).GetEnumeraor();   
enumerator = stack.GetEnumerator();
while (enumerator.MoveNext()) {
  number = en umerator.Current;
  Console.WriteLine(number);
}
```

### **Error Handling**

Since the classes that implement the ``` IEnumerator<T> ``` interface maintain the state, there are occasions when the state needs cleaning up after all iterations have completed. To achieve this, the ``` IEnumerator<T> ``` interface derives from ``` IDisposable ```. Enumerators that implement ``` IEnumerator ``` do not necessarily implement ``` IDisposable ```, but if they do, ``` Dispose() ``` will be called as well. This enables the calling of ``` Dispose() ``` after the foreach loop exits. The C# equivalent of the final CIL code, therefore, looks like Listing 5.

Listing 5: Compiled Result of foreach on Collections

```csharp
System.Collections.Generic.Stack<intstack> = new System.Collections.Generic.Stack<int>();
int number;
System.Collections.Generic.Stack<int>.Enumerator<int enumerator;
IDisposable disposable;
enumerator = stack.GetEnumerator();

try {
  while (enumerator.MoveNext()) {
    number = enumerator.Current;
    Console.WriteLine(number);
  }
} finally {
  // Explicit cast used for IEnumerator<T>.   
  disposable = (IDisposable) enumerator;
  disposable.Dispose();
  // IEnumerator will use the as operator unless IDisposable   
  // support determinable at compile time.   
  // disposable = (enumerator as IDisposable);   
  // if (disposable != null)   
  // {   
  //    disposable.Dispose();   
  // } 
}
```

Notice that because the ``` IDisposable ``` interface is supported by ``` IEnumerator<T> ```, the C# code can be simplified with the using keyword as shown in Listing 6.

Listing 6: Error handling and resource cleanup with using

```csharp
System.Collections.Generic.Stack<intstack>= new System.Collections.Generic.Stack<int>();
int number;

using(System.Collections.Generic.Stack<int> .Enumerator<int> enumerator = stack.GetEnumerator()) {
  while (enumerator.MoveNext()) {
    number = enumerator.Current;
    Console.WriteLine(number);
  }
}
```

However, recall that the using keyword is not directly supported by CIL either, so in reality the former code is a more accurate C# representation of the foreach CIL code.

Readers may recall that the compiler prevents assignment of the ``` foreach ``` variable identifier (``` number ```). As is demonstrated in Listing 6, an assignment of number would not be a change to the collection element itself so rather than mistakenly assume that, the C# compiler prevents such an assignment altogether.

In addition, the element count within a collection cannot be modified during the execution of a ``` foreach ``` loop. If, for example, we called ``` stack.Push(42) ``` inside the ``` foreach ``` loop, it would be ambiguous whether the iterator should ignore or incorporate the change to stack â€“ should iterator iterate over the newly added item or ignore it and assume the exact same state as when it was instantiated.

Because of this ambiguity, an exception of type ``` System.InvalidOperationException ``` is thrown if the collection is modified within a ``` foreach ``` loop, reporting that the collection was modified after the enumerator was instantiated.

(This content was largely taken from the Collections chapter of my book, [Essential C# 2.0 [Addison-Wesley]](/essentialcsharp7))
