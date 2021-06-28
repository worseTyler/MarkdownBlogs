
The Journey Framework is a great way for an MVC team to leverage their knowledge base for more dynamic applications and ease into the world of SPAs and JavaScript.

We recently built an intranet application for compliance logging. The application allows the user to both enter and browse data related to events on the electrical grid. Much of the data is interrelated and there are many different roles. This pushed us towards a drill-down methodology for the user interface having users navigate from general to more specific data. However, the typical drill-down user experience can be quite cumbersome, requiring many clicks and repeated context data on each page. For example, while navigating to a specific page it was important not to lose the hierarchical context provided on the previous pages. We needed to find a way to present the web site in a way that was intuitive to navigate without duplicating data on multiple screens.

This led us to look at Microsoft’s new beta Azure portal. The portal implements a drill-down approach using a left to right navigation pattern where each new page opens to the right of the existing one. The approach preserves context and makes navigating to a related context a single click operation. Additionally, this removes the need for an explicit back navigation feature. The user can go back simply by scrolling to the left and clicking a link. This approach seemed like a good fit our the customer’s needs.

For this project, IntelliTect provided the core application and ongoing support as their team built out the additional functionality. The development team’s primary skillset was with Model View Controller (MVC) web implementations. In general MVC applications are simpler and faster to write then their SPA counterparts. We needed to find a way to leverage the team’s existing skill set in MVC to build a SPA application. We opted for a hybrid approach that would account for different skill levels across the team and while focusing primarily on an MVC approach would also use traditional SPA pages where appropriate. 

So I had the challenge of providing a left-to-right SPA for a team that knows MVC. Unfortunately, there wasn’t an off-the-shelf implementation of this paradigm. A framework would need to be built that would work for this project and hopefully others that had similar requirements.

And so was born the Journey Framework. Journey provides an infrastructure that turns regular MVC pages into an integrated SPA application. Developers can build using a typical MVC paradigm with gets, posts, and redirects. With only a few minor modifications, existing MVC applications can be transformed to have a more fluid user experience. We also found that this paradigm has the added benefit of keeping each page focused on a single task because other pages with related data can be viewed simultaneously.

Journey accomplishes that by intercepting page requests and instead of loading a whole new page, it makes an AJAX request for the content. The new content is placed into a container to the right of the current page. All scrolling is handled automatically showing the user the most recent content. Some special responses are used to handle post redirects, refreshes, and closing pages, but they have intentionally been kept simple.

While Journey was designed on ASP.NET, it is a client-side HTML, CSS, and JavaScript framework. There is one class of simple C# helpers, but everything else is cross platform. The framework is not limited to MVC, and AJAX mechanisms can be used, but they are not required.

We have elected to place this project on GitHub at [https://github.com/IntelliTect/journey](https://github.com/IntelliTect/journey). Along with the source code, there is a demo site ([http://journeydemo.azurewebsites.net/Journey](https://journeydemo.azurewebsites.net/Journey)) that shows the basic functionality. You can download the repository and open the solution to try the project. The wiki on the repository has a getting started walkthrough and API documentation. There are also two NuGet packages:

- Journey Framework: Core framework files
- Journey Framework Jumpstart: Demo site that loads into a current project to demonstrate how to implement the various features.

We were pleasantly surprised at how our customer embraced the framework. The application developers have been able to quickly ramp up and finish the application. The end users, moving from a pencil and paper process, have found it easy to use.

Please check out the Journey Framework, and see if it might meet your needs. Let us know if you have any questions!
