
## ![](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2018/09/solve-any-year-game-puzzle/images/year-game.jpg)Ever try to write expressions for all the numbers from 1 to 100 using only the digits in the current year?

In 2017, I wrote a code to write these expressions for any year provided. [Click here](https://github.com/GrantErickson/Solve2017) for the Github link. Try it for yourself. [Click here](https://repl.it/@ericksong/solve2017) to see how many expressions can be found for your favorite year.

The original problem statement is to write an expression for every number from 1 to 100 using only the digits found in the current year (more info on the rules can be [found here](http://mathforum.org/yeargames/)).

Here is a description of the solution in the code.

Set up

1. Create an array with 2 0 1 7
2. Permutate this to get all possible combinations. for example 2 0 7 1, 7 1 0 2, etc.
3. The combine digits to get all combinations. for example: 21 7 0, 702 2, etc.
4. Add decimals between all variations and create more combinations. For example: .21 7 0, 2.1 7 0, 2.1 .7 0, etc.
5. We now have a set of all the possible input values. In fact, there are 960 of them.
6. Create a list of binary operators and unary operators as objects that can both evaluate the solution and check if a starting number is valid. These also do the syntactical formatting at the end.
7. It was also valid to negate numbers, but with minus and being able to divide the results of an exponent, that wasn't helpful.

Solution Approach

1. Rather than trying to figure out lots of equations which is an unbound problem, the thought was to figure out unique steps in the solution by doing a single operation each time. For example:
2. 2 0 1! 7 = 2 0 1 7. This isn't meaningful.
3. 2 0! 1 7 = 2 1 1 7. Then 2 1-1 7 = 2 0 7. However, this can also be gotten via 2 0\*1 7. And the latter is shorter so it is chosen as the go-ahead method to get 2 0 7 as an intermediate step.
4. The idea is to get from 4 numbers to 3 to 2 and finally to 1. These collections of 1 number should be all the possible solutions. By keeping track of all the unique combinations, this should find all the unique solutions.
5. Additionally, this is an unbound problem in a number of aspects. You can make super large numbers, especially with the unary factorial operator. I threw out any intermediate step with a number larger than 1000 and smaller than -1000. I also thought that there was no way to recover from numbers with lots of decimals like the results of square roots. These were also discarded. This may be the cause of the three missing numbers below, but I did run it with larger constraints and didn't find all the solutions.

Calculation Procedure

1. Start a do while loop with a bool that tracks whether any new intermediate solutions have been found.
2. Perform the unary operators (functions like sqrt or factorial that work on one number) on each number in each set. After each calculation on a single number see if that new combination is valid (not too big or have a decimal that is unusable) and unique. If it is, add it back to our list of combinations to try more operations.
3. Do the same thing for binary (cases with two input numbers like 2+1 or 2-1) operators. Keep any unique results.
4. The key to the above is that along with keeping the numbers we also keep the sequence of operations to get there that is the shortest. So is we find a simpler way to create a sequence of 2 numbers, we throw out the one from before and use the more efficient one.
5. We do this until we have a round where we don't find any new combinations of numbers.
6. At this point, we just take all the combinations that only have a single digit result and are between 1 and 100.

As you can see, this is a pretty brute force approach and many calculations are done more than once. It also calculates all the possible values, so we also know how to make lots of negative numbers and numbers greater than 100. Very useful things in life I hear.

No solution was found for 44, 87, and 88. These were the missing values on the original site as well (as of 8/28/2018). But if anyone has the solutions that would be great. The list below should be the solution with the minimum of operations. The complexity of operations wasn't rated so there may be other solutions with an equal number of operations.

1 = 217 ^ 0

2 = 210 / (7)!!

3 = 20 - 17

4 = (2 \* 7) - 10

5 = 012 - 7

6 = (20 - 17)!

7 = sqrt(70 - 21)

8 = (2.0 - 1) + 7

9 = (.20 / .1) + 7

10 = (2.0 + 1) + 7

11 = 12 - (7 ^ 0)

12 = 20 - (1 + 7)

13 = (20 \* 1) - 7

14 = 21.0 - 7

15 = 017 - 2

16 = 2.0 \* (1 + 7)

17 = 27 - 10

18 = (2 ^ 0) + 17

19 = 2 + 017

20 = 20 \* (1 ^ 7)

21 = 20 + (1 ^ 7)

22 = 21 + (7 ^ 0)

23 = .2 \* (10 + (7)!!)

24 = (2 \* 7) + 10

25 = (7 / .2) - 10

26 = 27.0 - 1

27 = 2.70 / .1

28 = 21.0 + 7

29 = (21 + (0)!) + 7

30 = 210 / 7

31 = (.2 \* (7)!!) + 10

32 = 2 \* (17 - (0)!)

33 = (2 \* 17) - (0)!

34 = 2 \* 017

35 = (01 / .2) \* 7

36 = (01 - 7) ^ 2

37 = 20 + 17

38 = ((sqrt((2 + 7)))!)!! - 10

39 = (7 ^ 2) - 10

40 = (01 + 7) / .2

41 = (0)! + ((1 + 7) / .2)

42 = ((2.0 + 1))! \* 7

43 = (10 / .2) - 7

44 = ?

45 = 10 + (7 / .2)

46 = ((07 - 1))!! - 2

47 = (7 ^ 2) - ((0)! + 1)

48 = (07 ^ 2) - 1

49 = 70 - 21

50 = (07)!! / 2.1

51 = 71 - 20

52 = ((07)!! - 1) / 2

53 = (01 + (7)!!) / 2

54 = 27 \* ((0)! + 1)

55 = (7)!! - (10 / .2)

56 = (10 - 2) \* 7

57 = (10 / .2) + 7

58 = 70 - 12

59 = 10 + (7 ^ 2)

60 = (7 - (.2 ^ 0)) / .1

61 = ((sqrt((2 + 7)))! / .1) + (0)!

62 = 72 - 10

63 = (2 ^ (7 - (0)!)) - 1

64 = 2.0 ^ (7 - 1)

65 = 70 - (1 / .2)

66 = 71 - ((0)! / .2)

67 = 70 - (2 + 1)

68 = (07 / .1) - 2

69 = 071 - 2

70 = (2 - 1) \* 70

71 = 072 - 1

72 = 01 \* 72

73 = 2 + 071

74 = (2 + (0)!) + 71

75 = (1 / .2) + 70

76 = ((0)! / .2) + 71

77 = (12 - (0)!) \* 7

78 = (((0)! + 7) / .1) - 2

79 = ((((2 + (0)!))!)! \* .1) + 7

80 = (17 - (0)!) / .2

81 = (2 + 7) ^ ((0)! + 1)

82 = 12 + 70

83 = (12 \* 7) - (0)!

84 = 012 \* 7

85 = 017 / .2

86 = (0)! + (17 / .2)

87 = ?

88 = ?

89 = ((2 + 7) / .1) - (0)!

90 = (2.0 + 7) / .1

91 = 20 + 71

92 = (7)!! - ((0)! + 12)

93 = (07)!! - 12

94 = ((0)! - 12) + (7)!!

95 = 102 - 7

96 = 201 - (7)!!

97 = (2 - 10) + (7)!!

98 = 2 \* (7 ^ ((0)! + 1))

99 = (07)!! - ((2 + 1))!

100 = (07)!! - (1 / .2)

_Note that this was just for fun and the code doesn't meet any particular coding standard aside from hopefully working. Don't use it as an example of how to write great code. Also, there are a number of things left intentionally commented out that are there as experiments._
