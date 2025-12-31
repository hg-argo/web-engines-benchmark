using UnityEngine;

/// <summary>
/// Represents the resolution of a shot.
/// </summary>
[System.Serializable]
public struct ShotResult
{

    [Tooltip("The keypopnt aimed by the player.")]
    public GoalKeypoint playerTargetKeypoint;

    [Tooltip("The keypoint aimed by the NPC goalkeeper.")]
    public GoalKeypoint goalkeeperTargetKeypoint;

    public bool IsSuccess => playerTargetKeypoint != goalkeeperTargetKeypoint;
    public bool IsMiss => playerTargetKeypoint == goalkeeperTargetKeypoint;

}
