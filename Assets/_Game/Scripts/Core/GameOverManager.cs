using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BossController bossController;
    [SerializeField] private UIDocument uiDocument;

    [Header("Settings")]
    [SerializeField] private float transitionTime = 1f;

    [Header("UI Element Names")]
    [SerializeField] private string gameOverOverlayName = "GameOverOverlay";
    [SerializeField] private string fadeOverlayName = "Cortina";
    [SerializeField] private string returnButtonName = "BtnVolverMenu";

    // UI Elements
    private VisualElement gameOverOverlay;
    private VisualElement fadeOverlay;
    private Button btnReturnToMenu;

    private void Awake()
    {
        ValidateDependencies();
    }

    private void OnEnable()
    {
        SubscribeToEvents();
        InitializeUI();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
        UnregisterUIEvents();
    }

    #region Initialization & Validation

    private void ValidateDependencies()
    {
        if (bossController == null)
        {
            bossController = FindFirstObjectByType<BossController>();
            if (bossController == null)
            {
                Debug.LogError("BossController not found! GameOverManager needs a reference to BossController.", this);
            }
        }

        if (uiDocument == null)
        {
            uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("UIDocument not found! GameOverManager needs a UIDocument component.", this);
            }
        }
    }

    private void SubscribeToEvents()
    {
        if (bossController != null)
        {
            bossController.OnPlayerCaught += OnPlayerCaught;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (bossController != null)
        {
            bossController.OnPlayerCaught -= OnPlayerCaught;
        }
    }

    private void InitializeUI()
    {
        if (uiDocument == null)
            return;

        VisualElement root = uiDocument.rootVisualElement;

        // Buscar elementos de UI
        gameOverOverlay = root.Q<VisualElement>(gameOverOverlayName);
        fadeOverlay = root.Q<VisualElement>(fadeOverlayName);
        btnReturnToMenu = root.Q<Button>(returnButtonName);

        // Validar que existan
        if (gameOverOverlay == null)
        {
            Debug.LogWarning($"GameOver overlay '{gameOverOverlayName}' not found in UI Document. Game over UI won't be displayed.", this);
        }
        else
        {
            // Ocultar overlay al inicio
            gameOverOverlay.AddToClassList("oculto");
        }

        // Registrar eventos de botones
        RegisterUIEvents();
    }

    private void RegisterUIEvents()
    {
        UnregisterUIEvents();

        if (btnReturnToMenu != null)
        {
            btnReturnToMenu.clicked += ReturnToMainMenu;
        }
    }

    private void UnregisterUIEvents()
    {
        if (btnReturnToMenu != null)
        {
            btnReturnToMenu.clicked -= ReturnToMainMenu;
        }
    }

    #endregion

    #region Game Over Logic

    private void OnPlayerCaught()
    {
        Debug.Log("GameOverManager: Player was caught! Starting game over sequence...");
        StartCoroutine(ShowGameOverSequence());
    }

    private IEnumerator ShowGameOverSequence()
    {
        // Pequeña pausa dramática antes de mostrar game over
        yield return new WaitForSeconds(0.5f);

        // Cambiar estado del juego
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeState(GameManager.GameState.GameOver);
        }

        // Mostrar overlay de game over
        if (gameOverOverlay != null)
        {
            gameOverOverlay.RemoveFromClassList("oculto");
        }
        else
        {
            Debug.LogWarning("GameOver overlay not found! Cannot display game over screen.");
        }

        // Desde aquí, el usuario debe hacer click en "Volver al Menú"
    }

    private void ReturnToMainMenu()
    {
        Debug.Log("Returning to main menu...");
        StartCoroutine(SceneLoad("Menu"));
    }

    private IEnumerator SceneLoad(string sceneName)
    {
        // Aplicar cortina de transición
        if (fadeOverlay != null)
        {
            fadeOverlay.AddToClassList("cortina-cerrada");
        }

        // Esperar animación de cortina
        yield return new WaitForSecondsRealtime(transitionTime);

        // Restaurar timeScale antes de cambiar escena
        Time.timeScale = 1f;

        // Cargar escena
        SceneManager.LoadScene(sceneName);
    }

    #endregion

    #region Public API

    /// <summary>
    /// Forzar game over manualmente (útil para pruebas)
    /// </summary>
    [ContextMenu("Test Game Over")]
    public void TestGameOver()
    {
        OnPlayerCaught();
    }

    #endregion
}
