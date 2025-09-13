using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("Health & Mana")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider manaBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private Image healthFill;
    [SerializeField] private Image manaFill;
    
    [Header("Player Portrait")]
    [SerializeField] private Image playerPortrait;
    [SerializeField] private Image portraitBorder;
    
    [Header("Skills")]
    [SerializeField] private Image skill1Icon;
    [SerializeField] private Image skill2Icon;
    [SerializeField] private TextMeshProUGUI skill1Cooldown;
    [SerializeField] private TextMeshProUGUI skill2Cooldown;
    [SerializeField] private Image skill1CooldownOverlay;
    [SerializeField] private Image skill2CooldownOverlay;
    
    [Header("Skill Settings")]
    [SerializeField] private Sprite defaultSkill1Icon;
    [SerializeField] private Sprite defaultSkill2Icon;
    [SerializeField] private KeyCode skill1Key = KeyCode.Q;
    [SerializeField] private KeyCode skill2Key = KeyCode.E;
    
    [Header("Color Settings")]
    [SerializeField] private Color highHealthColor = Color.green;
    [SerializeField] private Color mediumHealthColor = Color.yellow;
    [SerializeField] private Color lowHealthColor = Color.red;
    [SerializeField] private Color manaColor = new Color(0.2f, 0.5f, 1f);
    
    private PlayerStats playerStats;
    private float skill1CooldownTimer = 0f;
    private float skill2CooldownTimer = 0f;
    private float skill1MaxCooldown = 5f;
    private float skill2MaxCooldown = 10f;
    
    void Start()
    {
        // PlayerStats 찾기
        playerStats = FindObjectOfType<PlayerStats>();
        
        if (playerStats != null)
        {
            // 이벤트 구독
            playerStats.OnHealthChanged.AddListener(UpdateHealthBar);
            playerStats.OnManaChanged.AddListener(UpdateManaBar);
            
            // 초기값 설정
            UpdateHealthBar(playerStats.currentHealth);
            UpdateManaBar(playerStats.currentMana);
        }
        
        // 스킬 아이콘 설정
        if (skill1Icon != null && defaultSkill1Icon != null)
            skill1Icon.sprite = defaultSkill1Icon;
        if (skill2Icon != null && defaultSkill2Icon != null)
            skill2Icon.sprite = defaultSkill2Icon;
            
        // 초기 쿨다운 오버레이 비활성화
        if (skill1CooldownOverlay != null)
            skill1CooldownOverlay.fillAmount = 0;
        if (skill2CooldownOverlay != null)
            skill2CooldownOverlay.fillAmount = 0;
    }
    
    void Update()
    {
        // 스킬 쿨다운 업데이트
        UpdateSkillCooldowns();
        
        // 스킬 입력 처리
        HandleSkillInput();
    }
    
    void UpdateHealthBar(int currentHealth)
    {
        if (playerStats == null) return;
        
        float healthPercent = (float)currentHealth / playerStats.maxHealth;
        
        if (healthBar != null)
        {
            healthBar.value = healthPercent;
        }
        
        if (healthText != null)
        {
            healthText.text = $"{currentHealth}/{playerStats.maxHealth}";
        }
        
        // 체력에 따른 색상 변경
        if (healthFill != null)
        {
            if (healthPercent > 0.6f)
                healthFill.color = highHealthColor;
            else if (healthPercent > 0.3f)
                healthFill.color = mediumHealthColor;
            else
                healthFill.color = lowHealthColor;
        }
    }
    
    void UpdateManaBar(int currentMana)
    {
        if (playerStats == null) return;
        
        float manaPercent = (float)currentMana / playerStats.maxMana;
        
        if (manaBar != null)
        {
            manaBar.value = manaPercent;
        }
        
        if (manaText != null)
        {
            manaText.text = $"{currentMana}/{playerStats.maxMana}";
        }
        
        if (manaFill != null)
        {
            manaFill.color = manaColor;
        }
    }
    
    void UpdateSkillCooldowns()
    {
        // 스킬 1 쿨다운
        if (skill1CooldownTimer > 0)
        {
            skill1CooldownTimer -= Time.deltaTime;
            float cooldownPercent = skill1CooldownTimer / skill1MaxCooldown;
            
            if (skill1CooldownOverlay != null)
                skill1CooldownOverlay.fillAmount = cooldownPercent;
                
            if (skill1Cooldown != null)
            {
                if (skill1CooldownTimer > 0)
                    skill1Cooldown.text = Mathf.CeilToInt(skill1CooldownTimer).ToString();
                else
                    skill1Cooldown.text = "";
            }
        }
        
        // 스킬 2 쿨다운
        if (skill2CooldownTimer > 0)
        {
            skill2CooldownTimer -= Time.deltaTime;
            float cooldownPercent = skill2CooldownTimer / skill2MaxCooldown;
            
            if (skill2CooldownOverlay != null)
                skill2CooldownOverlay.fillAmount = cooldownPercent;
                
            if (skill2Cooldown != null)
            {
                if (skill2CooldownTimer > 0)
                    skill2Cooldown.text = Mathf.CeilToInt(skill2CooldownTimer).ToString();
                else
                    skill2Cooldown.text = "";
            }
        }
    }
    
    void HandleSkillInput()
    {
        // 스킬 1
        if (Input.GetKeyDown(skill1Key) && skill1CooldownTimer <= 0)
        {
            UseSkill1();
        }
        
        // 스킬 2
        if (Input.GetKeyDown(skill2Key) && skill2CooldownTimer <= 0)
        {
            UseSkill2();
        }
    }
    
    void UseSkill1()
    {
        // 마나 체크
        if (playerStats.currentMana < 10)
        {
            Debug.Log("Not enough mana for Skill 1!");
            return;
        }
        
        Debug.Log("Skill 1 activated!");
        playerStats.UseMana(10);
        skill1CooldownTimer = skill1MaxCooldown;
        
        // 여기에 실제 스킬 효과 추가
        // 예: 대시, 버프, 공격 등
    }
    
    void UseSkill2()
    {
        // 마나 체크
        if (playerStats.currentMana < 20)
        {
            Debug.Log("Not enough mana for Skill 2!");
            return;
        }
        
        Debug.Log("Skill 2 activated!");
        playerStats.UseMana(20);
        skill2CooldownTimer = skill2MaxCooldown;
        
        // 여기에 실제 스킬 효과 추가
        // 예: 광역 공격, 회복, 특수 능력 등
    }
    
    public void SetPlayerPortrait(Sprite portrait)
    {
        if (playerPortrait != null && portrait != null)
        {
            playerPortrait.sprite = portrait;
        }
    }
    
    public void SetSkill1(Sprite icon, float cooldown)
    {
        if (skill1Icon != null && icon != null)
        {
            skill1Icon.sprite = icon;
            skill1MaxCooldown = cooldown;
        }
    }
    
    public void SetSkill2(Sprite icon, float cooldown)
    {
        if (skill2Icon != null && icon != null)
        {
            skill2Icon.sprite = icon;
            skill2MaxCooldown = cooldown;
        }
    }
}