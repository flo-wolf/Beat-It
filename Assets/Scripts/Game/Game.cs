﻿using System;
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
    public enum State { LevelFadein, Playing, DeathOnNextBeat, Death, NextLevelFade, RestartFade, None }
    public static State state = State.None;

    public static GameStateChangeEvent onGameStateChange = new GameStateChangeEvent();

    public static bool quickSceneLoad = false;


    void Awake()
    {
        

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        GetLevelNumberFromLevelName();
        Debug.Log("LevelNumber: " + level);
    }

    private void OnLevelWasLoaded(int level)
    {
        GetLevelNumberFromLevelName();
        Debug.Log("LevelNumber: " + level);
    }

    void Start()
    {
        RythmManager.onBPM.AddListener(OnBPM);
    }

    public void OnBPM(BPMinfo bpm)
    {
        if(bpm.Equals(RythmManager.playerBPM))
        {
            if(state == State.DeathOnNextBeat)
            {
                SetState(State.Death);
            }
        }
    }

    public static void SetState(State newState)
    {
        State oldState = state;
        Debug.Log("newState: " + newState);
        state = newState;
        onGameStateChange.Invoke(state);


        if (newState == State.DeathOnNextBeat)
        {
            Player.allowMove = false;
            quickSceneLoad = true;
        }


        if (newState == State.Death) {
            quickSceneLoad = true;
            if (!Player.deathByMovingKillDot)
            {
                if (!Player.deathBySegment && !Player.deathByMovingKillDot)
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
            }
            else
                Player.deathByMovingKillDot = false;
            SetState(State.RestartFade);
        }
        else if(newState == State.RestartFade)
        {
            Debug.Log("RestartFade");
            quickSceneLoad = true;
            LevelTransition.instance.FadeOutLevelRespawn();
        }
        else if (newState == State.NextLevelFade && newState != oldState)
        {
            LevelTransition.instance.FadeOutLevel();
        }
    }

    public static void LoadNextLevel()
    {
        Debug.Log("Load the next level");
        if (instance.levels.Count > level + 1)
        {
            SceneManager.LoadScene(instance.levels[level + 1]);
        }
            
        else
            SceneManager.LoadScene(instance.levels[level]);
    }

    public static void RestartLevel()
    {
        SceneManager.LoadScene(instance.levels[level]);
    }

    public IEnumerator C_Restart()
    {
        Debug.Log("Restart the level");
        
        yield return new WaitForSeconds(levelSwitchDuration*1);
        
        yield return null;
    }


    private void GetLevelNumberFromLevelName()
    {
        int parsedLevel = -1;
        String levelName = SceneManager.GetActiveScene().name;
        string numbersOnly = System.Text.RegularExpressions.Regex.Replace(levelName, "[^0-9]", "");
        int.TryParse(numbersOnly, out parsedLevel);
        level = parsedLevel;
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


