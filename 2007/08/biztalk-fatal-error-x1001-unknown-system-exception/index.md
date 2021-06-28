
The other day I ran across a very puzzling error: fatal error X1001: unknown system exception

Fortunately, the error message was so descriptive; I was able to resolve my problem right away.

Ok, so I couldn't, for the life of me, figure out what I had done wrong that would produce such a nasty error. Upon closing Visual Studio and re-opening the solution, I still received the same error message. Google and newsgroups proved to be of little help - a rare occurrence for me. Unfortunately, "unknown system exception" could stand for a variety of problems. However, I did stumble across one link that managed to solve my problem.

Jan Eliasen's [post](http://blog.eliasen.dk/PermaLink,guid,5c2d6137-582b-4ead-8481-583aec28a0ac.aspx) definitely got me back on track. Essentially, I had created a decision block to "comment" out some code. However, in my else branch, I had a Call Orchestration shape. Apparently, BizTalk does not like to have a Call or Start Orchestration shape at an unreachable code location.

Since I was using a 1==1 expression as my first rule, the else branch would never be reached.

As I see it, there are 2 workarounds:

1. Delete the Start/Call Orchestration shape from the "unreachable code"
    1. The only problem with this option is you have to remember to put it back.
2. Use a variable instead of 1 == 1
    1. The trick would be to declare a variable (i) with a default value of 1. Then, use the expression i == 1. A little crude, but it works.
