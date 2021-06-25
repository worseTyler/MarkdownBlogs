---
title: "Check for Null/Not Null with is null and is { }"
date: "2019-09-24"
categories: 
  - "net"
  - "blog"
  - "c"
tags: 
  - "null"
---

## Determine Which Mechanism Is Most Effective to Check for Null or Not Null

It turns out there are numerous ways to check for null or not null, which begs the questions, which should you use?

Not surprisingly, it depends.

### Null/Not Null Mechanisms Table

Here's a table describing the various mechanisms to check for null and their advantages and disadvantages.

table td { word-break: break-word !important; }<br /> table tr td:nth-child(2) { font-family: monospace; white-space: pre-wrap }<br /> td .good { background: rgb(197, 224, 179); }<br /> td .bad { background: rgb(255, 229, 153); }<br />

<table border="1" width="631" cellspacing="0" cellpadding="0"><tbody><tr><td valign="top" width="12.285714285714286%"><strong>Check For</strong></td><td valign="top" width="29.555555555555557%"><strong>Code Checks For Null</strong></td><td valign="top" width="58.15873015873016%"><strong>Description</strong></td></tr><tr><td valign="top" width="12.285714285714286%">Is Null</td><td valign="top" width="29.555555555555557%">if(variable is null) return true;</td><td valign="top" width="58.15873015873016%"><ul><li><img draggable="false" src="https://s.w.org/images/core/emoji/12.0.0-1/svg/1f642.svg" alt="🙂"> This syntax supports static analysis such that later code will know whether variable is null or not.</li><li><img draggable="false" src="https://s.w.org/images/core/emoji/12.0.0-1/svg/1f641.svg" alt="🙁"> Doesn’t produce a warning even when comparing against a non-nullable value type making the code to check pointless.</li><li><img draggable="false" src="https://s.w.org/images/core/emoji/12.0.0-1/svg/1f610.svg" alt="😐"> Requires <a href="https://intellitect.com/csharp-7-msdn/">C# 7.0</a> because it leverages type pattern matching.</li></ul></td></tr><tr><td valign="top" width="12.285714285714286%">Is Not Null</td><td valign="top" width="29.555555555555557%">if(variable is { }) return false</td><td valign="top" width="58.15873015873016%"><ul><li><img draggable="false" src="https://s.w.org/images/core/emoji/12.0.0-1/svg/1f642.svg" alt="🙂"> This syntax supports static analysis such that later code will know whether variable is null or not.</li><li><img draggable="false" src="https://s.w.org/images/core/emoji/12.0.0-1/svg/1f610.svg" alt="😐"> Requires <a href="https://intellitect.com/c-sharp-8-video/">C# 8.0</a> since this is the method for checking for not null using property pattern matching.</li></ul></td></tr><tr><td valign="top" width="12.285714285714286%">Is Not Null</td><td valign="top" width="29.555555555555557%">if(variable is object) return false</td><td valign="top" width="58.15873015873016%"><ul><li><img draggable="false" src="https://s.w.org/images/core/emoji/12.0.0-1/svg/1f642.svg" alt="🙂"> Triggers a warning when comparing a non-nullable value type which could never be null</li><li><img draggable="false" src="https://s.w.org/images/core/emoji/12.0.0-1/svg/1f642.svg" alt="🙂"> This syntax works with <a href="https://intellitect.com/essentialcsharp/">C# 8.0’</a>s static analysis so later code will know that variable has been checked for null.</li><li>Checks if the value not null by testing whether it is of type object. &nbsp;(Relies on the fact that null values are not of type object.)</li></ul></td></tr><tr><td valign="top" width="12.285714285714286%">Is Null</td><td valign="top" width="29.555555555555557%">if(variable == null) return true</td><td valign="top" width="58.15873015873016%"><ul><li><img draggable="false" src="https://s.w.org/images/core/emoji/12.0.0-1/svg/1f642.svg" alt="🙂"> The only way to check for null prior to <a href="https://devblogs.microsoft.com/dotnet/new-features-in-c-7-0/">C# 7.0.</a></li><li><img draggable="false" src="https://s.w.org/images/core/emoji/12.0.0-1/svg/1f641.svg" alt="🙁"> However, because the equality operator can be overridden, this has the (remote) possibility of failing or introducing a performance issue.</li></ul></td></tr><tr><td valign="top" width="12.285714285714286%">Is Not Null</td><td valign="top" width="29.555555555555557%">if(variable != null) return false</td><td valign="top" width="58.15873015873016%"><ul><li><img draggable="false" src="https://s.w.org/images/core/emoji/12.0.0-1/svg/1f642.svg" alt="🙂"> The only way to check for not null prior to C# 7.0.</li><li><img draggable="false" src="https://s.w.org/images/core/emoji/12.0.0-1/svg/1f641.svg" alt="🙁" width="22" height="22">&nbsp;Since the not-equal operator can be overridden, this has the (remote) possibility of failing or introducing a performance issue.</li></ul></td></tr></tbody></table>

### Test Source Code

				 `#nullable enable
string? nullableText = "Inigo";
Assert.IsTrue(nullableText is object && nullableText is { });
nullableText = null;
Assert.IsFalse(nullableText is object || nullableText is { });
 
string notNullableText = "Inigo";
Assert.IsTrue(notNullableText is object && notNullableText is { });
notNullableText = null!;  // Initentionally ignore the null assignment
Assert.IsFalse(notNullableText is object || notNullableText is { });
 
int? nullableNumber = 42;
Assert.IsTrue(nullableNumber is object && nullableNumber is { });
nullableNumber = null;
Assert.IsFalse(nullableNumber is object || nullableNumber is { });
int notNullableNumber = 42;
Assert.IsTrue(
    // CS0183 - The given expression is always of the provided('object') type
    #pragma warning disable 0183
    notNullableNumber is object
    #pragma warning restore 0183
    && notNullableNumber is { });
// notNullableNumber = null;  // Error: Can't assing null to non-nullable value type` 
			

### Ready for more?

Check out this video on the [improvements and new features released in C# 9.0](/video-essential-c-sharp-9/)!

![](images/Blog-job-ad-1024x127.png)
