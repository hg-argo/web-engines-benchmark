[System.Flags]
public enum GoalKeypoint
{

    None = 0,

    // Components

    Up = 1 << 0,
    Ground = 1 << 1,

    Left = 1 << 2,
    Middle = 1 << 3,
    Right = 1 << 4,

    // Keypoints

    UpLeft = Up | Left,
    GroundLeft = Ground | Left,
    UpMiddle = Up | Middle,
    GroundMiddle = Ground | Middle,
    UpRight = Up | Right,
    GroundRight = Ground | Right,

}
