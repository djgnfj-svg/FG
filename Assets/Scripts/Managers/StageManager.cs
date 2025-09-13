using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    private static StageManager instance;
    public static StageManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<StageManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("StageManager");
                    instance = go.AddComponent<StageManager>();
                }
            }
            return instance;
        }
    }

    [Header("Stage Settings")]
    public int currentStage = 1;
    public const int MAX_STAGES = 2;  // 2스테이지만!
    
    public int CurrentStage => currentStage;
    
    [Header("Stage Scene Names")]
    private string[] stageSceneNames = new string[]
    {
        "GameScene",      // Stage 1
        "GameScene2"      // Stage 2 (마지막 스테이지!)
    };

    [Header("Stage Clear State")]
    private bool isStageCleared = false;
    private GameObject currentPortal;

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
    
    void OnEnable()
    {
        // 씬 로드 이벤트 구독
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnDisable()
    {
        // 씬 로드 이벤트 구독 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 새로운 씬이 로드될 때마다 스테이지 클리어 상태 초기화
        isStageCleared = false;
        Debug.Log($"Scene loaded: {scene.name}, isStageCleared reset to false");
    }

    void Start()
    {
        // 씬이 변경되면 스테이지 클리어 상태 초기화
        isStageCleared = false;
        
        // 현재 씬 이름으로 스테이지 번호 판단
        string currentSceneName = SceneManager.GetActiveScene().name;
        for (int i = 0; i < stageSceneNames.Length; i++)
        {
            if (stageSceneNames[i] == currentSceneName)
            {
                currentStage = i + 1;
                break;
            }
        }
        
        Debug.Log($"Current Stage: {currentStage}, isStageCleared: {isStageCleared}");
    }

    public void OnStageClear()
    {
        if (isStageCleared) return;
        
        isStageCleared = true;
        Debug.Log($"Stage {currentStage} Cleared!");
        
        // 도복 선택 UI 표시
        StartCoroutine(ShowDobokSelection());
    }

    private IEnumerator ShowDobokSelection()
    {
        yield return new WaitForSeconds(0.5f);
        
        // DobokSelector에게 도복 선택 UI 표시 요청
        DobokSelector selector = FindObjectOfType<DobokSelector>();
        if (selector != null)
        {
            selector.ShowDobokSelection();
        }
        else
        {
            Debug.LogError("DobokSelector not found!");
            // 도복 선택 없이 바로 포털 활성화
            ActivatePortal();
        }
    }

    public void OnDobokSelected()
    {
        Debug.Log("Dobok selected! Activating portal...");
        ActivatePortal();
    }

    private void ActivatePortal()
    {
        // 포털 찾아서 활성화
        Portal portal = FindObjectOfType<Portal>(true);
        if (portal != null)
        {
            portal.gameObject.SetActive(true);
            currentPortal = portal.gameObject;
            
            // 로비가 아닐 때만 포털의 다음 스테이지 설정
            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (currentSceneName != "LobbyScene")
            {
                // 포털에 다음 스테이지 설정
                if (currentStage >= MAX_STAGES)
                {
                    // Stage 2 클리어 시 타이틀로
                    portal.nextSceneName = "TitleScene";
                    Debug.Log("Final stage cleared! Portal will lead to Title.");
                }
                else
                {
                    // 다음 스테이지로
                    portal.nextSceneName = stageSceneNames[currentStage]; // currentStage는 1부터 시작이므로 다음 스테이지 인덱스
                    Debug.Log($"Portal will lead to Stage {currentStage + 1}: {portal.nextSceneName}");
                }
            }
            else
            {
                Debug.Log($"Lobby portal activated, keeping original destination: {portal.nextSceneName}");
            }
        }
        else
        {
            Debug.LogError("Portal not found in the scene!");
        }
    }

    public void LoadNextStage()
    {
        if (currentStage >= MAX_STAGES)
        {
            // 게임 완료! 타이틀로
            Debug.Log("Game Complete! Returning to title...");
            currentStage = 1; // 스테이지 초기화
            SceneManager.LoadScene("TitleScene");
        }
        else
        {
            currentStage++;
            string nextScene = stageSceneNames[currentStage - 1];
            Debug.Log($"Loading Stage {currentStage}: {nextScene}");
            SceneManager.LoadScene(nextScene);
        }
        
        isStageCleared = false;
    }

    public int GetCurrentStage()
    {
        return currentStage;
    }

    public bool IsLastStage()
    {
        return currentStage >= MAX_STAGES;
    }

    public void ResetStageProgress()
    {
        currentStage = 1;
        isStageCleared = false;
    }
}