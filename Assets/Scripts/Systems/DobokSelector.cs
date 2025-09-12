using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DobokSelector : MonoBehaviour
{
    [Header("Dobok Pool")]
    private List<SimpleDobokData> allDoboks = new List<SimpleDobokData>();
    private List<SimpleDobokData> selectedDoboks = new List<SimpleDobokData>();
    
    [Header("Current Selection")]
    private SimpleDobokData currentDobok;
    
    [Header("UI Reference")]
    private DobokSelectionUI selectionUI;
    
    [Header("Spawner Reference")]
    private DobokSpawner dobokSpawner;

    void Awake()
    {
        InitializeDoboks();
        selectionUI = FindObjectOfType<DobokSelectionUI>(true);
        dobokSpawner = FindObjectOfType<DobokSpawner>();
    }

    void InitializeDoboks()
    {
        // 도복 데이터 초기화 (총 9개 준비)
        allDoboks.Clear();
        
        // 빨간 계열
        allDoboks.Add(new SimpleDobokData("빨간 도복", Color.red));
        allDoboks.Add(new SimpleDobokData("진홍 도복", new Color(0.5f, 0f, 0f)));
        allDoboks.Add(new SimpleDobokData("주황 도복", new Color(1f, 0.5f, 0f)));
        
        // 파란 계열
        allDoboks.Add(new SimpleDobokData("파란 도복", Color.blue));
        allDoboks.Add(new SimpleDobokData("하늘 도복", Color.cyan));
        allDoboks.Add(new SimpleDobokData("남색 도복", new Color(0f, 0f, 0.5f)));
        
        // 초록 계열
        allDoboks.Add(new SimpleDobokData("초록 도복", Color.green));
        allDoboks.Add(new SimpleDobokData("연두 도복", new Color(0.5f, 1f, 0f)));
        allDoboks.Add(new SimpleDobokData("어둠 도복", new Color(0.2f, 0.2f, 0.2f)));
    }

    public void ShowDobokSelection()
    {
        Debug.Log("=== ShowDobokSelection() called ===");
        
        // 랜덤으로 3개 도복 선택
        selectedDoboks.Clear();
        List<SimpleDobokData> tempList = new List<SimpleDobokData>(allDoboks);
        
        Debug.Log($"Available doboks: {allDoboks.Count}");
        
        for (int i = 0; i < 3 && tempList.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, tempList.Count);
            selectedDoboks.Add(tempList[randomIndex]);
            Debug.Log($"Selected dobok {i+1}: {tempList[randomIndex].dobokName}");
            tempList.RemoveAt(randomIndex);
        }
        
        // Spawner로 도복 아이템 생성
        if (dobokSpawner != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Vector3 playerPos = player != null ? player.transform.position : transform.position;
            dobokSpawner.SpawnDoboks(selectedDoboks, playerPos);
            Debug.Log("Doboks spawned for selection!");
            
        }
        // UI에 표시
        else if (selectionUI != null)
        {
            selectionUI.ShowSelection(selectedDoboks);
        }
        else
        {
            Debug.LogError("No DobokSpawner or UI found!");
        }
    }

    public void SelectDobok(int index)
    {
        if (index >= 0 && index < selectedDoboks.Count)
        {
            currentDobok = selectedDoboks[index];
            ApplyDobokToPlayer();
            
            Debug.Log($"Selected: {currentDobok.dobokName}");
            
            // StageManager에게 선택 완료 알림
            if (StageManager.Instance != null)
            {
                StageManager.Instance.OnDobokSelected();
            }
        }
    }

    private void ApplyDobokToPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // 플레이어 색상 변경
            SpriteRenderer playerRenderer = player.GetComponent<SpriteRenderer>();
            if (playerRenderer != null)
            {
                playerRenderer.color = currentDobok.tintColor;
                Debug.Log($"Player color changed to {currentDobok.dobokName}");
            }
            
            // 나중에 스프라이트 교체 기능 추가 가능
            if (currentDobok.dobokSprite != null)
            {
                playerRenderer.sprite = currentDobok.dobokSprite;
            }
        }
        else
        {
            Debug.LogWarning("Player not found to apply dobok!");
        }
    }

    public SimpleDobokData GetCurrentDobok()
    {
        return currentDobok;
    }

    public List<SimpleDobokData> GetSelectedDoboks()
    {
        return selectedDoboks;
    }
    
    // DobokPickup에서 호출되는 메서드
    public void OnDobokPickupSelected(DobokPickup pickup)
    {
        if (pickup.dobokData != null)
        {
            currentDobok = pickup.dobokData;
            ApplyDobokToPlayer();
            
            Debug.Log($"Dobok selected via pickup: {currentDobok.dobokName}");
            
            // 선택되지 않은 도복들 제거
            if (dobokSpawner != null)
            {
                dobokSpawner.RemoveUnselectedDoboks();
            }
            
            // StageManager에게 선택 완료 알림
            if (StageManager.Instance != null)
            {
                StageManager.Instance.OnDobokSelected();
            }
        }
    }
}