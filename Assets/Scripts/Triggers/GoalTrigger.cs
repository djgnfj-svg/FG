using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    [Header("Visual Settings")]
    public Color gizmoColor = Color.yellow;
    
    private bool isTriggered = false;

    void Start()
    {
        // Trigger로 설정되어 있는지 확인
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
        else
        {
            Debug.LogWarning("GoalTrigger needs a Collider2D component!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isTriggered) return;
        
        if (other.CompareTag("Player"))
        {
            isTriggered = true;
            Debug.Log("Goal Reached! Stage Clear!");
            
            // StageManager에게 스테이지 클리어 알림
            if (StageManager.Instance != null)
            {
                StageManager.Instance.OnStageClear();
            }
            else
            {
                Debug.LogError("StageManager not found!");
            }
            
            // 시각적 피드백 (선택사항)
            GetComponent<Renderer>()?.material.SetColor("_Color", Color.green);
        }
    }

    void OnDrawGizmos()
    {
        // 씬 뷰에서 Goal 위치 표시
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
        
        // Goal 텍스트 표시
        UnityEditor.Handles.Label(transform.position + Vector3.up, "GOAL");
    }

    public void ResetGoal()
    {
        isTriggered = false;
    }
}