
Back in the August 2017 issue of MSDN Magazine I wrote an in-depth article on C# 7.0 and its support for tuples (msdn.com/magazine/mt493248). At the time I glossed over the fact that the tuple type introduced with C# 7.0 (internally of type ValueTuple<…>) breaks several guidelines of a well-structured value type, namely:

- Do Not declare fields that are public or protected (instead encapsulate with a property).
- Do Not define mutable value types.
- Do Not create value types larger than 16 bytes in size.

These guidelines have been in place since C# 1.0, and yet here in C# 7.0, they’ve been thrown to the wind to define the System.Value­Tuple<…> data type. Technically, System.ValueTuple<…> is a family of data types of the same name, but of varying arity (specifically, the number of type parameters). What’s so special about this particular data type that these long-respected guidelines no longer apply? And how can our understanding of the circumstances in which these guidelines apply—or don’t apply—help us refine their application to defining value types?

Let’s start the discussion with a focus on encapsulation and the benefits of properties versus fields. Consider, for example, an Arc value type representing a portion of the circumference of a circle. It’s defined by the radius of the circle, the start angle (in degrees) of the first point in the arc, and the sweep angle (in degrees) of the last point in the arc, as shown in **Figure 1**.

**Figure 1 Defining an Arc**

public struct Arc
{
  public Arc (double radius, double startAngle, double sweepAngle)
  {
    Radius = radius;
    StartAngle = startAngle;
    SweepAngle = sweepAngle;
  }

  public double Radius;
  public double StartAngle;
  public double SweepAngle;

  public double Length
  {
    get
    {
      return Math.Abs(StartAngle - SweepAngle)
        / 360 \* 2 \* Math.PI \* Radius;
    }
  }

  public void Rotate(double degrees)
  {
    StartAngle += degrees;
    SweepAngle += degrees;
  }

  // Override object.Equals
  public override bool Equals(object obj)
  {
    return (obj is Arc)
      && Equals((Arc)obj);
  }

        // Implemented IEquitable<T>
  public bool Equals(Arc arc)
  {
    return (Radius, StartAngle, SweepAngle).Equals(
      (arc.Radius, arc.StartAngle, arc.SweepAngle));
  }

  // Override object.GetHashCode
  public override int GetHashCode() =>
    return (Radius, StartAngle, SweepAngle).GetHashCode();

  public static bool operator ==(Arc lhs, Arc rhs) =>
    lhs.Equals(rhs);

  public static bool operator !=(Arc lhs, Arc rhs) =>
    !lhs.Equals(rhs);
}

### Do Not Declare Fields That Are Public or Protected

In this declaration, Arc is a value type (defined using the keyword struct) with three public fields that define the characteristics of the Arc. Yes, I could’ve used properties, but I chose to use public fields in this example specifically because it violates the first guideline—Do Not declare fields that are public or protected.

By leveraging public fields rather than properties, the Arc definition lacks the most basic of object-oriented design principles—­encapsulation. For example, what if I decided to change the internal data structure to use the radius, start angle and arc length, for example, rather than the sweep angle? Doing so would obviously break the interface for Arc and all clients would be forced to make a code change.

Similarly, with the definitions of Radius, StartAngle and Sweep­Angle, I have no validation. Radius, for example, could be assigned a negative value. And while negative values for StartAngle and SweepAngle might be allowable, a value greater than 360 degrees wouldn’t. Unfortunately, because Arc is defined using public fields, there’s no way to add validation to protect against these values. Yes, I could add validation in version 2 by changing the fields to properties, but doing so would break the version compatibility of the Arc structure. Any existing compiled code that invoked the fields would break at runtime, as would any code (even if recompiled) that passes the field as a by ref parameter.

Given the guideline that fields should not be public or protected, it’s worth noting that properties, especially with default values, became easier to define than explicit fields encapsulated by properties, thanks to support in C# 6.0 for property initializers. For example, this code:

public double SweepAngle { get; set; } = 180;

is simpler than this:

private double \_SweepAngle = 180;

public double SweepAngle {
  get { return \_SweepAngle; }
  set { \_SweepAngle = value; }
}

The property initializer support is important because, without it, an automatically implemented property that needs initialization would need an accompanying constructor. As a result, the guideline: “Consider automatically implemented properties over fields” (even private fields) makes sense, both because the code is more concise and because you can no longer modify fields from outside their containing property. All this favors yet another guideline, “Avoid accessing fields from outside their containing properties,” which emphasizes the earlier-described data encapsulation principle even from other class members.

