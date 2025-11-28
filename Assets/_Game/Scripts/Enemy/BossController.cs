using System;
using System.Collections;
using UnityEngine;

public enum BossState
{
    Patrolling,   // Movimiento orbital normal
    Distracted    // Pausado temporalmente (ej: por minijuego)
}

public class BossController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private BossData bossData;

    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform playerCubiclePosition;
    [SerializeField] private Transform[] waypoints;

    [Header("Debug")]
    [SerializeField] private bool showDetectionGizmos = true;

    // Events
    public event Action OnPlayerCaught;
    public event Action<BossState> OnBossStateChanged;

    // State
    private BossState currentState = BossState.Patrolling;
    private int currentWaypointIndex = 0;
    private float lastDetectionTime = -999f;
    private PlayerState cachedPlayerState = PlayerState.Awaken;
    private Coroutine distractionCoroutine;

    private void Awake()
    {
        ValidateDependencies();
    }

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void Start()
    {
        InitializeState();
    }

    private void Update()
    {
        switch (currentState)
        {
            case BossState.Patrolling:
                UpdatePatrolling();
                PerformDetectionCheck();
                break;

            case BossState.Distracted:
                // No hacer nada, solo esperar
                break;
        }
    }

    #region Initialization & Validation

    private void ValidateDependencies()
    {
        if (bossData == null)
        {
            Debug.LogError("BossData is not assigned to BossController!", this);
        }

        if (playerController == null)
        {
            Debug.LogError("PlayerController is not assigned to BossController!", this);
        }

        if (playerCubiclePosition == null)
        {
            Debug.LogError("PlayerCubiclePosition is not assigned to BossController!", this);
        }

        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("Waypoints array is empty! BossController needs at least one waypoint.", this);
        }
    }

    private void SubscribeToEvents()
    {
        if (playerController != null)
        {
            playerController.OnPlayerStateChanged += OnPlayerStateChanged;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (playerController != null)
        {
            playerController.OnPlayerStateChanged -= OnPlayerStateChanged;
        }
    }

    private void InitializeState()
    {
        ChangeState(BossState.Patrolling);
    }

    #endregion

    #region Movement System

    private void UpdatePatrolling()
    {
        if (waypoints == null || waypoints.Length == 0 || bossData == null)
            return;

        // Obtener waypoint actual
        Transform targetWaypoint = waypoints[currentWaypointIndex];

        if (targetWaypoint == null)
        {
            Debug.LogWarning($"Waypoint at index {currentWaypointIndex} is null!");
            return;
        }

        // Calcular dirección hacia el waypoint
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;

        // Mover hacia el waypoint
        transform.position += direction * bossData.MoveSpeed * Time.deltaTime;

        // Orientar hacia dirección de movimiento
        if (direction != Vector3.zero)
        {
            transform.right = direction;
        }

        // Verificar si llegó al waypoint
        float distance = Vector3.Distance(transform.position, targetWaypoint.position);
        if (distance < bossData.WaypointReachThreshold)
        {
            // Avanzar al siguiente waypoint
            if (bossData.RandomWaypoints)
            {
                // Elegir siguiente waypoint aleatoriamente (evitando el actual si hay más de uno)
                if (waypoints.Length > 1)
                {
                    int newIndex;
                    do
                    {
                        newIndex = UnityEngine.Random.Range(0, waypoints.Length);
                    }
                    while (newIndex == currentWaypointIndex);
                    currentWaypointIndex = newIndex;
                }
            }
            else
            {
                // Avanzar secuencialmente
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            }

            Debug.Log($"Boss reached waypoint! Moving to waypoint {currentWaypointIndex}");
        }
    }

    #endregion

    #region Detection System

    private void PerformDetectionCheck()
    {
        // Cooldown para evitar detecciones múltiples
        if (Time.time < lastDetectionTime + bossData.DetectionCooldown)
            return;

        if (IsPlayerInRange() && IsPlayerInFOV() && IsPlayerAsleep())
        {
            TriggerGameOver();
            lastDetectionTime = Time.time;
        }
    }

    private bool IsPlayerInRange()
    {
        if (playerCubiclePosition == null)
            return false;

        float distance = Vector3.Distance(transform.position, playerCubiclePosition.position);
        return distance <= bossData.DetectionRange;
    }

    private bool IsPlayerInFOV()
    {
        if (playerCubiclePosition == null)
            return false;

        Vector3 directionToPlayer = (playerCubiclePosition.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.right, directionToPlayer);
        return angle <= bossData.DetectionFOV / 2f;
    }

    private bool IsPlayerAsleep()
    {
        return cachedPlayerState == PlayerState.Asleep;
    }

    private void OnPlayerStateChanged(PlayerState newState)
    {
        cachedPlayerState = newState;
        Debug.Log($"Boss cached player state: {newState}");
    }

    private void TriggerGameOver()
    {
        Debug.Log("Player caught sleeping!");
        OnPlayerCaught?.Invoke();
    }

    #endregion

    #region State Management

    private void ChangeState(BossState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            OnBossStateChanged?.Invoke(currentState);
            Debug.Log($"Boss state changed to: {currentState}");
        }
    }

    #endregion

    #region Distraction System

    /// <summary>
    /// Distrae al jefe por una duración específica
    /// </summary>
    /// <param name="duration">Duración en segundos</param>
    public void Distract(float duration)
    {
        if (currentState == BossState.Patrolling)
        {
            ChangeState(BossState.Distracted);

            // Cancelar distracción anterior si existe
            if (distractionCoroutine != null)
            {
                StopCoroutine(distractionCoroutine);
            }

            distractionCoroutine = StartCoroutine(DistractionTimer(duration));
        }
    }

    /// <summary>
    /// Distrae al jefe por la duración por defecto
    /// </summary>
    public void Distract()
    {
        Distract(bossData.DefaultDistractionDuration);
    }

    private IEnumerator DistractionTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        ChangeState(BossState.Patrolling);
        distractionCoroutine = null;
    }

    /// <summary>
    /// Método de prueba para distraer al jefe desde el Inspector
    /// </summary>
    [ContextMenu("Test Distract 5 seconds")]
    public void TestDistract()
    {
        Distract(5f);
    }

    #endregion

    #region Debug Gizmos

    private void OnDrawGizmos()
    {
        if (!showDetectionGizmos)
            return;

        // Dibujar waypoints y camino
        if (waypoints != null && waypoints.Length > 0)
        {
            Gizmos.color = Color.cyan;

            // Dibujar cada waypoint como una esfera
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] != null)
                {
                    // Waypoint actual en color diferente
                    if (Application.isPlaying && i == currentWaypointIndex)
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawWireSphere(waypoints[i].position, 0.7f);
                    }
                    else
                    {
                        Gizmos.color = Color.cyan;
                        Gizmos.DrawWireSphere(waypoints[i].position, 0.5f);
                    }

                    // Dibujar número del waypoint
                    #if UNITY_EDITOR
                    UnityEditor.Handles.Label(waypoints[i].position + Vector3.up * 0.5f, $"WP {i}");
                    #endif
                }
            }

            // Dibujar líneas conectando waypoints (solo si no es aleatorio)
            if (bossData != null && !bossData.RandomWaypoints)
            {
                Gizmos.color = new Color(0, 1, 1, 0.5f); // Cyan semi-transparente
                for (int i = 0; i < waypoints.Length; i++)
                {
                    if (waypoints[i] != null)
                    {
                        int nextIndex = (i + 1) % waypoints.Length;
                        if (waypoints[nextIndex] != null)
                        {
                            Gizmos.DrawLine(waypoints[i].position, waypoints[nextIndex].position);
                        }
                    }
                }
            }

            // Dibujar línea hacia el waypoint actual
            if (Application.isPlaying && waypoints[currentWaypointIndex] != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, waypoints[currentWaypointIndex].position);
            }
        }

        // Dibujar rango de detección
        if (bossData != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, bossData.DetectionRange);
        }

        // Dibujar campo de visión (FOV)
        if (bossData != null && Application.isPlaying)
        {
            Gizmos.color = Color.red;
            DrawFOVCone();
        }

        // Dibujar línea hacia el jugador si está en rango
        if (playerCubiclePosition != null && Application.isPlaying && IsPlayerInRange())
        {
            Gizmos.color = IsPlayerInFOV() ? Color.green : Color.gray;
            Gizmos.DrawLine(transform.position, playerCubiclePosition.position);
        }
    }

    private void DrawFOVCone()
    {
        float halfFOV = bossData.DetectionFOV / 2f;
        float coneLength = bossData.DetectionRange;

        // Calcular los dos bordes del cono
        Quaternion leftRotation = Quaternion.Euler(0, 0, halfFOV);
        Quaternion rightRotation = Quaternion.Euler(0, 0, -halfFOV);

        Vector3 leftEdge = leftRotation * transform.right * coneLength;
        Vector3 rightEdge = rightRotation * transform.right * coneLength;

        // Dibujar líneas del cono
        Gizmos.DrawLine(transform.position, transform.position + leftEdge);
        Gizmos.DrawLine(transform.position, transform.position + rightEdge);

        // Dibujar arco del cono
        int arcSegments = 20;
        Vector3 prevPoint = transform.position + rightEdge;
        for (int i = 1; i <= arcSegments; i++)
        {
            float t = i / (float)arcSegments;
            float angle = Mathf.Lerp(-halfFOV, halfFOV, t);
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Vector3 newPoint = transform.position + (rotation * transform.right * coneLength);
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }

    #endregion
}
