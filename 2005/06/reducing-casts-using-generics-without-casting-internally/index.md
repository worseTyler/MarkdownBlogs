
I recently wrote the following code:

> PropertyInfo property;
> 
> // ...
> 
> **CommandLineSwitchAliasAttribute attribute =**
> 
> **(CommandLineSwitchAliasAttribute)property.GetCustomAttributes(**
> 
> **typeof(CommandLineSwitchAliasAttribute), false)\[0\];**

The part that raised a flag for me was the fact that in a single statement I specified the CommandLineSwitchAliasAttribute multiple times.  Surely with generics this could be improved?  Redeclaring GetCustomAttributes() using generics results in the following:

> public T\[\] GetCustomAttributes(bool inherits)
> 
> {
> 
> Type attributeType = typeof(T);
> 
> // ...
> 
> return new T\[0\];
> 
> }

This would improve the original code as follows:

> PropertyInfo property;
> 
> // ...
> 
> **CommandLineSwitchAliasAttribute attribute =**
> 
> **GetCustomAttributes <CommandLineSwitchAliasAttribute\>(****false)\[0\];**

Unfortunately, inference doesn't work on return types but even without it we are able to eliminate the cast.

At this point Microsoft is unlikely to update the framework but this is still a good thing to to look for in your own APIs.

By the way, I should mention that this cast elimination is distinctly different from the one specified at Don't use generics simply to bury a cast operation.  The reason is that internally we are actually retrieving the type specified by T so we don't need to cast.
