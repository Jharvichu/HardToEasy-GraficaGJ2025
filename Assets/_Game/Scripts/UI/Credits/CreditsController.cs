using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class CreditsController : MonoBehaviour
{
    private UIDocument _doc;
    private VisualElement _creditsContainer;
    private Button _btnExit;

    [Header("Configuración")]
    [SerializeField] private float scrollSpeed = 100f; 
    [SerializeField] private float turboSpeed = 100f;

    private float _currentPosition;
    private float _containerHeight;
    private bool _hasStarted = false;

    void Awake()
    {
        _doc = GetComponent<UIDocument>();
        VisualElement root = _doc.rootVisualElement;

        _creditsContainer = root.Q<VisualElement>("ContenedorCreditos");
        _btnExit = root.Q<Button>("BtnSalir");

        if (_btnExit != null)
            _btnExit.clicked += ReturnToMenu;
    }

    void Update()
    {
        // 1. FASE DE INICIALIZACIÓN (Esperar a que Unity dibuje la UI)
        if (!_hasStarted)
        {
            if (float.IsNaN(_doc.rootVisualElement.layout.height) || _doc.rootVisualElement.layout.height <= 0)
                return;
            
            InitializeScroll();
            return;
        }

        // 2. FASE DE MOVIMIENTO
        bool isAccelerating = false;
        
        // Comprobación segura de Input System
        if (Mouse.current != null && Mouse.current.leftButton.isPressed) isAccelerating = true;
        if (Keyboard.current != null && Keyboard.current.spaceKey.isPressed) isAccelerating = true;

        float currentSpeed = isAccelerating ? turboSpeed : scrollSpeed;

        _currentPosition -= currentSpeed * Time.deltaTime;
        _creditsContainer.style.top = _currentPosition;

        // 3. VERIFICAR FIN
        if (_currentPosition < -_containerHeight)
        {
            ReturnToMenu();
        }
    }

    private void InitializeScroll()
    {
        _containerHeight = _creditsContainer.layout.height;
        
        if (_containerHeight < 500) _containerHeight = 2000; 

        _currentPosition = _doc.rootVisualElement.layout.height;
        
        _creditsContainer.style.top = _currentPosition;
        
        Debug.Log("Creditos Iniciados. Altura Pantalla: " + _currentPosition + ", Altura Texto: " + _containerHeight);
        _hasStarted = true;
    }

    private void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}