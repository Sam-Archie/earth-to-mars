# Martian Robots

Robots move across Mars, a bounded rectangle of squares, following instructions sent from Earth. A robot that moves off the edge of Mars is lost and leaves a scent behind.

## Language

**Mars**:
The bounded rectangle of squares the robots move on, from `0 0` to the upper-right corner Earth names. It holds nothing but its size.
_Avoid_: Grid, world, board, plateau

**Square**:
One `x y` spot on Mars, e.g. `3 3`. Scents and claims sit on squares.
_Avoid_: Coordinate, point, cell

**Orientation**:
The compass direction a robot faces: N, E, S or W. It decides which square a forward move goes to.
_Avoid_: Heading, direction, facing

**Position**:
A square plus an orientation, e.g. `3 3 N`. A robot starts at a position and its result reports one.
_Avoid_: Location

**Mission**:
One transmission from Earth: the size of Mars and the robots sent to it, run one after another. Scents and claims last for one mission.
_Avoid_: Session, game

**Scent**:
A marker left by a lost robot on the last square it occupied. A scented square stops any later robot from moving off Mars from it, over any edge.
_Avoid_: Mark, flag, warning

**Lost robot**:
A robot that moved off the edge of Mars. It is gone for good and leaves a scent.
_Avoid_: Dead, crashed, fallen

**Claim**:
Earth's record that a robot finished its instructions on a square without getting lost. Each square can be claimed once.
_Avoid_: Park, stake, mark

**Claimed square**:
A square that holds a claim. Other robots may pass through or finish on it, but cannot claim it again.
_Avoid_: Parked, occupied, taken

**Unknown region**:
A square no robot has claimed yet, as Earth sees it.
_Avoid_: Empty square, unexplored, blank

**Mission map**:
Earth's picture of Mars during a mission: its claims, scents and unknown regions, with north at the top.
_Avoid_: Grid, board, world view

**Command**:
One letter's action on a robot: turn left (`L`), turn right (`R`) or move forward (`F`). New command types may be added later.
_Avoid_: Instruction (for a single letter), move, action

**Instructions**:
The line of commands Earth sends to one robot, carried out in order.
_Avoid_: Command string, program, moves
