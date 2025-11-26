using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public event System.Action OnStateChangeRequested;
    public event System.Action<Vector2> OnInteractionRequested;

    private PlayerInputActions inputActions;
    
    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }
    
    private void OnEnable()
    {
        inputActions.Enable();
        
        inputActions.Player.StateChange.performed += OnStateChangePerformed;
        inputActions.Player.Interact.performed += OnInteractPerformed;
    }

    private void OnDisable()
    {
        inputActions.Player.StateChange.performed -= OnStateChangePerformed;
        inputActions.Player.Interact.performed -= OnInteractPerformed;
        
        inputActions.Disable();
    }
    
    private void OnStateChangePerformed(InputAction.CallbackContext context)
    {
        OnStateChangeRequested?.Invoke();
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        OnInteractionRequested?.Invoke(mousePosition);
    }
}
