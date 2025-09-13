using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image healthBarFill;
    [SerializeField] private Image backgroundBar;
    [SerializeField] private Canvas canvas;
    
    [Header("Display Settings")]
    [SerializeField] private bool showOnlyWhenDamaged = true;
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private bool followEnemy = true;
    [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0);
    
    [Header("Visual Settings")]
    [SerializeField] private Color fullHealthColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;
    [SerializeField] private float lowHealthThreshold = 0.3f;
    
    private EnemyStats enemyStats;
    private float lastDamageTime;
    private bool isVisible = true;
    private Transform enemyTransform;
    
    void Awake()
    {
        // Canvas 설정 (World Space)
        if (canvas == null)
            canvas = GetComponentInChildren<Canvas>();
            
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;
        }
        
        // 기본적으로 숨김 상태
        if (showOnlyWhenDamaged)
        {
            SetVisible(false);
        }
    }
    
    void Start()
    {
        // EnemyStats 컴포넌트 찾기
        enemyStats = GetComponentInParent<EnemyStats>();
        enemyTransform = transform.parent;
        
        if (enemyStats == null)
        {
            Debug.LogWarning("[EnemyHealthBar] EnemyStats not found in parent!");
            return;
        }
        
        // 체력 변경 이벤트 구독
        enemyStats.OnHealthChanged += OnHealthChanged;
        
        // 초기 체력 표시
        UpdateHealthBar(enemyStats.CurrentHealth, enemyStats.MaxHealth);
    }
    
    void Update()
    {
        // 적을 따라다니기
        if (followEnemy && enemyTransform != null)
        {
            transform.position = enemyTransform.position + offset;
        }
        
        // 자동 숨기기 (데미지 받은 후 일정 시간)
        if (showOnlyWhenDamaged && isVisible)
        {
            if (Time.time - lastDamageTime > displayDuration)
            {
                SetVisible(false);
            }
        }
        
        // 카메라를 향해 회전 (빌보드 효과)
        if (Camera.main != null)
        {
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                           Camera.main.transform.rotation * Vector3.up);
        }
    }
    
    void OnHealthChanged(int currentHealth, int maxHealth)
    {
        UpdateHealthBar(currentHealth, maxHealth);
        
        // 데미지를 받았을 때만 표시
        if (showOnlyWhenDamaged && currentHealth < maxHealth)
        {
            SetVisible(true);
            lastDamageTime = Time.time;
        }
    }
    
    void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (healthBarFill == null) return;
        
        float healthPercentage = (float)currentHealth / maxHealth;
        healthBarFill.fillAmount = healthPercentage;
        
        // 체력에 따른 색상 변경
        Color healthColor = Color.Lerp(lowHealthColor, fullHealthColor, 
                                     healthPercentage / lowHealthThreshold);
        healthBarFill.color = healthColor;
    }
    
    void SetVisible(bool visible)
    {
        isVisible = visible;
        gameObject.SetActive(visible);
    }
    
    void OnDestroy()
    {
        // 이벤트 구독 해제
        if (enemyStats != null)
        {
            enemyStats.OnHealthChanged -= OnHealthChanged;
        }
    }
    
    // 외부에서 수동으로 표시/숨기기
    public void Show()
    {
        SetVisible(true);
        lastDamageTime = Time.time;
    }
    
    public void Hide()
    {
        SetVisible(false);
    }
    
    // 체력바 위치 조정
    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }
}