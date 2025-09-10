using UnityEngine;

[System.Serializable]
public class SimpleDobokData
{
    public string dobokName;
    public Color tintColor;
    public Sprite dobokSprite; // 나중에 스프라이트 교체용
    
    public SimpleDobokData(string name, Color color)
    {
        dobokName = name;
        tintColor = color;
        dobokSprite = null;
    }
}