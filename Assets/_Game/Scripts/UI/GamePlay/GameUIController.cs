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
        _txtState = root.Q<Label>("TxtEstado");
        _txtTimer = root.Q<Label>("TxtTimer");
        _progressBar = root.Q<ProgressBar>("BarraProgreso");
        _imgPlayer = root.Q<VisualElement>("Imagen Player");
        _btnPause = root.Q<Button>("BtnPausarHUD");

        _menuPauseOverlay = root.Q<VisualElement>("MenuPausa");
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
    }

    private void UnregisterEvents()
    {
        if( _btnPause != null ) _btnPause.clicked -= OpenOptions;
        if( _btnContinue != null ) _btnContinue.clicked -= CloseOptions;
        if( _btnReset != null ) _btnReset.clicked -= SceneReset;
        if( _btnQuit != null ) _btnQuit.clicked -= SceneQuit;
    }

    private void SceneReset()
    {
        GameManager.Instance.StartGame();
        StartCoroutine(SceneLoad("Office_1"));   
    }

    private void SceneQuit()
    {
        GameManager.Instance.StartGame();
        StartCoroutine(SceneLoad("Menu"));   
    }

    private IEnumerator SceneLoad(string nameScene)
    {
        if (_fadeOverlay != null) _fadeOverlay.AddToClassList("cortina-cerrada");
        yield return new WaitForSeconds(_transitionTime);
        SceneManager.LoadScene(nameScene);
    }

    private void OpenOptions()
    {
        GameManager.Instance.PauseGame();
        _menuPauseOverlay.RemoveFromClassList("oculto");
    }

    private void CloseOptions()
    {
        GameManager.Instance.StartGame();
        _menuPauseOverlay.AddToClassList("oculto");
    }

}
