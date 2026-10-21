# Developer Programming Challenge — The Doctors Laboratory

Transcribed from `TDL_Developer_Programming_Challenge.pdf`. This is the source of truth for the problem; the README describes the solution.

## Before you start

- Spend no more than two or three hours. If you run out of time, send what you have with a note on what you would do next.
- The deliverable is the program itself. No user interface, web application, database or deployment pipeline. Use the README to describe briefly what you would build around the program if it were going further: how you would surface it, where the data would live, how you would deploy and run it.
- Approach it as a piece of work delivered inside a cross-functional team. Include a README with instructions for running it, an explanation of the technical choices made, and an overview of how the problem was approached.
- Unit testing matters.
- Use whichever language and frameworks you are most comfortable with. The code should run without any unusual setup steps.
- Put the code in a public source repository and send the URL, including commit history.
- AI tooling is assumed and will not count against you. Say in the README how it was used and where you chose not to.
- Where the brief leaves something open, make your own call and note the assumption in the README. No questions are expected before submitting.

## Problem: Martian Robots

The surface of Mars can be modelled by a rectangular grid around which robots are able to move according to instructions provided from Earth. Write a program that determines each sequence of robot positions and reports the final position of the robot.

A robot position consists of a grid coordinate (a pair of integers: x-coordinate followed by y-coordinate) and an orientation (N, S, E, W for north, south, east, and west).

A robot instruction is a string of the letters "L", "R", and "F" which represent, respectively:

- **Left**: the robot turns left 90 degrees and remains on the current grid point.
- **Right**: the robot turns right 90 degrees and remains on the current grid point.
- **Forward**: the robot moves forward one grid point in the direction of the current orientation and maintains the same orientation.

The direction North corresponds to the direction from grid point (x, y) to grid point (x, y+1).

There is also a possibility that additional command types may be required in the future and provision should be made for this.

Since the grid is rectangular and bounded (yes, Mars is a strange planet), a robot that moves "off" an edge of the grid is lost forever. However, lost robots leave a robot "scent" that prohibits future robots from dropping off the world at the same grid point. The scent is left at the last grid position the robot occupied before disappearing over the edge. An instruction to move "off" the world from a grid point from which a robot has been previously lost is simply ignored by the current robot.

## The input

The first line of input is the upper-right coordinates of the rectangular world; the lower-left coordinates are assumed to be 0, 0.

The remaining input consists of a sequence of robot positions and instructions (two lines per robot). A position consists of two integers specifying the initial coordinates of the robot and an orientation (N, S, E, W), all separated by whitespace on one line. A robot instruction is a string of the letters "L", "R", and "F" on one line.

Each robot is processed sequentially, i.e. it finishes executing its instructions before the next robot begins.

- The maximum value for any coordinate is 50.
- All instruction strings will be less than 100 characters in length.

## The output

For each robot position/instruction in the input, the output should indicate the final grid position and orientation of the robot. If a robot falls off the edge of the grid the word "LOST" should be printed after the position and orientation.

## Sample

Input:

```
5 3
1 1 E
RFRFRFRF
3 2 N
FRRFLLFFRRFLL
0 3 W
LLFFFLFLFL
```

Output:

```
1 1 E
3 3 N LOST
2 3 S
```
