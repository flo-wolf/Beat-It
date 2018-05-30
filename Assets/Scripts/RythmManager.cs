using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RythmManager : MonoBehaviour {

    // we split a timeframe of 2 seconds into 8 parts. Tick1 = 0.25sek, Tick4 = 1sek, Tick6 = 1.5sek, Tick8 = 2sek. 
    public enum Ticks { tick1, tick2, tick3, tick4, tick5, tick6, tick7, tick8 }

    public static RythmManager instance;
    public static RythmEvent onRythm = new RythmEvent();
    public static RythmEvent onFixedRythm = new RythmEvent();

    public static float clock = 0f;
    public static float fixedClock = 0f;
    public float fixedClockBPM = 120f;
    public float clockBPM = 120f;

    // timeframe that gets divided into 8 ticks
    private float clockDuration = 2f;

    void Start()
    {
        instance = this;
        StartCoroutine(UpdateClock());
        StartCoroutine(UpdateFixedClock());
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator UpdateClock()
    {
        float clockDuration = (1 / clockBPM) * 60;

        float elapsedTime = 0f;
        while (elapsedTime <= clockDuration)
        {
            
            clockDuration = (1 / clockBPM) * 60;
            elapsedTime += Time.deltaTime;
            clock = elapsedTime;
            if (elapsedTime >= clockDuration)
            {
                if (Player._player.tempoUp)
                {
                    RythmManager.instance.clockBPM *= 2;
                    Player._player.tempoUp = false;
                }

                else if (Player._player.tempoDown)
                {
                    RythmManager.instance.clockBPM /= 2;
                    Player._player.tempoDown = false;
                }

                onRythm.Invoke(1f);
                elapsedTime = elapsedTime % clockDuration;

            }
            yield return null;
        }
        yield return null;
    }

    IEnumerator UpdateFixedClock()
    {
        float fixedClockDuration = (1 / fixedClockBPM) * 60;

        float elapsedTime = 0f;
        while (elapsedTime <= fixedClockDuration)
        {
            fixedClockDuration = (1 / fixedClockBPM) * 60;
            elapsedTime += Time.deltaTime;
            fixedClock = elapsedTime;
            if (elapsedTime >= fixedClockDuration)
            {
                if (Player._player.tempoUp)
                {
                    RythmManager.instance.clockBPM *= 2;
                    Player._player.tempoUp = false;
                }

                else if (Player._player.tempoDown)
                {
                    RythmManager.instance.clockBPM /= 2;
                    Player._player.tempoDown = false;
                }

                onFixedRythm.Invoke(1f);
                elapsedTime = elapsedTime % fixedClockDuration;

            }
            yield return null;
        }
        yield return null;
    }

    /*
    private void UpdateClock()
    {
        float clockDuration = (1 / clockBPM) * 60;

        if (clock + Time.deltaTime < clockDuration)
        {
            clock += Time.deltaTime;

            // rythm events go here (offbeat for loop spawning)
        }

        // the clock has reached its end => set back to 0
        else if (clock + Time.deltaTime >= clockDuration)
        {
            clock = clockDuration;
            
            onRythm.Invoke(clock);
            clock = 0f;
        }
    }

    private void UpdateFixedClock()
    {
        float fixedClockDuration = (1 / fixedClockBPM) * 60;

        if (fixedClock + Time.deltaTime < fixedClockDuration)
        {
            fixedClock += Time.deltaTime;

            // rythm events go here (offbeat for loop spawning)
        }

        // the clock has reached its end => set back to 0
        else if (fixedClock + Time.deltaTime >= fixedClockDuration)
        {
            fixedClock = fixedClockDuration;

            onFixedRythm.Invoke(fixedClock);
            fixedClock = 0f;
        }
    }
    */



    // events
    public class RythmEvent : UnityEvent<float> { }
}
