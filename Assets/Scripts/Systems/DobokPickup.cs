using UnityEngine;
using System.Collections;

public class DobokPickup : MonoBehaviour
{
    [Header("Dobok Data")]
    public SimpleDobokData dobokData;
    
    [Header("Visual Effects")]
    public float floatHeight = 0.3f;
    public float floatSpeed = 2f;
    public float hoverScale = 1.2f;
    public float scaleSpeed = 3f;
    
    [Header("Selection")]
    public float selectionRadius = 1.5f;
    public GameObject nameTextPrefab; // TextMeshPro 3D 텍스트용
    
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private bool isSelected = false;
    private bool playerNearby = false;
    
    private SpriteRenderer spriteRenderer;
    private GameObject nameText;
    private TextMesh textMesh;

    void Start()
    {
        originalPosition = transform.position;
        originalScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // 도복 색상 적용
        if (dobokData != null && spriteRenderer != null)
        {
            spriteRenderer.color = dobokData.tintColor;
        }
        
        // 이름 텍스트 생성
        CreateNameText();
        
        // Trigger 설정
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    void Update()
    {
        if (!isSelected)
        {
            // 부유 애니메이션
            FloatingAnimation();
            
            // 호버 효과
            HoverEffect();
            
            // E키 입력 체크
            if (playerNearby && Input.GetKeyDown(KeyCode.E))
            {
                SelectDobok();
            }
        }
    }

    private void FloatingAnimation()
    {
        float newY = originalPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void HoverEffect()
    {
        Vector3 targetScale = playerNearby ? originalScale * hoverScale : originalScale;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
        
        // 색상 효과
        if (spriteRenderer != null && dobokData != null)
        {
            Color targetColor = playerNearby ? 
                Color.Lerp(dobokData.tintColor, Color.white, 0.3f) : 
                dobokData.tintColor;
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, targetColor, Time.deltaTime * scaleSpeed);
        }
    }

    private void CreateNameText()
    {
        if (dobokData == null) return;
        
        // 3D Text 오브젝트 생성
        GameObject textObj = new GameObject("DobokName");
        textObj.transform.SetParent(transform);
        textObj.transform.localPosition = Vector3.up * 1.5f;
        
        // TextMesh 컴포넌트 추가
        textMesh = textObj.AddComponent<TextMesh>();
        textMesh.text = dobokData.dobokName;
        textMesh.fontSize = 20;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.color = Color.white;
        
        // 텍스트 렌더러 설정
        MeshRenderer textRenderer = textObj.GetComponent<MeshRenderer>();
        if (textRenderer != null)
        {
            textRenderer.sortingOrder = 10; // 다른 오브젝트 위에 표시
        }
        
        nameText = textObj;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            
            // 선택 가능 표시
            if (textMesh != null)
            {
                textMesh.color = Color.yellow;
            }
            
            Debug.Log($"플레이어가 {dobokData.dobokName} 근처에 있습니다. E키를 눌러 선택하세요!");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            
            if (textMesh != null)
            {
                textMesh.color = Color.white;
            }
        }
    }

    void OnMouseDown()
    {
        // 클릭으로도 선택 가능
        if (playerNearby && !isSelected)
        {
            SelectDobok();
        }
    }

    public void SelectDobok()
    {
        if (isSelected) return;
        
        isSelected = true;
        Debug.Log($"✅ 도복 선택됨: {dobokData.dobokName}");
        
        // DobokSelector에게 선택 알림
        DobokSelector selector = FindObjectOfType<DobokSelector>();
        if (selector != null)
        {
            selector.OnDobokPickupSelected(this);
        }
        
        // 선택 효과
        StartCoroutine(SelectionEffect());
    }

    private IEnumerator SelectionEffect()
    {
        // 선택 애니메이션
        float duration = 0.5f;
        float elapsedTime = 0f;
        
        Vector3 startScale = transform.localScale;
        Vector3 endScale = startScale * 1.5f;
        
        // 크기 증가 + 페이드아웃
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            
            // 크기 변화
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            
            // 페이드아웃
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = 1f - t;
                spriteRenderer.color = color;
            }
            
            // 텍스트도 페이드아웃
            if (textMesh != null)
            {
                Color textColor = textMesh.color;
                textColor.a = 1f - t;
                textMesh.color = textColor;
            }
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // 오브젝트 제거
        Destroy(gameObject);
    }

    public void DestroyUnselected()
    {
        if (!isSelected)
        {
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        float duration = 0.3f;
        float elapsedTime = 0f;
        
        Color originalColor = spriteRenderer != null ? spriteRenderer.color : Color.white;
        Color originalTextColor = textMesh != null ? textMesh.color : Color.white;
        
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float alpha = 1f - t;
            
            // 스프라이트 페이드아웃
            if (spriteRenderer != null)
            {
                Color color = originalColor;
                color.a = alpha;
                spriteRenderer.color = color;
            }
            
            // 텍스트 페이드아웃
            if (textMesh != null)
            {
                Color textColor = originalTextColor;
                textColor.a = alpha;
                textMesh.color = textColor;
            }
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        // 선택 범위 표시
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, selectionRadius);
    }
}