using UnityEngine;

public enum EnemyState
{
    Idle,
    Patrol,
    Chase,
    Attack,
    Dead
}

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [Header("AI Settings")]
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float patrolRange = 5f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float attackCooldown = 2f;
    
    [Header("References")]
    [SerializeField] private LayerMask playerLayer = -1;
    
    private EnemyState currentState;
    private Transform player;
    private Vector3 originalPosition;
    private int patrolDirection = 1;
    private float lastAttackTime;
    private bool playerInRange = false;
    
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private EnemyStats enemyStats;
    private Animator animator;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyStats = GetComponent<EnemyStats>();
        animator = GetComponent<Animator>();
        
        rb.freezeRotation = true;
        originalPosition = transform.position;
        
        // 플레이어 자동 찾기
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Start()
    {
        ChangeState(EnemyState.Patrol);
    }

    void Update()
    {
        if (enemyStats != null && enemyStats.IsDead)
        {
            ChangeState(EnemyState.Dead);
            return;
        }

        DetectPlayer();
        ExecuteCurrentState();
    }

    void DetectPlayer()
    {
        if (player == null) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // 플레이어 감지
        if (distanceToPlayer <= detectionRange && currentState != EnemyState.Dead)
        {
            playerInRange = true;
            
            // 공격 범위 내라면 공격 상태로
            if (distanceToPlayer <= attackRange && currentState != EnemyState.Attack)
            {
                ChangeState(EnemyState.Attack);
            }
            // 추적 범위라면 추적 상태로
            else if (distanceToPlayer > attackRange && currentState != EnemyState.Chase)
            {
                ChangeState(EnemyState.Chase);
            }
        }
        else
        {
            // 플레이어가 감지 범위를 벗어남
            if (playerInRange && currentState == EnemyState.Chase)
            {
                playerInRange = false;
                ChangeState(EnemyState.Patrol);
            }
        }
    }

    void ExecuteCurrentState()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                ExecuteIdle();
                break;
            case EnemyState.Patrol:
                ExecutePatrol();
                break;
            case EnemyState.Chase:
                ExecuteChase();
                break;
            case EnemyState.Attack:
                ExecuteAttack();
                break;
            case EnemyState.Dead:
                ExecuteDead();
                break;
        }
    }

    void ExecuteIdle()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        
        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
        }
    }

    void ExecutePatrol()
    {
        // 원래 위치에서 patrolRange 내에서 좌우로 순찰
        Vector2 targetPosition = originalPosition + Vector3.right * (patrolRange * patrolDirection);
        
        // 목표 지점에 도달하면 방향 전환
        if (Mathf.Abs(transform.position.x - targetPosition.x) < 0.5f)
        {
            patrolDirection *= -1;
        }
        
        // 이동
        rb.velocity = new Vector2(patrolDirection * moveSpeed * 0.5f, rb.velocity.y);
        
        // 스프라이트 방향 설정
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = patrolDirection < 0;
        }
        
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        }
    }

    void ExecuteChase()
    {
        if (player == null) return;
        
        // 플레이어 방향으로 이동
        float directionToPlayer = Mathf.Sign(player.position.x - transform.position.x);
        rb.velocity = new Vector2(directionToPlayer * moveSpeed, rb.velocity.y);
        
        // 스프라이트 방향 설정
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = directionToPlayer < 0;
        }
        
        if (animator != null)
        {
            animator.SetFloat("Speed", moveSpeed);
        }
    }

    void ExecuteAttack()
    {
        // 공격 중에는 이동 정지
        rb.velocity = new Vector2(0, rb.velocity.y);
        
        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
        }
        
        // 공격 쿨다운 체크
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            PerformAttack();
            lastAttackTime = Time.time;
        }
        
        // 플레이어가 공격 범위를 벗어나면 추적으로 전환
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer > attackRange)
            {
                ChangeState(EnemyState.Chase);
            }
        }
    }

    void ExecuteDead()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        
        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetBool("IsDead", true);
        }
        
        // 일정 시간 후 오브젝트 제거 (선택사항)
        Destroy(gameObject, 2f);
    }

    void PerformAttack()
    {
        Debug.Log($"[EnemyController] {name} performs attack!");
        
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
        
        // 플레이어에게 데미지 (공격 범위 내 플레이어 체크)
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer <= attackRange)
            {
                PlayerStats playerStats = player.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.TakeDamage(10);
                    Debug.Log($"[EnemyController] Dealt 10 damage to player");
                }
            }
        }
    }

    void ChangeState(EnemyState newState)
    {
        if (currentState == newState) return;
        
        Debug.Log($"[EnemyController] {name} state changed: {currentState} -> {newState}");
        currentState = newState;
        
        // 상태 진입 시 초기화
        switch (newState)
        {
            case EnemyState.Dead:
                rb.velocity = Vector2.zero;
                break;
        }
    }

    void OnDrawGizmosSelected()
    {
        // 감지 범위
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // 공격 범위
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // 순찰 범위
        Gizmos.color = Color.blue;
        Vector3 patrolCenter = Application.isPlaying ? originalPosition : transform.position;
        Gizmos.DrawLine(patrolCenter + Vector3.left * patrolRange, patrolCenter + Vector3.right * patrolRange);
    }

    // 외부에서 상태 확인용
    public EnemyState CurrentState => currentState;
    public bool IsPlayerInRange => playerInRange;
}