At this point lets return to the C# 7.0 tuple type ValueTuple<…>. Despite the guideline about exposed fields, ValueTuple <T1, T2>, for example, is defined as follows:

public struct ValueTuple<T1, T2>
  : IComparable<ValueTuple<T1, T2>>, ...
{
  public T1 Item1;
  public T2 Item2;
  // ...
}

What makes ValueTuple<…> special? Unlike most data structures, the C# 7.0 tuple, henceforth referred to as tuple, was not about the whole object (such as a Person or CardDeck object). Rather, it was about the individual parts grouped arbitrarily for transportation purposes, so they could be returned from a method without the bother of using out or ref parameters. Mads Torgersen uses the analogy of a bunch of people who happen to be on the same bus—where the bus is like a tuple and the people are like the items in the tuple. The Items are grouped together in a return tuple parameter because they are all destined to return to the caller, not because they necessarily have any other association to each other. In fact, it’s likely that the caller will then retrieve the values from the tuple and work with them individually rather than as a unit.

The importance of the individual items rather than the whole makes the concept of encapsulation less compelling. Given that items in a tuple can be wholly unrelated to each other, there’s often no need to encapsulate them in such a manner that changing Item1, for example, might affect Item2. (By contrast, changing the Arc length would require a change in one or both of the angles so encapsulation is a must.) Furthermore, there are no invalid values for the items stored within a tuple. Any validation would be enforced in the data type of the item itself, not in the assignment of one of the Item properties on the tuple.

For this reason, properties on the tuple don’t provide any value, and there’s no conceivable future value they could provide. In short, if you’re going to define a type whose data is mutable with no need for validation, you may as well use fields. Another reason you might want to leverage properties is to have varying accessibility between the getter and the setter. However, assuming mutability is acceptable, you aren’t going to take advantage of properties with differing getter/setter accessibility, either. This all raises another question—should the tuple type be mutable?

### Do Not Define Mutable Value Types

The next guideline to consider is that of the mutable value type. Once again, the Arc example (shown in the code in **Figure 2**) violates the guideline. It’s obvious if you think about it—a value type passes a copy, so changing the copy will not be observable from the caller. However, while the code in **Figure 2** demonstrates the concept of only modifying the copy, the readability of the code does not. From a readability perspective, it would seem the arc changes.

**Figure 2 Value Types Are Copied So The Caller Doesn’t Observe the Change**

\[TestMethod\]
public void PassByValue\_Modify\_ChangeIsLost()
{
  void Modify(Arc paramameter) { paramameter.Radius++; }
  Arc arc = new Arc(42, 0, 90);
  Modify(arc);
  Assert.AreEqual<double>(42, arc.Radius);
}

What’s confusing is that in order for a developer to expect value copy behavior, they would have to know that Arc was a value type. However, there’s nothing obvious from the source code that indicates the value type behavior (though to be fair, the Visual Studio IDE will show a value type as a struct if you hover over the data type). You could perhaps argue that C# programmers should know value type versus reference type semantics, such that the behavior in **Figure 2** is expected. However, consider the scenario in **Figure 3** when the copy behavior is not so obvious.

**Figure 3 Mutable Value Types Behave Unexpectedly**

public class PieShape
{
  public Point Center { get; }
  public Arc Arc { get; }

  public PieShape(Arc arc, Point center = default)
  {
    Arc = arc;
    Center = center;
  }
}

public class PieShapeTests
{
  \[TestMethod\]
  public void Rotate\_GivenArcOnPie\_Fails()
  {
    PieShape pie = new PieShape(new Arc(42, 0, 90));
    Assert.AreEqual<double>(90, pie.Arc.SweepAngle);
    pie.Arc.Rotate(42);
    Assert.AreEqual<double>(90, pie.Arc.SweepAngle);
  }
}

Notice that, in spite of invocation Arc’s Rotate function, the Arc, in fact, never rotates. Why? This confusing behavior is due to the combination of two factors. First, Arc is a value type that causes it to be passed by value rather than by reference. As a result, invoking pie.Arc returns a copy of Arc, rather than returning the same instance of Arc that was instantiated in the constructor. This wouldn’t be a problem, if it wasn’t for the second factor. The invocation of Rotate is intended to modify the instance of Arc stored within pie, but in actuality, it modifies the copy returned from the Arc property. And that’s why we have the guideline, “Do Not define mutable value types.”

