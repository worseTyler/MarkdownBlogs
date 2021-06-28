
Here’s a fact: Microsoft hosts around 2,000 open source software (OSS) repositories on GitHub, including some rather large ones like the .NET Compiler Platform, also known as “Roslyn,” which has as many as 4 million lines of code. For many developers, the prospect of submitting code changes to a project that runs on millions of computers might seem daunting. Fortunately, you don’t need a Ph.D. in programming languages and compilers to make your mark on Microsoft OSS projects. There are opportunities to contribute that span a broad spectrum of difficulty and experience, from beginner to expert.

I got my start in March of 2018 working with the .NET Core team to add a new set of APIs. I was able to jump on board thanks to my connections at Microsoft, specifically with Program Manager Kathleen Dollard. At the time I wondered, “How hard would it be for someone that wasn’t well connected at Microsoft to get involved?” With this question in mind, I decided to do some research and find out. In this article, I explore the topic of contributing to Microsoft OSS and what it takes for newcomers to get involved.

### Getting Started: Documentation and Pull Request Review

Perhaps the best place to begin is with documentation. If you navigate to any of the .NET documentation pages (for example, [bit.ly/2LAv7hA](https://bit.ly/2LAv7hA)) you’ll notice at the bottom of each page that there’s a footer soliciting feedback, as shown in **Figure 1**.

![Picture of footer soliciting feedback](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2018/10/how-to-contribute-to-microsoft-open-source-software-projects-msdn/images/Figure-1.png)

Figure 1 Submitting Suggestions and Changes to Documentation

From here you can click “Product feedback,” submit a new issue, or browse and search existing issues. Even better, the second button takes you directly to the GitHub issue list for the specific page you were browsing, so you can either create a GitHub issue or navigate to the documentation source code itself (such as [github.com/dotnet/docs](https://github.com/dotnet/docs)) and fix the issue directly. Frequently, just updating the documentation and submitting a pull request (PR) is easier than the work required to document the problem.

I’ve spoken with team members directly and they emphasize that all submissions are welcome, even spelling and grammar corrections. These changes may not be exciting, but they can make the difference between a successful API and an unsuccessful one.

Further, the docs team is one of the fastest to respond to raised issues and PRs, with staff assigned to each area to address contributions no matter how small. One reason documentation editing is easy: It typically doesn’t require you to clone the repo to submit changes. Rather, you can use the GitHub Web-based editing UI, which automatically forks and submits the PR for you.

PR review is also an important way to contribute. Every project needs PR feedback and the Microsoft team is grateful for PR review contributions. I know I’ve appreciated—and learned from—reviews of works I’ve submitted to .NET. The biggest lesson for me is that I can’t jump in and make a significant contribution casually or in the slivers of time between doing other things. Code at this level requires careful thought, careful implementation and significant collaboration. In the end, I’m grateful for both the PRs rejected and the PRs accepted. PR reviews are a great place to step in and help with open source development.

### First-Time Approachable Issues

For those ready to take on more than documentation, Microsoft offers some guidance. Issues that are tagged with descriptors like “first issue” and “easy” are great candidates for developers who are new to the game. Microsoft even asks active project members to steer clear of first-time approachable issues until near the end of a release, thus keeping easier issues available for developers new to the project and lowering the complexity bar for getting started. Furthermore, first-time approachable issues frequently document links that describe where to look for the problem, rather than leaving new developers flailing in an attempt to locate a specific defect in the haystack that’s a large repo. For the Roslyn team, for example, a good first issue must include:

- Links to the file where the fix is likely to be necessary
- Identification of where the tests need to go
- Setup instructions for getting started with Roslyn
- General contribution policy

The commitment to favor contributions from newcomers extends to how PRs are accepted. For PRs tagged as a “good first issue,” Microsoft gives preference to new contributors, accepting their PRs over those from existing contributors. What’s more, the Microsoft staffer who tags an issue as a “good first issue” will directly answer questions from people working to fix the issue. That bit of hand-holding can be important in the first stages of involvement.

Clearly, Microsoft goes out of its way to get first-time contributors engaged. Roslyn, for example, is a complicated, 4-million-line code base that’s not for the faint of heart. By engaging new contributors, Microsoft enables an active community of external developers in its OSS efforts.

### Regular Contributions

Once you have your first PR accepted, you’re likely going to take on more complex issues and features. There are issue tags like “help wanted” and “up for grabs” that indicate that an issue is likely a good target for the community—although not necessarily ideal for first-timers. (See [up-for-grabs.net](https://up-for-grabs.net) to browse through the list of projects and corresponding issues tags such as for first-time approachable or help wanted.) Issues labeled with these tags likely involve more work or greater knowledge of the project than a newcomer can provide; however, they’re well defined and don’t need extensive collaboration with the project team. On the other hand, there’s a defined workflow that you would be wise to follow:

- Contributions beyond the level of a bug fix should be discussed with the team and within the scope of the roadmap to avoid being declined
- Contributions should be against master—not against an experimental feature branch
- PRs must merge easily with the tip of the master branch
- Contributors should make sure to sign the [Contributor License Agreement](https://dotnetfoundation.org/projects/submit)

As developers experienced with Git would expect, be sure that you work in a local fork (cloned to your computer) and then submit code for consideration via a PR. Of course, you can create a branch locally, but when you submit your PR you’ll be submitting to master.

There are a few programming and workflow guidelines to keep in mind. First, there’s the coding style to consider. While you can find the C# Coding Style at [bit.ly/2woQv3u](https://bit.ly/2woQv3u), the general summary is to follow the standard of the existing file. This is true even if the existing file differs from the documented standard. This means that until you’re coding entire files (or adding items for which there’s no precedence already in the file), the guideline is easy—follow the example of the rest of the file. Even without precedence, however, there are only 16 items listed in the C# coding style document, none of which are particularly surprising. These items include:

- Curly braces on their own line
- Fours spaces for indentation (not tabs)
- \_camelCase for internal private fields and PascalCasing for constant local variables and fields
- Avoid this. unless required
- Always specify the visibility (that is, use private even if the member defaults to private)
- Avoid more than one empty line to break up the code
- Only use var when the assigned type is obvious (see itl.tc/UsingVar), with the exception of Roslyn projects, which use var everywhere
- Specify fields at the top within a type declaration

Generally, you’ll find an .editorconfig (see editorconfig.org) settings file for each directory, enforcing these standards. Be sure to use the file to ensure you follow the guidelines and avoid having your PR blocked.

For those coding in Visual Basic, follow the spirit and intent of the C# guidelines.

Although not mentioned in the list, unit tests are critical to producing the required level of quality.

### Design Help

Some repositories like language, CoreFX, and Dotnet CLI require a significantly greater level of experience and expertise and, as such, employ a different process. With these libraries, the entry point is at the discussion level rather than the code level. Directly submitting a code PR to these libraries with a new feature or language keyword is unlikely to be successful.

While the design process is generally visible, it’s not a free-for-all. In fact, submissions for these repositories don’t even start with proposals. (Check out the C# language proposals folder at [bit.ly/2BVUbjf](https://bit.ly/2BVUbjf) for a good look at the main features currently under consideration.) Rather, if you want to suggest a feature, you start by submitting an issue and tagging it with the “Discussion” label. If discussion items reach some level of consensus, such that further evaluation should be considered, they’re picked up in the Language Design Meetings, which in turn are informed by further discussion, experiments and offline design work. Proposals themselves—not finalized features—are then championed by members of the language design team.

While the process is intended to be open to feedback, not everyone can just make changes however they choose. The volume of distribution and the impact of change is too great to not be tightly controlled (very similar to the control that Linus Torvalds has with Linux). In the end, the project is still open source. If you want the freedom to change the code in whatever way your heart desires, simply fork the repo and get started.

This approach is an important way to collaborate well before code implementation begins. Even then, changes are likely to be in a separate branch for an extended period while they’re programmed (and repeatedly re-programmed) and evaluated. The community is essential in deciding what the shape of something is going to be. Opening a discussion issue, commenting, and providing feedback with existing proposals provides direct access to the team.

You’ll observe that contributing can run the gamut from simple to extremely difficult. Adding a method or class to an API, for example, is one thing, but adding a new language keyword is something else entirely.

### What Happens After Submitting a PR

In April of 2018, the Roslyn team realized that they had fallen behind on processing all the PRs that had been submitted. With all the changes that had occurred after the PRs, it was likely that some of them would no longer be relevant. To address the problem, Microsoft stepped up and assigned staff to each project. It was their job to respond to all future PRs to ensure that the effort put in was more likely to lead to success. Toward that effort, they put in place the following categorization of PRs:

- Approved by Project Lead: Approved PRs are assigned a buddy or coach from the project team to improve the chances of successful adoption and help incorporate the PR into the code base. The coach ensures that community members are engaged, following up with contributors within three business days if the PR is rejected for whatever reason.
- Pending Discussion: Sometimes significant concerns crop up—unit tests are missing, the purpose is unclear or the code fails significantly to meet the guidelines. In these instances, the project lead raises the concerns with the community contributor, identifying what needs to happen. The onus is on the contributor to follow up within two weeks. Furthermore, PRs in this grouping need to keep up with the code base during this time.
- Rejected: The PR isn’t in line with the vision of the project, involves too much risk or doesn’t successfully solve a priority. In this case the lead will reject the PR, clearly identifying the issue. While the PR can be resubmitted, it will require significant change or rework.

### Wrapping Up

Occasionally you can observe behavior within opens source projects that’s well below the standards of decency. This can include participants who are rude, intolerant or repeatedly fail to listen to opposing views. It also includes contributors who fail to accept Microsoft’s role as the final arbiter of Microsoft OSS projects, repeatedly spam a repo or otherwise disrupt the collaborative process. Anyone that can’t follow the rules of general civility will find themselves blocked from a repo (and returning under a pseudonym with the same behavior will not get you any further). Microsoft is committed to making participation a positive experience for all, and enforcing the Code of Conduct is core to that commitment.

I encourage you to review the Microsoft Open Source Code of Conduct at [bit.ly/2wmAYlB](https://bit.ly/2wmAYlB). Also review its associated FAQ at [bit.ly/2NwNNRa](https://bit.ly/2NwNNRa).

In my experience, how you approach making changes to Microsoft OSS depends largely on what generates your interest in wanting a change. I expect that for most of you the catalyst is a problem in the form of a defect or a missing feature. Initially, the trigger may be a flaw or issue in the documentation and the desire to fix it for other readers. Alternatively, perhaps you’re working in the Xamarin code base and discover that a method you wish to override isn’t virtual, so you submit a PR to make it such.

Some of you will want to take on an even bigger challenge. With .NET Core, I’ve been astounded by the fact that there (still) isn’t one command-line parser that can easily accept the command-line arguments and parse them into a strongly typed object from which I can access the values in my program. It was this itch that prompted me to start collaborating with Microsoft’s Jon Sequeira (who wrote the command-line parser for Dotnet CLI) to build such a parser. Alas, the code is still too unstable and our participation too casual for us to take the project open source. Hopefully it won’t be too long before this project is something we can open to the public, so it too can benefit from the engagement of the OSS community. In the interim, if you have significant time to dedicate and have an interest in our parser project, send an e-mail to Kathleen or me and we can work out a way to get you involved. And, yes, I just introduced yet another way to contribute—volunteering before the code is public.

_Thanks to the following Microsoft technical experts for their help collaborating and reviewing this article: Kevin Bost (IntelliTect), Kathleen Dollard, Neal Gafter, Sam Harwell, Immo Landwerth, Jared Parsons, Jon Sequeira, Bill Wagner, Maira Wenzel_

_This article was originally posted_ [_here_](https://docs.microsoft.com/en-us/archive/msdn-magazine/2018/october/essential-net-how-to-contribute-to-microsoft-open-source-software-projects) _in the October 2018 issue of MSDN Magazine._
