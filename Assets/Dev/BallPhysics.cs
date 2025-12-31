using UnityEngine;

public class BallPhysics : MonoBehaviour
{

    [System.Serializable]
    public struct BallForce
    {
        public GoalKeypoint targetKeypoint;
        public Vector3 direction;
        public float force;

        public BallForce(GoalKeypoint targetKeypoint)
        {
            this.targetKeypoint = targetKeypoint;
            this.direction = Vector3.forward;
            this.force = 10;
        }
    }

    public Rigidbody ball;
    public BallForce[] forcesByTarget = new BallForce[]
    {
        new BallForce(GoalKeypoint.UpLeft),
        new BallForce(GoalKeypoint.UpMiddle),
        new BallForce(GoalKeypoint.UpRight),
        new BallForce(GoalKeypoint.GroundLeft),
        new BallForce(GoalKeypoint.GroundMiddle),
        new BallForce(GoalKeypoint.GroundRight),
    };

    private Vector3 _initialPosition = Vector3.zero;
    private Quaternion _initialRotation = Quaternion.identity;

    private void Awake()
    {
        if (ball == null)
            ball = GetComponent<Rigidbody>();

        _initialPosition = transform.position;
        _initialRotation = transform.rotation;
    }

    private void OnValidate()
    {
        if (ball == null)
            ball = GetComponent<Rigidbody>();
    }

    public void OnShoot(ShotResult shotResult)
    {
        foreach (BallForce forces in forcesByTarget)
        {
            if (forces.targetKeypoint == shotResult.playerTargetKeypoint)
            {
                ball.AddForce(forces.direction * forces.force, ForceMode.Impulse);
                return;
            }
        }

        Debug.LogWarning("No forces settings defined to resolve a shot to target " + shotResult.playerTargetKeypoint, this);
    }

    public void OnRestart()
    {
        ball.linearVelocity = Vector3.zero;
        ball.angularVelocity = Vector3.zero;

        transform.position = _initialPosition;
        transform.rotation = _initialRotation;
    }

}
