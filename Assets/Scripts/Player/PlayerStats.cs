using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : MonoBehaviour
{
    [Header("Player Stats")]
    public int maxHealth = 100;
    public int currentHealth;
    public int maxMana = 50;
    public int currentMana;
    
    [Header("Invincibility Settings")]
    public float invincibilityDuration = 1f;
    public float flashInterval = 0.1f;
    
    public bool isInvincible = false;
    private SpriteRenderer spriteRenderer;
    private Coroutine invincibilityCoroutine;
    
    [Header("Events")]
    public UnityEvent<int> OnHealthChanged;
    public UnityEvent<int> OnManaChanged;
    public UnityEvent OnPlayerDeath;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // GameManager에서 데이터 로드
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadPlayerData(this);
        }
        else
        {
            // GameManager가 없으면 기본값으로 초기화
            currentHealth = maxHealth;
            currentMana = maxMana;
        }
        
        // UI 이벤트 발생
        OnHealthChanged?.Invoke(currentHealth);
        OnManaChanged?.Invoke(currentMana);
        
        Debug.Log($"[PlayerStats] Initialized - Health: {currentHealth}/{maxHealth}, Mana: {currentMana}/{maxMana}");
    }

    void OnDestroy()
    {
        // 씬 전환 전 데이터 저장
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SavePlayerData(this);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        OnHealthChanged?.Invoke(currentHealth);
        
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartInvincibility();
        }
        
        Debug.Log($"[PlayerStats] Took {damage} damage. Health: {currentHealth}/{maxHealth}");
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
        
        Debug.Log($"[PlayerStats] Healed {healAmount}. Health: {currentHealth}/{maxHealth}");
    }

    public void UseMana(int manaAmount)
    {
        currentMana -= manaAmount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);
        OnManaChanged?.Invoke(currentMana);
        
        Debug.Log($"[PlayerStats] Used {manaAmount} mana. Mana: {currentMana}/{maxMana}");
    }

    public void RestoreMana(int manaAmount)
    {
        currentMana += manaAmount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);
        OnManaChanged?.Invoke(currentMana);
        
        Debug.Log($"[PlayerStats] Restored {manaAmount} mana. Mana: {currentMana}/{maxMana}");
    }

    public bool HasMana(int requiredMana)
    {
        return currentMana >= requiredMana;
    }

    void StartInvincibility()
    {
        if (invincibilityCoroutine != null)
        {
            StopCoroutine(invincibilityCoroutine);
        }
        invincibilityCoroutine = StartCoroutine(InvincibilityCoroutine());
    }

    IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        float timer = 0f;

        while (timer < invincibilityDuration)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
                yield return new WaitForSeconds(flashInterval);
                spriteRenderer.color = Color.white;
                yield return new WaitForSeconds(flashInterval);
            }
            else
            {
                yield return new WaitForSeconds(flashInterval * 2);
            }
            
            timer += flashInterval * 2;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
        
        isInvincible = false;
        invincibilityCoroutine = null;
    }

    void Die()
    {
        Debug.Log("[PlayerStats] Player died!");
        OnPlayerDeath?.Invoke();
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }

    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }

    public float GetManaPercentage()
    {
        return (float)currentMana / maxMana;
    }

    // GameManager와의 데이터 동기화를 위한 메서드
    public void SyncWithGameManager()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SavePlayerData(this);
        }
    }
}