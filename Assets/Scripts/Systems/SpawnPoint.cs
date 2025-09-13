using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Header("Spawn Point Settings")]
    public bool isDefaultSpawn = true;
    public string spawnPointID = "Default";
    
    [Header("Visual Settings")]
    public bool showGizmo = true;
    public Color gizmoColor = Color.green;
    public float gizmoSize = 0.5f;

    void Awake()
    {
        // 기본 스폰포인트가 여러 개 있는지 확인
        if (isDefaultSpawn)
        {
            SpawnPoint[] allSpawnPoints = FindObjectsOfType<SpawnPoint>();
            int defaultCount = 0;
            foreach (SpawnPoint sp in allSpawnPoints)
            {
                if (sp.isDefaultSpawn) defaultCount++;
            }
            
            if (defaultCount > 1)
            {
                Debug.LogWarning($"[SpawnPoint] Multiple default spawn points found in scene! Using first one found.");
            }
        }
    }

    public static SpawnPoint GetDefaultSpawnPoint()
    {
        SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();
        
        foreach (SpawnPoint sp in spawnPoints)
        {
            if (sp.isDefaultSpawn)
            {
                return sp;
            }
        }
        
        Debug.LogWarning("[SpawnPoint] No default spawn point found!");
        return null;
    }
    
    public static SpawnPoint GetSpawnPointByID(string id)
    {
        SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();
        
        foreach (SpawnPoint sp in spawnPoints)
        {
            if (sp.spawnPointID == id)
            {
                return sp;
            }
        }
        
        Debug.LogWarning($"[SpawnPoint] Spawn point with ID '{id}' not found!");
        return GetDefaultSpawnPoint();
    }

    void OnDrawGizmos()
    {
        if (!showGizmo) return;
        
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, gizmoSize);
        
        // 기본 스폰포인트는 더 명확하게 표시
        if (isDefaultSpawn)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position + Vector3.up * gizmoSize, Vector3.one * gizmoSize * 0.5f);
        }
        
        // 라벨 표시
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * (gizmoSize + 0.5f), 
            isDefaultSpawn ? $"SPAWN (Default)\n{spawnPointID}" : $"SPAWN\n{spawnPointID}");
        #endif
    }
}