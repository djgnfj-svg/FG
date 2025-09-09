using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class MovementRigidbody2D : MonoBehaviour
{

	// 이동 제어
	[Header("Move Horizontal")]
	[SerializeField]
	private float moveSpeed = 8;      // 이동 속도

	//점프 제어
	[Header("Move Vertical (Jump)")]
	[SerializeField]
	private float jumpForce = 10;       // 점프 힘
	[SerializeField]
	private float lowGravity = 1.5f;       // 점프키를 오래 누르고 있을 때 적용되는 낮은 중력 계수
	[SerializeField]
	private float highGravity = 2.5f;  // 일반적으로 적용되는 높은 중력 계수
	[SerializeField]
	private int maxJumpCount = 2;   // 최대 점프 횟수
	private int currentJumpCount;   // 현재 남아있는 점프 횟수

	[Header("Collision")]
	[SerializeField]
	private LayerMask groundLayer;      // 바닥 충돌 체크를 위한 레이어

	private bool isGrounded;            // 바닥 체크 (바닥에 플레이어의 발이 닿아있을 때 true)
	private Vector2 footPosition;       // 바닥 체크를 위한 플레이어 발 위치
	private Vector2 footArea;           // 바닥 체크를 위한 플레이어 발 인식 범위

	private Rigidbody2D rigid2D;            // 속력 제어를 위한 Rigidbody2D
	private new Collider2D collider2D;          // 현재 오브젝트의 충돌 범위 정보

	public bool IsLongJump { set; get; } = false;

	//대쉬 제어
	[SerializeField] private float dashForce = 20f;   // 대시 속도
	[SerializeField] private float dashDuration = 0.2f; // 대시 유지 시간
	[SerializeField] private float dashCooldown = 1f; // 쿨타임

	// ===== 입력에서 전달받는 상태값 =====
	bool fastFallHold = false;

	// ===== 내부 상태 =====
	bool isDashing = false;
	bool canDash = true;
	// 대시 정책
	[SerializeField] private bool allowAirDash = true; // 공중에서도 대시 허용할지
	
	private bool hasAirDashed = false;                 // 공중에서 이미 대시했는지
	private bool wasGrounded = false;

	private void Awake()
	{
		rigid2D = GetComponent<Rigidbody2D>();
		collider2D = GetComponent<Collider2D>();
		Debug.Log("Move Awake");
	}

	private void FixedUpdate()
	{
		// 플레이어 오브젝트의 Collider2D min, center, max 위치 정보
		Bounds bounds = collider2D.bounds;
		// 플레이어의 발 위치 설정
		footPosition = new Vector2(bounds.center.x, bounds.min.y);
		// 플레이어의 발 인식 범위 설정
		footArea = new Vector2((bounds.max.x - bounds.min.x) * 0.5f, 0.2f);
		// 플레이어의 발 위치에 박스를 생성하고, 박스가 바닥과 닿아있으면 isGrounded = true
		isGrounded = Physics2D.OverlapBox(footPosition, footArea, 0, groundLayer);
		
		// 플레이어의 발이 땅에 닿아 있고, y축 속력이 0이하이면 점프 횟수 초기화
		// y축 속력이 + 값이면 점프를 하는중..
		if (isGrounded == true && rigid2D.velocity.y <= 0)
		{
			currentJumpCount = maxJumpCount;
		}
		// 착지한 프레임에 대시 회복 & 공중대시 카운트 초기화
		if (isGrounded && !wasGrounded)
		{
			canDash = true;        // ✅ 착지하면 바로 대시 가능
			hasAirDashed = false;  // ✅ 공중 대시 1회 제한 초기화
		}
		wasGrounded = isGrounded;

		// === 빠른 낙하 / 롱점프 중력 적용 (접지 처리 직후에 위치시키기) ===
		float targetGravity = highGravity;

		// 1) 대시가 최우선 (대시 동안 빠른낙하를 막고 싶으면 0f, 허용하고 싶으면 주석)
		if (isDashing)
		{
			targetGravity = 0f; // 대시 중 중력 끄기 (원하면 사용)
		}
		else if (fastFallHold) // 2) 빠른 낙하가 롱점프보다 우선! (↓키가 이기도록)
		{
			// 하강 중에만 한정하고 싶으면 조건 추가:
			// if (rigid2D.velocity.y <= 0f)
			targetGravity = highGravity * 2.0f; // 필요시 2.5~3.0까지 올려보세요

			// 즉시 낙하 감각을 주고 싶으면(선택):
			// rigid2D.velocity = new Vector2(rigid2D.velocity.x, Mathf.Min(rigid2D.velocity.y, -15f));
		}
		else if (IsLongJump && rigid2D.velocity.y > 0f) // 3) 롱점프(상승 중)일 때만 저중력
		{
			targetGravity = lowGravity;
		}
		else
		{
			targetGravity = highGravity;
		}

		rigid2D.gravityScale = targetGravity;



	}
	private void OnDrawGizmosSelected()
{
    if (!collider2D) collider2D = GetComponent<Collider2D>();
    Bounds b = collider2D.bounds;
    Vector2 probeCenter = new Vector2(b.center.x, b.min.y);
    Vector2 probeSize = new Vector2((b.max.x - b.min.x) * 0.5f, 0.2f);

    Gizmos.color = isGrounded ? Color.green : Color.red;
    Gizmos.DrawWireCube(probeCenter, probeSize);
}

	/// x 이동 방향 설정 (외부 클래스에서 호출)
	public void MoveTo(float x)
	{
		Debug.Log("Move MoveTo");

		if (isDashing) return; // 대시 중이면 이동 입력 무시
		rigid2D.velocity = new Vector2(x * moveSpeed, rigid2D.velocity.y);
	}

	/// <summary>
	/// 점프 (외부 클래스에서 호출)
	/// </summary>
	public bool JumpTo()
	{
		Debug.Log("Move JumpTo");
		Debug.Log("Jump : " + currentJumpCount);
		if (currentJumpCount > 0)
		{
			Debug.Log("Jump!");
			rigid2D.velocity = new Vector2(rigid2D.velocity.x, jumpForce);
			currentJumpCount--;
			return true;
		}

		return false;
	}

	// ↓ 홀드 여부(빠른 낙하)
	public void SetFastFallHold(bool hold) => fastFallHold = hold;

	// 대시 시도
	public void DashTo()
	{
		Debug.Log("Dash" + isGrounded);
		if (isDashing || !canDash) return;
		// 공중일 때 정책 적용
		if (!isGrounded)
		{
			if (!allowAirDash) return;   // 공중 대시 금지 옵션
			if (hasAirDashed) return;    // 공중 대시 1회 제한
			hasAirDashed = true;
		}

		StartCoroutine(DashRoutine());
		canDash = false;
	}

	private IEnumerator DashRoutine()
	{
		Debug.Log("Move 대쉬설정");
		isDashing = true;
		canDash = false;

		float dir = Mathf.Sign(transform.localScale.x);
		// 수평 대시, 상승/낙하 가속도는 유지
		rigid2D.velocity = new Vector2(dir * dashForce, rigid2D.velocity.y);

		yield return new WaitForSeconds(dashDuration);

		isDashing = false;

		// 쿨타임
		yield return new WaitForSeconds(dashCooldown);
		canDash = true;
	}



}
