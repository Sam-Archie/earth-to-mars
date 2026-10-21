namespace MartianRobots.Domain;

/// <summary>What happened to the square a robot finished on: it claimed it, found it already claimed, or claimed nothing because it was lost.</summary>
public enum ClaimOutcome { None, Claimed, AlreadyClaimed }
