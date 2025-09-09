using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [Header("Portal Settings")]
    public string nextSceneName = "GameScene2";
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Portal activated! Moving to next scene...");
            SceneManager.LoadScene(nextSceneName);
        }
    }
    
    void OnDrawGizmos()
    {
        // 씬 뷰에서 포털 위치 표시
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}