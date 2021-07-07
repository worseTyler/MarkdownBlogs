---
title: "Subscribing to TFS Alerts with TFS Power Tools&#8217; Alerts Explorer"
date: "2011-09-05"
categories: 
  - "alm"
  - "blog"
tags: 
  - "team-foundation-server"
---

Out of the box, Team Explorer includes the ability to subscribe to Project Alerts (available from the **Team->Project Alerts** menu in Visual Studio and from the Team Web->Settings Page):

> [![SNAGHTML1f4ff52b](/wp-content/uploads/2011/09/SNAGHTML1f4ff52b_thumb.png "SNAGHTML1f4ff52b")](/wp-content/uploads/2011/09/SNAGHTML1f4ff52b.png)

As the Alerts dialog image shows, Project Alerts only lets you subscribe to 4 alerts:

- My work items are changed by others
- Anything is checked in
- A build quality changes
- Any build completes
- My build completes

This is a relatively unsatisfactory list, however.  What if you want to subscribe to all work items changes of those on your sub-team or all work items that are Critical?

Fortunately, [Team Foundation Server Power Tools](https://aka.ms/tfpt) includes Alerts Explorer,  which this provides a means of creating custom subscription criteria.  After installing the Power Tools you can access the Alerts Explorer from the **Team->Alerts Explorer** Visual Studio Menu:

> [![SNAGHTML1f612d7a](/wp-content/uploads/2011/09/SNAGHTML1f612d7a_thumb.png "SNAGHTML1f612d7a")](/wp-content/uploads/2011/09/SNAGHTML1f612d7a.png)

This provides a list of all alerts currently setup:

> [![SNAGHTML1f67347c](/wp-content/uploads/2011/09/SNAGHTML1f67347c_thumb.png "SNAGHTML1f67347c")](/wp-content/uploads/2011/09/SNAGHTML1f67347c.png)

In addition, it provides a mechanism for creating additional “custom” alerts:

> [![SNAGHTML1f6890af](/wp-content/uploads/2011/09/SNAGHTML1f6890af_thumb.png "SNAGHTML1f6890af")](/wp-content/uploads/2011/09/SNAGHTML1f6890af.png)

The alerts can be against work items, check ins, or build events.  In addition to “Blank Alert” that you create criteria from scratch on, there are several canned alerts that you can simply subscribe to or use as  template to start from and edit as needed. 

> [![SNAGHTML1f745d9e](/wp-content/uploads/2011/09/SNAGHTML1f745d9e_thumb.png "SNAGHTML1f745d9e")](/wp-content/uploads/2011/09/SNAGHTML1f745d9e.png)
