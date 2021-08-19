## Using Microsoft Project Vs. Scrum 
#
Recently, as I was putting together an enterprise scale SharePoint migration plan for a client, a colleague asked “Why are you using Microsoft Project? Why don’t you use a Scrum style Product Backlog instead?”

An excellent question I thought and one I’ve encountered before. For the record, Scrum is my project management framework of choice for complex technical projects. That doesn’t mean it’s the only project management approach I use. I also regularly use Kanban when it fits, and, yes good old fashioned waterfall (meaning a classic work breakdown structure and gantt project plan) when appropriate.

I choose to use Project based on the following criteria:

- The project has a specific sequence of steps
- Specific resources or roles are required for specific tasks
- External dependencies exist which directly impact the timeline of the project
- Project costs need to be tracked at a detailed level

I also frequently use Project to generate project Gantt charts and to estimate costs during proposal and planning phases of projects.

### **Specific Sequence of Work**

A scrum style product backlog can certainly be sequenced, and, it can be easily resequenced as a project changes. Some projects, such as the large enterprise deployment, require a specific sequence of steps: subsequent steps cannot be started before prior steps are completed. With Project, I can configure these dependencies explicitly and maintain the plan easily.

### **Resource Specific Tasks**

One of the key tenets of Scrum based project management is that anyone on the project team can pick up any task on the product backlog. If specific skills only exist with certain team members, and there are a number of tasks for those team members, managing bottlenecks and the project critical path will be easier using a task based project management tool such as Microsoft Project.

### **External Dependencies**

While external dependencies can be managed via Scrum, it is difficult to show the impact of the impact of an external dependency in a Scrum product backlog. A good example in my SharePoint migration project is the building of server systems on which to install and configure a SharePoint farm. The client’s IT group is responsible for building the systems, and I don’t control the IT group’s priorities. However, other critical activities, such as installing software on those systems, cannot proceed until the systems are made available. The best that I can do is show the impact that the dependencies may have on the project timeline.

### **Project Costs**

Both when planning a project: specific costs for resources with estimates for each resource, or, when including external costs, Project has very good capabilities both for estimating up front costs and tracking project costs against a baseline as the project proceeds.

This isn’t to say that one can’t track costs when using agile project management, costs are tracked differently however and some costs outside of the a product backlog will be tracked separate from the backlog.

### **Keeping the Plan Updated**

Using Project inevitably leads to struggles to maintain a sensible plan while the project is executing. This is a big advantage of a good Scrum implementation as the plan will essentially update itself as items in the product backlog are completed or changed. Maintaining an accurate Project plan for a large and complex project can be a full time job. Updating the plan requires a good understanding of how Project works and how best to build the plan for maintenance. Too often, project managers are caught up in capturing minute detail in the project plan which will lead to unnecessary efforts to keep the plan sane. Strategies are the subjects of entire books. General things I keep in mind include only planning to the level of detail that matters and using milestones to track both internal project milestones and external dependencies.

### **Summary**

Any kind of project management requires thinking, planning, adapting to changes and resolving issues. Many of the issues identified above could be called “agile myths” and reasons to not adopt agile.  The project manager’s responsibility is to weigh the advantages of the different approaches and determine what will work best for the project at hand.

Just because we prefer agile project management including Scrum and Kanban, doesn’t mean we always use Scrum and Kanban. Sometimes a good project plan is just what you need.
