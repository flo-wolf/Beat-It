using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Game : MonoBehaviour {

    public static Game _game;

    public Text spawnText;
    public Slider timeSlider;


    public float respawnInfoDespawnTime = 0.5f;
    public float timeStepDuration = 0.5f;

    private static float timestep = 0f;

    // gamestate
    public enum State { Restart, Playing, Death, Respawn }
    public static State state = State.Restart;

    public static GameStateChangeEvent onGameStateChange = new GameStateChangeEvent();

    void Awake()
    {
        _game = this;
    }

    void Update()
    {
        if (state == State.Restart && (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)))
        {
            StartCoroutine(FadeSpawnText(false));
            Game.state = Game.State.Playing;
        }
    }

    public static void SetState(State newState)
    {
        if (newState == State.Death)
        {
            state = State.Death;
            //_game.StartCoroutine(_game.FadeSpawnText(true, Restart));
        }
        else if (newState == State.Restart)
        {
        }

        state = newState;
        onGameStateChange.Invoke(state);
    }

    public static void Restart()
    {
        SetState(State.Restart);
    }

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


    // events
    public class GameStateChangeEvent : UnityEvent<State> { }
}


