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
        private set 
        { 
            currentState = value; 
            OnGameStateChanged?.Invoke(value);
        }
    }
    
    public System.Action<GameState> OnGameStateChanged;

    private bool isPaused = false;
    public bool IsPaused => isPaused;

    [Header("Player Data Management")]
    private PlayerData playerData = new PlayerData();
    public PlayerData PlayerData => playerData;

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
        SceneManager.LoadScene("LobbyScene");
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

    public void StartNewGame()
    {
        ResetPlayerData();
        LoadGameScene();
    }

    public void ContinueGame()
    {
        string sceneToLoad = HasGameData() ? playerData.lastSceneName : "GameScene";
        SceneManager.LoadScene(sceneToLoad);
    }

    public bool HasGameData()
    {
        return !string.IsNullOrEmpty(playerData.lastSceneName);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScene");
    }

    // === Player Data Management ===
    
    public void SavePlayerData(PlayerStats playerStats)
    {
        if (playerStats == null) return;
        
        playerData.currentHealth = playerStats.currentHealth;
        playerData.maxHealth = playerStats.maxHealth;
        playerData.currentMana = playerStats.currentMana;
        playerData.maxMana = playerStats.maxMana;
        playerData.isInvincible = playerStats.isInvincible;
        playerData.lastPosition = playerStats.transform.position;
        playerData.lastSceneName = SceneManager.GetActiveScene().name;
        
        Debug.Log($"[GameManager] Player data saved - Health: {playerData.currentHealth}/{playerData.maxHealth}");
    }
    
    public void LoadPlayerData(PlayerStats playerStats)
    {
        if (playerStats == null) return;
        
        playerStats.currentHealth = playerData.currentHealth;
        playerStats.maxHealth = playerData.maxHealth;
        playerStats.currentMana = playerData.currentMana;
        playerStats.maxMana = playerData.maxMana;
        playerStats.isInvincible = playerData.isInvincible;
        
        Debug.Log($"[GameManager] Player data loaded - Health: {playerData.currentHealth}/{playerData.maxHealth}");
    }
    
    public void SetDobokColor(Color color)
    {
        playerData.dobokColor = color;
        playerData.hasDobokSelected = true;
        Debug.Log($"[GameManager] Dobok color saved: {color}");
    }
    
    public Color GetDobokColor()
    {
        return playerData.dobokColor;
    }
    
    public bool HasDobokSelected()
    {
        return playerData.hasDobokSelected;
    }
    
    public void SetPlayerPosition(Vector3 position)
    {
        playerData.lastPosition = position;
    }
    
    public Vector3 GetPlayerPosition()
    {
        return playerData.lastPosition;
    }
    
    public void ResetPlayerData()
    {
        playerData.ResetToDefaults();
        Debug.Log("[GameManager] Player data reset to defaults");
    }
}