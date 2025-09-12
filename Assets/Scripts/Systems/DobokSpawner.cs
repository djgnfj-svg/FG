using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DobokSpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    public GameObject dobokPickupPrefab;
    public float spawnRadius = 6f;  // 더 넓게 배치
    public float spawnHeight = 1f;
    public Vector3 spawnOffset = new Vector3(0, 2f, 0);  // 플레이어보다 위쪽에 배치
    
    [Header("Visual Effects")]
    public GameObject spawnEffectPrefab; // 파티클 효과용
    public float spawnDelay = 0.2f; // 각 도복 사이의 스폰 딜레이
    
    private List<GameObject> spawnedDoboks = new List<GameObject>();

    public void SpawnDoboks(List<SimpleDobokData> dobokDataList, Vector3 playerPosition)
    {
        if (dobokPickupPrefab == null || dobokDataList == null || dobokDataList.Count == 0)
        {
            Debug.LogError("DobokSpawner: Missing prefab or dobok data!");
            return;
        }

        // 기존 도복들 정리
        ClearSpawnedDoboks();

        // 플레이어 기준으로 스폰 위치 계산
        Vector3 basePosition = playerPosition + spawnOffset;
        
        StartCoroutine(SpawnDoboksSequentially(dobokDataList, basePosition));
    }

    private IEnumerator SpawnDoboksSequentially(List<SimpleDobokData> dobokDataList, Vector3 basePosition)
    {
        for (int i = 0; i < dobokDataList.Count; i++)
        {
            Vector3 spawnPosition = CalculateSpawnPosition(i, dobokDataList.Count, basePosition);
            SpawnSingleDobok(dobokDataList[i], spawnPosition);
            
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private Vector3 CalculateSpawnPosition(int index, int totalCount, Vector3 basePosition)
    {
        // 반원 형태로 배치
        float angleStep = Mathf.PI / (totalCount + 1); // 180도를 등분
        float angle = angleStep * (index + 1); // 각도 계산
        
        // 극좌표를 직교좌표로 변환
        float x = Mathf.Cos(angle) * spawnRadius;
        float z = Mathf.Sin(angle) * spawnRadius * 0.5f; // Z축 깊이는 절반으로
        
        Vector3 offset = new Vector3(x, spawnHeight, z);
        return basePosition + offset;
    }

    private void SpawnSingleDobok(SimpleDobokData dobokData, Vector3 position)
    {
        // 도복 오브젝트 생성
        GameObject dobokObj = Instantiate(dobokPickupPrefab, position, Quaternion.identity);
        
        // 필요한 컴포넌트들 확인 및 추가
        SpriteRenderer sr = dobokObj.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            sr = dobokObj.AddComponent<SpriteRenderer>();
        }
        
        // 강제로 큰 스프라이트 생성 및 적용
        Texture2D texture = new Texture2D(128, 128);
        Color[] colors = new Color[128 * 128];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.white;
        }
        texture.SetPixels(colors);
        texture.Apply();
        
        sr.sprite = Sprite.Create(texture, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f), 100);
        sr.color = dobokData.tintColor;
        sr.sortingOrder = 100; // 매우 높은 우선순위
        
        Debug.Log($"스프라이트 적용됨: {sr.sprite != null}, 색상: {sr.color}, 정렬순서: {sr.sortingOrder}");
        
        // 크기 설정 (적당한 크기)
        dobokObj.transform.localScale = Vector3.one * 0.8f;
        
        // DobokPickup 컴포넌트 추가 및 데이터 설정
        DobokPickup pickup = dobokObj.GetComponent<DobokPickup>();
        if (pickup == null)
        {
            pickup = dobokObj.AddComponent<DobokPickup>();
        }
        pickup.dobokData = dobokData;
        
        // Collider 확인 및 추가
        CircleCollider2D col = dobokObj.GetComponent<CircleCollider2D>();
        if (col == null)
        {
            col = dobokObj.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.5f;
        }
        
        // 스폰 효과
        if (spawnEffectPrefab != null)
        {
            GameObject effect = Instantiate(spawnEffectPrefab, position, Quaternion.identity);
            Destroy(effect, 3f); // 3초 후 이펙트 제거
        }
        
        // 스폰 애니메이션 (임시 비활성화)
        // StartCoroutine(SpawnAnimation(dobokObj));
        
        // 리스트에 추가
        spawnedDoboks.Add(dobokObj);
        
        Debug.Log($"Spawned dobok: {dobokData.dobokName} at {position}");
    }

    private IEnumerator SpawnAnimation(GameObject dobokObj)
    {
        if (dobokObj == null) yield break;
        
        // 초기 설정 (작고 투명하게 시작)
        Vector3 originalScale = dobokObj.transform.localScale;
        dobokObj.transform.localScale = Vector3.zero;
        
        SpriteRenderer spriteRenderer = dobokObj.GetComponent<SpriteRenderer>();
        Color originalColor = Color.white;
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            Color transparentColor = originalColor;
            transparentColor.a = 0f;
            spriteRenderer.color = transparentColor;
        }
        
        // 애니메이션 (크기 증가 + 페이드인)
        float duration = 0.5f;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration && dobokObj != null)
        {
            float t = elapsedTime / duration;
            
            // 크기 변화 (Bounce 효과)
            float bounceT = Mathf.Sin(t * Mathf.PI * 0.5f);
            dobokObj.transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, bounceT);
            
            // 페이드인
            if (spriteRenderer != null)
            {
                Color color = originalColor;
                color.a = t;
                spriteRenderer.color = color;
            }
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // 최종 상태 설정
        if (dobokObj != null)
        {
            dobokObj.transform.localScale = originalScale;
            if (spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }
        }
    }

    public void ClearSpawnedDoboks()
    {
        foreach (GameObject dobok in spawnedDoboks)
        {
            if (dobok != null)
            {
                Destroy(dobok);
            }
        }
        spawnedDoboks.Clear();
    }

    public void RemoveUnselectedDoboks()
    {
        List<GameObject> toRemove = new List<GameObject>();
        
        foreach (GameObject dobok in spawnedDoboks)
        {
            if (dobok != null)
            {
                DobokPickup pickup = dobok.GetComponent<DobokPickup>();
                if (pickup != null)
                {
                    pickup.DestroyUnselected();
                }
                toRemove.Add(dobok);
            }
        }
        
        // 리스트에서 제거
        foreach (GameObject dobok in toRemove)
        {
            spawnedDoboks.Remove(dobok);
        }
    }

    public int GetSpawnedDobokCount()
    {
        // null이 아닌 도복들만 카운트
        int count = 0;
        foreach (GameObject dobok in spawnedDoboks)
        {
            if (dobok != null)
            {
                count++;
            }
        }
        return count;
    }

    void OnDrawGizmos()
    {
        // 스폰 범위 표시
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + spawnOffset, spawnRadius);
        
        // 스폰 위치 미리보기
        if (Application.isPlaying == false)
        {
            Vector3 basePosition = transform.position + spawnOffset;
            
            for (int i = 0; i < 3; i++) // 3개 도복 위치 표시
            {
                Vector3 pos = CalculateSpawnPosition(i, 3, basePosition);
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(pos, Vector3.one * 0.5f);
            }
        }
    }
}