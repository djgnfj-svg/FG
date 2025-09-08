using System.Collections;
using System.Collections.Generic;
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
    
    private bool isInvincible = false;
    private SpriteRenderer spriteRenderer;
    
    [Header("Events")]
    public UnityEvent<int> OnHealthChanged;
    public UnityEvent<int> OnManaChanged;
    public UnityEvent OnPlayerDeath;

    void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        OnHealthChanged?.Invoke(currentHealth);
        OnManaChanged?.Invoke(currentMana);
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
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void UseMana(int manaAmount)
    {
        currentMana -= manaAmount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);
        OnManaChanged?.Invoke(currentMana);
    }

    public void RestoreMana(int manaAmount)
    {
        currentMana += manaAmount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);
        OnManaChanged?.Invoke(currentMana);
    }

    public bool HasMana(int requiredMana)
    {
        return currentMana >= requiredMana;
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
    }

    void Die()
    {
        Debug.Log("Player died!");
        OnPlayerDeath?.Invoke();
        GameManager.Instance.GameOver();
    }

    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }

    public float GetManaPercentage()
    {
        return (float)currentMana / maxMana;
    }
}