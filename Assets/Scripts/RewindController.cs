using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindSettings : ScriptableObject
{
    public bool IgnoreSmallMovements = true;
    public float PositionChangeThreshold = 1e-2f;
    public float RotationChangeThreshold = 1e-2f;
    public float VelocityChangeThreshold = 1e-3f;
}


[RequireComponent(typeof(GameManager))]
public class RewindController : MonoBehaviour
{
    public BoolVariable Rewinding;
    public RewindSettings Settings;
    public SharedControls Controls;
    void Awake()
    {
        Controls.Value.Gameplay.Rewind.performed += (ctx) =>
        {
            Rewinding.Value = true;
        };
        Controls.Value.Gameplay.Rewind.canceled += (ctx) =>
        {
            Rewinding.Value = false;
        };
    }
    
    private void OnEnable()
    {
        Controls.Value.Gameplay.Rewind.Enable();
    }

    private void OnDisable()
    {
        Controls.Value.Gameplay.Rewind.Disable();
    }
}
