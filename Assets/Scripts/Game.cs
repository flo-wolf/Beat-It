using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Game : MonoBehaviour {

    public static Game instance;

    public float levelSwitchDuration = 1f;
    public float timeStepDuration = 0.5f;

    private static float timestep = 0f;
    public static int level = 1;

    // gamestate
    public enum State { LevelFadein, Playing, Death, LevelFadeout }
    public static State state = State.LevelFadein;

    public static GameStateChangeEvent onGameStateChange = new GameStateChangeEvent();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        SetState(State.LevelFadein);
        StartCoroutine(LevelFadeTimer(true));
    }

    public static void SetState(State newState)
    {
        Debug.Log("newState: " + newState);
        state = newState;
        onGameStateChange.Invoke(state);
        if (newState == State.Death)
        {
            SetState(State.LevelFadeout);
        }
        else if(newState == State.LevelFadeout)
        {
            Debug.Log("LevelFadeOut");
            instance.StartCoroutine(instance.LevelFadeTimer(false));
        }
    }

    // sets the state after the fadein fadeout
    IEnumerator LevelFadeTimer(bool fadeIn)
    {
        Debug.Log("LevelFadeeTimer");
        if (fadeIn)
        {
            yield return new WaitForSeconds(levelSwitchDuration);
            SetState(State.Playing);
        }
        else
        {
            yield return new WaitForSeconds(levelSwitchDuration);
            SetState(State.Playing);
            // switch level
        }
        yield return null;
    }

    //public static void Restart()
    //{
        //SetState(State.Restart);
    //}

        /*
    IEnumerator FadeSpawnText(bool fadeIn, Action onComplete = null)
    {
        float elapsedTime = 0f;

        Color c = spawnText.color;

        while (elapsedTime <= respawnInfoDespawnTime)
        {
            elapsedTime += Time.deltaTime;
            if (!fadeIn)
            {
                c.a = Mathf.Lerp(1, 0, (elapsedTime / respawnInfoDespawnTime));
            }
            else
            {
                c.a = Mathf.Lerp(0, 1, (elapsedTime / respawnInfoDespawnTime));
            }
            spawnText.color = c;

            yield return null;
        }
        if (onComplete != null)
            onComplete();

        yield return null;
    }
    */


    // events
    public class GameStateChangeEvent : UnityEvent<State> { }
}


