using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour {

    public static Game _game;

    public Text spawnText;
    public float prespawnInfoDespawnTime = 0.5f;

    public enum State { Prestart, Playing, Death, Respawn }
    public static State state = State.Prestart;

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
    }

    public static void Restart()
    {
        SetState(State.Prestart);
    }

    IEnumerator FadeSpawnText(bool fadeIn, Action onComplete = null)
    {
        float elapsedTime = 0f;

        Color c = spawnText.color;

        while (elapsedTime <= prespawnInfoDespawnTime)
        {
            elapsedTime += Time.deltaTime;
            if (!fadeIn)
            {
                c.a = Mathf.Lerp(1, 0, (elapsedTime / prespawnInfoDespawnTime));
            }
            else
            {
                c.a = Mathf.Lerp(0, 1, (elapsedTime / prespawnInfoDespawnTime));
            }
            spawnText.color = c;

            yield return null;
        }
        if (onComplete != null)
            onComplete();

        yield return null;
    }
}
