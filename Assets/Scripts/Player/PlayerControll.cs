using UnityEngine;
using System.Collections;
public class PlayerControll : MonoBehaviour
{
	[SerializeField]
	private KeyCode jumpKey = KeyCode.Space;
	private KeyCode dashKey = KeyCode.LeftShift;

	private KeyCode rollKey = KeyCode.LeftControl;
	private Rigidbody2D rb;
	private MovementRigidbody2D movement2D;
	Animator animator;

	private void Awake()
	{
		movement2D = GetComponent<MovementRigidbody2D>();
		animator = GetComponent<Animator>();
	}

	private void Update()
	{
		UpdateMove();
		UpdateJump();
		UpdateDash();
		UpdateDown();
		UpdateRoll();
		if (animator && rb)
			{
				float speed = Mathf.Abs(rb.velocity.x);
				animator.SetFloat("Speed", speed, 0.05f, Time.deltaTime); // 댐핑으로 깜빡임 감소
			}
	}

	private void UpdateRoll()
	{
		if (Input.GetKeyDown(rollKey))
		{
		Debug.Log("FUcking Roll");
			if (animator)
			{
				Debug.Log("HappyRoll Roll");
				animator.SetTrigger("Roll");  // ✅ Animator Trigger("Roll")
			}
		}
	}

	private void UpdateMove()
	{

		// left, a = -1  /  none = 0  /  right, d = +1
		float x = Input.GetAxisRaw("Horizontal");

		// 좌우 이동
		movement2D.MoveTo(x);
		if (x > 0) transform.localScale = new Vector3(1, 1, 1);
		else if (x < 0) transform.localScale = new Vector3(-1, 1, 1);
	}

	private void UpdateJump()
	{

		if (Input.GetKeyDown(jumpKey))
		{
			movement2D.JumpTo();
		}
	}

	private void UpdateDash()
	{

		if (Input.GetKeyDown(dashKey))
		{
			movement2D.DashTo();
		}
	}

	private void UpdateDown()
	{
		movement2D.SetFastFallHold(Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S));
		
	}
}
