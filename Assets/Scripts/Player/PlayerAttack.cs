using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private int attackDamage = 25;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private LayerMask enemyLayer = -1;
    
    [Header("Attack Visual")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackPointOffset = 1f;
    
    private bool canAttack = true;
    private PlayerStats playerStats;
    private PlayerControll playerControll;
    private Animator animator;

    void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        playerControll = GetComponent<PlayerControll>();
        animator = GetComponent<Animator>();
        
        // 공격 포인트가 없으면 생성
        if (attackPoint == null)
        {
            GameObject attackPointObj = new GameObject("AttackPoint");
            attackPointObj.transform.SetParent(transform);
            attackPoint = attackPointObj.transform;
        }
    }

    void Update()
    {
        UpdateAttackPoint();
    }

    void UpdateAttackPoint()
    {
        // 플레이어가 바라보는 방향에 따라 공격 포인트 위치 조정
        if (playerControll != null && attackPoint != null)
        {
            int facing = playerControll.Facing;
            attackPoint.localPosition = new Vector3(facing * attackPointOffset, 0, 0);
        }
    }

    public bool TryAttack()
    {
        if (!canAttack)
        {
            Debug.Log("[PlayerAttack] Attack on cooldown!");
            return false;
        }

        // 공격 실행
        PerformAttack();
        return true;
    }

    void PerformAttack()
    {
        Debug.Log("[PlayerAttack] Performing attack!");
        
        // 쿨다운 시작
        StartCoroutine(AttackCooldownRoutine());
        
        // 애니메이션 재생
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
        
        // 공격 범위 내 적들에게 데미지
        DamageEnemies();
    }

    void DamageEnemies()
    {
        if (attackPoint == null) return;

        // 공격 범위 내의 모든 적 감지
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position, 
            attackRange, 
            enemyLayer
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            // 적 체력 시스템이 있으면 데미지 적용
            EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(attackDamage);
                Debug.Log($"[PlayerAttack] Dealt {attackDamage} damage to {enemy.name}");
            }
            else
            {
                // 기존 적 AI (EnemyAI.cs) 처리
                EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    Destroy(enemy.gameObject);
                    Debug.Log($"[PlayerAttack] Destroyed enemy: {enemy.name}");
                }
            }
        }

        // 공격 이펙트 생성 (선택사항)
        CreateAttackEffect();
    }

    void CreateAttackEffect()
    {
        // 공격 이펙트 생성
        if (attackPoint != null)
        {
            // 개발용 시각적 피드백
            Debug.DrawLine(attackPoint.position + Vector3.up * attackRange, attackPoint.position + Vector3.down * attackRange, Color.red, 0.3f);
            Debug.DrawLine(attackPoint.position + Vector3.left * attackRange, attackPoint.position + Vector3.right * attackRange, Color.red, 0.3f);
        }
    }

    IEnumerator AttackCooldownRoutine()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        Debug.Log("[PlayerAttack] Attack ready!");
    }

    // 공격력 업그레이드 메서드
    public void UpgradeAttackDamage(int amount)
    {
        attackDamage += amount;
        Debug.Log($"[PlayerAttack] Attack damage upgraded to {attackDamage}");
    }

    // 공격 속도 업그레이드
    public void UpgradeAttackSpeed(float reduction)
    {
        attackCooldown = Mathf.Max(0.1f, attackCooldown - reduction);
        Debug.Log($"[PlayerAttack] Attack cooldown reduced to {attackCooldown}s");
    }

    // Gizmo로 공격 범위 표시
    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
            
            // 공격 방향 표시
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, attackPoint.position);
        }
    }

    // 외부에서 공격 상태 확인용
    public bool CanAttack => canAttack;
    public float AttackRange => attackRange;
    public int AttackDamage => attackDamage;
}