using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    
    public delegate void StateChangeRequested();
    public delegate void InteractionRequested(Vector2 mousePosition);
    
    public event StateChangeRequested OnStateChangeRequested;
    public event InteractionRequested OnInteractionRequested;

    private void Update()
    {
        DetectStateChangeInput();
        DetectInteractionInput();
    }
    
    private void DetectStateChangeInput()
    {
        if (Input.GetKeyDown(playerData.StateChangeKey))
        {
            OnStateChangeRequested?.Invoke();
        }
    }
    
    private void DetectInteractionInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnInteractionRequested?.Invoke(Input.mousePosition);
        }
    }
}
