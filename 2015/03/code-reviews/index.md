## Code Reviews 
#
I absolutely love code reviews. My team uses a very informal asynchronous method for doing code reviews. When changes are made, a code review request is sent to the other members of the team. As people have time, they will look over the code review requests and provide feedback. Pretty painless; the way a code review should be.

The purpose of code reviews is to **_improve quality_** of both the code and the developers. With that in mind, I would like to present some pointers for doing code reviews.

**_Leave your ego at the door_**.

Everyone has something they can learn. Code reviews are not a time for senior developers to make sure junior developer’s work is up to par. Rather, it is an opportunity for developers to learn from each other. In the past, my code has been improved by developers with far less experience reviewing my work.

**_Ask questions on anything you cannot explain._**

When reviewing code, keep in mind that you may be the next developer that has to work with it. A code review request is an opportunity to ask the original author about their changes while the code is still fresh in their mind. Always make sure you can explain what the new code is doing, and when practical, why it is doing it. Always pose a question in the code review on anything you cannot explain.

**_Review your own changes before submitting them._**

The saying, “write your code like it will be maintained by a psychopath that has your home address” contains quite a bit of truth. Don’t waste your team’s time by sending out a code review before you have reviewed it yourself (you should do this before checking in the code too). A little proofreading goes a long way.

Keep the code review as small and as focused as possible. Try not to mix refactoring and bug fixes together.

**_Be ruthless to the code but kind to the developer._**

Though it may be tempting, avoid commenting on code that was not affected by the changes in the code review. Only critique things _affected_ by the changes you are reviewing. Typically, I consider any _method that was modified_ and any code that is _directly invoked_ or _directly invokes a modified method_ to be open for critique. This is not a hard and fast rule; it may come down to a judgment call.

As a general rule, the reviewer is always right. The burden lies on the author to either make the reviewer’s suggested change, or defend their original work. The goal is to improve the quality of code. Thinking critically about your code and defending your work will ultimately make you a better developer. The reviewers are likely people you work with on a daily basis. Take this opportunity to try and foster a good working relationship.

**_Pay attention to detail; the little things matter._**

Don’t waste your time clicking through a code review if you do not have the time to focus on it. Marking a code review as “Looks good” when it’s not, will not improve quality and will give a false sense of quality.

All developers should follow the same coding standard (your team does have one right?). [Anything that does not match your coding standard](https://blog.codinghorror.com/the-broken-window-theory/) should be caught and addressed during a code review. This will help avoid disputes over developer’s personal preferences.

Though not comprehensive, here are some questions to ask when doing a code review:

- Can you explain all of the changes?
- Does the code meet coding standards?
- Are relevant work items (bugs/user stories/tasks/etc.) properly referenced?
- Are things named appropriately?
- Is the code in the right location?
- Is the code well structured?
- Is the code tested?
- Are exception cases properly handled?

 

 

To all of the developers who have critiqued my code over the years, thank you.
