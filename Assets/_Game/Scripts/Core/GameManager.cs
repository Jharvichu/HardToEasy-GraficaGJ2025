using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public enum GameState { MainMenu, Playing, Paused}
    public GameState currentState { get; private set; }
    
    public delegate void GameStateChanged(GameState newState);
    public event GameStateChanged OnGameStateChanged;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeGame()
    {
        currentState = GameState.MainMenu;
        StartGame();
    }
    
    // State management
    public void ChangeState(GameState newState)
    {
        currentState = newState;
        OnGameStateChanged?.Invoke(newState);
        
        switch (newState)
        {
            case GameState.Playing:
                Time.timeScale = 1f;
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
                break;
        }
    }
    
    public void StartGame()
    {
        InitializeGame();
        ChangeState(GameState.Playing);
    }
    
    public void PauseGame()
    {
        ChangeState(GameState.Paused);
    }
    
    public void ResumeGame()
    {
        ChangeState(GameState.Playing);
    }
    
    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
