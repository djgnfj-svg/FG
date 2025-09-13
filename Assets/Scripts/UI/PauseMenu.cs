using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject playerHUD;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button titleButton;
    [SerializeField] private Button quitButton;

    void Start()
    {
        if (pauseMenuPanel == null)
        {
            pauseMenuPanel = transform.Find("PauseMenuPanel")?.gameObject;
        }

        SetupButtons();
        HidePauseMenu();
    }

    void SetupButtons()
    {
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        if (titleButton != null)
            titleButton.onClick.AddListener(GoToTitle);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    public void ShowPauseMenu()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }
        
        // PlayerHUD 숨기기
        if (playerHUD != null)
        {
            playerHUD.SetActive(false);
        }
    }

    public void HidePauseMenu()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        
        // PlayerHUD 다시 보이기
        if (playerHUD != null)
        {
            playerHUD.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        GameManager.Instance.ResumeGame();
    }

    public void GoToTitle()
    {
        GameManager.Instance.LoadTitleScene();
    }

    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }

}