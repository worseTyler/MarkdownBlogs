
While porting a C/C++ console application to C# I came across a construct that there doesn't seem to be any C# equivalent for.  Here is the C++ code:

> #include <iostream> // input and output functions using namespace std;
> 
> void main() { double i; // Prompt for the summation equation cout << "Enter the summation equation (e.g. 3+4.2-5=):";
> 
> // Assume user enters 9.2+15+3= // Read in the first number and write it out. cin >> i;
> 
> . . . }

The problem is that  cin >> i automatically retrieves the double portion of the text entered by the user.  To my knowledge, there is no equivalent C# construct.  Instead string parsing (perhaps regular expressions) is in order.
