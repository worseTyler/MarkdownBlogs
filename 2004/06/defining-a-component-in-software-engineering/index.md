
When .NET was first released I was disappointed by the fact that they had not provided a definition for the term "component."  The term was common in the COM days but it was never clearly defined as referring to a particular COM object or the DLL in which the COM object was implemented.

Under .NET the definitions remain ambiguous and at this point too many leading engineers have placed a stake in the ground one side or the other so it is unlikely to get resolved any time soon.  Some, like Juval Lowy, firmly believe it corresponds to a class.  Other sway more toward it referring to an entire assemble.

I share Michael Platt's amazement that these terms remain ambiguous given the length of time they have been pervasive in the industry.  Furthermore, the frequent need to create designs that include classes as well as the containers of the class definitions would lead one to expect that their definitions were firmly established.  Perhaps what makes this even worse is that "module" now has a definite meaning in the .NET space.  Therefore, it cannot be used as the generic term for a container of compiled code.

Personally, I prefer the component to mean assembly (or container of compiled code) as I don't believe there is any need to provide another word for object or class as these have firm O.O. definitions.  Furthermore, there is not really any generic term for the files (or streams) that contain a series of bytes implementing a feature (or class etc.)  Component seems like a great term to fill this hole.  Recently I read Michael Platt's discussion of the terms Object, Component, Model, and Service and was pleased to hear I am not alone in my leanings.

Thoughts?
