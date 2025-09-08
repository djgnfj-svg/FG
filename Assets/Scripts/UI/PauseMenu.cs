using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuPanel;
    public Button resumeButton;
    public Button titleButton;
    public Button quitButton;

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
    }

    public void HidePauseMenu()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
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