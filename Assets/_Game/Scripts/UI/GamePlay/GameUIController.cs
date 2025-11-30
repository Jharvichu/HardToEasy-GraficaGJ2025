using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(UIDocument))]
public class GameUIController : MonoBehaviour
{
    private UIDocument _doc;

    private Label _txtState, _txtTimer;
    private ProgressBar _progressBar;
    private VisualElement _imgPlayer;

    private Button _btnPause, _btnContinue, _btnReset, _btnQuit;
    private Slider _volumenSlider;
    private VisualElement _menuPauseOverlay, _menuVictoryOverlay, _menuDefeatOverlay, _menuDefeatTimeOverlay, _fadeOverlay;
    private Button _btnRestartWin, _btnMenuWin, _btnRestartLose, _btnMenuLose, _btnRestartTime, _btnMenuTime;

    [Header("Settings")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private float _transitionTime;
    [SerializeField] private int maxScore = 100;

    // Cronómetro del juego (9:00 AM → 10:00 AM)
    private float _gameTimeInSeconds = 0f;
    private const float TOTAL_GAME_DURATION = 3600f;  // 1 hora de juego = 3600 segundos
    private const float REAL_TIME_DURATION = 300f;    // 5 minutos reales = 300 segundos
    private float _timeScale;
    private bool _timerRunning = false;

    [Header("Player Visuals")]
    [SerializeField] private Sprite iconAwake;
    [SerializeField] private Sprite iconSleep;

    [Header("Bosses")]
    [SerializeField] private BossController _boss1;
    [SerializeField] private BossController _boss2;
    [SerializeField] private BossController _boss3;
    [SerializeField] private BossController _boss4;

    void Awake()
    {
        _doc = GetComponent<UIDocument>();
    }

    void Start()
    {
        AudioManager.Instance.UpdateBGMusic("Gameplay despierto");

        // Inicializar cronómetro
        _timeScale = TOTAL_GAME_DURATION / REAL_TIME_DURATION;
        _timerRunning = true;
        UpdateTimerUI();
    }

    void Update()
    {
        if (_timerRunning && GameManager.Instance.currentState == GameManager.GameState.Playing)
        {
            _gameTimeInSeconds += Time.deltaTime * _timeScale;
            UpdateTimerUI();

            if (_gameTimeInSeconds >= TOTAL_GAME_DURATION)
            {
                _timerRunning = false;
                ShowTimeExpiredDefeat();
            }
        }
    }

    void OnEnable()
    {
        VisualElement root = _doc.rootVisualElement;
        FindUIElements(root);
        RegisterEvents();
    }

    void OnDisable() 
    {
        UnregisterEvents();
    }

    private void FindUIElements(VisualElement root)
    {
        _txtState = root.Q<Label>("TxtEstado");
        _txtTimer = root.Q<Label>("TxtTimer");
        _progressBar = root.Q<ProgressBar>("BarraProgreso");
        _imgPlayer = root.Q<VisualElement>("Imagen Player");
        _btnPause = root.Q<Button>("BtnPausarHUD");

        _menuPauseOverlay = root.Q<VisualElement>("MenuPausa");
        _volumenSlider = root.Q<Slider>("SliderVolumen");
        _btnContinue = root.Q<Button>("BtnContinuar");
        _btnReset = root.Q<Button>("BtnReiniciar");
        _btnQuit = root.Q<Button>("BtnSalirMenu");

        _menuVictoryOverlay = root.Q<VisualElement>("MenuVictoria");
        _btnRestartWin = root.Q<Button>("BtnSiguienteNivel");
        _btnMenuWin = root.Q<Button>("BtnMenuVictoria");

        _menuDefeatOverlay = root.Q<VisualElement>("MenuDerrota");
        _btnRestartLose = root.Q<Button>("BtnReiniciarDerrota");
        _btnMenuLose = root.Q<Button>("BtnMenuDerrota");

        _menuDefeatTimeOverlay = root.Q<VisualElement>("MenuDerrotaTiempo");
        _btnRestartTime = root.Q<Button>("BtnReiniciarTiempo");
        _btnMenuTime = root.Q<Button>("BtnMenuTiempo");

        _fadeOverlay = root.Q<VisualElement>("Cortina");
    }

    private void RegisterEvents()
    {
        UnregisterEvents();

        _btnPause.clicked += OpenOptions;
        _btnContinue.clicked += CloseOptions;
        _btnReset.clicked += SceneReset;
        _btnQuit.clicked += SceneQuit;

        _btnRestartWin.clicked += SceneReset;
        _btnMenuWin.clicked += SceneQuit;

        _btnRestartLose.clicked += SceneReset;
        _btnMenuLose.clicked += SceneQuit;

        _btnRestartTime.clicked += SceneReset;
        _btnMenuTime.clicked += SceneQuit;

        if (_volumenSlider != null) 
        {
            _volumenSlider.value = AudioManager.Instance.GetGeneralVolume();
            _volumenSlider.RegisterValueChangedCallback(OnVolumeChanged);
        }

        UpdatePlayerVisuals(_playerController.CurrentState);

        ScoreManager.Instance.OnScoreChanged += UpdateProgressBar;
        _playerController.OnPlayerStateChanged += UpdatePlayerVisuals;
        _playerController.OnPlayerStateChanged += UpdateMusicPlayerState;
        RegisterEventsBoss();
    }

    private void UnregisterEvents()
    {
        if( _btnPause != null ) _btnPause.clicked -= OpenOptions;
        if( _btnContinue != null ) _btnContinue.clicked -= CloseOptions;
        if( _btnReset != null ) _btnReset.clicked -= SceneReset;
        if( _btnQuit != null ) _btnQuit.clicked -= SceneQuit;

        _volumenSlider?.UnregisterValueChangedCallback(OnVolumeChanged);

        ScoreManager.Instance.OnScoreChanged -= UpdateProgressBar;
        _playerController.OnPlayerStateChanged -= UpdatePlayerVisuals;
        _playerController.OnPlayerStateChanged -= UpdateMusicPlayerState;
        UnregisterEventsBoss();
    }

    private void RegisterEventsBoss()
    {
        _boss1.OnBossCaughtPlayer += ShowDefeatScreen;
        _boss2.OnBossCaughtPlayer += ShowDefeatScreen;
        _boss3.OnBossCaughtPlayer += ShowDefeatScreen;
        _boss4.OnBossCaughtPlayer += ShowDefeatScreen;
    }

    private void UnregisterEventsBoss()
    {
        _boss1.OnBossCaughtPlayer -= ShowDefeatScreen;
        _boss2.OnBossCaughtPlayer -= ShowDefeatScreen;
        _boss3.OnBossCaughtPlayer -= ShowDefeatScreen;
        _boss4.OnBossCaughtPlayer -= ShowDefeatScreen;
    }

    private void SceneReset()
    {
        AudioManager.Instance.Play("Boton");
        GameManager.Instance.StartGame();
        StartCoroutine(SceneLoad("Office_1"));   
    }

    private void SceneQuit()
    {
        AudioManager.Instance.Play("Boton");
        GameManager.Instance.StartGame();
        StartCoroutine(SceneLoad("Menu"));   
    }

    private IEnumerator SceneLoad(string nameScene)
    {
        if (_fadeOverlay != null) _fadeOverlay.AddToClassList("cortina-cerrada");
        yield return new WaitForSeconds(_transitionTime);
        AudioManager.Instance.StopBGM();
        SceneManager.LoadScene(nameScene);
    }

    private void OpenOptions()
    {
        AudioManager.Instance.Play("Boton");
        GameManager.Instance.PauseGame();
        _menuPauseOverlay.RemoveFromClassList("oculto");
    }

    private void CloseOptions()
    {
        AudioManager.Instance.Play("Boton");
        GameManager.Instance.StartGame();
        _menuPauseOverlay.AddToClassList("oculto");
    }

    private void OnVolumeChanged(ChangeEvent<float> evt)
    {
        float valueSlider = evt.newValue;
        float volumenNormalizado = valueSlider / 100f;

        AudioManager.Instance.UpdateGeneralVolume(volumenNormalizado);
    }

    public void ShowDefeatScreen()
    {
        GameManager.Instance.PauseGame();
        AudioManager.Instance.StopBGM();
        AudioManager.Instance.Play("Perdio");
        _menuDefeatOverlay?.RemoveFromClassList("oculto");
    }

    public void ShowTimeExpiredDefeat()
    {
        GameManager.Instance.PauseGame();
        AudioManager.Instance.StopBGM();
        AudioManager.Instance.Play("Perdio");
        _menuDefeatTimeOverlay?.RemoveFromClassList("oculto");
    }

    public void ShowVictoryScreen()
    {
        GameManager.Instance.PauseGame();
        AudioManager.Instance.StopBGM();
        AudioManager.Instance.Play("Gano");
        _menuVictoryOverlay?.RemoveFromClassList("oculto");
    }

    private void UpdateProgressBar(int newScore)
    {
        if (_progressBar != null)
        {
            _progressBar.value = newScore;
            _progressBar.title = $"Puntaje: {newScore}";
        }

        if (newScore >= maxScore) ShowVictoryScreen();
    }

    private void UpdatePlayerVisuals(PlayerState newState)
    {
        Sprite spriteToUse = (newState == PlayerState.Awaken) ? iconAwake : iconSleep;
        string textToUse = (newState == PlayerState.Awaken) ? "DESPIERTO" : "DORMIDO";

        if (_imgPlayer != null && spriteToUse != null)
        {
            _imgPlayer.style.backgroundImage = new StyleBackground(spriteToUse);
        }

        if (_txtState != null)
        {
            _txtState.text = textToUse;

            _txtState.style.color = (newState == PlayerState.Awaken) 
                ? new StyleColor(Color.green) 
                : new StyleColor(Color.magenta); 
        }
    }

    private void UpdateMusicPlayerState(PlayerState newState)
    {
        if (newState == PlayerState.Asleep) AudioManager.Instance.UpdateBGMusic("Gameplay dormido");
        if (newState == PlayerState.Awaken) AudioManager.Instance.UpdateBGMusic("Gameplay despierto");
    }

    private void UpdateTimerUI()
    {
        // Convertir segundos de juego a hora (empezando desde 9:00 AM)
        int totalMinutes = 9 * 60 + Mathf.FloorToInt(_gameTimeInSeconds / 60f);
        int hours = totalMinutes / 60;
        int minutes = totalMinutes % 60;

        string ampm = hours >= 12 ? "PM" : "AM";
        int displayHour = hours > 12 ? hours - 12 : (hours == 0 ? 12 : hours);

        _txtTimer.text = $"{displayHour:D2}:{minutes:D2} {ampm}";
    }
}
