using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    [SerializeField] private KeyCode jumpKey   = KeyCode.Space;
    [SerializeField] private KeyCode dashKey   = KeyCode.LeftShift;
    [SerializeField] private KeyCode rollKey   = KeyCode.LeftControl;

    private Rigidbody2D rb;
    private MovementRigidbody2D movement2D;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // 캐릭터 방향
    int facing = 1; // 1=오른쪽, -1=왼쪽
    
    // MovementRigidbody2D에서 접근 가능하도록 public 프로퍼티
    public int Facing => facing;

    private void Awake()
    {
        movement2D = GetComponent<MovementRigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Start()
    {
        // 스케일을 Start에서 한 번만 설정
        transform.localScale = new Vector3(1f, 1f, 1f);
    }

    private void Update()
    {
        // GameManager 상태 체크 추가
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Playing)
            return;

        UpdateMove();
        UpdateJump();
        UpdateDash();
        UpdateDown();
        UpdateRoll();

        // 애니메이션은 MovementRigidbody2D에서 처리하므로 여기서는 제거
    }


    private void UpdateMove()
    {
        float x = Input.GetAxisRaw("Horizontal");

        // 이동은 그대로 허용
        movement2D.MoveTo(x);

        // 바라보는 방향 갱신
        if (x > 0) facing = 1;
        else if (x < 0) facing = -1;

        // 스프라이트는 항상 마지막으로 기록한 방향을 유지
        if (spriteRenderer != null)
            spriteRenderer.flipX = (facing == -1);
    }


    private void UpdateRoll()
    {
        if (Input.GetKeyDown(rollKey))
            if (animator != null) animator.SetTrigger("Roll");
    }

    private void UpdateJump()
    {
        if (Input.GetKeyDown(jumpKey))
            movement2D.JumpTo();
    }

    private void UpdateDash()
    {
        if (Input.GetKeyDown(dashKey))
            movement2D.DashTo();
    }

    private void UpdateDown()
    {
        movement2D.SetFastFallHold(Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S));
    }
}
