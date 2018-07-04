using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour {

    public static Game instance = null;

    public static float levelSwitchDuration = 1f;
    public float timeStepDuration = 0.5f;

    private static float timestep = 0f;

    // level management
    public static int level = 0;
    public List<string> levels = new List<string>();

    // gamestate
    public enum State { LevelFadein, Playing, Death, NextLevelFade, RestartFade, None }
    public static State state = State.None;

    public static GameStateChangeEvent onGameStateChange = new GameStateChangeEvent();


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    void Start()
    {
        Debug.Log("Start");
        StartCoroutine(LevelFadeTimer(true));
    }

    private void OnLevelWasLoaded(int level)
    {
        StartCoroutine(LevelFadeTimer(true));
    }

    public static void SetState(State newState)
    {
        Debug.Log("newState: " + newState);
        state = newState;
        onGameStateChange.Invoke(state);
        if (newState == State.Death)
        {
            if (!Player.deathBySegment)
            {
                AudioManager.instance.Play("Death");
                //AudioManager.instance.Play("Death2");
                //
            }
            else
            {
                AudioManager.playDeathSounds = true;
                Player.deathBySegment = false;
            }
                
            SetState(State.RestartFade);
        }
        else if(newState == State.RestartFade)
        {
            Debug.Log("RestartFade");
            instance.StartCoroutine(instance.C_Restart());
        }
        else if (newState == State.NextLevelFade)
        {
            Debug.Log("RestartFade");
            instance.StartCoroutine(instance.C_NextLevel());
        }
    }

    public IEnumerator C_Restart()
    {
        Debug.Log("Restart the level");

        yield return new WaitForSeconds(levelSwitchDuration*1);
        SceneManager.LoadScene(levels[level]);
        yield return null;
    }

    public IEnumerator C_NextLevel()
    {
        Debug.Log("Restart the level");

        yield return new WaitForSeconds(levelSwitchDuration);
        SetState(State.Playing);
        yield return new WaitForSeconds(levelSwitchDuration);
        SetState(State.Playing);
        yield return null;
    }

    // sets the state after the fadein fadeout
    IEnumerator LevelFadeTimer(bool fadeIn)
    {
        SetState(State.LevelFadein);
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


