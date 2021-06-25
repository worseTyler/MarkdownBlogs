---
title: "Associating a Work Item to a Changeset After Checking in"
date: "2006-12-08"
categories: 
  - "net"
  - "blog"
tags: 
  - "net"
---

Today I received an email asking how to associate a work item with a changeset after all the code was checked in (without performing the association.)

It turns out the solution is relatively simple.  On the **Links** tab of the work item select **Add**.  By default, this shows the **Link Type** is set to **Work Item**.  However, you can change the type to **Changeset** and then browse for the changeset(s) you wish to associate with the code.

![Work Item Add Link](images/WorkItemAddLink.jpg)

(The browse functionality is a good reason why you always want to include comments with your check-in.)
