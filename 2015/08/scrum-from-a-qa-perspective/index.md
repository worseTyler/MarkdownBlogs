

I work QA but am not solely on a QA team; I am a member of a Scrum team. Like the other members, this means I am a part of the development team, I just happen to specialize in QA practices. This is the first position at which I feel like I have the capability to truly fulfill the role a QA engineer should be doing. Namely, I'm part of development, rather than post development. Being integrated and collaborating with the development team, rather than simply being an afterthought, has lead to some interesting challenges and great experiences:

- Draw from the developers’ knowledge
- Don’t get tunnel vision
- Plan QA _with_ development

**Draw from the Developers’ Knowledge**

The first and most immediate boon I experienced was the ability to draw from the programmers' knowledge directly. Not just of their changes, but also of underlying systems, other related changes, and tips on the technical details pertinent to their changes. The ability to share knowledge and bounce ideas off of the programmers was immediately beneficial. For example, I received insight into what types of variables were being used, where integration points were, and what types of controls were implemented. The additional information proved to be invaluable to making my tests more efficient. And when it comes to automated UI tests? I have an entire team happy to help me work out best coding practices and recommend patterns While this is not specific to scrum, the fact agile/scrum methodologies advocate everyone being on the same team helps to facilitate this, and I’ve not encountered it outside of scrum environments.

**Don’t get Tunnel Vision**

A big challenge, however, is that having this close interaction with developers, and the way work is tracked, makes it easy to lose the bigger picture. When work tracked is by “this user story has a solution coded, test the user story for acceptance criteria,” it was very easy to slip into a habit of only checking for the acceptance criteria, and to lose focus on the other parts of the application that work together. For example, one of the applications I test consists of a million lines of code. Situations arise where, sometimes, no single person knows how every piece of code is called. By not pushing for QA-specific stories in our backlog and sprints, I missed the opportunity to test interactions that should have been apparent to a skilled QA person.

**Plan** **_with_** **Development**

The solution should have been obvious. Get back to the basics: form and stick to an intelligent, efficient testing strategy. I'm not saying to never improvise. I am saying make sure improvisations supplement a well-fleshed out strategy. For instance, I might plan to test the developer change to a text field by performing some quick boundary analysis. But then if I also know that field eventually gets written to a table somewhere, I might perform some exploratory testing to see if it's possible to break the connection to that table, or get other values to overwrite the table row. Another huge benefit to this, is that we are able to pre-empt a lot of issues that could arise later on down the road. A developer is adding a text field that writes to a table? If I plan for that before it’s even complete, I can check for other conflicting changes, and be ready to test as soon as the change is available.

One caveat: I do think a QA team coming along after the fact and checking functionality towards the end of the development cycle can be hugely valuable. Much like skilled beta testers, I've seen these teams have a "heads-down" approach where they focus solely on finding issues without any preconceived notion of what is supposed to happen. As a result, they can catch many issues that may have gone unnoticed.

With a solid testing strategy to account for these issues, QA can transcend the popular viewpoint as a relatively unskilled, low-value member of the team. By collaborating with developers, ensuring developers are getting specs that actually describe the intended behavior, creating tests based off those specs, and checking for quality often and early, there is the potential for highly skilled QA engineers to increase software quality.
