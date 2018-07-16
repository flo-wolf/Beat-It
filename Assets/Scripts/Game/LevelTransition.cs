using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelTransition : MonoBehaviour {

    public class LevelTransitionEvent : UnityEvent<Action> { }
    public static LevelTransitionEvent onLevelTransition = new LevelTransitionEvent();

    public enum Action { FadeOutGridDots, FadeOutRadiusHandle, SpawnFadeIn, SpawnFadeOut}

    public static LevelTransition instance = null;

    public bool smoothDotFadeLerp = true;

    [Header("Level End Transition")]
    public float out_FadeDotsDuration = 1f;
    public float out_cameraToNewSpawnDuration = 1f;

    [Header("Level Start Transition")]
    public float start_ShowPlayerDuration = 1f;
    public float start_FadeDotsDuration = 2f;
    public float start_ShowGoalDuration = 1f;
    public float start_FadeObjectsDuration = 2f;

    public enum State { Default, ShowPlayer, FadeInDots, ShowGoal, FadeInLevelObjects, Done }

	// Use this for initialization
	void Start ()
    {
        if (instance == null)
            instance = this;

        // a new level gets played
        if (!Game.quickSceneLoad)
        {
            StopAllCoroutines();
            StartCoroutine(C_LevelIntro());
        }
        // respawning
        else
        {
            StopAllCoroutines();
            StartCoroutine(C_LevelIntroRespawn());
        }
	}

    private void OnLevelWasLoaded(int level)
    {
        if (this != instance) return;
        Debug.Log("Loades scene -- Game.quickSceneLoad: " + Game.quickSceneLoad);
        if (!Game.quickSceneLoad)
            StartCoroutine(C_LevelIntro());
        // respawning
        else
        {
            StartCoroutine(C_LevelIntroRespawn());
            Game.quickSceneLoad = false;
        }
    }

    public void FadeOutLevel()
    {
        StartCoroutine(C_LevelOutro());
    }

    public void FadeOutLevelRespawn()
    {
        StartCoroutine(C_LevelOutroRespawn());
    }

    // next level outro
    IEnumerator C_LevelOutro()
    {
        // show only the player

        Vector3 panStartPos = Grid.FindPlayerGoal().transform.position - Grid.instance.transform.position;
        Vector3 panEndPos = EditPlayerSpawnData.GetNextPlayerSpawnPositon() - Grid.instance.transform.position;
        Grid.Pan(panStartPos, panEndPos, out_cameraToNewSpawnDuration);
        yield return new WaitForSeconds(out_cameraToNewSpawnDuration / 2);
        onLevelTransition.Invoke(Action.FadeOutRadiusHandle);
        onLevelTransition.Invoke(Action.FadeOutGridDots);
        yield return new WaitForSeconds(out_cameraToNewSpawnDuration/2);
        

        yield return new WaitForSeconds(out_FadeDotsDuration);
        Game.LoadNextLevel();

        yield return null;
    }

    // restart level outro
    IEnumerator C_LevelOutroRespawn()
    {
        // show only the player

        //yield return new WaitForSeconds(out_cameraToNewSpawnDuration);
        onLevelTransition.Invoke(Action.FadeOutRadiusHandle);
        yield return new WaitForSeconds(out_FadeDotsDuration);
        
        Game.RestartLevel();
        yield return null;
    }

    // next level intro
    IEnumerator C_LevelIntro()
    {
        // show only the player
        onLevelTransition.Invoke(Action.SpawnFadeIn);
        yield return new WaitForSeconds(start_ShowPlayerDuration);

        Grid.FadeGridDotsGradually(Grid.FindPlayerSpawn().transform.position, start_FadeDotsDuration, 80f, true);
        yield return new WaitForSeconds(start_FadeDotsDuration);

        Grid.FadeLevelObjectsGradually(Grid.FindPlayerSpawn().transform.position, start_FadeDotsDuration, 80f, true);

        yield return new WaitForSeconds(start_ShowGoalDuration);

        yield return new WaitForSeconds(start_FadeObjectsDuration);
        Game.SetState(Game.State.Playing);
        yield return new WaitForSeconds(RythmManager.playerBPM.ToSecs() * 2);
        onLevelTransition.Invoke(Action.SpawnFadeOut);

        yield return null;
    }

    // respawn level intro
    IEnumerator C_LevelIntroRespawn()
    {
        onLevelTransition.Invoke(Action.SpawnFadeIn);
        yield return new WaitForSeconds(RythmManager.playerBPM.ToSecs());
        Debug.Log("LevelIntroRespawn");
        Grid.FadeGridDotsGradually(Grid.FindPlayerSpawn().transform.position, start_FadeDotsDuration, 80f, true);
        Grid.FadeLevelObjectsGradually(Grid.FindPlayerSpawn().transform.position, start_FadeDotsDuration, 80f, true);

        //yield return new WaitForSeconds(start_FadeDotsDuration);
        Game.SetState(Game.State.Playing);
        yield return new WaitForSeconds(RythmManager.playerBPM.ToSecs()*3);
        onLevelTransition.Invoke(Action.SpawnFadeOut);
        yield return null;
        Game.quickSceneLoad = false;
    }
}
