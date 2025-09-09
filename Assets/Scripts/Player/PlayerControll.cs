using UnityEngine;
using System.Collections;
public class PlayerControll : MonoBehaviour
{
	[SerializeField]
	private KeyCode jumpKey = KeyCode.Space;
	private KeyCode dashKey = KeyCode.LeftShift;

	private MovementRigidbody2D movement2D;

	private void Awake()
	{
		movement2D = GetComponent<MovementRigidbody2D>();
	}

	private void Update()
	{
		UpdateMove();
		UpdateJump();
		UpdateDash();
		// 4) 빠른 낙하(↓ 홀드 → 빠르게 떨어짐)
		UpdateDown();
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
