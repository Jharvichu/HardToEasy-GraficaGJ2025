using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameUIController : MonoBehaviour
{
    private UIDocument _doc;

    private Label _txtState, _txtTimer;
    private ProgressBar _progressBar;
    private VisualElement _imgPlayer;

    private Button _btnPause;
    private VisualElement _menuPauseOverlay, _menuVictoryOverlay, _menuDefeatOverlay, _fadeOverlay;
    private Button _btnContinue, _btnReset, _btnQuit, _btnRestartWin, _btnMenuWin, _btnRestartLose, _btnMenuLose;
    private Slider _volumenSlider;


    [Header("Settings")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private float _transitionTime;
    [SerializeField] private int maxScore = 100;

    [Header("Player Visuals")]
    [SerializeField] private Sprite iconAwake;
    [SerializeField] private Sprite iconSleep;

    void Awake()
    {
        _doc = GetComponent<UIDocument>();
    }

    void Start()
    {
        AudioManager.Instance.UpdateBGMusic("Gameplay despierto");
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

        _fadeOverlay = root.Q<VisualElement>("Cortina");
    }

    private void RegisterEvents()
    {
        UnregisterEvents();

        _btnPause.clicked += OpenOptions;
        _btnContinue.clicked += CloseOptions;
        _btnReset.clicked += SceneReset;
        _btnQuit.clicked += SceneQuit;

        if (_volumenSlider != null) 
        {
            _volumenSlider.value = AudioManager.Instance.GetGeneralVolume();
            _volumenSlider.RegisterValueChangedCallback(OnVolumeChanged);
        }

        _btnRestartWin.clicked += SceneReset;
        _btnMenuWin.clicked += SceneQuit;

        _btnRestartLose.clicked += SceneReset;
        _btnMenuLose.clicked += SceneQuit;

        ScoreManager.Instance.OnScoreChanged += UpdateProgressBar;
        _playerController.OnPlayerStateChanged += UpdatePlayerVisuals;
        UpdatePlayerVisuals(_playerController.CurrentState);
    }

    private void UnregisterEvents()
    {
        if( _btnPause != null ) _btnPause.clicked -= OpenOptions;
        if( _btnContinue != null ) _btnContinue.clicked -= CloseOptions;
        if( _btnReset != null ) _btnReset.clicked -= SceneReset;
        if( _btnQuit != null ) _btnQuit.clicked -= SceneQuit;

        if (_volumenSlider != null) _volumenSlider.UnregisterValueChangedCallback(OnVolumeChanged);

        ScoreManager.Instance.OnScoreChanged -= UpdateProgressBar;
        _playerController.OnPlayerStateChanged -= UpdatePlayerVisuals;
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
        _menuDefeatOverlay?.RemoveFromClassList("oculto");
    }

    public void ShowVictoryScreen()
    {
        GameManager.Instance.PauseGame();
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

}
