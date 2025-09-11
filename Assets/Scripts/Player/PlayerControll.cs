using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    [SerializeField] private KeyCode jumpKey   = KeyCode.Space;
    [SerializeField] private KeyCode dashKey   = KeyCode.LeftShift;
    [SerializeField] private KeyCode attackKey = KeyCode.X;
    [SerializeField] private KeyCode rollKey   = KeyCode.LeftControl;

    private Rigidbody2D rb;
    private MovementRigidbody2D movement2D;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // ---- 콤보 제어 ----
    [Header("Post Combo Window")]
    [SerializeField] private float postWindow = 0.5f;   // 공격 끝난 후 다음 입력 허용 시간(초)

    private bool canAttack = true;   // 현재 입력으로 공격 시작 가능?
    private int  comboStage = 1;     // 다음에 재생할 타 (1→2→3)
    private float comboWindowUntil = -1f; // 이 시간 전까지만 다음 타 허용
	
	// 클래스 맨 위 필드들 근처
	int facing = 1; // 1=오른쪽, -1=왼쪽
	bool IsAttackingState()
{
    var st = animator.GetCurrentAnimatorStateInfo(0);
    return st.IsName("attack1") || st.IsName("attack2") || st.IsName("attack3");
}

    // 중복 호출 방지 플래그
	private bool atk1EndFired, atk2EndFired, atk3EndFired;

	private void Awake()
	{
		movement2D = GetComponent<MovementRigidbody2D>();
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		rb = GetComponent<Rigidbody2D>();
		rb.constraints = RigidbodyConstraints2D.FreezeRotation; // 또는 rb.freezeRotation = true;

		comboStage = 1;                          // ← 내부값
		animator.SetInteger("ComboStage", 1);    // ← Animator 파라미터도 1로 동기화
    }

    private void Update()
    {
        transform.localScale = new Vector3(3f, 3f, 3f);

        UpdateMove();
        UpdateJump();
        UpdateDash();
        UpdateDown();
        UpdateRoll();
        UpdateAttack();

        if (animator && rb)
        {
            float speed = Mathf.Abs(rb.velocity.x);
            animator.SetFloat("Speed", speed, 0.05f, Time.deltaTime);
        }
    }

	private void ResetCombo()
{
    canAttack = true;
    comboStage = 1;
    comboWindowUntil = -1f;
    animator.SetInteger("ComboStage", 1);    // ← 리셋할 때도 동기화
}

private void UpdateMove()
	{
		float x = Input.GetAxisRaw("Horizontal");

		// 이동은 그대로 허용
		movement2D.MoveTo(x);

		bool attackingNow = IsAttackingState();

		// 공격 "아닐 때"만 바라보는 방향 갱신
		if (!attackingNow)
		{
			if (x > 0) facing = 1;
			else if (x < 0) facing = -1;
		}

		// 스프라이트는 항상 마지막으로 기록한 방향을 유지
		if (spriteRenderer != null)
			spriteRenderer.flipX = (facing == -1);
	}

    private void UpdateAttack()
    {
        var st = animator.GetCurrentAnimatorStateInfo(0);
        bool inAtk1 = st.IsName("attack1");
        bool inAtk2 = st.IsName("attack2");
        bool inAtk3 = st.IsName("attack3");
        bool attacking = inAtk1 || inAtk2 || inAtk3;

        // ---- 입력 처리 (공격 도중엔 무시, 공격이 없을 때만) ----
        if (Input.GetKeyDown(attackKey) && !attacking)
        {
            bool inWindow = Time.time <= comboWindowUntil;

            // 창이 열려있거나(다음 타), 아니면 새로 시작(1타)
            int stageToPlay = (inWindow ? comboStage : 1);

            if (canAttack)
            {
                canAttack = false;
                comboWindowUntil = -1f;             // 실행 중엔 창 닫기
                comboStage = stageToPlay;           // 이번에 재생할 단계
                animator.SetInteger("ComboStage", comboStage);
                animator.ResetTrigger("Attack");
                animator.SetTrigger("Attack");      // Any State -> attack{stage}
                // Debug.Log($"▶ attack{comboStage} start");
            }
        }

        // ---- 끝 감지(이벤트 없이) : 각 타 종료 시 0.5초 창 열기 ----
        if (inAtk1)
        {
            float t = st.normalizedTime % 1f;
            if (t >= 0.99f && !animator.IsInTransition(0) && !atk1EndFired)
            {
                atk1EndFired = true;
                OpenPostWindow(2);                 // 2타 창 열기
            }
        }
        else atk1EndFired = false;

        if (inAtk2)
        {
            float t = st.normalizedTime % 1f;
            if (t >= 0.99f && !animator.IsInTransition(0) && !atk2EndFired)
            {
                atk2EndFired = true;
                OpenPostWindow(3);                 // 3타 창 열기
            }
        }
        else atk2EndFired = false;

        if (inAtk3)
        {
            float t = st.normalizedTime;
            if (t >= 0.99f && !animator.IsInTransition(0) && !atk3EndFired)
            {
                atk3EndFired = true;
                ResetCombo();                      // 콤보 완전 종료
            }
        }
        else atk3EndFired = false;
    }

    // 공격이 끝나면 호출: 다음 타만 0.5초간 허용
    private void OpenPostWindow(int nextStage)
    {
        canAttack = true;                          // 다음 입력 허용
        comboStage = Mathf.Clamp(nextStage, 1, 3);
        comboWindowUntil = Time.time + postWindow; // 0.5초 창
        // Debug.Log($"□ open window for attack{comboStage} until {comboWindowUntil:0.00}");
    }

    // 3타가 끝났거나, 창이 지나면 기본 상태로

    private void UpdateRoll()
    {
        if (Input.GetKeyDown(rollKey))
            if (animator) animator.SetTrigger("Roll");
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
