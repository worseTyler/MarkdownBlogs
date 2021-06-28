
In order to keep our release builds as bug-proof as possible, our development team expects that code reviews are to be completed before checking in the code changes. This presents a problem with Pending Changes in Visual Studio. Let’s say I modify a few files for a particular user story or bug. After I submit the code review, and depending on the availability or responsiveness of the other team members, there will likely be a delay before I can check the code into Team Foundation Server (TFS). If I need to work on another issue, I would now have a mixed set of modified files in my Pending Changes, possibly with changes in some of the same files. Fortunately, since Visual Studio 2012, Microsoft has a solution. It’s called _Suspended Work_.

Under the Team Explorer tab in Visual Studio there is a collection of options including _My Work_, _Pending Changes_, _Source Control Explorer_, etc. Selecting _My Work_ shows _In Progress Work_, _Suspended Work_, _Available Work Items_, and _Code Reviews_.

[![Suspend Description](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2015/04/suspend-and-resume-in-visual-studio-using-tfs/images/Suspend-Description.png)](/wp-content/uploads/2015/04/Suspend-Description.png)Although an active Work Item (TFS User Story or Bug) is not required in order to use the _Suspended Work_ feature, Visual Studio does relate Work Items if you do. If you want to relate one or more work items, those under _Available Work Items_ can be dragged up to _In Progress Work_ and vice versa. A history comment will appear on each listed work item whenever the associated code is shelved, unshelved, and finally checked in.

[![Suspend Shelf](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2015/04/suspend-and-resume-in-visual-studio-using-tfs/images/Suspend-Shelf.png)](/wp-content/uploads/2015/04/Suspend-Shelf.png)All active Work Items and currently Pending Changes are considered _In Progress Work_. If the Suspend button is clicked (see image above) then the user will be offered a text area in which the default description (taken from the Work Item, or _n_ edit(s) if there is no Work Item selected) can be left, or a specific description can be entered. Clicking the second Suspend button under the text area (see image to the right) shelves the code along with references to the active Work Items, currently open files, breakpoints, etc. Basically, the current state of Visual Studio is saved for later recovery. Modified files will be reverted to the Latest Version.

Now the user is ready to start on a new Work Item without worrying about changing the files that have been shelved for code review.

[![Suspend Switch](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2015/04/suspend-and-resume-in-visual-studio-using-tfs/images/Suspend-Switch.png)](/wp-content/uploads/2015/04/Suspend-Switch.png) When the code review is complete, there are three possibilities for recovering the suspended work. If there are other modified files, as shown in the image on the left \[note 2 edit(s)\], the options will be to ‘Switch’ the current work with the suspended work or to ‘Merge’ the suspended work with the current _In Progress Work_.

[![Suspend Resume](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2015/04/suspend-and-resume-in-visual-studio-using-tfs/images/Suspend-Resume.png)](/wp-content/uploads/2015/04/Suspend-Resume.png)

If there are no current edits, as shown in the image on the right, the only option will be to ‘Resume’ the suspended work.

To check in the reviewed code, the user would select ‘Switch’ or ‘Resume’ and the suspended work would be recovered along with the previous state of Visual Studio. The code can then be checked in. The shelveset will be deleted automatically. If ‘Switch’ is chosen, then the current work is suspended before the selected work set is restored. After check-in is complete, then the previous work \[for instance, 2 edit(s)\] can be ‘Resumed’.

This process also works for other scenarios. Say you’re working on a change and get interrupted with a more important task: you can suspend your current work and take on the new task. When finished, you can resume your previous work exactly where you left off; bookmarks, breakpoints and all.

This is a powerful feature that has many uses. I hope you find it as useful as I do.
