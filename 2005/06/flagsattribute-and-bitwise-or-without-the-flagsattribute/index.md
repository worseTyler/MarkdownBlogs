
I read recently in the remarks on the FlagsAttribute of the MSDN documentation that,

> "Bit fields can be combined using a bitwise OR operation, whereas enumerated constants cannot."

My interpretation of enumerated constants is, well, enumerations without the FlagsAttribute.  And if that is the case, then the documentation doesn't seem quite right.  I threw together a test to verify one way or the other:

> \[TestClass\] class EnumTest { enum NoFlagAttribute { Zero, One, Two, Three, Four, Five }
> 
> \[TestMethod\] public void BitwiseORWithNoFlagAttribute() { Assert.AreEqual(NoFlagAttribute.One, (NoFlagAttribute.Zero | NoFlagAttribute.One)); Assert.AreEqual((NoFlagAttribute)7, (NoFlagAttribute.Zero | NoFlagAttribute.One | NoFlagAttribute.Two | NoFlagAttribute.Three | NoFlagAttribute.Four | NoFlagAttribute.Five)); } }

As the test demonstrates, using the a bitwise OR operator on an enumeration without the FlagsAttribute decoration works just fine.

The next question, however, is what exactly does the FlagsAttribute do?  It essentially changes the ToString() behavior on the enumeration instance.  Calling ToString() on an enumeration that is decorated with the FlagsAttribute writes out the strings for each enumerator flag that is set.  Consider the following example:

> using System.IO;
> 
> class Program { public static void Main() { string fileName \= @"enumtest.txt"; FileInfo file \= new FileInfo(fileName); file.Attributes \= FileAttributes.Hidden | FileAttributes.ReadOnly;
> 
> Trace.Assert( "ReadOnly, Hidden" == file.Attributes.ToString()); Console.WriteLine("{0} = {1}", file.Attributes.ToString().Replace(",", " |"), file.Attributes); } }

Here, file.Attributes.ToString() returns "ReadOnly, Hidden" rather than the '3' it would have returned without the FileAttribute attribute.  We get the same result when using the debugger.  In other words, in addition to simply documenting that the enumeration is intended for combinations, the FlagsAttribute provides a nicety to reading an enumerator value as a string, even when that enumerator is a combination of enumerators.  Note, however, this nicety is not localizable, so only use it if localization is irrelevant - such as in trace messages that only developers will read.

One other important note about what the FlagsAttribute does not do.  It does not assign the enumerators unique flag values or check that they have such values.  It is still required that the values of each enumerator be assigned a bit mask flag explicitly as in:

> \[FlagsAttribute\] enum DistributedChannel { Transacted **\= 1**, Queued **\= 2**, Encrypted **\= 4**, Persisted **\= 16**, FaultTolerant \= Transacted | Queued | Persisted }
