using UnityEngine;

[System.Serializable]
public class PlayerData
{
    [Header("Health System")]
    public int currentHealth = 100;
    public int maxHealth = 100;
    public int currentMana = 50;
    public int maxMana = 50;
    
    [Header("Visual Settings")]
    public Color dobokColor = Color.white;
    public bool hasDobokSelected = false;
    
    [Header("Position & Progress")]
    public Vector3 lastPosition = Vector3.zero;
    public int currentStage = 1;
    public string lastSceneName = "GameScene";
    
    [Header("Gameplay Stats")]
    public bool isInvincible = false;
    public float invincibilityTimer = 0f;
    
    // 기본 생성자
    public PlayerData()
    {
        ResetToDefaults();
    }
    
    // 기본값으로 초기화
    public void ResetToDefaults()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        dobokColor = Color.white;
        hasDobokSelected = false;
        lastPosition = Vector3.zero;
        currentStage = 1;
        lastSceneName = "GameScene";
        isInvincible = false;
        invincibilityTimer = 0f;
    }
    
    // 체력 비율 반환
    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }
    
    // 마나 비율 반환
    public float GetManaPercentage()
    {
        return (float)currentMana / maxMana;
    }
    
    // 데이터 복사
    public void CopyFrom(PlayerData other)
    {
        currentHealth = other.currentHealth;
        maxHealth = other.maxHealth;
        currentMana = other.currentMana;
        maxMana = other.maxMana;
        dobokColor = other.dobokColor;
        hasDobokSelected = other.hasDobokSelected;
        lastPosition = other.lastPosition;
        currentStage = other.currentStage;
        lastSceneName = other.lastSceneName;
        isInvincible = other.isInvincible;
        invincibilityTimer = other.invincibilityTimer;
    }
}