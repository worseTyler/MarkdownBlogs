---
title: "Missing: Guidance for fixing architecture without re-writing"
date: "2005-03-31"
categories: 
  - "net"
  - "blog"
tags: 
  - "net"
---

Last night I participated in a Distributed Architecture birds-of-a-feather session with Steve Swartz. During the talks Clemens Vasters discussed how the architect of today needs to plan for data being distributed across multiple data sources and the data component on top of these data sources should take care of splitting and re-joining the data across these data sources. Steve mentioned how all major sites (eBay, Amazon, etc) are built this way. My participation centered on how business apps just aren't built this way today and that the major Internet sites are the exception. Even major business apps like popular ERP and CRM systems are not constructed that way. Steve responded that he didn't get my point. (Since I hadn't made one yet this was understandable.)

I responded as that architects we can't rely on just giving correct architecture. Re-writing a system to support the right architecture is seldom an option except for new software. As architects we need to provide a migration plan, one that doesn't require a big bang change. Interestingly there aren't architecture books or even articles on migrating architecture. You wonâ€™t find a book that describes how to take the Microsoft recommended DNA architecture of the 1990s (which many .NET apps used due to the lack of architecture guidance when .NET emerged) to the SOA of the mid-to-late 2000s.  Hmmm....  Microsoftâ€™s Practices and Patterns team is tackling the communication of architecture.  They too, however, seldom touch on the topic of migrating from poorly performing architecture to right architecture.

I wonder if such guidance exists for fixing structural problems in buildings?
