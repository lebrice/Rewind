using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// Monobehaviour used to make an object and all its children 'rewindable' in time.
/// </summary>
public class RewindBehaviour : MonoBehaviour
{
    public BoolVariable Rewinding;
    public RewindSettings Settings;
    private ObjectTracker[] Trackers;
    public const int TrimBufferExcessInterval = 50;

    private long FixedUpdateCount;

    private void Awake()
    {
        AddTrackers();
    }


    /// <summary>
    /// Adds trackers to keep track of the state all non-static child objects over time, as well as this object itself.
    /// </summary>
    private void AddTrackers()
    {
        foreach (Transform child in GetComponentInChildren<Transform>())
        {
            GameObject childGameObject = child.gameObject;
            
            if (childGameObject.GetComponent<ObjectTracker>() == null)
            {
                var rewindElement = childGameObject.AddComponent<ObjectTracker>();
                rewindElement.Settings = Settings;
            }
        }
        var tracker = gameObject.AddComponent<ObjectTracker>();
        tracker.Settings = Settings;

        Trackers = GetComponentsInChildren<ObjectTracker>();
    }

    private void FixedUpdate()
    {
        if (Rewinding)
        {
            foreach (var tracker in Trackers)
            {
                tracker.RewindTime();
            }
        }
        else
        {
            // we are recording.
            foreach (var tracker in Trackers)
            {
                tracker.RecordPositionInTime();
            }
        }
        if (FixedUpdateCount++ % TrimBufferExcessInterval == 0)
        {
            // once in a while (every second or so), trim the excess of the timebuffers.
            foreach (var tracker in Trackers)
            {
                tracker.timeBuffer.TrimExcess();
            }
        }
    }

    public class TimePositionInfo
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 velocity;
        public long numStillFrames;
    }

    public class ObjectTracker : MonoBehaviour
    {
        public RewindSettings Settings;
        public Stack<TimePositionInfo> timeBuffer;
        private Rigidbody rigidBody;

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
            TimePositionInfo lastPosition = timeBuffer.Peek();
            transform.position = lastPosition.position;
            transform.rotation = lastPosition.rotation;
            if (rigidBody != null)
            {
                rigidBody.velocity = lastPosition.velocity;
            }
            // we don't go back in time before the initial position
            if (timeBuffer.Count == 1)
            {
                return;
            }
            lastPosition.numStillFrames -= 1;
            if (lastPosition.numStillFrames < 0)
            {
                timeBuffer.Pop();
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
                || (rigidBody ? Vector3.Distance(rigidBody.velocity, LastPosition.velocity) > Settings.VelocityChangeThreshold : false);
        }

        private bool ObjectMoved()
        {
            TimePositionInfo LastPosition = timeBuffer.Peek();
            return transform.position != LastPosition.position
                || transform.rotation != LastPosition.rotation
                || rigidBody?.velocity != LastPosition.velocity;
        }

        public void RecordPositionInTime()
        {
            if (Settings.IgnoreSmallMovements ? ObjectMovedSignificantly() : ObjectMoved())
            {
                var pos = new TimePositionInfo
                {
                    position = transform.position,
                    rotation = transform.rotation,
                    numStillFrames = 0,
                };
                if (rigidBody)
                {
                    pos.velocity = rigidBody.velocity;
                }
                timeBuffer.Push(pos);
            }
            else
            {
                //print("before" + timeBuffer.Peek().numStillFrames);
                TimePositionInfo lastPosition = timeBuffer.Peek();
                lastPosition.numStillFrames += 1;
                //print("after" + timeBuffer.Peek().numStillFrames);
                //print("Object has been still for " + lastPosition.numStillFrames + " Now.");
            }
        }
    }
}
