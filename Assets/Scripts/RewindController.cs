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

    public RewindableObjects TrackedObjects;
    private ulong FixedUpdateCount;
    private const uint TrimBufferExcessInterval = 250;

    private void Awake()
    {
        Controls.Value.Gameplay.Rewind.performed += (ctx) =>
        {
            Rewinding.Value = true;
            OnRewindStart?.Raise();
            //StartCoroutine(Rewind());
        };
        Controls.Value.Gameplay.Rewind.canceled += (ctx) =>
        {
            Rewinding.Value = false;
            OnRewindEnd?.Raise();
        };
    }

    private void FixedUpdate()
    {
        if (Rewinding)
        {
            foreach (var tracker in TrackedObjects.Items)
            {
                tracker.RewindTime();
            }
        }
        else
        {
            // we are recording.
            foreach (var tracker in TrackedObjects.Items)
            {
                tracker.RecordPositionInTime();
            }
        }
        if (FixedUpdateCount++ % 200 == 0)
        {
            // once in a while (every second or so), trim off the excess in the timebuffers.
            foreach (var tracker in TrackedObjects.Items)
            {
                tracker.timeBuffer.TrimExcess();
            }
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

    /// <summary>
    /// TODO: maybe use this to make a rewind coroutine, instead of using fixedUpdate?
    /// </summary>
    /// <returns></returns>
    IEnumerator RewindCoroutine()
    {
        while (Rewinding)
        {
            // this could be vectorized.
            foreach (Tracker obj in TrackedObjects.Items)
            {
                obj.RewindTime();
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
