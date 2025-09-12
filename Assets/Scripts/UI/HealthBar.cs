/* 
 * TODO: 체력 시스템 재구현 필요
 * PlayerStats 클래스 의존성 제거 후 활성화
 */
/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("UI References")]
    public Slider healthSlider;
    public Image fillImage;
    
    [Header("Colors")]
    public Color highHealthColor = Color.green;
    public Color mediumHealthColor = Color.yellow;
    public Color lowHealthColor = Color.red;
    
    [Header("Thresholds")]
    public float mediumHealthThreshold = 0.6f;
    public float lowHealthThreshold = 0.3f;

    private PlayerStats playerStats;

    void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();
        
        if (playerStats != null)
        {
            playerStats.OnHealthChanged.AddListener(UpdateHealthBar);
            UpdateHealthBar(playerStats.currentHealth);
        }

        if (healthSlider == null)
            healthSlider = GetComponent<Slider>();
    }

    public void UpdateHealthBar(int currentHealth)
    {
        if (healthSlider != null && playerStats != null)
        {
            float healthPercentage = (float)currentHealth / playerStats.maxHealth;
            healthSlider.value = healthPercentage;
            
            if (fillImage != null)
            {
                if (healthPercentage > mediumHealthThreshold)
                {
                    fillImage.color = highHealthColor;
                }
                else if (healthPercentage > lowHealthThreshold)
                {
                    fillImage.color = mediumHealthColor;
                }
                else
                {
                    fillImage.color = lowHealthColor;
                }
            }
        }
    }

    void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.OnHealthChanged.RemoveListener(UpdateHealthBar);
        }
    }
}
*/