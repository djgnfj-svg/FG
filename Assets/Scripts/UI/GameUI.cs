using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("Player UI")]
    [SerializeField] private HealthBar playerHealthBar;
    [SerializeField] private Image manaBar;
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private TextMeshProUGUI scoreText;
    
    [Header("Game State UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject victoryPanel;
    
    [Header("Buttons")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    
    [Header("Settings")]
    [SerializeField] private Color fullManaColor = Color.blue;
    [SerializeField] private Color emptyManaColor = Color.gray;
    
    private PlayerStats playerStats;
    private GameManager gameManager;
    private StageManager stageManager;
    
    void Awake()
    {
        // 버튼 이벤트 설정
        if (pauseButton != null)
            pauseButton.onClick.AddListener(OnPauseClicked);
        if (resumeButton != null)
            resumeButton.onClick.AddListener(OnResumeClicked);
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
    }
    
    void Start()
    {
        // 컴포넌트 참조 설정
        gameManager = GameManager.Instance;
        stageManager = StageManager.Instance;
        
        // 플레이어 찾기
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerStats = playerObj.GetComponent<PlayerStats>();
        }
        
        // 초기 UI 상태 설정
        UpdateStageText();
        HideAllPanels();
        
        // GameManager 이벤트 구독
        if (gameManager != null)
        {
            gameManager.OnGameStateChanged += OnGameStateChanged;
        }
    }
    
    void Update()
    {
        UpdatePlayerUI();
        HandleInputs();
    }
    
    void UpdatePlayerUI()
    {
        if (playerStats == null) return;
        
        // 마나바 업데이트
        if (manaBar != null)
        {
            float manaPercentage = playerStats.GetManaPercentage();
            manaBar.fillAmount = manaPercentage;
            manaBar.color = Color.Lerp(emptyManaColor, fullManaColor, manaPercentage);
        }
        
        // 점수 업데이트
        // UpdateScore();
    }
    
    void UpdateStageText()
    {
        if (stageText != null && stageManager != null)
        {
            stageText.text = $"Stage {stageManager.CurrentStage}";
        }
    }
    
    void HandleInputs()
    {
        // ESC 키로 일시정지/재개
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameManager != null)
            {
                if (gameManager.CurrentState == GameManager.GameState.Playing)
                {
                    OnPauseClicked();
                }
                else if (gameManager.CurrentState == GameManager.GameState.Paused)
                {
                    OnResumeClicked();
                }
            }
        }
    }
    
    void OnGameStateChanged(GameManager.GameState newState)
    {
        switch (newState)
        {
            case GameManager.GameState.Playing:
                HideAllPanels();
                break;
            case GameManager.GameState.Paused:
                ShowPausePanel();
                break;
            case GameManager.GameState.GameOver:
                ShowGameOverPanel();
                break;
            case GameManager.GameState.Victory:
                ShowVictoryPanel();
                break;
        }
    }
    
    void HideAllPanels()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
    }
    
    void ShowPausePanel()
    {
        HideAllPanels();
        if (pausePanel != null) pausePanel.SetActive(true);
    }
    
    void ShowGameOverPanel()
    {
        HideAllPanels();
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }
    
    void ShowVictoryPanel()
    {
        HideAllPanels();
        if (victoryPanel != null) victoryPanel.SetActive(true);
    }
    
    // 버튼 이벤트 핸들러
    void OnPauseClicked()
    {
        if (gameManager != null)
        {
            gameManager.PauseGame();
        }
    }
    
    void OnResumeClicked()
    {
        if (gameManager != null)
        {
            gameManager.ResumeGame();
        }
    }
    
    void OnRestartClicked()
    {
        if (gameManager != null)
        {
            gameManager.RestartGame();
        }
    }
    
    void OnMainMenuClicked()
    {
        if (gameManager != null)
        {
            gameManager.LoadMainMenu();
        }
    }
    
    void OnDestroy()
    {
        // 이벤트 구독 해제
        if (gameManager != null)
        {
            gameManager.OnGameStateChanged -= OnGameStateChanged;
        }
    }
    
    // 외부에서 UI 업데이트용 메서드들
    public void SetStageText(string text)
    {
        if (stageText != null)
            stageText.text = text;
    }
    
    public void SetScoreText(int score)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }
    
    public void ShowMessage(string message, float duration = 2f)
    {
        // 메시지 표시 (Toast 형태)
        StartCoroutine(ShowTemporaryMessage(message, duration));
    }
    
    System.Collections.IEnumerator ShowTemporaryMessage(string message, float duration)
    {
        // 메시지를 콘솔에 출력 (UI 구현 전 대체)
        Debug.Log($"[GameUI] Message: {message}");
        yield return new WaitForSeconds(duration);
    }
}