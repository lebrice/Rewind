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
    private List<Tracker> Trackers = new List<Tracker>();


    private void Awake()
    {
        // add a tracker for each child.
        foreach (GameObject childGameObject in ChildGameObjects(gameObject))
        {
            Tracker tracker = GetOrAddTracker(childGameObject);
            Trackers.Add(tracker);
        }
    }

    private void OnEnable()
    {
        foreach (Tracker tracker in Trackers)
        {
            controller.RegisterRewindableObject(tracker);
        }
    }

    private void Reset()
    {
        foreach (Tracker tracker in Trackers)
        {
            tracker.Reset();
        }
    }


    private void OnDisable()
    {
        foreach (Tracker tracker in Trackers)
        {
            controller.UnRegisterRewindableObject(tracker);
        }
    }

    private void OnDestroy()
    {
        foreach (var tracker in Trackers)
        {
            Destroy(tracker);
        }
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

public class Tracker : MonoBehaviour
{
    public class RewindSettings
    {
        public bool IgnoreSmallMovements = true;
        public float PositionChangeThreshold = 0.05f;
        public float RotationChangeThreshold = 1f;
        public float VelocityChangeThreshold = 0.5f;
    }

    public struct TimePositionInfo
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 velocity;
        public long framesSpendInThisState;
    }


    public static RewindSettings Settings = new RewindSettings();

    public Stack<TimePositionInfo> timeBuffer = new Stack<TimePositionInfo>();
    public bool IsAtInitialPosition => timeBuffer.Count == 0;
    public int BufferLength;

    private Rigidbody rigidBody;
    private TimePositionInfo InitialPosition;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>(); // the object might not have one.
    }

    internal void Reset()
    {
        timeBuffer.Clear();
        timeBuffer.TrimExcess();
    }

    private void Start()
    {
        Reset();
        InitialPosition = new TimePositionInfo
        {
            position = transform.position,
            rotation = transform.rotation,
            velocity = rigidBody != null ? rigidBody.velocity : Vector3.zero,
            framesSpendInThisState = long.MaxValue / 2, // the object has been there 'forever'
        };
        timeBuffer.Push(InitialPosition);
    }

    private void Update()
    {
        BufferLength = timeBuffer.Count;
    }

    public void RewindTime()
    {
        // we can't go back in time further than the initial position
        if (!IsAtInitialPosition)
        {
            TimePositionInfo pos = timeBuffer.Pop();
            transform.position = pos.position;
            transform.rotation = pos.rotation;
            if (rigidBody != null)
            {
                rigidBody.velocity = pos.velocity;
            }
            // push it back if it is still viable
            if (pos.framesSpendInThisState > 1)
            {
                pos.framesSpendInThisState -= 1;
                timeBuffer.Push(pos);
            }
        }
    }

    public void RecordPositionInTime()
    {
        TimePositionInfo pos;
        if (Settings.IgnoreSmallMovements ? MovedSignificantly() : Moved())
        {
            pos.position = transform.position;
            pos.rotation = transform.rotation;
            pos.framesSpendInThisState = 1;
            pos.velocity = rigidBody ? rigidBody.velocity : Vector3.zero;
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
            // Object didn't move.
            if (IsAtInitialPosition)
            {
                return; // we don't change the initial position.
            }
            else
            {
                // Just increment a counter.
                pos = timeBuffer.Pop();
                pos.framesSpendInThisState += 1;
                timeBuffer.Push(pos);
            }
        }
    }

    /// <summary>
    /// Dumb (inneficient) mechanism for testing if the object moved since the last frame, since `transform.hasChanged` seems really buggy.
    /// </summary>
    /// <returns></returns>
    private bool MovedSignificantly()
    {
        TimePositionInfo LastPosition = timeBuffer.Peek();
        return Vector3.Distance(transform.position, LastPosition.position) > Settings.PositionChangeThreshold
            || Quaternion.Angle(transform.rotation, LastPosition.rotation) > Settings.RotationChangeThreshold
            || (rigidBody && Vector3.Distance(rigidBody.velocity, LastPosition.velocity) > Settings.VelocityChangeThreshold);
    }

    private bool Moved()
    {
        TimePositionInfo LastPosition = timeBuffer.Peek();
        return transform.position != LastPosition.position
            || transform.rotation != LastPosition.rotation
            || (rigidBody && (rigidBody.velocity != LastPosition.velocity));
    }


}

