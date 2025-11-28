using UnityEngine;

[System.Serializable]
public class BossData
{
    [Header("Movement Settings")]
    [Tooltip("Velocidad de movimiento del jefe")]
    [SerializeField] private float moveSpeed = 2f;

    [Tooltip("Distancia mínima para considerar que llegó al waypoint")]
    [SerializeField] private float waypointReachThreshold = 0.5f;

    [Tooltip("Elegir waypoints de forma aleatoria en lugar de secuencial")]
    [SerializeField] private bool randomWaypoints = false;

    [Header("Detection Settings")]
    [Tooltip("Distancia máxima para detectar al jugador")]
    [SerializeField] private float detectionRange = 3f;

    [Tooltip("Campo de visión en grados")]
    [SerializeField] private float detectionFOV = 90f;

    [Tooltip("Tiempo de cooldown entre detecciones")]
    [SerializeField] private float detectionCooldown = 0.5f;

    [Header("State Settings")]
    [Tooltip("Duración de la distracción por defecto")]
    [SerializeField] private float defaultDistractionDuration = 5f;

    // Properties
    public float MoveSpeed => moveSpeed;
    public float WaypointReachThreshold => waypointReachThreshold;
    public bool RandomWaypoints => randomWaypoints;
    public float DetectionRange => detectionRange;
    public float DetectionFOV => detectionFOV;
    public float DetectionCooldown => detectionCooldown;
    public float DefaultDistractionDuration => defaultDistractionDuration;
}
