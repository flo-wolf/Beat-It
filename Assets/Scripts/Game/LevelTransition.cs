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
        onLevelTransition.Invoke(Action.FadeOutRadiusHandle);
        yield return new WaitForSeconds(out_cameraToNewSpawnDuration / 3);
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
        yield return new WaitForEndOfFrame();

        // prepare showing only the player by making its griddot bigger
        PlayerSpawn.instance.gridDot.transform.localScale = PlayerSpawn.instance.gridDot.defaultSize;
        PlayerSpawn.instance.gridDot.wasFadedIn = true;

        // show only the player
        PlayerSpawn.instance.Fade(start_ShowPlayerDuration, true);
        yield return new WaitForSeconds(start_ShowPlayerDuration);

        // fade in the grid dots
        Grid.FadeGridDotsGradually(Grid.FindPlayerSpawn().transform.position, start_FadeDotsDuration, 80f, true);
        yield return new WaitForSeconds(start_FadeDotsDuration);

        // fade in the goal
        PlayerGoal.instance.Fade(start_ShowGoalDuration, true);
        yield return new WaitForSeconds(start_ShowGoalDuration);

        // fade in all other levelobjects
        Grid.FadeLevelObjectsGradually(Grid.FindPlayerSpawn().transform.position, start_FadeObjectsDuration, 80f, true);
        yield return new WaitForSeconds(start_FadeObjectsDuration);

        // start the game
        Game.SetState(Game.State.Playing);

        // fade out the spawn
        yield return new WaitForSeconds(RythmManager.playerBPM.ToSecs() * 2);
        PlayerSpawn.instance.Fade(start_ShowPlayerDuration, false);

        yield return null;
    }

    // respawn level intro
    IEnumerator C_LevelIntroRespawn()
    {
        yield return new WaitForEndOfFrame();

        PlayerSpawn.instance.gridDot.Fade(true);
        PlayerSpawn.instance.gridDot.wasFadedIn = true;
        PlayerSpawn.instance.Fade(start_ShowPlayerDuration, true);
        yield return new WaitForSeconds(RythmManager.playerBPM.ToSecs());

        Grid.FadeGridDotsGradually(Grid.FindPlayerSpawn().transform.position, start_FadeDotsDuration, 80f, true);
        Grid.FadeLevelObjectsGradually(Grid.FindPlayerSpawn().transform.position, start_FadeDotsDuration, 80f, true);

        Game.SetState(Game.State.Playing);
        Game.quickSceneLoad = false;

        yield return new WaitForSeconds(RythmManager.playerBPM.ToSecs() * 2);
        PlayerSpawn.instance.Fade(start_ShowPlayerDuration, false);


        yield return null;
    }
}
