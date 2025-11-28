using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    private UIDocument _doc;
    private Button _btnPlay, _btnOptions, _btnCredits, _btnQuit; // Menu
    private Button _btnBack;
    private VisualElement _optionsWindow, _fadeOverlay;
    private Slider _volumenSlider;

    [SerializeField] private float _transitionTime;

    void Awake()
    {
        _doc = GetComponent<UIDocument>();
    }

    void Start()
    {
        GameManager.Instance.InitializeGame();
        AudioManager.Instance.PlayBGM("Musica Menu");
        
        if (_volumenSlider != null)
        {
            _volumenSlider.value = AudioManager.Instance.GetGeneralVolume() * 100f; 
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
        _btnPlay    = root.Q<Button>("BtnJugar");
        _btnOptions = root.Q<Button>("BtnOpciones");
        _btnCredits = root.Q<Button>("BtnCreditos");
        _btnQuit    = root.Q<Button>("BtnSalir");

        _optionsWindow = root.Q<VisualElement>("PantallaOpciones");
        _volumenSlider = root.Q<Slider>("SliderVolumen");
        _btnBack = root.Q<Button>("BtnVolver");

        _fadeOverlay = root.Q<VisualElement>("Cortina");
    }

    private void RegisterEvents()
    {
        UnregisterEvents(); 

        _btnPlay.clicked += StartGame;
        _btnOptions.clicked += OpenOptions;
        _btnCredits.clicked += ShowCredits;
        _btnQuit.clicked += QuitGame;
        _btnBack.clicked += ReturnToMenu;

        if (_volumenSlider != null) _volumenSlider.RegisterValueChangedCallback(OnVolumeChanged);
    }

    private void UnregisterEvents()
    {
        if (_btnPlay != null) _btnPlay.clicked -= StartGame;
        if (_btnOptions != null) _btnOptions.clicked -= OpenOptions;
        if (_btnCredits != null) _btnCredits.clicked -= ShowCredits;
        if (_btnQuit != null) _btnQuit.clicked -= QuitGame;
        if (_btnBack != null) _btnBack.clicked -= ReturnToMenu;

        if (_volumenSlider != null) _volumenSlider.UnregisterValueChangedCallback(OnVolumeChanged);
    }

    private void StartGame()
    {
        AudioManager.Instance.Play("Boton");
        GameManager.Instance.StartGame();
        StartCoroutine(SceneLoad("Office_1"));
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
        _optionsWindow.RemoveFromClassList("oculto");
    }

    private void ShowCredits()
    {
        AudioManager.Instance.Play("Boton");
    }

    private void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }

    private void ReturnToMenu()
    {
        AudioManager.Instance.Play("Boton");
        _optionsWindow.AddToClassList("oculto");
    }

    private void OnVolumeChanged(ChangeEvent<float> evt)
    {
        float valueSlider = evt.newValue;
        float volumenNormalizado = valueSlider / 100f;

        AudioManager.Instance.UpdateGeneralVolume(volumenNormalizado);
    }
}
