using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using utils;

public class RewindController : MonoBehaviour
{
    public BoolVariable Rewinding;
    public SharedControls Controls;

    public GameEvent OnRewindStart;
    public GameEvent OnRewindEnd;

    private List<Tracker> TrackedObjects = new List<Tracker>();
    
    private void Start()
    {
        Controls.Value.Gameplay.Rewind.performed += (ctx) =>
        {
            print("Rewind Started.");
            Rewinding.Value = true;
            OnRewindStart?.Raise();
            //StartCoroutine(Rewind());
        };
        Controls.Value.Gameplay.Rewind.canceled += (ctx) =>
        {
            print("Rewind Finished.");
            Rewinding.Value = false;
            OnRewindEnd?.Raise();
            TrimAllBuffers();
        };
    }

    private void FixedUpdate()
    {
        foreach (var tracker in TrackedObjects)
        {
            if (tracker.isActiveAndEnabled)
            {
                if (Rewinding)
                {
                    tracker.RewindTime();
                }
                else
                {
                    tracker.RecordPositionInTime();
                }
            }
        }
    }

    private void TrimAllBuffers()
    {
        foreach (var tracker in TrackedObjects)
        {
            tracker.timeBuffer.TrimExcess();
        }
    }

    private void OnEnable()
    {
        Controls.Value.Gameplay.Rewind.Enable();
    }

    private void OnDisable()
    {
        Controls.Value.Gameplay.Rewind.Disable();
    }

    public void RegisterRewindableObject(Tracker obj)
    {
        TrackedObjects.Add(obj);
    }

    public void UnRegisterRewindableObject(Tracker obj)
    {
        int index = TrackedObjects.IndexOf(obj);
    }
}


