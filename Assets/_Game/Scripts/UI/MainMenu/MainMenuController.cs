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

    [SerializeField] private float _transitionTime;

    void Awake()
    {
        _doc = GetComponent<UIDocument>();
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
    }

    private void UnregisterEvents()
    {
        if (_btnPlay != null) _btnPlay.clicked -= StartGame;
        if (_btnOptions != null) _btnOptions.clicked -= OpenOptions;
        if (_btnCredits != null) _btnCredits.clicked -= ShowCredits;
        if (_btnQuit != null) _btnQuit.clicked -= QuitGame;
        if (_btnBack != null) _btnBack.clicked -= ReturnToMenu;
    }

    private void StartGame()
    {
        StartCoroutine(SceneLoad("Office_1"));
    }

    private IEnumerator SceneLoad(string nameScene)
    {
        if (_fadeOverlay != null) _fadeOverlay.AddToClassList("cortina-cerrada");
        yield return new WaitForSeconds(_transitionTime);
        SceneManager.LoadScene(nameScene);
    }

    private void OpenOptions()
    {
        _optionsWindow.RemoveFromClassList("oculto");
    }

    private void ShowCredits()
    {
        
    }

    private void QuitGame()
    {
        Application.Quit();
    }

    private void ReturnToMenu()
    {
        _optionsWindow.AddToClassList("oculto");
    }

}
