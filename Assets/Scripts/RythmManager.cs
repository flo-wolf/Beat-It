using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RythmManager : MonoBehaviour {

    public static RythmManager instance;
    public static BPMEvent onBPM = new BPMEvent();

    public enum BPM { bpm60, bpm90, bpm120, bpm180 }
    public BPM playerBPM = BPM.bpm90;
    public BPM levelBPM = BPM.bpm60;

    public static float playerClock = 0f;
    public static float levelClock = 0f;

    // the current player bpm as an integer
    public static int _playerBPM;
    public static int _levelBPM;

    void Start()
    {
        instance = this;
        UpdateBPM();

        StartCoroutine(UpdateClock60());
        StartCoroutine(UpdateClock90());
        StartCoroutine(UpdateClock120());
        StartCoroutine(UpdateClock180());
    }

    // update the bpm integer value depending on the BPM selected
    void UpdateBPM()
    {
        switch (playerBPM)
        {
            case BPM.bpm60:
                _playerBPM = 60;
                break;
            case BPM.bpm90:
                _playerBPM = 90;
                break;
            case BPM.bpm120:
                _playerBPM = 120;
                break;
            case BPM.bpm180:
                _playerBPM = 180;
                break;
        }

        switch (levelBPM)
        {
            case BPM.bpm60:
                _levelBPM = 60;
                break;
            case BPM.bpm90:
                _levelBPM = 90;
                break;
            case BPM.bpm120:
                _levelBPM = 120;
                break;
            case BPM.bpm180:
                _levelBPM = 180;
                break;
        }
    }

    // 60 bpm clock
    IEnumerator UpdateClock60()
    {
        // set this to the appropiate bpm
        float b = 60;

        float duration = (1 / b) * 60;
        float elapsedTime = 0f;

        // run two second intervalls and check the bpm
        while (elapsedTime <= duration)
        {
            if (levelBPM == BPM.bpm60)
                levelClock = elapsedTime;
            if (playerBPM == BPM.bpm60)
                playerClock = elapsedTime;

            elapsedTime += Time.deltaTime;

            // the bpm intervall has ended.
            if (elapsedTime >= duration)
            {
                onBPM.Invoke(BPM.bpm60);
                Debug.Log("BPM Call: 60");

                elapsedTime = elapsedTime % duration;

                if (levelBPM == BPM.bpm60)
                    levelClock = elapsedTime;
                if (playerBPM == BPM.bpm60)
                    playerClock = elapsedTime;
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

        // run two second intervalls and check the bpm
        while (elapsedTime <= duration)
        {
            if (levelBPM == BPM.bpm90)
                levelClock = elapsedTime;
            if (playerBPM == BPM.bpm90)
                playerClock = elapsedTime;

            elapsedTime += Time.deltaTime;

            // the bpm intervall has ended.
            if (elapsedTime >= duration)
            {
                onBPM.Invoke(BPM.bpm90);
                Debug.Log("BPM Call: 90");

                elapsedTime = elapsedTime % duration;

                if (levelBPM == BPM.bpm90)
                    levelClock = elapsedTime;
                if (playerBPM == BPM.bpm90)
                    playerClock = elapsedTime;
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

        // run two second intervalls and check the bpm
        while (elapsedTime <= duration)
        {
            if (levelBPM == BPM.bpm120)
                levelClock = elapsedTime;
            if (playerBPM == BPM.bpm120)
                playerClock = elapsedTime;

            elapsedTime += Time.deltaTime;

            // the bpm intervall has ended.
            if (elapsedTime >= duration)
            {
                onBPM.Invoke(BPM.bpm120);
                Debug.Log("BPM Call: 120");

                elapsedTime = elapsedTime % duration;

                if (levelBPM == BPM.bpm120)
                    levelClock = elapsedTime;
                if (playerBPM == BPM.bpm120)
                    playerClock = elapsedTime;
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

        // run two second intervalls and check the bpm
        while (elapsedTime <= duration)
        {
            if (levelBPM == BPM.bpm180)
                levelClock = elapsedTime;
            if (playerBPM == BPM.bpm180)
                playerClock = elapsedTime;

            elapsedTime += Time.deltaTime;

            // the bpm intervall has ended.
            if (elapsedTime >= duration)
            {
                onBPM.Invoke(BPM.bpm180);
                Debug.Log("BPM Call: 180");

                elapsedTime = elapsedTime % duration;

                if (levelBPM == BPM.bpm180)
                    levelClock = elapsedTime;
                if (playerBPM == BPM.bpm180)
                    playerClock = elapsedTime;
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
    public class BPMEvent : UnityEvent<BPM> { }
}
