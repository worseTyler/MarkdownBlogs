
I wrote on Configuration Management Application Block for .NET  - from Microsoft Patterns & Practices a few weeks ago.  Since then my team and I have investigated it further.  Here are the reasons we decided to go with our own implementation, one that we had already written.

1. No explicit support for scope.  Scope is the ability to specify that a setting is related to a user, roaming user, group of users, application, and system.
2. The API does not allow you to specify a default such that if there is no setting then a default would be returned.
3. No integrated security support such that only certain users can write and read specific settings.
4. Data persistence into a database is not configurable and we needed to do custom schema stuff.

As a result of these we would end up having to wrap the front end with our own to add additional features to the interface and then plug-in our own data store at the back end...  This is essentially replacing the key pieces so why use it.

Our settings related interfaces can be found here.
