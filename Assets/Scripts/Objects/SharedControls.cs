using UnityEngine;

[CreateAssetMenu(menuName = "customObject/Controls")]
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
