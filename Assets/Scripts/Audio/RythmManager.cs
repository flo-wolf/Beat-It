using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RythmManager : MonoBehaviour {

    public static RythmManager instance;
    public static BPMEvent onBPM = new BPMEvent();

    // bpm clocks
    public static Dictionary<BPM, float> bpmClocks = new Dictionary<BPM, float>();

    // bpm values - bpm60 means 1 call every second, bpm60h means one call every second with half a second offset
    public enum BPM { bpm15, bpm30, bpm60, bpm90, bpm120, bpm180, bpm15h, bpm30h, bpm60h, bpm90h, bpm120h, bpm180h }

    public BPM playerRythm = BPM.bpm90;
    public BPM playerDashRythm = BPM.bpm90h;
    public BPM loopSpawnRythm = BPM.bpm15;
    public BPM loopSegmentShootingRythm = BPM.bpm15h;
    public BPM animationRythm = BPM.bpm60;
    public BPM movingKillDotRythm = BPM.bpm60;

    // bpm references. Use these to compare the bpm type reported with the onBPM event
    public static BPMinfo playerBPM;
    public static BPMinfo playerDashBPM;
    public static BPMinfo loopSpawnBPM;
    public static BPMinfo loopSegmentShootingBPM;
    public static BPMinfo animationBPM;
    public static BPMinfo movingKillDotBPM;

    void Start()
    {
        instance = this;

        // set the static references based on the inspector public settings (workaround to public BPMinfo not showing up)
        playerBPM = new BPMinfo(playerRythm);
        playerDashBPM = new BPMinfo(playerDashRythm);
        loopSpawnBPM = new BPMinfo(loopSpawnRythm);
        loopSegmentShootingBPM = new BPMinfo(loopSegmentShootingRythm);
        animationBPM = new BPMinfo(animationRythm);
        movingKillDotBPM = new BPMinfo(movingKillDotRythm);

        // fill up the clock dictionary
        FillBPMclockDictionary();

        StartCoroutine(UpdateClock15());
        StartCoroutine(UpdateClock30());
        StartCoroutine(UpdateClock60());
        StartCoroutine(UpdateClock90());
        StartCoroutine(UpdateClock120());
        StartCoroutine(UpdateClock180());
    }

    private void FillBPMclockDictionary()
    {
        foreach (BPM bpm in Enum.GetValues(typeof(BPM)))
        {
            bpmClocks.Add(bpm, 0f);
        }
    }

    // convert a bpm type to the corresponding bpm as integer
    public static int BPMtoInt(BPM b)
    {
        switch (b)
        {
            // full bpm
            case BPM.bpm15:
                return 15;
            case BPM.bpm30:
                return 30;
            case BPM.bpm60:
                return 60;
            case BPM.bpm90:
                return 90;
            case BPM.bpm120:
                return 120;
            case BPM.bpm180:
                return 180;

            // half offset bpm
            case BPM.bpm15h:
                return 15;
            case BPM.bpm30h:
                return 30;
            case BPM.bpm60h:
                return 60;
            case BPM.bpm90h:
                return 90;
            case BPM.bpm120h:
                return 120;
            case BPM.bpm180h:
                return 180;
            default:
                return 90;
        }
    }

    // convert a bpm type to bpm halfs
    public static BPM BPMtoBPMh(BPM b)
    {
        switch (b)
        {
            case BPM.bpm15:
                return BPM.bpm15h;
            case BPM.bpm30:
                return BPM.bpm30h;
            case BPM.bpm60:
                return BPM.bpm60h;
            case BPM.bpm90:
                return BPM.bpm90h;
            case BPM.bpm120:
                return BPM.bpm120h;
            case BPM.bpm180:
                return BPM.bpm180h;
            default:
                return BPM.bpm90h;
        }
    }    

    // 15 bpm clock
    IEnumerator UpdateClock15()
    {
        // set this to the appropiate bpm
        float b = 15;

        float duration = (1 / b) * 60;
        float elapsedTime = 0f;

        // this bpm event gets called twice. 
        // Once when the time runs up after one second (bpm60), 
        // once when the time is half done after half a second (bpm60h). 
        // That half call is the offset call, also happening once a second, just half a second offset.
        bool offsetCalled = false;

        // run two second intervalls and check the bpm
        while (elapsedTime <= duration)
        {
            bpmClocks[BPM.bpm15] = elapsedTime;

            elapsedTime += Time.deltaTime;

            // half offset bpm 
            if (elapsedTime >= duration / 2 && !offsetCalled)
            {
                onBPM.Invoke(new BPMinfo(BPM.bpm15h));
                offsetCalled = true;
            }

            // bpm
            if (elapsedTime >= duration)
            {
                onBPM.Invoke(new BPMinfo(BPM.bpm15));

                elapsedTime = elapsedTime % duration;

                bpmClocks[BPM.bpm15] = elapsedTime;

                offsetCalled = false;
            }
            yield return null;
        }
        yield return null;
    }

    // 30 bpm clock
    IEnumerator UpdateClock30()
    {
        // set this to the appropiate bpm
        float b = 30;

        float duration = (1 / b) * 60;
        float elapsedTime = 0f;

        // the half offset call also happends once every 30 bpm, just 15 bpm offset
        bool offsetCalled = false;

        // run two second intervalls and check the bpm
        while (elapsedTime <= duration)
        {
            bpmClocks[BPM.bpm30] = elapsedTime;

            elapsedTime += Time.deltaTime;

            // half offset bpm call
            if (elapsedTime >= duration / 2 && !offsetCalled)
            {
                onBPM.Invoke(new BPMinfo(BPM.bpm30h));
                offsetCalled = true;
            }

            // bpm
            if (elapsedTime >= duration)
            {
                onBPM.Invoke(new BPMinfo(BPM.bpm30));

                elapsedTime = elapsedTime % duration;

                bpmClocks[BPM.bpm30] = elapsedTime;

                offsetCalled = false;
            }
            yield return null;
        }
        yield return null;
    }

    // 60 bpm clock
    IEnumerator UpdateClock60()
    {
        // set this to the appropiate bpm
        float b = 60;

        float duration = (1 / b) * 60;
        float elapsedTime = 0f;

        // this bpm event gets called twice. 
        // Once when the time runs up after one second (bpm60), 
        // once when the time is half done after half a second (bpm60h). 
        // That half call is the offset call, also happening once a second, just half a second offset.
        bool offsetCalled = false;

        // run two second intervalls and check the bpm
        while (elapsedTime <= duration)
        {
            bpmClocks[BPM.bpm60] = elapsedTime;

            elapsedTime += Time.deltaTime;

            // half offset bpm 
            if (elapsedTime >= duration / 2 && !offsetCalled)
            {
                onBPM.Invoke(new BPMinfo(BPM.bpm60h));
                offsetCalled = true;
            }

            // bpm
            if (elapsedTime >= duration)
            {
                onBPM.Invoke(new BPMinfo(BPM.bpm60));

                elapsedTime = elapsedTime % duration;

                bpmClocks[BPM.bpm60] = elapsedTime;

                offsetCalled = false;
            }
            yield return null;
        }
        yield return null;
    }

    // 90 bpm clock
    IEnumerator UpdateClock90()
    {
        // set this to the appropiate bpm
        float b = 90;

        float duration = (1 / b) * 60;
        float elapsedTime = 0f;

        bool offsetCalled = false;

        // run two second intervalls and check the bpm
        while (elapsedTime <= duration)
        {
            bpmClocks[BPM.bpm90] = elapsedTime;

            elapsedTime += Time.deltaTime;

            // half offset bpm 
            if (elapsedTime >= duration / 2 && !offsetCalled)
            {
                onBPM.Invoke(new BPMinfo(BPM.bpm90h));
                offsetCalled = true;
            }

            // the bpm intervall has ended.
            if (elapsedTime >= duration)
            {
                onBPM.Invoke(new BPMinfo(BPM.bpm90));

                elapsedTime = elapsedTime % duration;

                bpmClocks[BPM.bpm90] = elapsedTime;

                offsetCalled = false;
            }
            yield return null;
        }
        yield return null;
    }

    // 120 bpm clock
    IEnumerator UpdateClock120()
    {
        // set this to the appropiate bpm
        float b = 120;

        float duration = (1 / b) * 60;
        float elapsedTime = 0f;

        bool offsetCalled = false;

        // run two second intervalls and check the bpm
        while (elapsedTime <= duration)
        {
            bpmClocks[BPM.bpm120] = elapsedTime;

            elapsedTime += Time.deltaTime;

            // half offset bpm 
            if (elapsedTime >= duration / 2 && !offsetCalled)
            {
                onBPM.Invoke(new BPMinfo(BPM.bpm120h));
                offsetCalled = true;
            }

            // the bpm intervall has ended.
            if (elapsedTime >= duration)
            {
                onBPM.Invoke(new BPMinfo(BPM.bpm120));

                elapsedTime = elapsedTime % duration;

                bpmClocks[BPM.bpm120] = elapsedTime;

                offsetCalled = false;
            }
            yield return null;
        }
        yield return null;
    }

    // 180 bpm clock
    IEnumerator UpdateClock180()
    {
        // set this to the appropiate bpm
        float b = 180;

        float duration = (1 / b) * 60;
        float elapsedTime = 0f;

        bool offsetCalled = false;

        // run two second intervalls and check the bpm
        while (elapsedTime <= duration)
        {
            bpmClocks[BPM.bpm180] = elapsedTime;

            elapsedTime += Time.deltaTime;

            // half offset bpm 
            if (elapsedTime >= duration / 2 && !offsetCalled)
            {
                onBPM.Invoke(new BPMinfo(BPM.bpm180h));
                offsetCalled = true;
            }

            // the bpm intervall has ended.
            if (elapsedTime >= duration)
            {
                onBPM.Invoke(new BPMinfo(BPM.bpm180));

                elapsedTime = elapsedTime % duration;

                bpmClocks[BPM.bpm180] = elapsedTime;

                offsetCalled = false;
            }
            yield return null;
        }
        yield return null;
    }

    /*
    IEnumerator UpdateClock()
    {
        float clockDuration = 4f;
        float elapsedTime = 0f;
        float tolerance = 0.08f;

        // run two second intervalls and check the bpm
        while (elapsedTime <= clockDuration)
        {
            Debug.Log("elapsed Time: " + elapsedTime);

            if (elapsedTime % 1f < tolerance && rythmEventCalled[0] == false) // 60bpm
            {
                onBPM.Invoke(BPM.bpm60);
                //rythmEventCalled[0] = true;
                Debug.Log("BPM Call: 60");
            }
            else if (elapsedTime % 0.66f < tolerance && rythmEventCalled[1] == false) // 90bpm
            {
                onBPM.Invoke(BPM.bpm90);
                //rythmEventCalled[1] = true;
                Debug.Log("BPM Call: 90");
            }
            else if (elapsedTime % 0.5f < tolerance && rythmEventCalled[2] == false) // 120bpm
            {
                onBPM.Invoke(BPM.bpm120);
                //rythmEventCalled[2] = true;
                Debug.Log("BPM Call: 120");
            }
            else if (elapsedTime % 0.33f < tolerance && rythmEventCalled[3] == false) // 180bpm
            {
                onBPM.Invoke(BPM.bpm180);
                //rythmEventCalled[3] = true;
                Debug.Log("BPM Call: 180");
            }
            else if (elapsedTime % 0.25f < tolerance && rythmEventCalled[4] == false) // 240bpm
            {
                onBPM.Invoke(BPM.bpm240);
                //rythmEventCalled[4] = true;
                Debug.Log("BPM Call: 240");
            }

            elapsedTime += Time.deltaTime;
            clock = elapsedTime;

            // the bpm intervall has ended.
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

                // set all values back to false
                rythmEventCalled = new bool[6];

                elapsedTime = elapsedTime % clockDuration;
            }
            yield return null;
        }
        yield return null;
    }
    */

    /*
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
    */

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
    public class BPMEvent : UnityEvent<BPMinfo> { }
}
