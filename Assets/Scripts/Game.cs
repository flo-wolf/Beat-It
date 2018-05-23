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
    public enum State { Prestart, Playing, Death, Respawn }
    public static State state = State.Prestart;
    
    public static GameStateChangeEvent onGameStateChange = new GameStateChangeEvent();
    public static TimeStepEvent onTimeStepChange = new TimeStepEvent();

    void Awake ()
    {
        _game = this;
	}

    void Update()
    {
        if (state == State.Prestart && (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)))
        {
            StartCoroutine(FadeSpawnText(false));
            Game.state = Game.State.Playing;
        }

        UpdateTimeStep();
    }

    private void UpdateTimeStep()
    {
        if (timestep + Time.deltaTime < timeStepDuration)
        {
            timestep += Time.deltaTime;
            timeSlider.value = Map(timestep, 0, timeStepDuration, 0, 1);
        }

        else if (timestep + Time.deltaTime >= timeStepDuration)
        {
            timestep = timeStepDuration;
            timeSlider.value = Map(timestep, 0, timeStepDuration, 0, 1);
            // the timestep has reached its end => move player
            onTimeStepChange.Invoke(timestep);
            timestep = 0f;
        }
    }

    public static void SetState(State newState)
    {
        if (newState == State.Death)
        {
            state = State.Death;
            _game.StartCoroutine(_game.FadeSpawnText(true, Restart));
        }
        else if(newState == State.Prestart)
        {
        }

        state = newState;
        onGameStateChange.Invoke(state);
    }

    public static void Restart()
    {
        SetState(State.Prestart);
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
    public class TimeStepEvent : UnityEvent<float> { }

    float Map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}


