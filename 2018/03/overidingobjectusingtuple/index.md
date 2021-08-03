

The implementation of Equals() and GetHashCode() used to be complex, but with C# 7.0 Tuples, the actual code is boilerplate.

- For `Equals():` It’s necessary to compare all the contained identifying data structures while avoiding infinite recursion or null reference exceptions.
- For `GetHashCode():` It’s necessary to combine the unique hash code of each of the non-null contained identifying data structures in an exclusive OR operation.

With C# 7.0 tuples, overiding Equals() and GetHashCode() turns out to be quite simple as demonstrated in **Figure 1**.

```csharp
public struct Arc {
  public Arc(double radius, double startAngle, double sweepAngle) {
    Radius = radius;
    StartAngle = startAngle;
    SweepAngle = sweepAngle;
  }
  public double Radius;
  public double StartAngle;
  public double SweepAngle;
  public double Length {
    get {
      return Math.Abs(StartAngle - SweepAngle) /
        360\ * 2\ * Math.PI\ * Radius;
    }
  }
  public void Rotate(double degrees) {
    StartAngle += degrees;
    SweepAngle += degrees;
  }
  // override object.Equals
  public override bool Equals(object obj) {
    return (obj is Arc) &&
      Equals((Arc) obj);
  }
  // Implemented IEquitable<T>
  public bool Equals(Arc arc) {
    return (Radius, StartAngle, SweepAngle).Equals(
      (arc.Radius, arc.StartAngle, arc.SweepAngle));
  }
  // override object.GetHashCode
  public override int GetHashCode() =>
    (Radius, StartAngle, SweepAngle).GetHashCode();
  public static bool operator == (
    Arc lhs, Arc rhs) => lhs.Equals(rhs);
  public static bool operator != (
    Arc lhs, Arc rhs) => !lhs.Equals(rhs);
}
```

For Equals, one member can check that the type is the same, while a second member groups each of the identifying members into a tuple and compares them to the target parameter of the same type, like this:

```csharp
public bool Equals(Arc arc) =>

return (Radius, StartAngle, SweepAngle).Equals(

(arc.Radius, arc.StartAngle, arc.SweepAngle));
```

One might argue that the second function could be more readable if each identifying member were explicitly compared, instead. But, I leave that for the reader to arbitrate. That said, note that internally the tuple (`System.ValueTuple<…>`) uses `EqualityComparer<T>`, which relies on the type parameters implementation of `IEquatable<T>` (which only contains a single `Equals<T>(T other)` member).

Therefore, to correctly override Equals, you need to follow the guideline:

#### **DO** implement **`IEquatable<T>`** when **overriding Equals.**

That way your own custom data types will leverage your custom implementation of `Equals()` rather than `Object.Equals()`.

Perhaps the more compelling of the two overloads is `GetHashCode()` and its use of the tuple. Rather than engage in the complex gymnastics of an exclusive OR operation of the non-null identifying members, you can simply instantiate a tuple of all identifying members and return the `GetHashCode()` value for the said tuple.  Like so:

public override int GetHashCode() => return (Radius, StartAngle, SweepAngle).GetHashCode();

In summary, C# 7.0's new tuple type provides a great shortcut to overriding `object`'s `Equals()` and `GetHashCode()` members.
