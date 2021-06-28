
I glanced briefly at Eric Gunnerson's post entitled Hangin' with generics.  I think it is worth pointing out that using a cast within a generic method should be done with caution.  At first it is appealing may seem appealing but in cases such as Eric's, where there is not way to specify a constraint on the cast, developers should be careful.

Take the following example:

> public static T Deserialize<T>(Stream stream, IFormatter formatter)
> 
> {
> 
> return (T)formatter.Deserialize(stream);
> 
> }

A user of this method would not know the internal implementation but be very excited to use it because it represents strongly typed deserialization.  Unfortunately, however, it is not strongly typed deserialization at all.  Rather it simply hides and internal cast making the users less likely to check for an InvalideCastException.

Using the generic version of Deserialize would look something like this:

> string greeting = Deserialization.Deserialize<string\>(stream, formatter);

However, I suspect that this is not preferable to the non-generic call:

> string greeting = (string)Deserialization.Deserialize(stream, formatter);

I believe Eric casts appropriately but if another reasonable way could be found in which the cast was explicit I think it would be preferable.

The moral of the story:  Don't use generics simply to bury a cast operation.
