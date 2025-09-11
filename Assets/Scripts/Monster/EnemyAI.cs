using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    [SerializeField] Transform target;        // player Transform 드래그
    [SerializeField] float speed = 3f;
    [SerializeField] float chaseRange = 10f;
    [SerializeField] float stopDistance = 0.5f;

    Rigidbody2D rb;
    SpriteRenderer sr;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        rb.freezeRotation = true;

        // 자동 할당(선택): Player 태그로 찾기
        if (!target)
        {
            var p = GameObject.FindWithTag("Player");
            if (p) target = p.transform;
        }
    }

    void FixedUpdate()
    {
        if (!target) { Debug.LogWarning("[EnemyAI] No target"); return; }

        float dx = target.position.x - rb.position.x;
        float absDx = Mathf.Abs(dx);

        if (Time.frameCount % 30 == 0)
            Debug.Log($"[EnemyAI] dx={absDx:0.00}, range={chaseRange}, stop={stopDistance}, body={rb.bodyType}, sim={rb.simulated}");

        // 추격 범위 밖 → 정지
        if (absDx > chaseRange) { rb.velocity = new Vector2(0f, rb.velocity.y); return; }

        // 정지 거리 이내 → 정지
        if (absDx <= stopDistance + 0.02f) { rb.velocity = new Vector2(0f, rb.velocity.y); return; }

        // 이동
        float dir = Mathf.Sign(dx);
        rb.velocity = new Vector2(dir * speed, rb.velocity.y);
        if (sr) sr.flipX = (dir < 0f);

    }

void OnCollisionEnter2D(Collision2D col)
{
    if (col.collider.CompareTag("Player"))
        Destroy(gameObject);
}


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.cyan;   Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}
