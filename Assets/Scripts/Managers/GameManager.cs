using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    instance = go.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }

    public enum GameState
    {
        Playing,
        Paused,
        GameOver,
        Victory
    }

    private GameState currentState = GameState.Playing;
    public GameState CurrentState
    {
        get { return currentState; }
        private set { currentState = value; }
    }

    private bool isPaused = false;
    public bool IsPaused => isPaused;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Playing || currentState == GameState.Paused)
            {
                TogglePause();
            }
        }
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        CurrentState = GameState.Paused;
        Time.timeScale = 0f;
        
        PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
        if (pauseMenu != null)
        {
            pauseMenu.ShowPauseMenu();
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        CurrentState = GameState.Playing;
        Time.timeScale = 1f;
        
        PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
        if (pauseMenu != null)
        {
            pauseMenu.HidePauseMenu();
        }
    }

    public void LoadTitleScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScene");
    }

    public void LoadGameScene()
    {
        Time.timeScale = 1f;
        CurrentState = GameState.Playing;
        SceneManager.LoadScene("GameScene");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        CurrentState = GameState.Playing;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void GameOver()
    {
        CurrentState = GameState.GameOver;
        Time.timeScale = 0f;
    }

    public void Victory()
    {
        CurrentState = GameState.Victory;
    }
}