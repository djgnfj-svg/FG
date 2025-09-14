using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    [SerializeField] private KeyCode jumpKey   = KeyCode.Space;
    [SerializeField] private KeyCode dashKey   = KeyCode.LeftShift;
    [SerializeField] private KeyCode rollKey   = KeyCode.LeftControl;
    [SerializeField] private KeyCode attackKey = KeyCode.Z;

    [Header("Roll Tuning")]
    [SerializeField] private float rollSpeed = 12f;      // 구를 때 수평 속도
    [SerializeField] private float rollDuration = 0.25f; // 구르기 지속 시간
    [SerializeField] private float rollCooldown = 0.5f;  // 쿨다운

    private Rigidbody2D rb;
    private MovementRigidbody2D movement2D;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PlayerAttack playerAttack;

    // 캐릭터 방향
    int facing = 1; // 1=오른쪽, -1=왼쪽
    public int Facing => facing; // MovementRigidbody2D 등에서 필요 시 참고

    // 구르기 상태
    bool isRolling = false;
    float rollCooldownTimer = 0f;

    private void Awake()
    {
        movement2D = GetComponent<MovementRigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        playerAttack = GetComponent<PlayerAttack>();

        if (rb) rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Start()
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
        ApplySavedDobokColor();
        ApplySpawnPoint();
    }

    void ApplySavedDobokColor()
    {
        if (GameManager.Instance != null && GameManager.Instance.HasDobokSelected())
        {
            Color savedColor = GameManager.Instance.GetDobokColor();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = savedColor;
                Debug.Log($"[PlayerControll] Applied saved dobok color: {savedColor}");
            }
        }
    }

    void ApplySpawnPoint()
    {
        SpawnPoint spawnPoint = SpawnPoint.GetDefaultSpawnPoint();
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.transform.position;
            Debug.Log($"[PlayerControll] Spawned at: {spawnPoint.spawnPointID} ({spawnPoint.transform.position})");
        }
        else
        {
            Debug.Log("[PlayerControll] No spawn point found - keeping current position");
        }
    }

    void ApplySavedPosition()
    {
        if (GameManager.Instance != null)
        {
            Vector3 savedPosition = GameManager.Instance.GetPlayerPosition();
            if (savedPosition != Vector3.zero)
            {
                transform.position = savedPosition;
                Debug.Log($"[PlayerControll] Applied saved position: {savedPosition}");
            }
        }
    }

    // 도복 색상 변경 메서드 (외부에서 호출 가능)
    public void SetDobokColor(Color color)
    {
        if (spriteRenderer != null) spriteRenderer.color = color;
        if (GameManager.Instance != null) GameManager.Instance.SetDobokColor(color);
    }

    private void Update()
    {
        // 게임 진행 상태가 아니면 입력 무시
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Playing)
            return;

        // 쿨다운 타이머 갱신
        if (rollCooldownTimer > 0f) rollCooldownTimer -= Time.deltaTime;

        UpdateMove();
        UpdateJump();
        UpdateDash();
        UpdateDown();
        UpdateRoll();
        UpdateAttack();
    }

    private void UpdateMove()
    {
        // 구르는 중엔 이동 입력을 무시해 굴림이 끊기지 않게 함
        if (isRolling) return;

        float x = Input.GetAxisRaw("Horizontal");

        // 이동
        movement2D.MoveTo(x);

        // 바라보는 방향 갱신
        if (x > 0) facing = 1;
        else if (x < 0) facing = -1;

        // 스프라이트 좌우 반전
        if (spriteRenderer != null)
            spriteRenderer.flipX = (facing == -1);
    }

    private void UpdateRoll()
    {
        if (!Input.GetKeyDown(rollKey)) return;
        if (isRolling) return;                   // 이미 구르는 중
        if (rollCooldownTimer > 0f) return;      // 쿨다운

        // 애니메이션 트리거 & 디버그
        if (animator != null)
        {
            animator.SetTrigger("Roll");
            Debug.Log("[PlayerControll] Rolling Triggered");
        }
        else
        {
            Debug.LogWarning("[PlayerControll] Animator is NULL - check component/reference");
        }

        // 코루틴으로 구르기 시작
        StartCoroutine(RollRoutine());
    }

    private System.Collections.IEnumerator RollRoutine()
    {
        isRolling = true;
        rollCooldownTimer = rollCooldown;

        // 현재 바라보는 방향으로 수평 속도 부여 (y는 유지)
        float dir = Mathf.Sign(facing);
        rb.velocity = new Vector2(dir * rollSpeed, rb.velocity.y);

        // 필요하면 여기서 무적/충돌 무시 같은 처리 가능
        // Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

        yield return new WaitForSeconds(rollDuration);

        // 롤 종료: 수평 속도만 정지 (y는 유지)
        rb.velocity = new Vector2(0f, rb.velocity.y);
        isRolling = false;

        // Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
    }

    private void UpdateJump()
    {
        if (isRolling) return; // 구르는 중 점프 잠시 금지 (원하면 제거)
        if (Input.GetKeyDown(jumpKey))
            movement2D.JumpTo();
    }

    private void UpdateDash()
    {
        if (isRolling) return; // 구르는 중 대시 금지 (원하면 제거)
        if (Input.GetKeyDown(dashKey))
            movement2D.DashTo();
    }

    private void UpdateDown()
    {
        // 구르는 중엔 빠른 낙하 입력만 유지해도 무방
        movement2D.SetFastFallHold(Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S));
    }

    private void UpdateAttack()
    {
        if (isRolling) return; // 구르는 중 공격 금지 (원하면 제거)
        if (Input.GetKeyDown(attackKey))
        {
            if (playerAttack != null)
            {
                playerAttack.TryAttack();
            }
            else
            {
                Debug.LogWarning("[PlayerControll] PlayerAttack component not found!");
            }
        }
    }
}
