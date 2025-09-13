using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;
    
    [Header("Follow Settings")]
    [SerializeField] private float followSpeed = 2f;
    [SerializeField] private Vector3 offset = new Vector3(0, 1, -10);
    
    [Header("Boundary Settings")]
    [SerializeField] private bool useBoundaries = true;
    [SerializeField] private Vector2 minBounds = new Vector2(-10, -5);
    [SerializeField] private Vector2 maxBounds = new Vector2(10, 5);
    
    [Header("Smooth Settings")]
    [SerializeField] private bool smoothFollow = true;
    [SerializeField] private float smoothTime = 0.3f;
    
    private Vector3 velocity = Vector3.zero;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;

        if (useBoundaries)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
        }

        if (smoothFollow)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetBoundaries(Vector2 min, Vector2 max)
    {
        minBounds = min;
        maxBounds = max;
    }

    void OnDrawGizmosSelected()
    {
        if (useBoundaries)
        {
            Gizmos.color = Color.yellow;
            Vector3 center = new Vector3((minBounds.x + maxBounds.x) / 2, (minBounds.y + maxBounds.y) / 2, transform.position.z);
            Vector3 size = new Vector3(maxBounds.x - minBounds.x, maxBounds.y - minBounds.y, 0);
            Gizmos.DrawWireCube(center, size);
        }

        if (target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(target.position + offset, 0.5f);
            Gizmos.DrawLine(transform.position, target.position + offset);
        }
    }
}