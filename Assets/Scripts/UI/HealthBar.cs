using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image fillImage;
    
    [Header("Colors")]
    [SerializeField] private Color highHealthColor = Color.green;
    [SerializeField] private Color mediumHealthColor = Color.yellow;
    [SerializeField] private Color lowHealthColor = Color.red;
    
    [Header("Thresholds")]
    [SerializeField] private float mediumHealthThreshold = 0.6f;
    [SerializeField] private float lowHealthThreshold = 0.3f;

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