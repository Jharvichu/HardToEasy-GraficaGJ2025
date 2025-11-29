using System;
using UnityEngine;

public class BossController : MonoBehaviour
{
    #region Configuration

    [Header("Patrol Settings")]
    [SerializeField, Tooltip("Current target waypoint.")] 
    private Waypoint currentWaypoint;

    [SerializeField, Range(0f, 10f)] 
    private float movementSpeed = 3f;

    [SerializeField, Tooltip("Distance to waypoint to consider it reached.")]
    private float waypointThreshold = 0.1f;

    [Header("Detection Settings")]
    [SerializeField] private Transform playerTarget;
    [SerializeField, Range(1f, 20f)] private float detectionRange = 5f;
    [SerializeField, Range(0f, 360f)] private float viewAngle = 90f;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Animation Settings")]
    [SerializeField] private Animator _animator;

    #endregion

    #region Internal State

    private Waypoint _previousWaypoint;
    private PlayerController _playerController;
    private Vector3 _currentMoveDirection;
    private Vector3 _facingDirection = Vector3.right;
    private bool _hasCaughtPlayer = false;

    public event Action OnBossCaughtPlayer;

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        if (playerTarget != null)
        {
            _playerController = playerTarget.GetComponent<PlayerController>();
        }
    }

    private void Update()
    {
        if (_hasCaughtPlayer) return;
        HandleDetection();
        UpdateAnimations();
    }

    private void FixedUpdate() 
    {
        if (_hasCaughtPlayer) return;
        HandleMovement();
    }

    private void OnDrawGizmos()
    {
        DrawDetectionGizmos();
    }

    #endregion

    #region Main Logic

    private void HandleMovement()
    {
        if (currentWaypoint == null) 
        {
            _currentMoveDirection = Vector3.zero;
            return;
        }

        Vector3 moveDirection = (currentWaypoint.transform.position - transform.position).normalized;
        _currentMoveDirection = moveDirection; 

        UpdateFacingDirection(moveDirection);

        transform.position = Vector2.MoveTowards(
            transform.position, 
            currentWaypoint.transform.position, 
            movementSpeed * Time.deltaTime
        );

        if (HasReachedDestination()) SetNextWaypoint();

    }

    private void UpdateFacingDirection(Vector3 direction)
    {
        if (direction == Vector3.zero) return;

        _facingDirection = direction;
        
        Vector3 currentScale = transform.localScale;
        currentScale.x = direction.x > 0 ? Mathf.Abs(currentScale.x) : -Mathf.Abs(currentScale.x);
        transform.localScale = currentScale;
    }

    private bool HasReachedDestination()
    {
        return Vector2.Distance(transform.position, currentWaypoint.transform.position) < waypointThreshold;
    }

    private void SetNextWaypoint()
    {
        Waypoint temp = currentWaypoint;
        currentWaypoint = currentWaypoint.GetRandomNeighbor(_previousWaypoint);
        _previousWaypoint = temp;
    }

    private void HandleDetection()
    {
        if (playerTarget == null || _playerController == null) return;

        Vector3 dirToPlayer = playerTarget.position - transform.position;
        if (dirToPlayer.sqrMagnitude > detectionRange * detectionRange) return;

        if (!IsPlayerInFieldOfView(dirToPlayer)) return;

        CheckLineOfSight(dirToPlayer);
    }

    private bool IsPlayerInFieldOfView(Vector3 directionToPlayer)
    {
        float angleToPlayer = Vector3.Angle(_facingDirection, directionToPlayer);
        return angleToPlayer < viewAngle / 2f;
    }

    private void CheckLineOfSight(Vector3 directionToPlayer)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, detectionRange, obstacleLayer);

        if (hit.collider != null && hit.collider.transform == playerTarget)
        {
            EvaluatePlayerState();
        }
    }

    private void EvaluatePlayerState()
    {
        if (_playerController.CurrentState == PlayerState.Asleep)
        {
            _hasCaughtPlayer = true;
            OnBossCaughtPlayer?.Invoke();
            Debug.Log("Boss caught the player!");
        }
    }

    #endregion

    #region Animation

    private void UpdateAnimations()
    {
        float movementThreshold = 0.1f;

        _animator.SetBool("HorizontalWalk", Mathf.Abs(_currentMoveDirection.x) > movementThreshold);
        _animator.SetBool("DownWalk", _currentMoveDirection.y < -movementThreshold);
        _animator.SetBool("UpWalk", _currentMoveDirection.y > movementThreshold);
    }

    #endregion

    #region Debugging Tools

    private void DrawDetectionGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        float currentAngle = Mathf.Atan2(_facingDirection.y, _facingDirection.x) * Mathf.Rad2Deg;
        Vector3 viewAngleA = DirectionFromAngle(currentAngle, -viewAngle / 2);
        Vector3 viewAngleB = DirectionFromAngle(currentAngle, viewAngle / 2);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * detectionRange);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * detectionRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + _facingDirection * 2f);
    }

    private Vector3 DirectionFromAngle(float globalAngle, float angleOffset)
    {
        float totalAngle = globalAngle + angleOffset;
        return new Vector3(Mathf.Cos(totalAngle * Mathf.Deg2Rad), Mathf.Sin(totalAngle * Mathf.Deg2Rad), 0);
    }

    #endregion
}