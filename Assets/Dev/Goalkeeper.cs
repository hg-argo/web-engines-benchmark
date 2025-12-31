using UnityEngine;

public class Goalkeeper : MonoBehaviour
{

    private const string HeightParam = "height";
    private const string SideParam = "side";
    private const string IsBlockingParam = "isBlocking";

    public Animator animator;
    [Tooltip("The collider to enable if the player misses its shot, or to enable if the goalkeeper can't catch it.")]
    public Collider goalBlocker;

    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        _initialPosition = transform.position;
        _initialRotation = transform.rotation;
    }

    private void OnValidate()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void OnShoot(ShotResult result)
    {
        // Set "height" param
        animator.SetInteger(HeightParam, result.goalkeeperTargetKeypoint.HasFlag(GoalKeypoint.Up) ? 1 : 0);
        
        // Set "side" param
        if (result.goalkeeperTargetKeypoint.HasFlag(GoalKeypoint.Left))
            animator.SetInteger(SideParam, -1);
        else if (result.goalkeeperTargetKeypoint.HasFlag(GoalKeypoint.Right))
            animator.SetInteger(SideParam, 1);
        else
            animator.SetInteger(SideParam, 0);

        // Play block anim accordingly
        animator.SetBool(IsBlockingParam, true);
        goalBlocker.enabled = result.IsMiss;
    }

    public void OnRestart()
    {
        animator.SetBool(IsBlockingParam, false);
        transform.position = _initialPosition;
        transform.rotation = _initialRotation;
    }

}
