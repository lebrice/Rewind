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
    //private ulong FixedUpdateCount;
    private const uint TrimBufferExcessInterval = 250;
    
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
            //TrimAllBuffers();
        };
    }

    private void FixedUpdate()
    {
        if (Rewinding)
        {
            foreach (var tracker in TrackedObjects)
            {
                tracker.RewindTime();
            }
        }
        else
        {
            // we are recording.
            foreach (var tracker in TrackedObjects)
            {
                if (tracker.isActiveAndEnabled)
                {
                    tracker.RecordPositionInTime();
                }
            }
        }
    }

    private void TrimAllBuffers()
    {
        print("Trimming excess in buffers.");
        // once in a while (every second or so), trim off the excess in the timebuffers.
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

    ///// <summary>
    ///// TODO: maybe use this to make a rewind coroutine, instead of using fixedUpdate?
    ///// </summary>
    ///// <returns></returns>
    //IEnumerator RewindCoroutine()
    //{
    //    while (Rewinding)
    //    {
    //        // this could be vectorized.
    //        foreach (Tracker obj in TrackedObjects.Items)
    //        {
    //            obj.RewindTime();
    //        }
    //        yield return new WaitForFixedUpdate();
    //    }
    //}


    //}

    //public class Rewindable : MonoBehaviour
    //{

    //private List<NativeList<TimePositionInfo>> timeBuffers = new List<NativeList<TimePositionInfo>>();


    //private List<Rigidbody> rigidBodies = new List<Rigidbody>();


    //private List<Transform> transforms = new List<Transform>();

    //List<JobHandle> jobHandles = new List<JobHandle>();

    //private NativeArray<Vector3> velocities;

    //private TransformAccessArray jobTransforms;

    //public int JobCount { get; private set; }
    //private void Start()
    //{
    //    //timeBuffers = new List<NativeList<TimePositionInfo>>();
    //    //rigidBodies = new List<Rigidbody>();
    //    //transforms = new List<Transform>();
    //    //jobHandles = new List<JobHandle>();
    //    //for (int i = 0; i < JobCount; i++)
    //    //{
    //    //    var timeBuffer = new NativeList<TimePositionInfo>(32, Allocator.Persistent);
    //    //    timeBuffers[i] = timeBuffer;

    //    //    GameObject obj = TrackedObjects.Items[i].gameObject;
    //    //    rigidBodies[i] = obj.GetComponent<Rigidbody>();
    //    //    transforms[i] = obj.GetComponent<Transform>();
    //    //}
    //}


    public void RegisterRewindableObject(Tracker obj)
    {
        TrackedObjects.Add(obj);
    }

    public void UnRegisterRewindableObject(Tracker obj)
    {
        int index = TrackedObjects.IndexOf(obj);
        //TrackedObjects.RemoveAt(index);
        //timeBuffers.RemoveAt(index);
        //transforms.RemoveAt(index);
        //rigidBodies.RemoveAt(index);
        //JobCount--;
    }
    //public void Update()
    //{

    //    velocities = new NativeArray<Vector3>(JobCount, Allocator.TempJob);
    //    jobTransforms = new TransformAccessArray(transforms.ToArray());
    //    if (Rewinding)
    //    {
    //        RewindPositionJob job;
    //        for (int i = 0; i < JobCount; i++)
    //        {
    //            job.timeBuffer = timeBuffers[i];
    //            job.transforms = jobTransforms;
    //            job.index = i;
    //            job.velocities = velocities;
    //            jobHandles.Add(job.Schedule());
    //        }
    //    }
    //    else
    //    {
    //        for (int i = 0; i < JobCount; i++)
    //        {
    //            velocities[i] = rigidBodies[i] != null ? rigidBodies[i].velocity : Vector3.zero;
    //        }


    //        RecordPositionJob job;

    //        for (int i = 0; i < JobCount; i++)
    //        {
    //            job.index = i;
    //            job.timeBuffer = timeBuffers[i];
    //            job.velocities = velocities;
    //            job.transforms = jobTransforms;
    //            jobHandles.Add(job.Schedule());
    //        }
    //    }
    //}

    //public void LateUpdate()
    //{
    //    foreach (JobHandle jobHandle in jobHandles)
    //    {
    //        jobHandle.Complete();
    //    }
    //    velocities.Dispose();
    //    jobTransforms.Dispose();
    //}

    //public struct RewindPositionJob : IJob
    //{
    //    [ReadOnly]
    //    [NativeDisableUnsafePtrRestriction]
    //    public int index;

    //    [NativeDisableUnsafePtrRestriction]
    //    public NativeList<TimePositionInfo> timeBuffer;

    //    [NativeDisableUnsafePtrRestriction]
    //    public TransformAccessArray transforms;

    //    [NativeDisableUnsafePtrRestriction]
    //    public NativeArray<Vector3> velocities;

    //    public void Execute()
    //    {
    //        TimePositionInfo pos = timeBuffer.Pop();
    //        var t = transforms[index];
    //        t.position = pos.position;
    //        t.rotation = pos.rotation;
    //        transforms[index] = t;

    //        var v = velocities[index];
    //        v = pos.velocity;
    //        velocities[index] = v;

    //        // we don't go back in time before the initial position
    //        if (timeBuffer.Length != 1)
    //        {
    //            pos.numStillFrames -= 1;
    //            if (pos.numStillFrames >= 0)
    //            {
    //                timeBuffer.Push(pos);
    //            }
    //        }
    //    }
    //}

    //public struct RecordPositionJob : IJob
    //{
    //    [ReadOnly]
    //    [NativeDisableUnsafePtrRestriction]
    //    public TransformAccessArray transforms;
    //    [ReadOnly]
    //    [NativeDisableUnsafePtrRestriction]
    //    public NativeArray<Vector3> velocities;
    //    [ReadOnly]
    //    [NativeDisableUnsafePtrRestriction]
    //    public int index;

    //    [NativeDisableUnsafePtrRestriction]
    //    public NativeList<TimePositionInfo> timeBuffer;


    //    public const bool IgnoreSmallMovements = true;
    //    public const float PositionChangeThreshold = 0.05f;
    //    public const float RotationChangeThreshold = 1f;
    //    public const float VelocityChangeThreshold = 0.5f;

    //    public void Execute()
    //    {
    //        TimePositionInfo pos;
    //        if (IgnoreSmallMovements ? ObjectMovedSignificantly(index) : ObjectMoved(index))
    //        {
    //            pos.position = transforms[index].position;
    //            pos.rotation = transforms[index].rotation;
    //            pos.numStillFrames = 0;
    //            pos.velocity = velocities[index];
    //            timeBuffer.Push(pos);
    //        }
    //        else
    //        {
    //            // Object didn't move. Just increment a counter.
    //            pos = timeBuffer.Pop();
    //            pos.numStillFrames += 1;
    //            timeBuffer.Push(pos);
    //        }
    //    }


    //    private bool ObjectMovedSignificantly(int i)
    //    {
    //        TimePositionInfo LastPosition = timeBuffer.Peek();
    //        return Vector3.Distance(transforms[i].position, LastPosition.position) > PositionChangeThreshold
    //            || Quaternion.Angle(transforms[i].rotation, LastPosition.rotation) > RotationChangeThreshold
    //            || (Vector3.Distance(velocities[i], LastPosition.velocity) > VelocityChangeThreshold);
    //    }

    //    private bool ObjectMoved(int i)
    //    {
    //        TimePositionInfo LastPosition = timeBuffer.Peek();
    //        return transforms[i].position != LastPosition.position
    //            || transforms[i].rotation != LastPosition.rotation
    //            || velocities[i] != LastPosition.velocity;
    //    }
    //}
}


