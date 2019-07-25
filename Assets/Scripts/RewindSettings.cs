using UnityEngine;

[CreateAssetMenu]
public class RewindSettings : ScriptableObject
{
    public bool IgnoreSmallMovements = true;
    public float PositionChangeThreshold = 1e-2f;
    public float RotationChangeThreshold = 1e-2f;
    public float VelocityChangeThreshold = 1e-3f;
}
