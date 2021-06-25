---
title: "Data Connection String for a CSV file on VSTS Data Driven Test"
date: "2008-08-22"
categories: 
  - "net"
  - "blog"
tags: 
  - "net"
---

In Visual Studio 2008 you can specify a database, CSV, or XML file for the data connection string.  I decided to try using a CSV file because I had a sample CSV file of test data.  Unfortunately, on initial import I received an error message, â€œError trying to sample data from <file name>, please make sure the file is valid.â€  Hmmmâ€¦?  I tried several CSV file formats, changed the quote style, and the like - but still, I couldnâ€™t get it to work.

[![Error trying to sample data from <file name>, please make sure the file is valid](images/image_thumb_1.png "Error trying to sample data from <file name>, please make sure the file is valid")](/wp-content/uploads/binary/WindowsLiveWriter/DataConnectionStringforaCSVfileonVSTSDat_94EB/image_4.png)

It turns out, it was my file name.  I had two periods in the file name, NYSE.SymbolList.csv.  As soon as I removed the extra period, the file imported without issue.  By the way, quoting the file name also fails â€“ the Next button remains disabled.

Also, column headers are expected in the import file.
