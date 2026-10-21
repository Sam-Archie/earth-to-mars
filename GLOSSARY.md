# Martian Robots

Robots move on a bounded rectangular grid on Mars, following instructions sent from Earth. A robot that moves off the grid is lost and leaves a scent behind.

## Language

**Mission**:
One transmission from Earth: a grid and the robots sent to it, run one after another. Scents and claims last for one mission.
_Avoid_: Session, game

**Scent**:
A marker left by a lost robot on the last square it occupied, recording the edge it fell over. A scent guards only that edge of that square.
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
