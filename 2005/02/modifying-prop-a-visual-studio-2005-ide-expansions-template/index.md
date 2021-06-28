
I love the VS 2005 Expansion.  In fact, even before I saw it in VS 2005 my team had implemented a VS 2003 add-in that provided very similar functionality (significantly more in fact).

Of course, being able to customize them is one of the key features....  One change I make is to customize the property template, prop.  Out-of-the-box the template requires three fill-in-the-blanks: the property data type, the property name, and the field name.  I like to customize it down to just two.  The way I look at it, the field name should have some type of naming convention based off of the property name.  The expansion template syntax doesn't support stuff like varying casing or regular expansion transforms (nor does it need to in my opinion) so I have resorted to the simple convention of prefixing my field name with an underscore as follows:

> public int Count { get { return \_Count; } set { \_Count \= value; } } private int \_Count;

The underscore in a private field only and it has no exposure on my interface.  Furthermore, it maintains distinction from parameters and local variables, an important factor that eliminates many name ambiguity scenarios that otherwise would force the use of the keyword this.

The template for this property can be downloaded [here](/wp-content/uploads/binary/05fa2f77-a4ea-4291-931c-3397d805a1c3/Prop.zip).  The xml file needs to be saved to .\\Microsoft Visual Studio 8\\VC#\\Expansions\\1033\\Expansions.
