using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Portal : MonoBehaviour
{
    [Header("Portal Settings")]
    public string nextSceneName = "GameScene";
    public bool isActivated = false;
    
    [Header("Visual Settings")]
    public float activationDelay = 0.5f;
    public Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    public Color activeColor = Color.cyan;
    
    private SpriteRenderer spriteRenderer;
    private bool playerEntered = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // 시작 시 포털 비활성화
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        // 포털이 활성화될 때 시각 효과
        StartCoroutine(ActivatePortal());
    }

    private IEnumerator ActivatePortal()
    {
        isActivated = false;
        
        // 색상 변경
        if (spriteRenderer != null)
        {
            spriteRenderer.color = inactiveColor;
        }
        
        yield return new WaitForSeconds(activationDelay);
        
        isActivated = true;
        
        // 활성화 색상
        if (spriteRenderer != null)
        {
            spriteRenderer.color = activeColor;
        }
        
        Debug.Log($"Portal activated! Ready to go to: {nextSceneName}");
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActivated || playerEntered) return;
        
        if (other.CompareTag("Player"))
        {
            playerEntered = true;
            Debug.Log($"Portal entered! Moving to {nextSceneName}...");
            
            // 플레이어 데이터 저장 (위치는 저장하지 않음 - SpawnPoint 시스템 사용)
            if (GameManager.Instance != null)
            {
                // PlayerStats가 있으면 체력/마나 데이터만 저장
                PlayerStats playerStats = other.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    GameManager.Instance.SavePlayerData(playerStats);
                }
            }
            
            // 현재 씬이 로비면 직접 씬 로드, 아니면 StageManager 사용
            string currentSceneName = SceneManager.GetActiveScene().name;
            
            if (currentSceneName == "LobbyScene")
            {
                // 로비에서는 설정된 nextSceneName으로 직접 이동
                SceneManager.LoadScene(nextSceneName);
            }
            else if (StageManager.Instance != null)
            {
                if (nextSceneName == "TitleScene")
                {
                    // Stage 완료 - 타이틀로
                    StageManager.Instance.ResetStageProgress();
                    
                    // 게임 완료 시 플레이어 데이터 리셋
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.ResetPlayerData();
                    }
                    
                    SceneManager.LoadScene("TitleScene");
                }
                else
                {
                    StageManager.Instance.LoadNextStage();
                }
            }
            else
            {
                // StageManager가 없으면 직접 씬 로드
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerEntered = false;
        }
    }
    
    void OnDrawGizmos()
    {
        // 씬 뷰에서 포털 위치 표시
        if (isActivated || Application.isPlaying == false)
        {
            Gizmos.color = activeColor;
        }
        else
        {
            Gizmos.color = inactiveColor;
        }
        
        Gizmos.DrawWireCube(transform.position, transform.localScale);
        
        // 포털 라벨
        UnityEditor.Handles.Label(transform.position + Vector3.up * 2, 
            $"Portal → {nextSceneName}");
    }
    
    public void SetNextScene(string sceneName)
    {
        nextSceneName = sceneName;
    }
}