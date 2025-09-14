using UnityEngine;
using UnityEngine.UI;

public class CinematicBars : MonoBehaviour
{
    [Header("Letterbox Settings")]
    [SerializeField] private float barHeight = 100f;
    [SerializeField] private Color barColor = Color.black;
    [SerializeField] private bool showOnStart = true;

    private GameObject topBar;
    private GameObject bottomBar;
    private Canvas letterboxCanvas;

    void Start()
    {
        CreateCinematicBars();

        if (showOnStart)
        {
            ShowBars();
        }
        else
        {
            HideBars();
        }
    }

    void CreateCinematicBars()
    {
        // Canvas 생성
        GameObject canvasObj = new GameObject("LetterboxCanvas");
        letterboxCanvas = canvasObj.AddComponent<Canvas>();
        letterboxCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        letterboxCanvas.sortingOrder = 999; // 최상위

        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // 상단 바
        topBar = CreateBar("TopBar", true);

        // 하단 바
        bottomBar = CreateBar("BottomBar", false);
    }

    GameObject CreateBar(string name, bool isTop)
    {
        GameObject bar = new GameObject(name);
        bar.transform.SetParent(letterboxCanvas.transform, false);

        Image image = bar.AddComponent<Image>();
        image.color = barColor;

        RectTransform rect = bar.GetComponent<RectTransform>();

        if (isTop)
        {
            // 상단 바 설정
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
        }
        else
        {
            // 하단 바 설정
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 0);
            rect.pivot = new Vector2(0.5f, 0);
        }

        rect.sizeDelta = new Vector2(0, barHeight);
        rect.anchoredPosition = Vector2.zero;

        return bar;
    }

    public void ShowBars()
    {
        if (topBar) topBar.SetActive(true);
        if (bottomBar) bottomBar.SetActive(true);
    }

    public void HideBars()
    {
        if (topBar) topBar.SetActive(false);
        if (bottomBar) bottomBar.SetActive(false);
    }

    public void SetBarHeight(float height)
    {
        barHeight = height;

        if (topBar)
        {
            RectTransform topRect = topBar.GetComponent<RectTransform>();
            topRect.sizeDelta = new Vector2(0, barHeight);
        }

        if (bottomBar)
        {
            RectTransform bottomRect = bottomBar.GetComponent<RectTransform>();
            bottomRect.sizeDelta = new Vector2(0, barHeight);
        }
    }
}