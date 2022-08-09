using UnityEngine;


public enum WheelLevel
{
    Bronz,
    Silver,
    Gold
}

[System.Serializable]
public class WheelComponents
{
    public Sprite wheelImage;
    public Sprite wheelCursur;
    public WheelLevel level;
}

[CreateAssetMenu(fileName = "Wheel Visuals", menuName = "Create/Wheel")]
public class WheelVisuals : ScriptableObject
{
    public WheelComponents[] components;
}
