
While attending the Whidbey Compiler Development lab earlier this week, Jim Miller posed the following problem:

Given the following two methods, which one should the function Foo(1.2, "one") invoke?

> public static int Foo(T a, V v) { return 1; } public static int Foo(double a, string b) { return 2; }

From the C# perspective this is allowed and it resolves as "expected."

> \[Test\] public void OverloadingWithTypeParameters() { Assert.AreEqual( 1, Foo<double, string\>(1, "one") ); Assert.AreEqual( 2, Foo(1.2, "one") ); }

However, what should the runtime do to resolve such a call?  To complicate matters, what if the call was instead Foo(1, "one"), not a double but an int? Remember that even without method int Foo(double a, string b) declaration, the call Foo(1.2, "one") would resolve using inferencing.

As Jim Miller indicated, ideally such overloading should be considered non-CLS compliant so that cross assembly calls are unlikely to occur and each language can simply make its own determination about what to do.  Unfortunately, however, there are classes in the .NET 2.0 framework that actually expose such an API and Microsoft is obviously reluctant to change .NET 2.0 Framework classes this late in the development (sure tool development changes ...but language or framework is more risk than they are inclined to bear).

Personally, I believe that since Microsoft has not released yet, surely they should bite the bullet and remove the ambiguity from their own code?  It wasn't like this ambiguity was in .NET 1.1.  It doesn't seem quite "fair" (whatever that means in this case) that Microsoft would prevent the preferred solution, disallowing this in CLS code, because of the impact it would have on "fixing" _beta_ code.  Obviously no other language vendors would deserve such consideration (but no one else has the same level of impact obviously).  Regardless of CLS compliance or not, I believe an exact signature match, the non-generic version, should be called.  This allows one to provide a special implementation at compile time for any special handling.
