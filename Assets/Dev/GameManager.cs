using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The main script that drives the gameplay.<br/>
/// It also acts as a relay for global game events.
/// </summary>
public class GameManager : MonoBehaviour
{

    [Tooltip("Called when the player shoots and this manager has resolved the output.")]
    public UnityEvent<ShotResult> onShoot = new UnityEvent<ShotResult>();

    [Tooltip("Called at the moment the successful shot happens in the scene.")]
    public UnityEvent onSuccess = new UnityEvent();

    [Tooltip("Called when the player retries.")]
    public UnityEvent onRestart = new UnityEvent();

    [Header("Debug")]

    [Tooltip("Defines the keypoint that will be targetted by the NPC goalkeeper (for debug only).")]
    public GoalKeypoint forceGoalkeeperKeypoint = GoalKeypoint.None;

    /// <summary>
    /// Flag enabled if the player is allowed to shoot.
    /// </summary>
    private bool _canShoot = true;

    /// <inheritdoc cref="_canShoot"/>
    public bool CanShoot => _canShoot;

    /// <summary>
    /// Notifies the game that the player is about to shoot at a specific keypoint.
    /// </summary>
    /// <param name="targetKeypoint">The target keypoint the player is shooting at.</param>
    public bool RequireShot(GoalKeypoint targetKeypoint)
    {
        if (!_canShoot)
            return false;

        _canShoot = false;

        // Make the NPC goalkeeper pick a random keypoint
        GoalKeypoint goalkeeperKeypoint = GoalKeypoint.None;

        if (forceGoalkeeperKeypoint != GoalKeypoint.None && (Debug.isDebugBuild || Application.isEditor))
        {
            goalkeeperKeypoint = forceGoalkeeperKeypoint;
        }
        else
        {
            bool up = Random.Range(0, 1 + 1) == 1;
            if (up)
                goalkeeperKeypoint |= GoalKeypoint.Up;
            else
                goalkeeperKeypoint |= GoalKeypoint.Ground;

            int side = Random.Range(-1, 1 + 1);
            switch (side)
            {
                case -1:
                    goalkeeperKeypoint |= GoalKeypoint.Left;
                    break;
                case 1:
                    goalkeeperKeypoint |= GoalKeypoint.Right;
                    break;
                default:
                    goalkeeperKeypoint |= GoalKeypoint.Middle;
                    break;
            }
        }

        ShotResult result = new ShotResult
        {
            playerTargetKeypoint = targetKeypoint,
            goalkeeperTargetKeypoint = goalkeeperKeypoint
        };

        onShoot.Invoke(result);
        return true;
    }

    /// <summary>
    /// Notigies the game the feedbacks of the successful shot should happen.
    /// </summary>
    public void NotifySuccess()
    {
        onSuccess?.Invoke();
    }

    /// <summary>
    /// Notifies the game that the player retries.
    /// </summary>
    public void RequireRestart()
    {
        _canShoot = true;
        onRestart.Invoke();
    }

}
