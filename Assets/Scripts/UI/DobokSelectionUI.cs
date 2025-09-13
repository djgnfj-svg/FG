using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DobokSelectionUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject selectionPanel;
    [SerializeField] private GameObject[] dobokButtons = new GameObject[3];
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI[] dobokNameTexts = new TextMeshProUGUI[3];
    [SerializeField] private Image[] dobokImages = new Image[3];
    
    [Header("Animation")]
    [SerializeField] private float fadeInDuration = 0.5f;
    
    private DobokSelector dobokSelector;
    private List<SimpleDobokData> currentDoboks;

    void Awake()
    {
        dobokSelector = FindObjectOfType<DobokSelector>();
        
        // 시작 시 UI 숨기기
        if (selectionPanel != null)
        {
            selectionPanel.SetActive(false);
        }
    }

    public void ShowSelection(List<SimpleDobokData> doboks)
    {
        currentDoboks = doboks;
        
        // UI 패널 활성화
        if (selectionPanel != null)
        {
            selectionPanel.SetActive(true);
            
            // 타이틀 설정
            if (titleText != null)
            {
                int stage = StageManager.Instance != null ? StageManager.Instance.GetCurrentStage() : 1;
                titleText.text = $"Stage {stage} Clear!\n도복을 선택하세요";
            }
            
            // 도복 버튼 설정
            for (int i = 0; i < 3 && i < doboks.Count; i++)
            {
                SetupDobokButton(i, doboks[i]);
            }
            
            // 페이드 인 애니메이션
            StartCoroutine(FadeInAnimation());
        }
        else
        {
            Debug.LogWarning("Selection Panel is not assigned!");
            // UI가 없으면 자동 선택
            AutoSelectDobok();
        }
    }

    private void SetupDobokButton(int index, SimpleDobokData dobok)
    {
        // 버튼 활성화
        if (dobokButtons[index] != null)
        {
            dobokButtons[index].SetActive(true);
            
            // 버튼 클릭 이벤트 설정
            Button btn = dobokButtons[index].GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                int buttonIndex = index; // 클로저를 위한 로컬 변수
                btn.onClick.AddListener(() => OnDobokButtonClick(buttonIndex));
            }
        }
        
        // 도복 이름 표시
        if (dobokNameTexts[index] != null)
        {
            dobokNameTexts[index].text = dobok.dobokName;
        }
        
        // 도복 색상 표시 (이미지로)
        if (dobokImages[index] != null)
        {
            dobokImages[index].color = dobok.tintColor;
        }
    }

    private void OnDobokButtonClick(int index)
    {
        Debug.Log($"Dobok button {index} clicked!");
        
        // 버튼 클릭 효과음 추가 지점
        
        // 도복 선택
        if (dobokSelector != null)
        {
            dobokSelector.SelectDobok(index);
        }
        
        // UI 숨기기
        HideSelection();
    }

    public void HideSelection()
    {
        if (selectionPanel != null)
        {
            StartCoroutine(FadeOutAnimation());
        }
    }

    private IEnumerator FadeInAnimation()
    {
        CanvasGroup canvasGroup = selectionPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = selectionPanel.AddComponent<CanvasGroup>();
        }
        
        canvasGroup.alpha = 0f;
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeInDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOutAnimation()
    {
        CanvasGroup canvasGroup = selectionPanel.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            float elapsedTime = 0f;
            
            while (elapsedTime < fadeInDuration)
            {
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeInDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            canvasGroup.alpha = 0f;
        }
        
        selectionPanel.SetActive(false);
    }

    private void AutoSelectDobok()
    {
        // UI가 없을 때 자동으로 첫 번째 도복 선택
        Debug.Log("Auto-selecting first dobok...");
        if (dobokSelector != null)
        {
            dobokSelector.SelectDobok(0);
        }
    }
}