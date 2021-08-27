

## Converting UTC to Local Time With Power BI 
#
Estimated reading time: 5 minutes

**Update:** _Recent updates to the Power BI service need to be reviewed and appropriate updates to the post will be made._

Power BI provides exceptional business analytic services. Additionally, the [Power](https://intellitect.com/power-bi-data-importation/) BI component Power Query provides a powerful and dynamic tool for loading and transforming data into Power BI's data model. However, Power Query lacks a good way to convert a Coordinated Universal Time (UTC) to a local time of your choosing while preserving Daylight Savings Time (DST).  Time zone functions do exist. However, one has to be careful how and where they are used. Below I explain the problem with Power Query's ToLocal() function in the Power BI Service in more detail. I also provide my solution for converting UTC to the Pacific Time Zone.

### The Problem: Need to Convert UTC

We built an online event management and ticketing system for one of our clients. Events are scheduled and published. Customers purchase tickets online or from agents at the event sites. The date and time of each event are recorded in UTC. Also, the date and time that purchases are made for each event are recorded in UTC.

The client’s accounting team needs to report on and analyze ticket purchases. The accountants are in US Pacific Time (PT), and all events are held in Pacific Time, so it makes sense to convert UTC to PT. This would be an easy modification except for the time change that takes place twice a year in most of the US and various parts of the world, called Daylight Savings Time (DST).  Under DST, clocks are adjusted ahead one hour in the spring and one hour back in the fall. These clock changes are also known as “Spring forward” and “Fall back.”

A simple PowerQuery expression can transform Coordinated Universal Time to any local time:

```csharp
DateTimeZone.RemoveZone(
  DateTimeZone.ToLocal(
    DateTime.AddZone([EventStartDateTime], 0)
  )
)
```

This expression works great as long as users are in Power BI Desktop in Pacific Time. However, as soon as the model is published to the Power BI Service and the data refreshes, the date-times are no longer Pacific Time. ToLocal() converts to the local time of the Power BI servers, which are set for Universal Coordinated Time.

### The Solution

To resolve the Daylight Savings Time issue, we need to determine if a date is in DST or not before adding an hour offset. My solution does this with a table of DST periods, including UTC. I created a PowerQuery function to look up a UTC-based value and return 1 if the value occurs in DST and 0 if it does not. I then add that value to the timezone offset.

My table of Daylight Savings Times:

![Table of US Daylight Savings Times](https://intellitect.com/wp-content/uploads/2017/12/dst-table-1024x342.png "Table of US Daylight Savings Times")

The Power Query function:

```csharp
let
  Source = (DateToCheck) => let
    CountOfRows = if DateToCheck is null
      then 0
    else let Source = DaylightSavings,
      DateToCheckDate = DateTime.From(DateToCheck),
      StartDates = Table.SelectRows(Source, each [DSTStartDateInUTC] <= DateToCheckDate),   EndDates = Table.SelectRows(StartDates, each [DstEndDateInUTC] > DateToCheckDate),
      CountOfRows = Table.RowCount(EndDates)
    in
      CountOfRows
  in
    CountOfRows
in
  Source
```

The function selects rows from the table where the UTC DateToCheck parameter provided is between a DST Start Date and DST End Date. If the table is configured correctly, at most one row will be returned when a date is in a DST period, and no rows will be returned when the DateToCheck is not within a DST start and end date range.

Note that the date time function parameter, DateToCheck, does not have a type declaration making it of type any. In Power Query, the any type can be null while DateTime is not nullable. As some of the source datetime values are null the any type for the parameter is required.

The value of the function is then used when offsetting UTC to local time. If the original date time is in a DST range, an additional hour will be added to the offset.

The next function makes it easy to adjust our Pacific Time conversion to consider Daylight Savings Time. Our custom column formula now looks like this:

```csharp
  DateTimeZone.RemoveZone(
    DateTimeZone.SwitchZone(
      DateTime.AddZone([CreationDate], 0),
      -8 + CheckDaylightSavings([CreationDate])
    )
  )
```

DateTimeZone.SwitchZone() replaces ToLocal(). We pass in -8 hours which is the standard offset of US Pacific Time from UTC. We then call the custom function CheckDaylightSavings() which will move the time one hour forward if the passed in date time is in DST.

### Summary

This seems like a lot of work to adjust for Daylight Savings Time, but it is the simplest and most straightforward way I could come up with. This isn’t a perfect solution by any means. Here are some drawbacks:

- Only daylight savings times for which there is an entry in the Daylight Savings Time table are considered
- This approach only converts to Pacific Times. Other time zones could easily be supported, but additional work is required to handle a time zone more dynamically if some users are in a different time zone and would like their reports localized.

There are other approaches I’ve seen from [implementing an Azure Function](https://chris.koester.io/index.php/2017/03/28/call-an-azure-function-from-power-bi/) to calling a [DateTime web service](https://radacad.com/solving-dax-time-zone-issue-in-power-bi) as well as a fairly [in depth function with extensive parameterization](https://www.youtube.com/watch?v=wrEyYTBnYfU).

### Have a Question?

If you’d like my code, have other approaches, or questions or feedback on this approach, please post a comment.

![](https://intellitect.com/wp-content/uploads/2021/04/Blog-job-ad-1024x127.png)
