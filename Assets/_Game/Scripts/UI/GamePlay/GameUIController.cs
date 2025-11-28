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
    private VisualElement _menuPauseOverlay, _fadeOverlay;
    private Button _btnContinue, _btnReset, _btnQuit;
    private Slider _volumenSlider;

    [SerializeField] private float _transitionTime;

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
    }

    private void UnregisterEvents()
    {
        if( _btnPause != null ) _btnPause.clicked -= OpenOptions;
        if( _btnContinue != null ) _btnContinue.clicked -= CloseOptions;
        if( _btnReset != null ) _btnReset.clicked -= SceneReset;
        if( _btnQuit != null ) _btnQuit.clicked -= SceneQuit;

        if (_volumenSlider != null) _volumenSlider.UnregisterValueChangedCallback(OnVolumeChanged);
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
}
