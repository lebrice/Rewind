using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.InputSystem;
using utils;

/// <summary>
/// Monobehaviour used to make an object and all its children 'rewindable' in time.
/// </summary>
public class RewindBehaviour : MonoBehaviour
{
    public RewindController controller;

    public BoolVariable Rewinding;
    private List<Tracker> Trackers;
    public const int TrimBufferExcessInterval = 50;

    private void Reset()
    {
        Trackers?.Clear();
        Trackers = null;
    }

    private void OnEnable()
    {
        Trackers = AddTrackers();
        foreach (Tracker tracker in Trackers)
        {
            controller.TrackedObjects.Add(tracker);
        }
    }

    private void OnDisable()
    {
        foreach (Tracker tracker in Trackers)
        {
            controller.TrackedObjects.Remove(tracker);
        }
    }

    private List<Tracker> AddTrackers()
    {
        List<Tracker> Trackers = new List<Tracker>();
        // add a tracker for each child.
        foreach (GameObject childGameObject in ChildGameObjects(gameObject))
        {
            Tracker tracker = GetOrAddTracker(childGameObject);
            Trackers.Add(tracker);
        }
        // add a tracker for this object. (might not be required?)
        //TrackerBehaviour parentTracker = GetOrAddTracker(gameObject);
        //Trackers.Add(parentTracker);
        return Trackers;
    }

    public static IEnumerable<GameObject> ChildGameObjects(GameObject obj)
    {
        foreach (Transform child in obj.GetComponentsInChildren<Transform>())
        {
            yield return child.gameObject;
        }
    }

    public static Tracker GetOrAddTracker(GameObject obj)
    {
        Tracker tracker;
        if ((tracker = obj.GetComponent<Tracker>()) == null)
        {
            tracker = obj.AddComponent<Tracker>();
        }
        return tracker;
    }
}

public struct TimePositionInfo
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 velocity;
    public long numStillFrames;
}


public class RewindSettings
{
        public bool IgnoreSmallMovements = true;
        public float PositionChangeThreshold = 0.05f;
        public float RotationChangeThreshold = 1f;
        public float VelocityChangeThreshold = 0.5f;
}

public class Tracker : MonoBehaviour
{
    public static RewindSettings Settings = new RewindSettings();
    public Stack<TimePositionInfo> timeBuffer;
    public Rigidbody rigidBody;

    public int BufferLength;

    private void Awake()
    {
        timeBuffer = new Stack<TimePositionInfo>();
        rigidBody = GetComponent<Rigidbody>(); // the object might not have one.
    }

    private void Reset()
    {
        timeBuffer?.Clear();
        timeBuffer?.TrimExcess();
    }

    private void Update()
    {
        BufferLength = timeBuffer.Count;
    }

    private void Start()
    {
        var initialPosition = new TimePositionInfo
        {
            position = transform.position,
            rotation = transform.rotation,
            numStillFrames = long.MaxValue / 2 // saying the object has been there 'forever'
        };
        if (rigidBody)
        {
            initialPosition.velocity = rigidBody.velocity;
        }
        timeBuffer.Push(initialPosition);
    }

    public void RewindTime()
    {
        TimePositionInfo pos = timeBuffer.Pop();
        transform.position = pos.position;
        transform.rotation = pos.rotation;
        if (rigidBody != null)
        {
            rigidBody.velocity = pos.velocity;
        }
        // we don't go back in time before the initial position
        if (timeBuffer.Count != 1)
        {
            pos.numStillFrames -= 1;
            if (pos.numStillFrames >= 0)
            {
                timeBuffer.Push(pos);
            }
        }
    }

    /// <summary>
    /// Dumb mechanism for testing if the object moved since the last frame, since `transform.hasChanged` seems really buggy.
    /// </summary>
    /// <returns></returns>
    private bool ObjectMovedSignificantly()
    {
        TimePositionInfo LastPosition = timeBuffer.Peek();
        return Vector3.Distance(transform.position, LastPosition.position) > Settings.PositionChangeThreshold
            || Quaternion.Angle(transform.rotation, LastPosition.rotation) > Settings.RotationChangeThreshold
            || (rigidBody && Vector3.Distance(rigidBody.velocity, LastPosition.velocity) > Settings.VelocityChangeThreshold);
    }

    private bool ObjectMoved()
    {
        TimePositionInfo LastPosition = timeBuffer.Peek();
        return transform.position != LastPosition.position
            || transform.rotation != LastPosition.rotation
            || (rigidBody ? rigidBody.velocity != LastPosition.velocity : false);
    }

    public void RecordPositionInTime()
    {
        TimePositionInfo pos;
        if (Settings.IgnoreSmallMovements ? ObjectMovedSignificantly() : ObjectMoved())
        {
            pos.position = transform.position;
            pos.rotation = transform.rotation;
            pos.numStillFrames = 0;
            pos.velocity = rigidBody?.velocity ?? Vector3.zero;
            //var pos = new TimePositionInfo
            //{
            //    position = transform.position,
            //    rotation = transform.rotation,
            //    numStillFrames = 0,
            //    velocity = rigidBody ? rigidBody.velocity : Vector3.zero;
            //};
            timeBuffer.Push(pos);
        }
        else
        {
            // Object didn't move. Just increment a counter.
            pos = timeBuffer.Pop();
            pos.numStillFrames += 1;
            timeBuffer.Push(pos);
        }
    }
}
