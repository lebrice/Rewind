using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using utils;

[RequireComponent(typeof(Text))]
[RequireComponent(typeof(GameEventListener))]
[RequireComponent(typeof(GameEventListener))]
public class CountDownTimer : MonoBehaviour
{
    public float StartingTimeSeconds = 30;
    public float HurryUpThreshold = 10;

    private Text text;

    public Color NormalColor = Color.black;
    public Color HurryUpColor = Color.red;
    
    public float TimeLeft { get; private set; }

    /// <summary>
    /// Wether or not we are currently rewinding time.
    /// (Should ideally be set by using a GameEventListener)
    /// </summary>
    public bool Rewinding { get; set; }

    public GameEvent OnTimerEnded;
    public GameEvent OnHurryUp;

    private bool timerEndedEventWasRaised;
    private bool hurryUpEventWasRaised;

    private void Awake()
    {
        text = GetComponent<Text>();
        TimeLeft = StartingTimeSeconds;
    }

    private void Reset()
    {
        TimeLeft = StartingTimeSeconds;
        timerEndedEventWasRaised = false;
        hurryUpEventWasRaised = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (Rewinding)
        {
            TimeLeft += Time.deltaTime;
        }
        else
        {
            TimeLeft -= Time.deltaTime;
        }
        
        if (TimeLeft < HurryUpThreshold && !hurryUpEventWasRaised)
        {
            OnHurryUp?.Raise();
            hurryUpEventWasRaised = true;
        }
        if (TimeLeft < 0 && !timerEndedEventWasRaised)
        {
            OnTimerEnded?.Raise();
            timerEndedEventWasRaised = true;
        }

        text.color = TimeLeft > HurryUpThreshold ? NormalColor : HurryUpColor;
        TimeLeft = Mathf.Clamp(TimeLeft, 0, StartingTimeSeconds);
        text.text = TimeLeft.ToString("0.00");
    }
}
