using UnityEngine;
using utils;

[CreateAssetMenu]
public class SharedControls : ScriptableObject
{
    private TimeControls value;
    public TimeControls Value
    {
        get {
            if (value == null)
            {
                value = new TimeControls();
            }
            return value;
        }
    }
}
