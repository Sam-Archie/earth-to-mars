namespace MartianRobots.Commands;

/// <summary>One letter's action on a robot. It only proposes the next position; the mission decides what happens at the edge of Mars.</summary>
internal interface ICommand
{
    Position Propose(Position current);
}
