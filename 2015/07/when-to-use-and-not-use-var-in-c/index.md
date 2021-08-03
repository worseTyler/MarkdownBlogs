

## Settling the Debate Surrounding `var` and C#

Estimated reading time: 4 minutes

Many languages, particularly scripting languages, have a loosely typed variable type named var. In these languages, var can hold any type of data. If you place a number into a var then it will be interpreted as a number whenever possible. If you enter text it will be interpreted as a string, etc. ‘var’s can even hold various objects and will behave properly.

As you probably already know, C# has supported the variable type var since version 3.0. Ever since, the debate has raged on: you should always use var; you should never use var. There are arguments for both sides that sound good, as we’ll see below. What I will say is that it depends. I propose that there are places to use var and places not to use var.

One important point to remember with C#, however, is that var is _strongly typed_. Once a var is declared it can only be of the type with which it was initialized. And a var must be initialized in order to be declared.

### Some Arguments for Variable Type `var`

- var requires less typing. It also is shorter and easier to read, for instance, than Dictionary<int,IList<string>>.
- var requires less code changes if the return type of a method call changes. You only have to change the method declaration, not every place it’s used.
- var encourages you to use a descriptive name for the variable. This means the instance, not the type name. For instance:
    - var customer = new Customer() rather than var c = new Customer().

### Some Arguments Against Variable Type `var`

- var obscures the actual variable type. If the initializer doesn’t return a clearly defined type then you may not be able to tell the variable’s type.
- Using var is lazy. While var is certainly easier to type than Dictionary<int,IList<string>>, if the variable isn’t named well, you’d never know what it refers to.
- Using var makes it hard to know what type the underlying variable actually is. Again, a properly named variable speaks for itself.
- var can’t contain nullable types such as int?. This is actually untrue as you can cast a value to a nullable type
    - var nullableInt = (int?)null;

### How I Use `var`

Although I agree with some of the arguments above, I have fairly specific rules that I use to determine whether I will use var or specify the type literally.

I use var any time that the initialization of the variable clearly tells me what the variable will contain.

```csharp
var count = 17;
var primeNumbers = new [] { 1, 3, 5, 7, 11, 13, 17 };
var customer = new Customer();
var activeOrders = GetAllOrders().Where(o => o.Active);
foreach (var activeOrder in activeOrders) { … }
```

Note that in all of these cases, the variable names are descriptive _and_ the initializer is clear. I also pluralize enumerations and arrays.

Cases where I do not use var, even though I still name the variable descriptively, are when the initializer is not clear.

```csharp
decimal customerBalance = GetCustomerBalance();
CustomerStatus customerStatus = GetCustomerStatus();
```

I declare customerBalance as decimal to know its type for clarity. Reasonable alternatives might include double or even _int_ or _long_. The point is, I don’t know by looking at the code.

I declare _customerStatus_ as the Enum that it is. This makes it clear there are a limited number of possible values that can be referenced or tested by name.

Michael Brennan, in his blog post [Why You Should Always Use the 'var' Keyword in C#](https://blog.michaelbrennan.net/2010/06/why-you-should-always-use-var-keyword.html), makes some compelling points. I recommend it for further reading. However, I prefer the clarity of specifying otherwise obscure types just to make things as clear as possible to the reader who may have to maintain my code in the future.

### Want More?

Curious about how else variables are utilized? Check out our blog _[Painless Bug Testing through the Isolation of Variables](/bug-testing-isolation-variables/)_!

![](https://intellitect.com/wp-content/uploads/2021/04/blog-job-ad-2-1024x129.png)
