
![](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2018/09/tic-tac-toe-game/images/tictactoe.jpg)

## Have you ever needed to complete a sample project during an interview process?

Some tech companies ask potential applicants to complete a tic tac toe game project to assess their coding abilities. The applicants are asked what they intend to create and how long they expect the project to take. As an interviewer, I wanted to assess how well it demonstrated my knowledge and determine how long it would take me to do a quick and dirty implementation.

### My Requirements:

1. A console app of standard Tic Tac Toe
2. .NET Core
3. Computer should to make smart moves like win and block
4. Computer needs to learn from successes and failures
5. Computer should play itself to get smart

### [](https://github.com/IntelliTect-Samples/TicTacToe#i-wanted-to-answer-a-few-questions)I wanted to answer a few questions:

1. Could I do it?
2. Is this a reasonable assignment just in terms of effort required?
3. What does this assess?

### Did I do it?

I guess the answer is yes. Try my game [here](https://repl.it/@ericksong/TicTacToe). I didn't do unit testing (see below). Does it work correctly? Maybe, but that is a non-trivial question, lots of cases. I thought it played a decent game. I actually saw it using a strategy that I had never considered (trying to trap the player using the side middle squares). It is easy to poison the cache by throwing games. However, the program should eventually recover from this by factoring in lost games.

### [](https://github.com/IntelliTect-Samples/TicTacToe#reasonableness)Reasonableness

It took me two and a half hours to complete the code to the initial check-in. That seems decent since most of the examples I have seen don't implement any type of learning which was by far the most time-consuming part, probably two hours. However, most applicants would probably do something more than a console app to make it pretty, which would most likely add another couple of hours to the project. Also, doing unit testing would add a few hours. So, this is about an eight-hour project to do 'right.'

### [](https://github.com/IntelliTect-Samples/TicTacToe#skill-assessment)Skill Assessment

This project forced me to do several things:

1. Prove general coding abilities and show my style
2. Create classes and separation of responsibilities
3. Devise patterns for learning algorithms
4. Demonstrate basic User Experience understanding
5. Use looping and logic structures
6. Store and look up data

### [](https://github.com/IntelliTect-Samples/TicTacToe#stuff-learned)Stuff Learned

1. This is a tricky problem with edge cases, is non-trivial and can be a bit humbling
2. I couldn't find a way just to have the machine learn with no knowledge of how to block and win.
3. Having two random players play against each other seems to produce more randomness. Maybe this is a factor of not playing enough games and needing to have better logic around eliminating bad games. It could also be due to how I had initially implemented the board matching, so it could subsequently work.
4. I enjoy solving the hard/interesting parts of problems and not the ones that I have done before.

### Was my code perfect?

No. I would say it was decent, maybe. This is far from ideal code, just one way to implement. There was little to no rework of the initial check in.

### [](https://github.com/IntelliTect-Samples/TicTacToe#note-on-testing)Note on Testing

I did this for fun and was more interested in the logic than the unit testing. I figured that adding unit testing would have double the effort because unit testing the learning code would have been a big hassle, and I didn't consider that fun. It would be great (and impressive) if someone else who really enjoys writing unit tests wanted to add that. Maybe another learning program could be written to play against this one to test it (or them both) thoroughly. I am concerned that the 'learning' logic has some issues.

### So, is tic tac toe a good interview task?

My major concern was in determining if the problem was reasonable to solve in the time allotted. I think the answer here is a qualified yes if the test is structured in such a way that the developer sets the expectations ahead of time on what the final product entails and also determines how much time they are willing to invest. The test demonstrates how someone can estimate a project's rough size, set expectations and meet deadlines. This is a huge part of being a developer.

On the technical side, tic tac toe is tricky because it doesn't have an obvious object-oriented approach. Unlike a college course guide or a salary tracking app, the core objects only get part way to a solution. I love the degree of latitude there is here in coming up with a 'good' solution, although I would consider myself an object-oriented pragmatist, others may disagree.

The task falls short in that it is a mostly intellectual exercise. At IntelliTect, we often write software for businesses which typically involves databases, websites, integrations and other practical use cases. The tic tac toe app doesn't really address any of these areas. However, we have tried other projects that might assess these areas, but these types of projects require substantially more hours to implement.

In the end, I think the task is a fair one that fundamentally assesses how well someone can think and demonstrates their basic grasp of software fundamentals as well as some interpersonal skills thrown in for flavor.

_Note: As an_ interviewer_, I love the tooling out there for searching for_ plagiarized _code._
