using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public float dashForce = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayerMask = 1;

    [Header("Combat")]
    public float attackRange = 1f;
    public int attackDamage = 10;
    public float attackCooldown = 0.5f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool facingRight = true;
    private bool isDashing = false;
    private bool canDash = true;
    private float lastAttackTime;

    private float horizontalInput;
    private bool jumpInput;
    private bool dashInput;
    private bool attackInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform);
            groundCheckObj.transform.localPosition = new Vector3(0, -0.5f, 0);
            groundCheck = groundCheckObj.transform;
        }
    }

    void Update()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing)
            return;

        HandleInput();
        CheckGrounded();
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing)
            return;

        HandleMovement();
        HandleJump();
        HandleDash();
    }

    void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        jumpInput = Input.GetButtonDown("Jump");
        dashInput = Input.GetKeyDown(KeyCode.LeftShift);
        attackInput = Input.GetButtonDown("Fire1") || Input.GetMouseButtonDown(0);

        if (attackInput && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }
    }

    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);
    }

    void HandleMovement()
    {
        if (!isDashing)
        {
            rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);

            if (horizontalInput > 0 && !facingRight)
            {
                Flip();
            }
            else if (horizontalInput < 0 && facingRight)
            {
                Flip();
            }
        }
    }

    void HandleJump()
    {
        if (jumpInput && isGrounded && !isDashing)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void HandleDash()
    {
        if (dashInput && canDash && !isDashing)
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;

        float dashDirection = facingRight ? 1f : -1f;
        rb.velocity = new Vector2(dashDirection * dashForce, 0f);

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    void Attack()
    {
        lastAttackTime = Time.time;

        Vector2 attackPosition = transform.position;
        attackPosition.x += (facingRight ? attackRange : -attackRange);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition, attackRange);
        
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.gameObject != gameObject && enemy.CompareTag("Enemy"))
            {
                Debug.Log("Hit enemy: " + enemy.name);
            }
        }

        Debug.Log("Player attacks!");
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        Gizmos.color = Color.blue;
        Vector3 attackPos = transform.position;
        attackPos.x += (facingRight ? attackRange : -attackRange);
        Gizmos.DrawWireSphere(attackPos, attackRange);
    }
}