using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RewindController : MonoBehaviour
{
    public BoolVariable Rewinding;
    public RewindSettings Settings;
    public SharedControls Controls;

    public GameEvent OnRewindStart;
    public GameEvent OnRewindEnd;

    void Awake()
    {
        Controls.Value.Gameplay.Rewind.performed += (ctx) =>
        {
            Rewinding.Value = true;
            OnRewindStart?.Raise();
        };
        Controls.Value.Gameplay.Rewind.canceled += (ctx) =>
        {
            Rewinding.Value = false;
            OnRewindEnd?.Raise();
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
