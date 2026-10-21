# Martian Robots

Robots move on a bounded rectangular grid on Mars, following instructions sent from Earth. A robot that moves off the grid is lost and leaves a scent behind.

## Language

**Mission**:
One transmission from Earth: a grid and the robots sent to it, run one after another. Scents and claims last for one mission.
_Avoid_: Session, game

**Scent**:
A marker left by a lost robot on the last square it occupied. A scented square stops any later robot from moving off the grid from it, over any edge.
_Avoid_: Mark, flag, warning

**Lost robot**:
A robot that moved off the edge of the grid. It is gone for good and leaves a scent.
_Avoid_: Dead, crashed, fallen

**Claim**:
Earth's record that a robot finished its instructions on a square without getting lost. Each square can be claimed once.
_Avoid_: Park, stake, mark

**Claimed square**:
A square that holds a claim. Other robots may pass through or finish on it, but cannot claim it again.
_Avoid_: Parked, occupied, taken

**Command**:
One letter's action on a robot: turn left (`L`), turn right (`R`) or move forward (`F`). New command types may be added later.
_Avoid_: Instruction (for a single letter), move, action

**Instructions**:
The line of commands Earth sends to one robot, carried out in order.
_Avoid_: Command string, program, moves
