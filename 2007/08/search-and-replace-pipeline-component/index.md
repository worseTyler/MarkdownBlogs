---
title: "Search and Replace Pipeline Component"
date: "2007-08-07"
categories: 
  - "biztalk"
  - "blog"
  - "pipeline-components"
tags: 
  - "biztalk"
  - "pipeline-components"
---

Ok. So this isn't the fanciest of things, but it really saved my bacon.

**Problem:** I have a predefined flat file schema to transform a fixed-length positional file. However, for some unknown reason, random records in the file extend beyond extend beyond the specified record length. I was able to deduce that those records consistently contained a certain value instead of what was expected. They should have been passing a series of zeroes, but instead converted them to decimals. Instead of 00000000 I was getting 00000000.00000000. I tried a few other solutions, but wound up with the need for a custom pipeline component.

I will assume you are somewhat familiar with the creation of pipeline components. I am certainly no expert myself. For further details, check this page out ([MSDN - Developing Pipeline Components](https://msdn2.microsoft.com/en-us/library/ms946690.aspx)) or give [Google](https://www.google.com/) a shot.

First, I declared my 2 properties:

> private string searchString = null; \[System.ComponentModel.Description("Find what:")\] public string SearchString { get { return searchString; } set { searchString = value; } }
> 
> private string replaceString = null; \[System.ComponentModel.Description("Replace with:")\] public string ReplaceString { get { return replaceString; } set { replaceString = value; } }

Then, I created my IBaseComponent Members: Description, Name, and Version.

And then, I created my IPersistPropertyBag Members: GetClassID, InitNew, Load, and Save.

And then, I created my IComponentUI Members: Icon.

Finally, I created my IComponent Members: Execute. This is the heart of the component where the real work is performed. Essentially, I take the message, extract the BodyPart and load the BodyPart's Data in to a StreamReader. Now I am able to load the context of the message in to a string and perform my simple search and replace. This sample does not handle for case sensitivity, or anything super fancy as I only did what fit my needs.

> IBaseMessage Microsoft.BizTalk.Component.Interop.IComponent.Execute(IPipelineContext pContext, IBaseMessage pInMsg) { System.Diagnostics.Debug.WriteLine("Begin Execute method for Search and Replace pipeline component."); IBaseMessagePart bodyPart = pInMsg.BodyPart; string tmpString = ""; if (bodyPart != null) { try { System.IO.StreamReader sr = new System.IO.StreamReader(bodyPart.Data);
> 
> tmpString = sr.ReadToEnd();
> 
> System.Diagnostics.Debug.WriteLine(String.Format("Replacing \[{0}\] with \[{1}\].", this.searchString, this.replaceString)); tmpString = tmpString.Replace(this.searchString, this.replaceString);
> 
> System.IO.MemoryStream strm = new System.IO.MemoryStream(ASCIIEncoding.Default.GetBytes(tmpString)); strm.Position = 0; bodyPart.Data = strm; pContext.ResourceTracker.AddResource(strm); } catch (System.Exception ex) { throw ex;
> 
> }
> 
> } return pInMsg; }

If you are interested in full source, please drop me a line.
