---
title: "Dynamically Typed Objects with C# 4.0"
date: "2008-11-04"
categories: 
  - "net"
  - "blog"
tags: 
  - "net"
---

One of the announcements that emerged from the PDC is C# 4.0's support for dynamically typed objects.Â  These are objects whose type is not determined until runtime.Â  Variables that point to such types are declared with a new contextual keyword, dynamic.Â  Support was added so that API calls into dynamically type languages (such as scripting languages) could be supported.Â  Another example where dynamically typed object support is useful is calling into IDispatch objects (something supported by VB but not C# until now).

In order to investigate how dynamic objects worked I decided to create a working sample that dynamically went against an XML element. Working with Michael Stokesbary, I was able to put together the following sample:

Consider the following XElement:

> XElement element = XElement.Parse(
>     @"<Person>
>              <FirstName>Inigo</FirstName>
>              <LastName>Montoya</LastName>
>         </Person>");

From here we can assign it to a dynamic variable and read out the data as follows:

> dynamic personXml = new DynamicXml(element);
> Console.WriteLine("Hello, my name is {0} {1}", personXml.FirstName, personXml.LastName);

and

> personXml.FirstName = "Bob";

As this code shows, with a dynamic type over XML, you can use XML element names as the property names to retrieve data from the XML element.

To implement the DynamicXML type, all you need to do is define an type that implements System.Scripting.Actions.IDynamicObject with its one MetaObject GetMetaObject(Expression parameter) method.Â  The easier way to do this is to derive from Dynamic, a class that is currently available in the Iron Python implementation.Â  (In the future, we should expect that Microsoft will provide such a class in the framework but no specific plans on this have been announced.)Â  Once deriving from Dynamic, the only remaining task is to override the object GetMember(GetMemberAction action) and void SetMember(SetMemberAction action, object value) members.Â  Rudimentary implementations are shown below:

using System;
using System.Collections.Generic;
using System.Linq;
using System.Scripting.Actions;
using System.Xml.Linq;
using System.Xml.XPath;
// comment here
public class DynamicXml : Dynamic
{
    private XElement Element { get; set; }

    public DynamicXml(System.Xml.Linq.XElement element)
    {
        Element = element;
    }

    public override object GetMember(GetMemberAction action)
    {
        object result = null;
        XElement firstDescendant = Element.Descendants(action.Name).FirstOrDefault();
        if (firstDescendant != null)
        {
            if (firstDescendant.Descendants().Count() > 0)
            {
                result = new DynamicXml(firstDescendant);
            }
            else
            {
                result = firstDescendant.Value;
            }
        }
        return result;
    }

    public override void SetMember(SetMemberAction action, object value)
    {
        XElement firstDescendant = Element.Descendants(action.Name).FirstOrDefault();
        if (firstDescendant != null)
        {
            if(value.GetType() == typeof(XElement))
            {
                firstDescendant.ReplaceWith(value);
            }
            else
            {
                firstDescendant.Value = value.ToString();
            }
        }
        else
        {
            throw new ArgumentException(string.Format("Element name, '{0}', does not exist.", action.Name));
        }
    }
}

> using System;
> using System.Collections.Generic;
> using System.Linq;
> using System.Scripting.Actions;
> using System.Xml.Linq;
> using System.Xml.XPath;
> 
> public class DynamicXml : Dynamic
> {
>     private XElement Element { get; set; }
> 
>     public DynamicXml(System.Xml.Linq.XElement element)
>     {
>         Element = element;
>     }
> 
>     public override object GetMember(GetMemberAction action)
>     {
>         object result = null;
>         XElement firstDescendant = Element.Descendants(action.Name).FirstOrDefault();
>         if (firstDescendant != null)
>         {
>             if (firstDescendant.Descendants().Count() > 0)
>             {
>                 result = new DynamicXml(firstDescendant);
>             }
>             else
>             {
>                 result = firstDescendant.Value;
>             }
>         }
>         return result;
>     }
> 
>     public override void SetMember(SetMemberAction action, object value)
>     {
>         XElement firstDescendant = Element.Descendants(action.Name).FirstOrDefault();
>         if (firstDescendant != null)
>         {
>             if(value.GetType() == typeof(XElement))
>             {
>                 firstDescendant.ReplaceWith(value);
>             }
>             else
>             {
>                 firstDescendant.Value = value.ToString();
>             }
>         }
>         else
>         {
>             throw new ArgumentException(string.Format("Element name, '{0}', does not exist.", action.Name));
>         }
>     }
> }

That's it... that's all that is needed to understand how to implement a dynamic object.

Caveats:

- Multiple elements with the same name are not supported in this implementation.Â  To do so we wanted to use the index operator with the dynamic type but this is not supported in the PDC 2008 CTP bits - it will be.
- This example does not work with attributes.Â  What would the syntax be if it did?Â  One cool idea of Mike's was to use the verbatim identifier as in personXml.@FirstName. - the XPath like way to retrieve attributes. Unfortunately, this wouldn't work work since the @ sign is removed at compile time so the IDynamicObject.GetMember() call receives FirstName for the action Name.
- There is no support for reading the root element name.Â  The example assumes you only navigate into the children (to avoid infinite recursion) when navigating.

In the process of writing this dynamic XML implementation, I found a few idiosyncrasies in the current dynamic type support found in the PDC 2008 CTP:

- There is no type checking for dynamic arguments.Â  Consider the sample code below:
    
     string text1 = "Text";
         dynamic text2 = text1;
         Assert.AreEqual<string\>(text1, text2);
    

> Not only does the code sample compile (by design), but it throws an exception of type ArgumentNullException (not by design - Mads agree's this probably isn't correct).
