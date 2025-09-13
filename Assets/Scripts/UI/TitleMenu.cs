using UnityEngine;
using UnityEngine.UI;

public class TitleMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    void Start()
    {
        SetupButtons();
        Time.timeScale = 1f;
    }

    void SetupButtons()
    {
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    public void StartGame()
    {
        GameManager.Instance.LoadGameScene();
    }

    public void OpenSettings()
    {
        Debug.Log("[TitleMenu] Opening settings menu");
    }

    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }
}