As before, tuples in C# 7.0 ignore this guideline and exposes public fields that, by definition, make ValueTuple<…> mutable. Despite this violation, ValueTuple<…> doesn’t suffer the same drawbacks as Arc. The reason is that the only way to modify the tuple is via the Item field. However, the C# compiler doesn’t allow the modification of a field (or property) returned from a containing type (whether the containing type is a reference type, value type or even an array or other type of collection). For example, the following code will not compile:

pie.Arc.Radius = 0;

Nor will this code:

pie.Arc.Radius++;

These statements fail with the message, “Error CS1612: Cannot modify the return value of ‘PieShape.Arc’ because it is not a variable.” In other words, the guideline is not necessarily accurate. Rather than avoiding all mutable value types, the key is to avoid mutating functions (read/write properties are allowable). That wisdom, of course, assumes that the value semantics shown in **Figure 2** are obvious enough such that the intrinsic value type behavior is expected.

### Do Not Create Value Types Larger Than 16 Bytes

This guideline is needed because of how often the value type is copied. In fact, with the exception of a ref or out parameter, value types are copied virtually every time they’re accessed. This is true whether assigning one value type instance to another (such as Arc = arc in **Figure 3**) or a method invocation (such as Modify(arc) shown in **Figure 2**). For performance reasons, the guideline is to keep value type size small.

The reality is that the size of a ValueTuple<…> can often be larger than 128 bits (16 bytes) because a ValueTuple<…> may contain seven individual items (and even more if you specify another tuple for the eighth type parameter). Why, then, is the C# 7.0 tuple defined as a value type?

As mentioned earlier, the tuple was introduced as a language feature to enable multiple return values without the complex syntax required by out or ref parameters. The general pattern, then, was to construct and return a tuple and then deconstruct it back at the caller. In fact, passing a tuple down the stack via a return parameter is similar to passing a group of arguments up the stack for a method call. In other words, return tuples are symmetric with individual parameter lists as far as memory copies are concerned.

If you declared the tuple as a reference type, then it would be necessary to construct the type on the heap and initialize it with the Item values—potentially copying either a value or reference to the heap. Either way, a memory copy operation is required, similar to that of a value type’s memory copy. Furthermore, at some later point in time when the reference tuple is no longer accessible, the garbage collector will need to recover the memory. In other words, a reference tuple still involves memory copying, as well as additional pressure on the garbage collector, making a value type tuple the more efficient option. (In the rare cases that a value tuple isn’t more efficient, you could still resort to the reference type version, Tuple<…>.)

While completely orthogonal to the main topic of the article, notice the implementation of Equals and GetHashCode in **Figure 1**. You can see how tuples provide a shortcut for implementing Equals and GetHashCode. For more information, see “[Using Tuples to Override Equality and GetHashCode](/overidingobjectusingtuple/).”

### Wrapping Up

At first glance it can seem surprising for tuples to be defined as immutable value types. After all, the number of immutable value types found in .NET Core and the .NET Framework is minimal, and there are long-standing programming guidelines that call for value types to be immutable and encapsulated with properties. There’s also the influence of the immutable-by-default approach characteristic to F#, which pressured C# language designers to provide a shorthand to either declare immutable variables or define immutable types. (While no such shorthand is currently under consideration for C# 8.0, read-only structs were added to C#7.2 as a means to verify that a struct was immutable.)

However, when you delve into the details, you see a number of important factors. These include:

- Reference types impose an additional performance impact with garbage collection.
- Tuples are generally ephemeral.
- Tuple items have no foreseeable need for encapsulation with properties.
- Even tuples that are large (by value type guidelines) don’t have significant memory copy operations beyond that of a reference tuple implementation.

In summary, there are plenty of factors that favor a value type tuple with public fields in spite of the standard guidelines. In the end, guidelines are just that, guidelines. Don’t ignore them, but given sufficient—and I would suggest, explicitly documented—cause, it’s OK to color outside the lines on occasion.

For more information on the guidelines for both defining value types and overriding Equals and GetHashCode, check out chapters 9 and 10 in my Essential C# book: “Essential C# 7.0” ([IntelliTect.com/EssentialCSharp](/EssentialCSharp/)), which is expected to be out in May.

_This article was originally posted [here](https://docs.microsoft.com/en-us/archive/msdn-magazine/2018/june/csharp-tuple-trouble-why-csharp-tuples-get-to-break-the-guidelines) in the June 2018 issue of MSDN Magazine._
