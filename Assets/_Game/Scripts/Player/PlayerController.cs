using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerData playerData;
    [SerializeField] private Camera mainCamera;

    [Header("Current State")]
    [SerializeField] private PlayerState currentState = PlayerState.Awaken;
    
    public event Action<PlayerState> OnPlayerStateChanged;
    public event Action OnInteractionPerformed;
    public event Action OnStateChangeBlocked;

    private float _startStateChangeCooldownTime;

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

    private void ValidateDependencies()
    {
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput is not assigned to PlayerController!", this);
        }

        if (playerData == null)
        {
            Debug.LogError("PlayerData is not assigned to PlayerController!", this);
        }
    }
    
    private void SubscribeToEvents()
    {
        if (playerInput != null)
        {
            playerInput.OnStateChangeRequested += HandleStateChangeRequest;
            playerInput.OnInteractionRequested += HandleInteractionRequest;
        }
    }
    
    private void UnsubscribeFromEvents()
    {
        if (playerInput != null)
        {
            playerInput.OnStateChangeRequested -= HandleStateChangeRequest;
            playerInput.OnInteractionRequested -= HandleInteractionRequest;
        }
    }
    
    private void InitializeState()
    {
        ChangeState(currentState);
    }
    
    private void HandleStateChangeRequest()
    {
        if (Time.time < _startStateChangeCooldownTime + playerData.StateChangeCooldown)
        {
            OnStateChangeBlocked?.Invoke();
            Debug.Log("State change blocked - cooldown active");
            return;
        }
        
        PlayerState newState = currentState == PlayerState.Awaken 
            ? PlayerState.Asleep 
            : PlayerState.Awaken;

        ChangeState(newState);
        
        _startStateChangeCooldownTime = Time.time;
    }
    
    private void ChangeState(PlayerState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            OnPlayerStateChanged?.Invoke(currentState);
            Debug.Log($"Player state changed to: {currentState}");
        }
    }
    
    private void HandleInteractionRequest(Vector2 mousePosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        
        if (hit.collider != null)
        {
            IClickable clickable = hit.collider.GetComponent<IClickable>();
            clickable?.OnClick();
        }
    }
}
