using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

public class BallController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    private struct SwipePoint
    {
        public Vector2 position;
        public int frame;
        public float time;
    }

    public GameManager gameManager = null;

    [Tooltip("The distance (in pixels) to travel before considering the input as a swipe gesture.")]
    public float swipeDistanceThreshold = 150f;

    [Tooltip("Defines the maximum time interval (in seconds) to consider a swipe gesture as fast (targetting an \"Up\" keypoint). " +
        "\nThis interval is evaluated as soon as the swipe distance reaches the threshold.")]
    public float fastSwipeTimeInterval = 0.1f;

    [Tooltip("Defines the time (in seconds) for a registred swipe point to stay in the list before being removed.")]
    public float swipePointLifetime = 0.3f;

    private bool _canShoot = true;
    private bool _isSwiping = false;
    private Vector2 _swipeOrigin = Vector2.zero;
    private List<SwipePoint> _swipePoints = new List<SwipePoint>();

    private void Awake()
    {
        if (gameManager == null)
            gameManager = FindAnyObjectByType<GameManager>();
    }

    private void OnValidate()
    {
        if (gameManager == null)
            gameManager = FindAnyObjectByType<GameManager>();
    }

    public void OnRestart()
    {
        _canShoot = true;
    }

    private void Update()
    {
        if (!_isSwiping)
            return;

        // Remove all the points from the list that have been registred too long ago
        for (int i = _swipePoints.Count - 1; i >= 0; i--)
        {
            if (Time.timeSinceLevelLoad - _swipePoints[i].time >= swipePointLifetime)
                _swipePoints.RemoveAt(i);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_canShoot)
            return;

        _isSwiping = true;
        _swipePoints.Clear();
        _swipeOrigin = eventData.position;
        _swipePoints.Add(new SwipePoint { position = eventData.position, time = Time.timeSinceLevelLoad });
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_canShoot || !_isSwiping)
            return;

        _swipePoints.Add(new SwipePoint { position = eventData.position, time = Time.timeSinceLevelLoad });
        if (_swipePoints.Count >= 2)
        {
            // Get vector length from origin point
            float distance = Vector2.Distance(_swipePoints[_swipePoints.Count - 1].position, _swipeOrigin);

            // Cancel if the swipe gesture has not been completed yet
            if (distance < swipeDistanceThreshold)
                return;

            // Calculate average direction based on all segments between each registered points
            Vector2 averageDirection = Vector2.zero;
            for (int i = 1; i < _swipePoints.Count; i++)
                averageDirection += _swipePoints[i].position - _swipePoints[i - 1].position;
            averageDirection /= _swipePoints.Count;
            averageDirection.Normalize();

            float dotUp = Vector2.Dot(Vector2.up, averageDirection);
            // Cancel if the swipe gesture is performed downward
            if (dotUp < 0)
                return;

            GoalKeypoint targetKeypoint = GoalKeypoint.None;

            // Evaluate horizontal target based on swipe angle
            float dotRight = Vector2.Dot(Vector2.right, averageDirection);
            if (dotRight <= -0.25f)
                targetKeypoint |= GoalKeypoint.Left;
            else if (dotRight >= 0.25f)
                targetKeypoint |= GoalKeypoint.Right;
            else
                targetKeypoint |= GoalKeypoint.Middle;

            // Evaluate vertical target based on swipe gesture speed
            if (_swipePoints[_swipePoints.Count - 1].time - _swipePoints[0].time <= fastSwipeTimeInterval)
                targetKeypoint |= GoalKeypoint.Up;
            else
                targetKeypoint |= GoalKeypoint.Ground;

            ResolveShot(targetKeypoint);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isSwiping = false;
    }

    public void ResolveShot(GoalKeypoint targetKeypoint)
    {
        if (gameManager.RequireShot(targetKeypoint))
            _canShoot = false;
    }

}
