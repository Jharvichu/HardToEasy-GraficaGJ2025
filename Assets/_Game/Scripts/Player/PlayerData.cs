using UnityEngine;

public enum PlayerState
{
    Awaken,
    Asleep
}

[System.Serializable]
public class PlayerData
{
    [Header("State Change Settings")]
    [SerializeField] private float stateChangeCooldown = 2f;
    [SerializeField] private KeyCode stateChangeKey = KeyCode.Space;
    public float StateChangeCooldown => stateChangeCooldown;
    public KeyCode StateChangeKey => stateChangeKey;
}
