using UnityEngine;

// Unity 에디터에서 DobokPickup 프리팹 설정을 돕는 임시 스크립트
// 사용 후 삭제 가능
public class DobokPickupPrefabSetup : MonoBehaviour
{
    [ContextMenu("Setup Dobok Pickup")]
    void SetupDobokPickup()
    {
        // Sprite Renderer 추가 및 기본 스프라이트 생성
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            sr = gameObject.AddComponent<SpriteRenderer>();
        }
        
        // 기본 흰색 스프라이트 생성
        if (sr.sprite == null)
        {
            Texture2D texture = new Texture2D(64, 64);
            for (int x = 0; x < 64; x++)
            {
                for (int y = 0; y < 64; y++)
                {
                    texture.SetPixel(x, y, Color.white);
                }
            }
            texture.Apply();
            sr.sprite = Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 100);
            sr.sortingOrder = 10;
        }
        
        // Collider 추가
        CircleCollider2D col = GetComponent<CircleCollider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.5f;
        }
        
        // DobokPickup 스크립트 추가
        DobokPickup pickup = GetComponent<DobokPickup>();
        if (pickup == null)
        {
            pickup = gameObject.AddComponent<DobokPickup>();
        }
        
        Debug.Log("DobokPickup prefab setup complete!");
    }
}