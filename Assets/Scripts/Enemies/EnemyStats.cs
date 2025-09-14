using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 50;
    [SerializeField] private int currentHealth;
    
    [Header("Visual Feedback")]
    [SerializeField] private float damageFlashDuration = 0.2f;
    [SerializeField] private Color damageFlashColor = Color.red;
    
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isDead = false;
    
    public bool IsDead => isDead;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public float HealthPercentage => (float)currentHealth / maxHealth;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        
        Debug.Log($"[EnemyStats] {name} took {damage} damage. Health: {currentHealth}/{maxHealth}");
        
        // 데미지 시각적 피드백
        if (spriteRenderer != null)
        {
            StartCoroutine(DamageFlashRoutine());
        }
        
        // 체력이 0이 되면 사망 처리
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (isDead) return;
        
        currentHealth += amount;
        currentHealth = Mathf.Min(maxHealth, currentHealth);
        
        Debug.Log($"[EnemyStats] {name} healed {amount}. Health: {currentHealth}/{maxHealth}");
    }

    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }

    void Die()
    {
        if (isDead) return;
        
        isDead = true;
        Debug.Log($"[EnemyStats] {name} has died!");
        
        // EnemyController에게 죽음을 알림 (있다면)
        EnemyController controller = GetComponent<EnemyController>();
        if (controller != null)
        {
            // EnemyController가 상태를 관리하므로 직접 제거하지 않음
        }
        else
        {
            // EnemyController가 없다면 기본 적 AI 처리 (기존 EnemyAI 호환)
            Destroy(gameObject, 1f);
        }
    }

    System.Collections.IEnumerator DamageFlashRoutine()
    {
        if (spriteRenderer == null) yield break;
        
        // 데미지 색상으로 변경
        spriteRenderer.color = damageFlashColor;
        
        // 잠시 대기
        yield return new WaitForSeconds(damageFlashDuration);
        
        // 원래 색상으로 복원
        spriteRenderer.color = originalColor;
    }

    void OnValidate()
    {
        // Inspector에서 값 변경 시 체력 동기화
        if (Application.isPlaying)
        {
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        }
        else
        {
            currentHealth = maxHealth;
        }
    }

    // 체력 바 UI를 위한 이벤트 (선택사항)
    public System.Action<int, int> OnHealthChanged;
    
    void NotifyHealthChanged()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    // 외부에서 체력 직접 설정 (치트, 디버그용)
    public void SetHealth(int health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
        
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
        
        NotifyHealthChanged();
    }

    // 데미지 받을 수 있는지 확인
    public bool CanTakeDamage()
    {
        return !isDead;
    }
}