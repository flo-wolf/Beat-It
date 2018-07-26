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
    public int startWithIndex = 1;

    public static int level = 1;
    public List<string> levels = new List<string>();

    // gamestate
    public enum State { LevelFadein, Playing, DeathOnNextBeat, Death, NextLevelFade, RestartFade, None }
    public static State state = State.None;

    public static GameStateChangeEvent onGameStateChange = new GameStateChangeEvent();

    public static bool quickSceneLoad = false;

    public bool waitForInputBeforePlaying = false;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        level = startWithIndex;
        GetLevelNumberFromLevelName();
        Debug.Log("LevelNumber: " + level);
    }

    private void OnLevelWasLoaded(int level)
    {
        if (this != instance) return;
        GetLevelNumberFromLevelName();
        Debug.Log("LevelNumber: " + level);
        StopCoroutine("C_WaitForInputToPlay");
    }

    void Start()
    {
        RythmManager.onBPM.AddListener(OnBPM);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Joystick1Button7) && Game.state == Game.State.Playing)
        {
            state = State.None;
            RestartGame();
            AudioManager.RestartPiano();
        }
    }

    public void OnBPM(BPMinfo bpm)
    {
        if(bpm.Equals(RythmManager.playerBPM))
        {
            if(state == State.DeathOnNextBeat)
            {
                SetState(State.Death);
                Teleporter.teleportEnabled = true;
            }
        }
    }

    public static void SetState(State newState)
    {
        State oldState = state;
        Debug.Log("newState: " + newState);
        state = newState;
        onGameStateChange.Invoke(state);


        if (newState == State.DeathOnNextBeat && oldState == State.Playing)
        {
            Player.allowMove = false;
            quickSceneLoad = true;
        }

        if (newState == State.Death && oldState != State.Death && oldState != State.NextLevelFade && oldState != State.RestartFade && oldState != State.None) {
            quickSceneLoad = true;
            if (!Player.deathByMovingKillDot)
            {
                if (!Player.deathBySegment && !Player.deathByMovingKillDot)
                {
                    AudioManager.deathCounter++;

                    if(AudioManager.deathCounter >= AudioManager.instance.deathCount)
                    {
                        AudioManager.instance.PlayRandomDeathSound();
                        AudioManager.deathCounter = 0;
                    }
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

        else if(newState == State.Playing)
        {
            instance.StartCoroutine(instance.C_WaitForInputToPlay());
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

    public static void RestartGame()
    {
        SceneManager.LoadScene(instance.levels[Game.instance.startWithIndex]);
    }

    private void GetLevelNumberFromLevelName()
    {
        int parsedLevel = -1;
        String levelName = SceneManager.GetActiveScene().name;
        string numbersOnly = System.Text.RegularExpressions.Regex.Replace(levelName, "[^0-9]", "");
        int.TryParse(numbersOnly, out parsedLevel);
        level = parsedLevel;
    }

    IEnumerator C_WaitForInputToPlay()
    {
        while(Player.allowMove != true)
        {
            if ((InputDeviceDetector.instance.RecievingStartInput() && waitForInputBeforePlaying) || !waitForInputBeforePlaying)
            {
                Player.allowMove = true;
                yield return new WaitForSeconds(RythmManager.playerBPM.ToSecs() * 2);
                PlayerSpawn.instance.FadeOutSpawn();
                break;
            }
            yield return null;
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


