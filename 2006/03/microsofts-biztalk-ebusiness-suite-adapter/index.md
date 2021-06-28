
I have been working with Microsft's eBuisiness Suite and Oracle adapter (the one they purchased from iWay) for several months now and I would describe the experience as disappointing at best.  The core problem is that the adapter is built using the ODBC adapter.  This is a critical problem, however, because the vast majority of the Oracle eBusiness Suite Adapter APIs use REC TYPE parameters and ODBC doesn't support REC TYPE parameters.  You can't even generate the schema for these APIs.  In other words, Microsoft's adapter for connecting to eBusiness Suite is incompatible with Oracle's approach for putting data into eBusiness Suite.  For all practical purposes, this renders the adapter useless for the vast majority of scenarios.  In fact, the eBusiness Suite adapter, therefore, has no additional functionality beyond that provided by the Oracle adapter (and I am not exaggerating). By the way, the Oracle adapter is also built using ODBC and, therefore, it suffers from the exact same problem.

**What to do?**

The workarounds are by no means attractive but there are some:

1. Take the Oracle API that you wish to call, the one that uses REC TYPE parameters, and wrap it in a custom API that flattens out the REC TYPE parameters into simple parameters instead.  The result is very long parameter lists, but at least you can now call the APIs.
2. Define a staging table into which data is pushed and then call a custom PL/SQL procedure that sends the data into the Oracle APIs.  This forces you to use an orchestration but it will likely work.
3. Take the Oracle BEPL engine, what Oracle offers to compete with BizTalk, and call into it using the SOAP adapter.  (Personally, I haven't tried this but it was Oracle's proposed solution when I encountered this issue.)
