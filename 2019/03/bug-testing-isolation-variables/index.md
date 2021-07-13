

##  "Painless Bug Testing through the Isolation of Variables"

## Do you know why it’s crucial to catch your broken functionality before sending it to production?

QA resources waste time through long, protracted test efforts that try to hammer all of their bug testing through the UI at the end of (or after) development. One of the main problems with this approach is that it becomes tough to isolate functionality so that we are only testing one thing at a time with a known codebase. Isolating variables can help produce more reliable, accurate results in testing.

Let’s say there’s a defect at the tail end of a long development cycle; we’re three months into working on a new feature, and I find out I can’t log in with one specific password. It’s a valid password, so I file a defect. By the time the developer looks into it, they end up several layers deep trying to modify a class potentially written by someone no longer at the company. All the developer knows is that when they tweak the class to accept the broken password found by QA, it breaks their own test for validating passwords.

If we isolate variables appropriately and quickly, then when that one thing breaks we can write a concise, accurate bug report for the developer to turn around and implement a fix, ideally within a sprint of the original implementation. If we test one thing at a time, we have a better idea of what is broken.

### The ultimate goal: no broken functionality sent to production.

_If_ functionality is broken, then it can be fixed quickly and efficiently before production. To that end, proper functionality isolation happens at various levels of an application and has many benefits. Here’s a brief list of isolation points and some example associated testing methodologies:

1. Individual functions (automated unit testing)
2. Data access layer (automated service tests)
3. Data repository (boundary analysis, referential integrity checks)
4. Integration layer (automated service tests, integrated UI tests)
5. UI layer (functional UI tests)

Find some excellent concrete examples [here](https://martinfowler.com/articles/practical-test-pyramid.html).

### Which bug testing category does your team fall into?

One benefit to isolating these various systems is that, should a defect occur, we can provide better results (whether it be a bug report or something else) because we can identify the layers affected by a defect. If we think of bug testing and bug reports in a progression of least informational to most, teams tend to fall into one of the following categories:

1. **No testing/No bug reports:** Either the team doesn’t test, or there is a critical disconnect between developers and defect reports. "Painless Bug Testing through the Isolation of Variables"
2. **Unfocused testing/Inaccurate bug reports:** In this case, bug reports aren’t indicative of the actual problem. These tests and bug reports are prone to producing red herrings and require the developer to spend inordinate amounts of time investigating issues that end up either not being issues or take more time to reproduce than they do to fix. "Painless Bug Testing through the Isolation of Variables"
3. **Good testing/Good bug reports:** Tests are focused on fulfilling a need (testing for a business requirement, testing for a functional requirement, etc.) and bug reports have accurate reproduction steps for how the issue occurred and approximately when the problem started. "Painless Bug Testing through the Isolation of Variables"
4. **Great testing:** Tests effectively isolate different functionality in the application, and as a result, bug reports can isolate the exact layer that failed, when the failure started, and list accurate reproduction steps. "Painless Bug Testing through the Isolation of Variables"

### Strive for good testing or great testing.

Good testing offers some known room for improvement, and it helps the developer quickly identify what is wrong. It could be the highest attainable goal if a QA person is not working directly with the dev team.

By having QA and devs work together to identify and test small pieces of functionality quickly, the turnaround on bug fixing will often be quick and painless. Prolonging the testing cycle, moving it to a group with little contact with the developers, or bug testing without this mindset will produce bug reports that are hard to identify and fix.

I am a big fan of teams that continually assess their current strengths and weaknesses and try to increase their efficiency, quality and reliability by maximizing their strengths and improving on their weaknesses.

### Where would you rate your own team on the effectiveness of their testing?

While thinking about it, don’t forget that you are [not alone](https://james-willett.com/2016/09/the-evolution-of-the-testing-pyramid/) in trying to [figure out](https://medium.com/@fistsOfReason/testing-is-good-pyramids-are-bad-ice-cream-cones-are-the-worst-ad94b9b2f05f) how to get your team to the [next level](https://www.mountaingoatsoftware.com/blog/the-forgotten-layer-of-the-test-automation-pyramid).
