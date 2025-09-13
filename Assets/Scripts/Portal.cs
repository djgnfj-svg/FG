using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Portal : MonoBehaviour
{
    [Header("Portal Settings")]
    public string nextSceneName = "GameScene2";
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
            
            // 플레이어 위치 저장
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetPlayerPosition(other.transform.position);
                
                // PlayerStats가 있으면 데이터 저장
                PlayerStats playerStats = other.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    GameManager.Instance.SavePlayerData(playerStats);
                }
            }
            
            // 스테이지 매니저가 있으면 사용, 없으면 직접 로드
            if (StageManager.Instance != null)
            {
                if (nextSceneName == "TitleScene")
                {
                    // Stage 3 완료 - 타이틀로
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